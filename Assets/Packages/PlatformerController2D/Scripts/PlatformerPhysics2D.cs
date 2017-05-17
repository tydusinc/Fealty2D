using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformerPhysics2D : MonoBehaviour {
    public Vector2 gravity;
    public bool isGrounded;

    public LayerMask staticCollisionMask;
    public float skinWidth;
    public int verticalRayCount;
    public int horizontalRayCount;

    public float velocityDrag;
    public Vector2 velocity;
    public Vector2 frameVelocity;

    private BoxCollider2D boxCollider;
    private Bounds skinBounds;

    //public Action OnCollision2D = (Collision2D collision) => { };

    void Awake() {
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void FixedUpdate() {
        velocity = Vector2.Lerp(velocity, Vector2.zero, velocityDrag);
        velocity += gravity * Time.fixedDeltaTime;
        frameVelocity = velocity * Time.fixedDeltaTime;
        HandleCollisions();
        transform.Translate(frameVelocity);
    }

    void HandleCollisions() {
        isGrounded = false;

        skinBounds = boxCollider.bounds;
        skinBounds.Expand(-skinWidth * 2);

        horizontalRayCount = Mathf.Max(2, horizontalRayCount);
        float horizontalSpacing = skinBounds.size.y / (horizontalRayCount - 1);
        Vector2 horizontalOrigin = frameVelocity.x >= 0 ? skinBounds.max : skinBounds.min;
        Vector2 horizontalIncrementDirection = frameVelocity.x >= 0 ? Vector2.down : Vector2.up;
        Vector2 horizontalRayDirection = frameVelocity.x >= 0 ? Vector2.right : Vector2.left;
        float closestHorizontalDistance = Mathf.Abs(frameVelocity.x) + skinWidth;
        for(int i = 0; i < horizontalRayCount; i++) {
            Debug.DrawRay(horizontalOrigin + horizontalIncrementDirection * i * horizontalSpacing, horizontalRayDirection * (Mathf.Abs(frameVelocity.x) + skinWidth));
            RaycastHit2D hit = Physics2D.Raycast(horizontalOrigin + horizontalIncrementDirection * i * horizontalSpacing, horizontalRayDirection, Mathf.Abs(frameVelocity.x) + skinWidth, staticCollisionMask);
            if(hit) {
                //Call OnCollision2D
                Debug.DrawRay(horizontalOrigin + horizontalIncrementDirection * i * horizontalSpacing, horizontalRayDirection * hit.distance, Color.red);
                closestHorizontalDistance = Mathf.Min(closestHorizontalDistance, hit.distance);
                velocity.x = 0;
            }
        }
        frameVelocity.x = (closestHorizontalDistance - skinWidth) * Mathf.Sign(frameVelocity.x);

        verticalRayCount = Mathf.Max(2, verticalRayCount);
        float verticalSpacing = skinBounds.size.x / (verticalRayCount - 1);
        Vector2 verticalOrigin = frameVelocity.y >= 0 ? skinBounds.max : skinBounds.min;
        Vector2 verticalIncrementDirection = frameVelocity.y >= 0 ? Vector2.left : Vector2.right;
        Vector2 verticalRayDirection = frameVelocity.y >= 0 ? Vector2.up : Vector2.down;
        float closestVerticalDistance = Mathf.Abs(frameVelocity.y) + skinWidth;
        for(int i = 0; i < verticalRayCount; i++) {
            Debug.DrawRay(verticalOrigin + verticalIncrementDirection * i * verticalSpacing, verticalRayDirection * (Mathf.Abs(frameVelocity.y) + skinWidth));
            RaycastHit2D hit = Physics2D.Raycast(verticalOrigin + verticalIncrementDirection * i * verticalSpacing, verticalRayDirection, Mathf.Abs(frameVelocity.y) + skinWidth, staticCollisionMask);
            if(hit) {
                //Call OnCollision2D
                if (velocity.y < 0) {
                    isGrounded = true;
                }

                Debug.DrawRay(verticalOrigin + verticalIncrementDirection * i * verticalSpacing, verticalRayDirection * hit.distance, Color.red);
                closestVerticalDistance = Mathf.Min(closestVerticalDistance, hit.distance);
                velocity.y = 0;
            }
        }
        frameVelocity.y = (closestVerticalDistance - skinWidth) * Mathf.Sign(frameVelocity.y);
    }
}

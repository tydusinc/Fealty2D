using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class PlatformerController2D : MonoBehaviour {
    public float runSpeed;
    public float timeToRunSpeed;
    public float timeToStopRunning;

    public float crawlSpeed;
    public float timeToCrawlSpeed;
    public float timeToStopCrawling;
    public bool isCrouching { get { return _isCrouching; } private set { _isCrouching = value; } }
    [SerializeField]
    private bool _isCrouching;

    public float airSpeed;
    public float timeToAirSpeed;
    public float timeToStopAirMovement;

    public float jumpHeight;
    public float timeToJumpApex;

    public float maximumClimbAngle;
    private bool _isClimbingSlope;

    private Vector2 _normalizedInput;
    private float _currentFacingDirection = 1;

    public Vector2 gravity;
    public bool isGrounded { get { return _isGrounded; } private set { _isGrounded = value; } }
    [SerializeField]
    private bool _isGrounded;

    public LayerMask staticCollisionMask;
    public float skinWidth;
    public int verticalRayCount;
    public int horizontalRayCount;

    public Vector2 velocity;
    private Vector2 _frameVelocity;

    private BoxCollider2D _boxCollider;
    private Bounds _skinBounds;

    private Action _currentState;
    public ControllerStates previousState;
    public Action<ControllerStates> stateChangeEvent = (state) => { };

    void Awake() {
        _boxCollider = GetComponent<BoxCollider2D>();
        gravity.y = -(2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
        _currentState = IdleState;
    }
    void FixedUpdate() {
        velocity += gravity * Time.fixedDeltaTime;
        _currentState.Invoke();
        _frameVelocity = velocity * Time.fixedDeltaTime;
        HandleCollisions();
        transform.Translate(_frameVelocity);
        _normalizedInput.x = 0;
    }

    public void Move(float direction) {
        _normalizedInput.x = Mathf.Clamp(direction, -1, 1);
    }
    public void Jump() {
        if(isGrounded) {
            gravity.y = -(2 * jumpHeight) / (timeToJumpApex * timeToJumpApex);
            velocity.y = -gravity.y * timeToJumpApex;
            _currentState = JumpState;
        }
    }
    public void ToggleCrouch() {
        isCrouching = !isCrouching;
    }

    void ResetFlags() {
        isGrounded = false;
        _isClimbingSlope = false;
    }
    void RecalcualteBounds() {
        _skinBounds = _boxCollider.bounds;
        _skinBounds.Expand(-skinWidth * 2);
    }
    void HandleCollisions() {
        ResetFlags();
        RecalcualteBounds();

        if (_frameVelocity.x != 0) {
            HandleHorizontalCollisions();
        }
        if (_frameVelocity.y != 0) {
            HandleVerticalCollisions();
        }
        if (isGrounded && _frameVelocity.x != 0) {
            HandleSlopeDescent();
        }
    }
    void HandleHorizontalCollisions() {
        horizontalRayCount = Mathf.Max(2, horizontalRayCount);
        float raySpacing = _skinBounds.size.y / (horizontalRayCount - 1);
        Vector2 rayOrigin = _frameVelocity.x >= 0 ? _skinBounds.min + _skinBounds.size.x * Vector3.right : _skinBounds.min;
        Vector2 rayDirection = _frameVelocity.x >= 0 ? Vector2.right : Vector2.left;
        int hitCount = 0;
        float closestHit = Mathf.Abs(_frameVelocity.x) + skinWidth;
        for(int i = 0; i < horizontalRayCount; i++) {
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin + Vector2.up * i * raySpacing, rayDirection, Mathf.Abs(_frameVelocity.x) + skinWidth, staticCollisionMask);
            if(hit) {
                hitCount++;
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if(i == 0 && (slopeAngle > 0 && slopeAngle <= maximumClimbAngle)) {
                    _isClimbingSlope = true;
                    isGrounded = true;
                    if(velocity.y < 0) {
                        velocity.y = 0;
                    }

                    Vector2 movementVector = Vector3.Cross(Vector3.forward * Mathf.Sign(-_frameVelocity.x), hit.normal).normalized;
                    _frameVelocity = movementVector * Mathf.Abs(_frameVelocity.x);
                } else {
                    closestHit = Mathf.Min(closestHit, hit.distance);
                    velocity.x = 0;
                }
            }
        }
        if (!_isClimbingSlope || hitCount > 1)
            _frameVelocity.x = (closestHit - skinWidth) * Mathf.Sign(_frameVelocity.x);
    }
    void HandleVerticalCollisions() {
        verticalRayCount = Mathf.Max(2, verticalRayCount);
        float verticalSpacing = _skinBounds.size.x / (verticalRayCount - 1);
        Vector2 verticalOrigin = _frameVelocity.y >= 0 ? _skinBounds.max : _skinBounds.min;
        Vector2 verticalIncrementDirection = _frameVelocity.y >= 0 ? Vector2.left : Vector2.right;
        Vector2 verticalRayDirection = _frameVelocity.y >= 0 ? Vector2.up : Vector2.down;
        float closestVerticalDistance = Mathf.Abs(_frameVelocity.y) + skinWidth;
        for(int i = 0; i < verticalRayCount; i++) {
            RaycastHit2D hit = Physics2D.Raycast(verticalOrigin + verticalIncrementDirection * i * verticalSpacing, verticalRayDirection, Mathf.Abs(_frameVelocity.y) + skinWidth, staticCollisionMask);
            if(hit) {
                if(velocity.y < 0) {
                    isGrounded = true;
                }

                closestVerticalDistance = Mathf.Min(closestVerticalDistance, hit.distance);
                velocity.y = 0;
            }
        }
        _frameVelocity.y = (closestVerticalDistance - skinWidth) * Mathf.Sign(_frameVelocity.y);
    }
    void HandleSlopeDescent() {
        Vector2 rayOrigin = _frameVelocity.x > 0 ? _skinBounds.min : _skinBounds.min + Vector3.right * _skinBounds.size.x;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, staticCollisionMask);
        if(hit) {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if(angle != 0) {
                if(Mathf.Sign(hit.normal.x) == Mathf.Sign(_frameVelocity.x)) {
                    isGrounded = true;
                    Vector2 movementVector = Vector3.Cross(hit.normal, Vector3.forward * Mathf.Sign(hit.normal.x));
                    _frameVelocity = movementVector * Mathf.Abs(_frameVelocity.x);
                    RaycastHit2D slopeChangeCheck = Physics2D.Raycast(rayOrigin, _frameVelocity, _frameVelocity.magnitude, staticCollisionMask);
                    if(slopeChangeCheck) {
                        _frameVelocity = slopeChangeCheck.fraction * _frameVelocity;
                    }
                }
            }
        }
    }
    void UpdateStateFlag(ControllerStates targetEvent) {
        if(previousState != targetEvent) {
            stateChangeEvent.Invoke(targetEvent);
        }
        previousState = targetEvent;
    }
    void Flip() {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        _currentFacingDirection = -_currentFacingDirection;
    }
    bool ShouldFlip() {
        return Mathf.Sign(_normalizedInput.x) != _currentFacingDirection;
    }
    float GetDeceleration(float speed, float timeToStop, bool stopAtZero) {
        float deceleration = (speed / timeToStop) * Mathf.Sign(- velocity.x) * Time.fixedDeltaTime;
        if(stopAtZero) {
            if(velocity.x > 0 && deceleration + velocity.x < 0 || velocity.x < 0 && deceleration + velocity.x > 0) {
                deceleration = - velocity.x;
            }
        }
        return deceleration;
    }
    float GetAcceleration(float speed, float timeToSpeed) {
        float acceleration = (speed / timeToSpeed) * Mathf.Sign(_normalizedInput.x) * Time.fixedDeltaTime;
        if(acceleration +  velocity.x > speed) {
            acceleration = speed -  velocity.x;
        } else if(acceleration +  velocity.x < -speed) {
            acceleration = -speed -  velocity.x;
        }
        return acceleration;
    }

    void IdleState() {
        UpdateStateFlag(ControllerStates.Idle);
        if (isCrouching) {
            _currentState = CrouchState;
        }

        if (_normalizedInput.x != 0) {
            if(ShouldFlip())
                Flip();
            if (isCrouching) {
                _currentState = CrawlState;
            } else {
                _currentState = RunState;
            }
        } else {
            velocity.x = 0;
        }

        if(!isGrounded) {
            _currentState = FallState;
        }
    }
    void RunState() {
        UpdateStateFlag(ControllerStates.Run);
        if (isCrouching) {
            _currentState = CrawlState;
        }

        if (_normalizedInput.x != 0) {
            if(ShouldFlip())
                Flip();
            if (Mathf.Sign(_normalizedInput.x) != Mathf.Sign(velocity.x) && velocity.x != 0) {
                float deceleration = GetDeceleration(runSpeed, timeToStopRunning, false);
                velocity.x += deceleration;
            } else {
                float acceleration = GetAcceleration(runSpeed, timeToRunSpeed);
                velocity.x += acceleration;
            }
        } else {
            if ( velocity.x != 0) {
                float deceleration = GetDeceleration(runSpeed, timeToStopRunning, true);
                 velocity.x += deceleration;
            } else {
                if (isCrouching) {
                    _currentState = CrouchState;
                } else {
                    _currentState = IdleState;
                }
            }
        }

        if(! isGrounded) {
            _currentState = FallState;
        }
    }
    void CrouchState() {
        UpdateStateFlag(ControllerStates.Crouch);
        if(!isCrouching) {
            _currentState = IdleState;
        }

        if(_normalizedInput.x != 0) {
            if(ShouldFlip())
                Flip();
            if(isCrouching) {
                _currentState = CrawlState;
            } else {
                _currentState = RunState;
            }
        } else {
             velocity.x = 0;
        }

        if(! isGrounded) {
            _currentState = FallState;
        }
    }
    void CrawlState() {
        UpdateStateFlag(ControllerStates.Crawl);
        if(!isCrouching) {
            _currentState = RunState;
        }

        if(_normalizedInput.x != 0) {
            if(ShouldFlip())
                Flip();
            if(Mathf.Sign(_normalizedInput.x) != Mathf.Sign( velocity.x) &&  velocity.x != 0) {
                float deceleration = GetDeceleration(crawlSpeed, timeToStopCrawling, false);
                 velocity.x += deceleration;
            } else {
                float acceleration = GetAcceleration(crawlSpeed, timeToCrawlSpeed);
                 velocity.x += acceleration;
            }
        } else {
            if( velocity.x != 0) {
                float deceleration = GetDeceleration(crawlSpeed, timeToStopCrawling, true);
                 velocity.x += deceleration;
            } else {
                if(isCrouching) {
                    _currentState = CrouchState;
                } else {
                    _currentState = IdleState;
                }
            }
        }
    }
    void JumpState() {
        UpdateStateFlag(ControllerStates.Jump);
        if(_normalizedInput.x != 0) {
            if(ShouldFlip())
                Flip();
            if (Mathf.Abs( velocity.x) > airSpeed) {
                float deceleration = GetDeceleration(airSpeed, timeToStopAirMovement, false);
                 velocity.x += deceleration;
            } else {
                if(Mathf.Sign(_normalizedInput.x) != Mathf.Sign( velocity.x) &&  velocity.x != 0) {
                    float deceleration = GetDeceleration(airSpeed, timeToStopAirMovement, false);
                     velocity.x += deceleration;
                } else {
                    float acceleration = GetAcceleration(airSpeed, timeToAirSpeed);
                     velocity.x += acceleration;
                }
            }
        } else {
            if( velocity.x != 0) {
                float deceleration = GetDeceleration(airSpeed, timeToStopAirMovement, true);
                 velocity.x += deceleration;
            }
        }

        if ( velocity.y <= 0) {
            _currentState = FallState;
        }
    }
    void FallState() {
        UpdateStateFlag(ControllerStates.Fall);
        if(_normalizedInput.x != 0) {
            if(ShouldFlip())
                Flip();
            if(Mathf.Abs( velocity.x) > airSpeed) {
                float deceleration = GetDeceleration(airSpeed, timeToStopAirMovement, false);
                 velocity.x += deceleration;
            } else {
                if(Mathf.Sign(_normalizedInput.x) != Mathf.Sign( velocity.x) &&  velocity.x != 0) {
                    float deceleration = GetDeceleration(airSpeed, timeToStopAirMovement, false);
                     velocity.x += deceleration;
                } else {
                    float acceleration = GetAcceleration(airSpeed, timeToAirSpeed);
                     velocity.x += acceleration;
                }
            }
        } else {
            if( velocity.x != 0) {
                float deceleration = GetDeceleration(airSpeed, timeToStopAirMovement, true);
                 velocity.x += deceleration;
            }
        }

        if(isGrounded) {
            _currentState = IdleState;
        }
    }
    void CornerGrabState() {
        UpdateStateFlag(ControllerStates.Corner_Grab);
        //Not implemented yet.
    }
    void WallSlideState() {
        UpdateStateFlag(ControllerStates.Wall_Slide);
        //Not implemented yet.
    }
}

public enum ControllerStates {
    Idle,
    Run,
    Crouch,
    Crawl,
    Jump,
    Fall,
    Corner_Grab,
    Wall_Slide
}
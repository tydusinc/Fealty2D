using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class col : MonoBehaviour {
    public BoxCollider2D box;
    public CircleCollider2D circle;

    void Start() {
        box = this.GetComponent<BoxCollider2D>();
        circle = this.GetComponent<CircleCollider2D>();
    }

    void Update() {
        Physics2D.IgnoreLayerCollision(8, 9);
    }

    void OnCollisionEnter2D(Collision2D collisioner) {
        if (collisioner.gameObject.tag == "player")
        {
            Physics2D.IgnoreCollision(collisioner.collider, circle);
            Physics2D.IgnoreCollision(collisioner.collider, box);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(PlatformerController2D))]
public class PlayerAnimator : MonoBehaviour {
    PlatformerController2D controller;
    Animator animator;

    void Awake() {
        controller = GetComponent<PlatformerController2D>();
        animator = GetComponent<Animator>();
    }

	void Update () {
        animator.SetFloat("Speed", Mathf.Abs(controller.velocity.x));
        animator.SetBool("Ground", controller.isGrounded);
        animator.SetBool("Crouch", controller.isCrouching);
        animator.SetFloat("vSpeed", controller.velocity.y);
    }
}

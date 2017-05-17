using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    PlatformerController2D controller;

    void Awake() {
        controller = GetComponent<PlatformerController2D>();
    }

    void Update() {
        controller.Move(Input.GetAxisRaw("Horizontal"));

        if (Input.GetKeyDown(KeyCode.Space)) {
            controller.Jump();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl)) {
            controller.ToggleCrouch();
        }
    }
}
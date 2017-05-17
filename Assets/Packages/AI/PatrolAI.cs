using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PatrolAI : MonoBehaviour {
    PlatformerController2D controller;
    Action currentState;

    float direction = 1;
    float timer = 1;

    void Awake() {
        controller = GetComponent<PlatformerController2D>();
        currentState = Idle;
    }

    void Update() {
        currentState.Invoke();
    }

    void Idle() {
        timer -= Time.deltaTime;

        if (timer <= 0) {
            timer = 1;
            direction = -direction;
            currentState = Patrol;
        }
    }

    void Patrol() {
        controller.Move(direction);
        timer -= Time.deltaTime;

        if (timer <= 0) {
            timer = 2;
            currentState = Idle;
        }
    }
}
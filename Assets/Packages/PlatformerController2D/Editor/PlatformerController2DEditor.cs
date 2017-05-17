using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlatformerController2D))]
public class PlatformerController2DEditor : Editor {
    PlatformerController2D controller;

    static bool showRunningSettings;
    static bool showCrouchingSettings;
    static bool showSlopeSettings;
    static bool showJumpingSettings;
    static bool showCollisionSettings;
    static bool showPhysicsInformation;
    static bool showControllerInformation;

    public override void OnInspectorGUI() {
        serializedObject.Update();
        controller = (PlatformerController2D)serializedObject.targetObject;

        showRunningSettings = EditorGUILayout.Foldout(showRunningSettings, "Running Settings");
        if(showRunningSettings) {
            controller.runSpeed = EditorGUILayout.FloatField("Run Speed", controller.runSpeed);
            controller.timeToRunSpeed = EditorGUILayout.FloatField("Time To Run Speed", controller.timeToRunSpeed);
            controller.timeToStopRunning = EditorGUILayout.FloatField("Time To Stop Running", controller.timeToStopRunning);
        }

        showCrouchingSettings = EditorGUILayout.Foldout(showCrouchingSettings, "Crouching Settings");
        if(showCrouchingSettings) {
            controller.crawlSpeed = EditorGUILayout.FloatField("Crawl Speed", controller.runSpeed);
            controller.timeToCrawlSpeed = EditorGUILayout.FloatField("Time To Crawl Speed", controller.timeToRunSpeed);
            controller.timeToStopCrawling = EditorGUILayout.FloatField("Time To Stop Crawling", controller.timeToStopRunning);
        }

        showSlopeSettings = EditorGUILayout.Foldout(showSlopeSettings, "Slope Settings");
        if (showSlopeSettings) {
            controller.maximumClimbAngle = EditorGUILayout.FloatField("Maximum Climb Angle", controller.maximumClimbAngle);
        }

        showJumpingSettings = EditorGUILayout.Foldout(showJumpingSettings, "Jumping Settings");
        if(showJumpingSettings) {
            controller.airSpeed = EditorGUILayout.FloatField("Aerial Speed", controller.airSpeed);
            controller.timeToAirSpeed = EditorGUILayout.FloatField("Time To Aerial Speed", controller.timeToAirSpeed);
            controller.timeToStopAirMovement = EditorGUILayout.FloatField("Time To Stop Air Movement", controller.timeToStopAirMovement);
            controller.jumpHeight = EditorGUILayout.FloatField("Jump Height", controller.jumpHeight);
            controller.timeToJumpApex = EditorGUILayout.FloatField("Time To Jump Apex", controller.timeToJumpApex);
        }

        showCollisionSettings = EditorGUILayout.Foldout(showCollisionSettings, "Collision Settings");
        if(showCollisionSettings) {
            controller.horizontalRayCount = Mathf.Max(EditorGUILayout.IntField("Horizontal Rays", controller.horizontalRayCount), 2);
            controller.verticalRayCount = Mathf.Max(EditorGUILayout.IntField("Vertical Rays", controller.verticalRayCount), 2);
            controller.staticCollisionMask = EditorTools.LayerMaskField("Static Collision Mask", controller.staticCollisionMask);
            controller.skinWidth = EditorGUILayout.FloatField("Skin Width", controller.skinWidth);
        }

        showPhysicsInformation = EditorGUILayout.Foldout(showPhysicsInformation, "Physics Information");
        if (showPhysicsInformation) {
            controller.velocity = EditorGUILayout.Vector2Field("Velocity", controller.velocity);
            EditorGUILayout.Toggle("Grounded", controller.isGrounded);
        }

        EditorGUILayout.HelpBox("Current State: " + controller.previousState.ToString(), MessageType.Info);

        if(GUI.changed)
            EditorUtility.SetDirty(controller);
    }
}
// Copyright (c) 2018 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

#region Defines
#if UNITY_2020 || UNITY_2021 || UNITY_2022 || UNITY_2023 || UNITY_2024 || UNITY_2025
#define UNITY_2020_PLUS
#endif
#if UNITY_2019 || UNITY_2020_PLUS
#define UNITY_2019_PLUS
#endif
#if UNITY_2018 || UNITY_2019_PLUS
#define UNITY_2018_PLUS
#endif
#if UNITY_2017 || UNITY_2018_PLUS
#define UNITY_2017_PLUS
#endif
#if UNITY_5 || UNITY_2017_PLUS
#define UNITY_5_PLUS
#endif
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_1_PLUS
#endif
#if UNITY_5_2 || UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_2_PLUS
#endif
#if UNITY_5_3_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_3_PLUS
#endif
#if UNITY_5_4_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_4_PLUS
#endif
#if UNITY_5_5_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_5_PLUS
#endif
#if UNITY_5_6_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_6_PLUS
#endif
#if UNITY_5_7_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_7_PLUS
#endif
#if UNITY_5_8_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_8_PLUS
#endif
#if UNITY_5_9_OR_NEWER || UNITY_2017_PLUS
#define UNITY_5_9_PLUS
#endif
#pragma warning disable 0219
#pragma warning disable 0618
#pragma warning disable 0649
#pragma warning disable 0067
#endregion

using UnityEngine;
using System.Collections.Generic;
using Rewired.ControllerExtensions;
using Rewired;
using UnityEngine.SceneManagement;

[AddComponentMenu("")]
public class DS4Controllor : MonoBehaviour
{

    public int playerId = 0;
    public PlayerController pc;
    public Transform anchor;
    public SpriteRenderer anchorSprite;

    private Queue<Touch> unusedTouches;
    private bool isFlashing;
    private GUIStyle textStyle;
    private int controlMode = 0;

    private Player player { get { return ReInput.players.GetPlayer(playerId); } }

    IDualShock4Extension ds4;

    private void Awake()
    {
        // Get the first DS4 found assigned to the Player
        ds4 = GetFirstDS4(player);
        controlMode = GameManager.instance.GetComponent<GameManager>().controlMod;
    }

    private void Update()
    {
        if (!ReInput.isReady) return;

        float X = 0, Y = 0;
        if (ds4 != null)
        {
            if(controlMode == 1)
            {
                X = -Input.GetAxis("Joy1Axis1");
                Y = Input.GetAxis("Joy1Axis2");
            }
            else
            {
                X = ds4.GetOrientation().z;
                Y = -ds4.GetOrientation().x;
            }

            // Set the model's rotation to match the controller's
            if(new Vector2(X,Y).magnitude < 0.07f)
            {
                X = 0;
                Y = -1;
                anchorSprite.enabled = false;
            }
            else
            {
                anchorSprite.enabled = true;
            }
            anchor.LookAt(anchor.position + new Vector3(X, 0, Y));
        }

        if (Input.GetKeyDown("joystick button 0")) //square
        {
            ResetOrientation();
        }
        if (Input.GetKeyDown("joystick button 1")) //X
        {
        }
        if (Input.GetKeyDown("joystick button 2")) //circlr
        {
        }
        if (Input.GetKeyDown("joystick button 3")) //triangle
        {
        }
        if (Input.GetKeyDown("joystick button 5")) //R1
        {
            if (controlMode == 1)
            {
                controlMode = 0;
                GameManager.instance.GetComponent<GameManager>().controlMod = 0;

            }
            else
            {
                controlMode = 1;
                GameManager.instance.GetComponent<GameManager>().controlMod = 1;
            }
        }
        if (Input.GetKeyUp("joystick button 7")) //trigger
        {
            pc.ShootWithGamepad(-X, -Y);
        }
    }

    public void Vibrate(int index, float level, float duration)
    {
        ds4.SetVibration(index, level, duration);
    }

//    private void OnGUI()
//    {
//        if (textStyle == null)
//        {
//            textStyle = new GUIStyle(GUI.skin.label);
//            textStyle.fontSize = 20;
//            textStyle.wordWrap = true;
//        }

//        if (GetFirstDS4(player) == null) return; // no DS4 is assigned

//        GUILayout.BeginArea(new Rect(200f, 100f, Screen.width - 400f, Screen.height - 200f));

//        GUILayout.Label("Rotate the Dual Shock 4 to see the model rotate in sync.", textStyle);

//        GUILayout.Label("Touch the touchpad to see them appear on the model.", textStyle);

//        ActionElementMap aem;

//        aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "ResetOrientation", true);
//        if (aem != null)
//        {
//            GUILayout.Label("Press " + aem.elementIdentifierName + " to reset the orientation. Hold the gamepad facing the screen with sticks pointing up and press the button.", textStyle);
//        }

//        aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "CycleLight", true);
//        if (aem != null)
//        {
//            GUILayout.Label("Press " + aem.elementIdentifierName + " to change the light color.", textStyle);
//        }

//#if !UNITY_PS4

//        // Light flash is not supported on the PS4 platform.
//        aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "ToggleLightFlash", true);
//        if (aem != null)
//        {
//            GUILayout.Label("Press " + aem.elementIdentifierName + " to start or stop the light flashing.", textStyle);
//        }

//#endif

//        aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "VibrateLeft", true);
//        if (aem != null)
//        {
//            GUILayout.Label("Press " + aem.elementIdentifierName + " vibrate the left motor.", textStyle);
//        }

//        aem = player.controllers.maps.GetFirstElementMapWithAction(ControllerType.Joystick, "VibrateRight", true);
//        if (aem != null)
//        {
//            GUILayout.Label("Press " + aem.elementIdentifierName + " vibrate the right motor.", textStyle);
//        }

//        GUILayout.EndArea();
//    }

    private void ResetOrientation()
    {
        var ds4 = GetFirstDS4(player);
        if (ds4 != null)
        {
            ds4.ResetOrientation();
        }
    }

    private void SetRandomLightColor()
    {
        var ds4 = GetFirstDS4(player);
        if (ds4 != null)
        {
            Color color = new Color(
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                Random.Range(0f, 1f),
                1f
            );
            ds4.SetLightColor(color);
        }
    }

    private void StartLightFlash()
    {
        // This is not supported on PS4 so get the Standalone DualShock4Extension
        DualShock4Extension ds4 = GetFirstDS4(player) as DualShock4Extension;
        if (ds4 != null)
        {
            ds4.SetLightFlash(0.5f, 0.5f);
            // Light flash is handled by the controller hardware itself and not software.
            // The current value cannot be obtained from the controller so it
            // cannot be reflected in the 3D model without just recreating the flash to approximate it.
        }
    }

    private void StopLightFlash()
    {
        // This is not supported on PS4 so get the Standalone DualShock4Extension
        DualShock4Extension ds4 = GetFirstDS4(player) as DualShock4Extension;
        if (ds4 != null)
        {
            ds4.StopLightFlash();
        }
    }

    private IDualShock4Extension GetFirstDS4(Player player)
    {
        foreach (Joystick j in player.controllers.Joysticks)
        {
            // Use the interface because it works for both PS4 and desktop platforms
            IDualShock4Extension ds4 = j.GetExtension<IDualShock4Extension>();
            if (ds4 == null) continue;
            return ds4;
        }
        return null;
    }

    private class Touch
    {
        public GameObject go;
        public int touchId = -1;
    }
}
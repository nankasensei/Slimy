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
    public Vector2 oriVec;

    private const int maxTouches = 2;
    private Queue<Touch> unusedTouches;
    private bool isFlashing;
    private GUIStyle textStyle;
    private int controlMode = 0;
    private bool touchStart = false;
    private bool isTamaLargePermited = false;
    private int fingerId = 0;
    private Vector2 fingerStartPos;
    private float chargeTime = 3f;

    private List<Touch> touches;
    private Timer chargeTimer;

    private Player player { get { return ReInput.players.GetPlayer(playerId); } }

    IDualShock4Extension ds4;

    private void Awake()
    {
        // Get the first DS4 found assigned to the Player
        ds4 = GetFirstDS4(player);
        controlMode = GameManager.instance.GetComponent<GameManager>().controlMod;
        InitializeTouchObjects();
        chargeTimer = new Timer();
    }

    private void Update()
    {
        if (!ReInput.isReady) return;

        float X = 0, Y = 0;

        if (ds4 != null)
        {
            oriVec = new Vector2(-ds4.GetOrientation().z, ds4.GetOrientation().x);

            if (controlMode == 0 || controlMode == 1)
            {
                if (controlMode == 1)
                {
                    X = -Input.GetAxis("Joy1Axis1");
                    Y = Input.GetAxis("Joy1Axis2");
                }
                else
                {
                    X = ds4.GetOrientation().z;
                    Y = -ds4.GetOrientation().x;
                }

                // set anchor
                if (new Vector2(X, Y).magnitude < 0.05f)
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

                //set trigger
                if (Input.GetKeyDown("joystick button 7")) //trigger
                {
                    chargeTimer.Start();
                }
                if (Input.GetKeyUp("joystick button 7")) //trigger
                {
                    if(chargeTimer.isStart && chargeTimer.elapasedTime> chargeTime && isTamaLargePermited)
                    {
                        pc.ChangeTama(1);
                    }
                    else
                    {
                        if (pc.mode == 0)
                            pc.ChangeTama(0);
                        else if (pc.mode == 1)
                            pc.ChangeTama(2);
                        else if (pc.mode == 2)
                            pc.ChangeTama(3);
                    }

                    pc.ShootWithGamepad(-X, -Y);
                    chargeTimer.Stop();
                    isTamaLargePermited = false;
                }
            }
            else if(controlMode == 2)
            {
                HandleTouchpad(ds4);
            }
        }

        if (chargeTimer.isStart)
        {
            float x = Random.Range(-0.05f, 0.05f);
            float y = Random.Range(-0.05f, 0.05f);

            if(chargeTimer.elapasedTime > 0.5f)
            {
                if (pc.hp >= 150)
                    isTamaLargePermited = true;

                if (isTamaLargePermited)
                {
                    if (chargeTimer.elapasedTime <= chargeTime)
                    {
                        pc.SetHp(-30 * Time.deltaTime);
                        Vibrate(0, 0.5f, 0.1f);
                        Vibrate(1, 0.5f, 0.1f);
                        pc.spriteTransform.localPosition = new Vector3(x, y, 0);
                    }
                    if (chargeTimer.elapasedTime > chargeTime && chargeTimer.elapasedTime < chargeTime + 0.2f)
                    {
                        Vibrate(0, 1, 0.1f);
                        Vibrate(1, 1, 0.1f);
                        pc.spriteTransform.localPosition = new Vector3(x, y, 0);
                    }
                    if (chargeTimer.elapasedTime > chargeTime + 0.2f)
                        pc.spriteTransform.localPosition = new Vector3(x, y, 0);
                }
                else
                {
                    if (chargeTimer.elapasedTime <= chargeTime/2)
                    {
                        Vibrate(0, 0.5f, 0.1f);
                        Vibrate(1, 0.5f, 0.1f);
                        pc.spriteTransform.localPosition = new Vector3(x, y, 0);
                    }
                }
            }
        }

        if (Input.GetKeyDown("joystick button 5")) //R1
        {
            controlMode = (controlMode + 1) % 3;
            GameManager.instance.GetComponent<GameManager>().controlMod = controlMode;
            SlimyEvents.modeSwitchEvent.Invoke();
        }

        if (Input.GetKeyDown("joystick button 0")) //square
        {
            ResetOrientation();
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

    private void InitializeTouchObjects()
    {

        touches = new List<Touch>(maxTouches);
        unusedTouches = new Queue<Touch>(maxTouches);

        // Setup touch objects
        for (int i = 0; i < maxTouches; i++)
        {
            Touch touch = new Touch();
            // Create spheres to reprensent touches
            touch.go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            touch.go.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
#if UNITY_5_PLUS
            touch.go.transform.SetParent(transform, true);
#else
                touch.go.transform.parent = touchpadTransform;
#endif
            touch.go.GetComponent<MeshRenderer>().material.color = i == 0 ? Color.red : Color.green;
            touch.go.SetActive(false);
            unusedTouches.Enqueue(touch);
        }
    }

    private class Touch
    {
        public GameObject go;
        public int touchId = -1;
    }

    private void HandleTouchpad(IDualShock4Extension ds4)
    {
        if(controlMode == 2)
        {
            if (!touchStart && ds4.IsTouching(0))
            {
                fingerId = ds4.GetTouchId(0);
                touchStart = true;
                ds4.GetTouchPosition(0, out fingerStartPos);
                //Debug.Log("start   " + fingerStartPos);

                chargeTimer.Start();
            }

            if (!ds4.IsTouchingByTouchId(fingerId) && touchStart)
            {
                Touch touch = touches.Find(x => x.touchId == fingerId);
                touchStart = false;

                Vector2 fingerEndPos, fingerDir;
                fingerEndPos = new Vector2(touch.go.transform.localPosition.x + 0.5f, touch.go.transform.localPosition.z + 0.5f);
                fingerDir = new Vector2((fingerEndPos - fingerStartPos).x * 2.25f, (fingerEndPos - fingerStartPos).y); //because the touchpad is not a square but scales from (0,0) to (1,1)

                if (controlMode == 2 && fingerDir.magnitude > 0.15f)
                {
                    if (chargeTimer.isStart && chargeTimer.elapasedTime > chargeTime && isTamaLargePermited)
                    {
                        pc.ChangeTama(1);
                    }
                    else
                    {
                        if(pc.mode == 0)
                            pc.ChangeTama(0);
                        else if(pc.mode == 1)
                            pc.ChangeTama(2);
                        else if (pc.mode == 2)
                            pc.ChangeTama(3);     
                    }

                    pc.ShootWithGamepad(fingerDir.x, fingerDir.y);
                    chargeTimer.Stop();
                    isTamaLargePermited = false;
                }

                //Debug.Log("end   " + fingerEndPos);
            }
        }

        // Expire old touches first
        for (int i = touches.Count - 1; i >= 0; i--)
        {
            Touch touch = touches[i];
            if (!ds4.IsTouchingByTouchId(touch.touchId))
            { // the touch id is no longer valid
                touch.go.SetActive(false); // disable the game object
                unusedTouches.Enqueue(touch); // return to the pool
                touches.RemoveAt(i); // remove from active touches list
            }
        }

        // Process new touches
        for (int i = 0; i < ds4.maxTouches; i++)
        {
            if (!ds4.IsTouching(i)) continue;
            int touchId = ds4.GetTouchId(i);
            Touch touch = touches.Find(x => x.touchId == touchId); // find the touch with this id
            if (touch == null)
            {
                touch = unusedTouches.Dequeue(); // get a new touch from the pool
                touches.Add(touch); // add to active touches list
            }
            touch.touchId = touchId; // store the touch id
            //touch.go.SetActive(true); // show the object

            // Get the touch position
            Vector2 position;
            ds4.GetTouchPosition(i, out position);

            // Set the new position of the touch
            touch.go.transform.localPosition = new Vector3(
                position.x - 0.5f,
                0.5f + (touch.go.transform.localScale.y * 0.5f),
                position.y - 0.5f
            );
        }
    }
}
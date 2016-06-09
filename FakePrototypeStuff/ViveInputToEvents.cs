﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.InputNew;
using UnityEngine.VR;
using Valve.VR;

public class ViveInputToEvents
    : MonoBehaviour {
    private enum Handedness { Left, Right }
    private enum XorY { X, Y }
    private int[] HandDeviceId = new int[] { -1, -1 };

    public void Update() {
        for (Handedness hand = Handedness.Left; (int)hand <= (int)Handedness.Right; hand++) {
            var deviceIdx = HandDeviceId[(int)hand];

            if (deviceIdx == -1) {
                deviceIdx = SteamVR_Controller.GetDeviceIndex(hand == Handedness.Left
                     ? SteamVR_Controller.DeviceRelation.Leftmost
                     : SteamVR_Controller.DeviceRelation.Rightmost);

                if (deviceIdx == -1)
                    continue;

                if (hand == Handedness.Left)
                {
                    HandDeviceId[(int)hand] = deviceIdx;
                    Debug.Log("Assigned Left " + deviceIdx);

                }
                else if(deviceIdx != HandDeviceId[(int)Handedness.Left]) // Do not assign device to right hand if it is same device as left hand
                {
                    HandDeviceId[(int)hand] = deviceIdx;
                    Debug.Log("Assigned Right " + deviceIdx);        
                }
                else
                {
                    continue;
                }


            }

            SendButtonEvents(deviceIdx, hand);
            SendAxisEvents(deviceIdx, hand);
            SendTrackingEvents(deviceIdx, hand);
        }
    }

    public const int controllerCount = 10;
    public const int buttonCount = 64;
    public const int axisCount = 10; // 5 axes in openVR, each with X and Y.
    private float[,] m_LastAxisValues = new float[controllerCount, axisCount];
    private Vector3[] m_LastPositionValues = new Vector3[controllerCount];
    private Quaternion[] m_LastRotationValues = new Quaternion[controllerCount];

    private void SendAxisEvents(int deviceIdx, Handedness hand) {
        int a = 0;
        for (int axis = (int)EVRButtonId.k_EButton_Axis0; axis <= (int)EVRButtonId.k_EButton_Axis4; ++axis) {
            Vector2 axisVec = SteamVR_Controller.Input(deviceIdx).GetAxis((EVRButtonId)axis);
            for (XorY xy = XorY.X; (int)xy <= (int)XorY.Y; xy++, a++) {
                var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
                inputEvent.deviceType = typeof(VRInputDevice);
                inputEvent.deviceIndex = hand == Handedness.Left ? 3 : 4; // TODO change 3 and 4 based on virtual devices defined in InputDeviceManager (using actual hardware available)
                inputEvent.controlIndex = a;
                inputEvent.value = xy == XorY.X ? axisVec.x : axisVec.y;

                if (Mathf.Approximately(m_LastAxisValues[deviceIdx, a], inputEvent.value)) {
                    continue;
                }
                m_LastAxisValues[deviceIdx, a] = inputEvent.value;
                // Debug.Log("Axis event: " + inputEvent);

                InputSystem.QueueEvent(inputEvent);
            }
        }
    }

    private void SendButtonEvents(int deviceIdx, Handedness hand) {
        foreach (EVRButtonId button in Enum.GetValues(typeof(EVRButtonId))) {
            bool keyDown = SteamVR_Controller.Input(deviceIdx).GetPressDown(button);
            bool keyUp = SteamVR_Controller.Input(deviceIdx).GetPressUp(button);

            if (keyDown || keyUp) {
                var inputEvent = InputSystem.CreateEvent<GenericControlEvent>();
                inputEvent.deviceType = typeof(VRInputDevice);
                inputEvent.deviceIndex = inputEvent.deviceIndex = hand == Handedness.Left ? 3 : 4; // TODO change 3 and 4 based on virtual devices defined in InputDeviceManager (using actual hardware available)
                inputEvent.controlIndex = axisCount + (int)button;
                inputEvent.value = keyDown ? 1.0f : 0.0f;

                Debug.Log(string.Format("event: {0}; button: {1}; hand: {2}", inputEvent, button, hand));

                InputSystem.QueueEvent(inputEvent);
            }
        }
    }

    private void SendTrackingEvents(int deviceIdx, Handedness hand) {
        var inputEvent = InputSystem.CreateEvent<VREvent>();
        inputEvent.deviceType = typeof(VRInputDevice);
        inputEvent.deviceIndex = inputEvent.deviceIndex = hand == Handedness.Left ? 3 : 4; // TODO change 3 and 4 based on virtual devices defined in InputDeviceManager (using actual hardware available)
        var pose = new SteamVR_Utils.RigidTransform(SteamVR_Controller.Input(deviceIdx).GetPose().mDeviceToAbsoluteTracking);
        inputEvent.localPosition = pose.pos;
        inputEvent.localRotation = pose.rot;

        if (inputEvent.localPosition == m_LastPositionValues[deviceIdx] &&
            inputEvent.localRotation == m_LastRotationValues[deviceIdx])
            return;

        m_LastPositionValues[deviceIdx] = inputEvent.localPosition;
        m_LastRotationValues[deviceIdx] = inputEvent.localRotation;

        InputSystem.QueueEvent(inputEvent);
    }
}
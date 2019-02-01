﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputComponent : MonoBehaviour
{
    public bool headIsTracked;
    public bool leftHandIsTracked;
    public bool rightHandIsTracked;

    // A SteamVR device got connected/disconnected
    private void OnDeviceConnected(int index, bool connected)
    {
        if (OpenVR.System != null)
        {
            if (connected)
            {
                ETrackedDeviceClass deviceClass = OpenVR.System.GetTrackedDeviceClass((uint)index);
                if (deviceClass == ETrackedDeviceClass.Controller)
                {
                    Debug.Log("Got Controller");
                    leftHandIsTracked = true;
                }
                else if (deviceClass == ETrackedDeviceClass.GenericTracker)
                {
                    Debug.Log("Got Head");
                    headIsTracked = true;
                }
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        string[] controllers = Input.GetJoystickNames();
        Debug.Log("Controller count: " + controllers.Length + " " + controllers.ToString());
        SteamVR_Events.DeviceConnected.Listen(OnDeviceConnected);
    }

    // Update is called once per frame
    void Update()
    {
        //Manipulation ermöglichen
        /*UIAnchorManager.setTrackedHead();
        ...
        UIAnchorManager.setInitialised();
        UIAnchorManager.initialiseAnchors();*/
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class InputComponent : MonoBehaviour
{
    public bool headIsShown;
    public bool leftHandIsShown;
    public bool rightHandIsShown;

    public Transform head;
    public Transform left;
    public Transform right;

    public UIAnchor headAnchor;
    public UIAnchor leftAnchor;
    public UIAnchor rightAnchor;

    // A SteamVR device got connected/disconnected
    /*private void OnDeviceConnected(int index, bool connected)
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
    }*/

    // Use this for initialization
    void Start()
    {
        //string[] controllers = Input.GetJoystickNames();
        //Debug.Log("Controller count: " + controllers.Length + " " + controllers.ToString());
        //SteamVR_Events.DeviceConnected.Listen(OnDeviceConnected);
        if (headIsShown)
        {
            UIAnchorManager.setTrackedHead(head);
        }
        if (leftHandIsShown)
        {
            UIAnchorManager.setTrackedLeftHand(left);
        }
        if (rightHandIsShown)
        {
            UIAnchorManager.setTrackedRightHand(right);
        }
        UIAnchorManager.setMainAnchors(headAnchor, leftAnchor, rightAnchor);
        UIAnchorManager.setInitialised();
        UIAnchorManager.initialiseAnchors();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(head.position, head.TransformDirection(Vector3.forward), out hit, 2, 5))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            Debug.Log("Hit");
            //set selected object
        }
        //Manipulation ermöglichen
        /*UIAnchorManager.setTrackedHead();
        ...
        UIAnchorManager.setInitialised();
        UIAnchorManager.initialiseAnchors();*/
    }
}

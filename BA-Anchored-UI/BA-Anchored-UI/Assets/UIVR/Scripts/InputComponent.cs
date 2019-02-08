using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.UI;

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

    private GameObject hoveringOver;
    private float triggerTimer = 0;

    public static GameObject selectedObject;
    public Image cursorIndicator;

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
        if (Physics.Raycast(head.position, head.TransformDirection(Vector3.forward), out hit, 10))
        {
            if (hit.collider.gameObject.Equals(hoveringOver) && hit.collider.gameObject.layer.Equals(5))
            {
                Debug.Log("Hit");
                triggerTimer += Time.deltaTime;
                if(triggerTimer > 2 && hoveringOver.GetComponent<AnchoredUI>().isInteractable())
                {
                    //TODO: save Scale
                    selectedObject.transform.parent = hoveringOver.transform;
                    selectedObject.transform.position = Vector3.zero;
                }
            }
            else
            {
                hoveringOver = hit.collider.gameObject;
                triggerTimer = 0;
            }
            //set selected object
        }
        Debug.DrawRay(head.position, head.TransformDirection(Vector3.forward) * 200, Color.yellow);
        if (cursorIndicator)
        {
            cursorIndicator.fillAmount = triggerTimer / 2;
        }
    }
}

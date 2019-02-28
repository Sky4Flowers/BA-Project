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

    public Trader trader;
    public static Trader staticTrader;
    public UISwitch switchButton;
    public GameObject[] auswahl;
    public static GameObject statAuswahl;

    public UIAnchor[] leftObjects;
    public UIAnchor[] rightObjects;

    // Use this for initialization
    void Start()
    {
        staticTrader = trader;

        if (headIsShown)
        {
            UIAnchorManager.setTrackedHead(head);
        }
        if (leftHandIsShown)
        {
            UIAnchorManager.setTrackedLeftHand(left);
        }
        else
        {
            switchButton.gameObject.SetActive(true);
        }
        if (rightHandIsShown)
        {
            UIAnchorManager.setTrackedRightHand(right);
            statAuswahl = auswahl[2];
        }
        else
        {
            if (leftHandIsShown)
            {
                statAuswahl = auswahl[1];
            }
            else
            {
                statAuswahl = auswahl[0];
            }
        }
        UIAnchorManager.setMainAnchors(headAnchor, leftAnchor, rightAnchor);
        UIAnchorManager.setInitialised();
        UIAnchorManager.initialiseAnchors();
    }

    // Update is called once per frame
    void Update()
    {
        if (!rightHandIsShown && !leftHandIsShown)
        {
            switchButton.gameObject.SetActive(false);
        }

        if (SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            for (int i = 0; i < leftObjects.Length; i++)
            {
                leftObjects[i].gameObject.SetActive(true);
            }
        }

        if (SteamVR_Input._default.inActions.Teleport.GetStateUp(SteamVR_Input_Sources.LeftHand))
        {
            for (int i = 0; i < leftObjects.Length; i++)
            {
                leftObjects[i].gameObject.SetActive(false);
            }
        }

        if (SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.RightHand))
        {
            for (int i = 0; i < leftObjects.Length; i++)
            {
                rightObjects[i].gameObject.SetActive(true);
            }
        }

        if (SteamVR_Input._default.inActions.Teleport.GetStateUp(SteamVR_Input_Sources.RightHand))
        {
            for (int i = 0; i < leftObjects.Length; i++)
            {
                rightObjects[i].gameObject.SetActive(false);
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(head.position, head.TransformDirection(Vector3.forward), out hit, 10))
        {
            if (hit.collider.gameObject.Equals(hoveringOver) && hit.collider.gameObject.layer.Equals(5))
            {
                triggerTimer += Time.deltaTime;
                if (triggerTimer > 1)
                {
                    ISelectable selectComponent = hoveringOver.GetComponent<ISelectable>();
                    if (selectComponent != null)
                    {
                        selectComponent.select();
                        triggerTimer = 0;
                    }
                }
            }
            else
            {
                hoveringOver = hit.collider.gameObject;
                triggerTimer = 0;
            }
            //set selected object
        }
        else
        {
            triggerTimer = 0;
        }
        //Debug.DrawRay(head.position, head.TransformDirection(Vector3.forward) * 200, Color.yellow);
        if (cursorIndicator)
        {
            cursorIndicator.fillAmount = triggerTimer;
        }


        //if(trackedStaticControllerLeft.poseAction.actionSet.)
    }

    public static GameObject setSelectedObject(GameObject selected)
    {
        GameObject toBeReturned = null;
        if (selectedObject != null)
        {
            toBeReturned = selectedObject;
        }

        if (selected == null)
        {
            selectedObject = null;
        }
        else
        {
            selectedObject = selected;
            selected.transform.SetParent(statAuswahl.transform);
            selected.transform.localPosition = Vector3.zero;
            selected.transform.localScale = Vector3.one * 10;
        }
        return toBeReturned;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIAnchorManager {

    private static Transform trackedHeadTransform;
    private static Transform trackedLeftHandTransform;
    private static Transform trackedRightHandTransform;
    private static bool isInitialised = false;

    public enum AnchorType
    {
        HEAD,
        LEFT_HAND,
        RIGHT_HAND
    };

    public enum AnchorStyle
    {
        RECTANGLE,
        CYLINDER
    };

    public enum AnchorExpansionType
    {
        SWITCH,
        DIRECTION_LEFT,
        DIRECTION_TOP,
        DIRECTION_RIGHT,
        DIRECTION_BOTTOM,
    }

    [SerializeField]
    [Tooltip("Order should be:\n 0 - Head\n 1 - LeftHand\n 2 - RightHand")]
    private static UIAnchor[] mainAnchors = new UIAnchor[3];

    public static void setTrackedHead(Transform head)
    {
        trackedHeadTransform = head;
    }

    public static void setTrackedLeftHand(Transform left)
    {
        trackedLeftHandTransform = left;
    }

    public static void setTrackedRightHand(Transform right)
    {
        trackedRightHandTransform = right;
    }

    public static void setInitialised()
    {
        isInitialised = true;
    }

    /*public static bool addAnchor(UIAnchor anchor)
    {
        if (!isInitialised)
        {
            return false;
        }
        switch (anchor.getType())
        {
            case AnchorType.HEAD:
                if (anchors[0] == null)
                {
                    anchors[0] = new ArrayList();
                }
                anchors[0].Add(anchor);
                anchor.setAnchorObjectTransform(trackedHeadTransform);
                break;

            case AnchorType.LEFT_HAND:
                if (anchors[1] == null)
                {
                    anchors[1] = new ArrayList();
                }
                anchors[1].Add(anchor);
                anchor.setAnchorObjectTransform(trackedLeftHandTransform);
                break;

            case AnchorType.RIGHT_HAND:
                if (anchors[2] == null)
                {
                    anchors[2] = new ArrayList();
                }
                anchors[2].Add(anchor);
                anchor.setAnchorObjectTransform(trackedRightHandTransform);
                break;
        }
        return true;
    }*/

    public static UIAnchor getAnchorFallback(UIAnchor anchor)
    {
        switch (anchor.getType())
        {
            case AnchorType.LEFT_HAND:
                if(mainAnchors[2] != null)
                {
                    return mainAnchors[2];
                }
                break;
            case AnchorType.RIGHT_HAND:
                if (mainAnchors[1] != null)
                {
                    return mainAnchors[1];
                }
                break;
        }
        if(mainAnchors[0] != null)
        {
            return mainAnchors[0];
        }
        else
        {
            return null;
        }
    }

    public static Vector3 getHeadPosition()
    {
        return trackedHeadTransform.position;
    }

    public static void setMainAnchors(UIAnchor head, UIAnchor left, UIAnchor right)
    {
        mainAnchors[0] = head;
        mainAnchors[1] = left;
        mainAnchors[2] = right;
    }

    public static void initialiseAnchors()
    {
        if (!isInitialised)
        {
            Debug.LogError("UIAnchorManager failed initialising: Trying to initialise anchors without setting initialised-state");
            return;
        }
        mainAnchors[0].setAnchorObjectTransform(trackedHeadTransform);
        mainAnchors[1].setAnchorObjectTransform(trackedLeftHandTransform);
        mainAnchors[2].setAnchorObjectTransform(trackedRightHandTransform);
    }
}
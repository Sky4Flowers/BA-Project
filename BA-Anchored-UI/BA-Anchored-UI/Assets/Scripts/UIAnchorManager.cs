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
    private static UIAnchor[] mainAnchors = new UIAnchor[3];

    public static void setTrackedTransforms(Transform head, Transform left, Transform right)
    {
        isInitialised = true;
        trackedHeadTransform = head;
        trackedLeftHandTransform = left;
        trackedRightHandTransform = right;
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

    public static UIAnchor getAnchorFallback(UIAnchor anchor, AnchoredUI.Priority priority)
    {
        switch (anchor.getType())
        {
            case AnchorType.LEFT_HAND:
                if(mainAnchors[2] != null)
                {
                    return (UIAnchor)mainAnchors[2];
                }
                else
                {
                    if(priority == AnchoredUI.Priority.NONE)
                    {
                        return null;//Evtl überdenken
                    }
                }
                break;
            case AnchorType.RIGHT_HAND:
                if (mainAnchors[1] != null)
                {
                    return (UIAnchor)mainAnchors[1];
                }
                else
                {
                    if (priority == AnchoredUI.Priority.NONE)
                    {
                        return null;//Evtl überdenken
                    }
                }
                break;
        }
        if(mainAnchors[0] != null)
        {
            return (UIAnchor)mainAnchors[0];
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
}
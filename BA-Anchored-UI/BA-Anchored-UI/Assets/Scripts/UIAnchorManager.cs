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
        LEFTHAND,
        RIGHTHAND
    };

    public enum AnchorStyle
    {
        RECTANGLE,
        CYLINDER
    };

    private static ArrayList[] anchors = new ArrayList[3];

    public static void setTrackedTransforms(Transform head, Transform left, Transform right)
    {
        isInitialised = true;
        trackedHeadTransform = head;
        trackedLeftHandTransform = left;
        trackedRightHandTransform = right;
    }

    public static bool addAnchor(UIAnchor anchor)
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

            case AnchorType.LEFTHAND:
                if (anchors[1] == null)
                {
                    anchors[1] = new ArrayList();
                }
                anchors[1].Add(anchor);
                anchor.setAnchorObjectTransform(trackedLeftHandTransform);
                break;

            case AnchorType.RIGHTHAND:
                if (anchors[2] == null)
                {
                    anchors[2] = new ArrayList();
                }
                anchors[2].Add(anchor);
                anchor.setAnchorObjectTransform(trackedRightHandTransform);
                break;
        }
        return true;
    }

    public static UIAnchor getAnchorFallback(UIAnchor anchor, AnchoredUI.Priority priority)
    {
        switch (anchor.getType())
        {
            case AnchorType.LEFTHAND:
                if(anchors[2] != null && anchors[2].Count != 0)
                {
                    return (UIAnchor)anchors[2][0];
                }
                else
                {
                    if(priority == AnchoredUI.Priority.NONE)
                    {
                        return null;//Evtl überdenken
                    }
                }
                break;
            case AnchorType.RIGHTHAND:
                if (anchors[1] != null && anchors[1].Count != 0)
                {
                    return (UIAnchor)anchors[1][0];
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
        if(anchors[0] != null && anchors[1].Count != 0)
        {
            return (UIAnchor)anchors[0][0];
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
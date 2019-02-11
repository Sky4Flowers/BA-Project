using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AnchoredUI : MonoBehaviour
{

    /// <summary>
    /// High:       Should always be shown
    /// - Medium:   Should always be available
    /// - Low:      Doesn't have to always be available
    /// - None:     Could be removed
    /// </summary>
    public enum Priority
    {
        NONE,
        LOW,
        MEDIUM,
        HIGH
    };

    public enum PositioningType
    {
        KEEP_POSITION,
        USE_UI_INDEX,
        USE_PRIORITY
    }

    private UIAnchor anchor;
    public Priority priority;
    public PositioningType type;
    [Tooltip("Negative values will be ignored")]
    public int UIPositionID = -1;
    public bool isInteractableUI = false;
    [Tooltip("Should this object be deformed according to the anchor style")]
    public bool shouldBeDeformed = false;
    public bool shouldMoveInFieldOfView = false;
    [SerializeField]
    private bool isFallbackElement = false;
    [SerializeField]
    private bool isInContainerElement = false;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setAnchor(UIAnchor anchor)
    {
        this.anchor = anchor;
        if (anchor.getType() == UIAnchorManager.AnchorType.HEAD && shouldBeDeformed) //TODO Typ abfragen
        {
            try
            {
                createCurvedMesh();
                setLocal3DPosition(Vector3.zero);
            }
            catch (Exception e)
            {
                shouldBeDeformed = false;
                Debug.Log("Changed shouldBeDeformed on " + gameObject.name + " to false. Caused by: " + e);
            }
        }
    }

    public void moveToFallbackAnchor(UIAnchor anchor)
    {
        switch (type)
        {
            case PositioningType.KEEP_POSITION:
                //Expand other canvas
                break;
            case PositioningType.USE_PRIORITY:
                if (priority == Priority.NONE && anchor.getType() == UIAnchorManager.AnchorType.HEAD)
                {
                    //Disable
                }
                else
                {
                    anchor.addElement(this);
                }
                break;
            case PositioningType.USE_UI_INDEX:
                anchor.activateElementWithID(UIPositionID);
                gameObject.SetActive(false);
                break;
        }
    }

    /*public void searchAnchorFallback(UIAnchor anchor)                             //Removed: Should be moved to anchor
    {
        this.anchor = UIAnchorManager.getAnchorFallback(anchor, priority);
        if(this.anchor == null)
        {
            gameObject.SetActive(false);
        }
    }*/

    public void createCurvedMesh()
    {
        if (!shouldBeDeformed)
        {
            return;
        }

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        //Calculate rangepoints
        RectTransform rect = ((RectTransform)transform);
        float lowerX = anchor.convertRectXToCylinderX(rect.anchoredPosition.x - rect.sizeDelta.x / 2);
        float higherX = anchor.convertRectXToCylinderX(rect.anchoredPosition.x + rect.sizeDelta.x / 2);

        //Project vertices on cylinder
        for (int i = 0; i < vertices.Length; i++)
        {
            float circlePos = lowerX + vertices[i].x * (higherX - lowerX);
            vertices[i].x = Mathf.Sin(circlePos) * anchor.getDistanceFromObject();
            vertices[i].y = (rect.anchoredPosition.y - rect.sizeDelta.y / 2) + vertices[i].y * rect.sizeDelta.y;
            vertices[i].z = Mathf.Cos(circlePos) * anchor.getDistanceFromObject();
        }
        mesh.vertices = vertices;
    }

    public void createCurvedText()
    {
        if (!shouldBeDeformed)
        {
            return;
        }

        //TODO
    }

    public void resize(Vector2 oldParentSize, Vector2 newParentSize)
    {
        RectTransform rectTransform = ((RectTransform)transform);
        float newHeight = rectTransform.rect.height * (newParentSize.y / oldParentSize.y);
        float newWidth = rectTransform.rect.width * (newParentSize.x / oldParentSize.x);
        ((RectTransform)transform).sizeDelta.Set(newWidth, newHeight);
    }

    public void setLocal3DPosition(Vector3 localPosition)
    {
        RectTransform rectTransform = ((RectTransform)transform);
        if (anchor.isUsedAsAnchor() && !isInContainerElement)
        {
            rectTransform.localPosition = localPosition;
        }
        else
        {
            Vector3 currentLocalXOffset = transform.parent.transform.localPosition;
            rectTransform.localPosition = localPosition - currentLocalXOffset;
        }
    }

    public bool isFallback()
    {
        return isFallbackElement;
    }

    public bool isInteractable()
    {
        return isInteractableUI;
    }
}
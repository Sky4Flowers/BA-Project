﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnchor : MonoBehaviour, UIContainer
{
    #region variables
    public AnchoredUI.Priority minPriority;
    private AnchoredUI[] elements;
    [SerializeField]
    private UIAnchor childAnchor;
    private UIContainer[] subContainers;
    [SerializeField]
    private bool isStaticToObject = true;
    [SerializeField]
    [Tooltip("Only used when not static to object")]
    private bool rotatesWithObject = true;
    [Tooltip("Only used when not static to object.\nFor delayed rotation")]
    public float rotationDelayOffset = 0;
    private bool isUsed = false;
    private Vector3 rotateDirection;
    private Transform anchorObjectTransform;
    [SerializeField]
    [Tooltip("Distance to anchor object")]
    private float distance;
    [SerializeField]
    [Tooltip("Height Factor")]
    private float height = 1;
    [SerializeField]
    [Tooltip("Width Faktor")]
    private float width = 1;
    [SerializeField]
    [Tooltip("Rotation relative to parent")]
    private Vector3 relativeRotation;
    private Vector3 lastRotationState = Vector3.zero;
    private Vector3 rotationOffsetBuffer = Vector3.zero;
    [SerializeField]
    private UIAnchorManager.AnchorType type;
    [SerializeField]
    private UIAnchorManager.AnchorStyle style;
    [SerializeField]
    private UIAnchorManager.AnchorExpansionType expType;

    private float oldObjectXRotation = 0;
    #endregion

    // Use this for initialization
    void Start()
    {
        if (type == UIAnchorManager.AnchorType.HEAD)
        {
            if (isStaticToObject)
            {
                height = height * distance * 6 / 5;
                width = width * distance * 6 / 5;
            }
            distance *= -1;
        }

        elements = GetComponentsInChildren<AnchoredUI>();
        subContainers = GetComponentsInChildren<UIContainer>();

        foreach (AnchoredUI element in elements)
        {
            element.setAnchor(this);
            if (element.isFallback())
            {
                element.gameObject.SetActive(false);
            }
        }

        setupCylinderElements();
    }

    // Update is called once per frame
    void Update()
    {
        if (isUsed)
        {
            if (!isStaticToObject)
            {
                move();
                rotate();
            }
            else
            {
                moveHPHElements();
            }
        }
    }

    public void setupCylinderElements()
    {
        if (style == UIAnchorManager.AnchorStyle.CYLINDER)
        {
            foreach (AnchoredUI element in elements)
            {
                if (!element.shouldBeDeformed)
                {
                    RectTransform elementTrans = (RectTransform)element.transform;
                    element.setLocal3DPosition(new Vector3(Mathf.Sin(convertRectXToCylinderX(elementTrans.anchoredPosition.x)) * distance, elementTrans.anchoredPosition.y, Mathf.Cos(convertRectXToCylinderX(elementTrans.anchoredPosition.x)) * distance));
                    elementTrans.LookAt(new Vector3(transform.position.x, elementTrans.position.y, transform.position.z));
                }
            }
        }
        /*foreach (UIContainer container in subContainers)
        {
            container.setupCylinderElements();
        }*/
    }

    private void move()
    {
        transform.position = anchorObjectTransform.position + transform.forward * distance;
    }

    private void rotate()
    {
        if (style == UIAnchorManager.AnchorStyle.CYLINDER && type != UIAnchorManager.AnchorType.HEAD)
        {
            foreach (AnchoredUI element in elements)
            {
                element.transform.LookAt(UIAnchorManager.getHeadPosition());
            }
        }
        else
        {
            if (type == UIAnchorManager.AnchorType.HEAD && rotatesWithObject)
            {
                if (anchorObjectTransform.rotation.eulerAngles.Equals(lastRotationState))
                {
                    return;
                }
                rotationOffsetBuffer.x += anchorObjectTransform.rotation.eulerAngles.x - lastRotationState.x;
                rotationOffsetBuffer.y += anchorObjectTransform.rotation.eulerAngles.y - lastRotationState.y;
                rotationOffsetBuffer.z = anchorObjectTransform.rotation.eulerAngles.z - lastRotationState.z;
                lastRotationState = anchorObjectTransform.rotation.eulerAngles;

                rotationOffsetBuffer = handleEndsOfAngles(rotationOffsetBuffer);

                float rotationX = 0;
                float rotationY = 0;

                if (rotationOffsetBuffer.x > rotationDelayOffset)
                {
                    rotationX = rotationOffsetBuffer.x - rotationDelayOffset;
                    rotationOffsetBuffer.x = rotationDelayOffset;
                }
                else if (rotationOffsetBuffer.x < -rotationDelayOffset)
                {
                    rotationX = rotationOffsetBuffer.x + rotationDelayOffset;
                    rotationOffsetBuffer.x = -rotationDelayOffset;
                }

                if (rotationOffsetBuffer.y > rotationDelayOffset)
                {
                    rotationY = rotationOffsetBuffer.y - rotationDelayOffset;
                    rotationOffsetBuffer.y = rotationDelayOffset;
                }
                else if (rotationOffsetBuffer.y < -rotationDelayOffset)
                {
                    rotationY = rotationOffsetBuffer.y + rotationDelayOffset;
                    rotationOffsetBuffer.y = -rotationDelayOffset;
                }
                transform.RotateAround(anchorObjectTransform.position, transform.right, rotationX);
                transform.RotateAround(anchorObjectTransform.position, transform.up, rotationY);
                //transform.RotateAround(anchorObjectTransform.position, transform.forward, rotationOffsetBuffer.z);
            }
            else
            {
                transform.LookAt(UIAnchorManager.getHeadPosition());
            }
        }
    }

    private Vector3 handleEndsOfAngles(Vector3 rotationOffsetBuffer)
    {
        if (rotationOffsetBuffer.x > 300)
        {
            rotationOffsetBuffer.x -= 360;
        }
        else if (rotationOffsetBuffer.x < -300)
        {
            rotationOffsetBuffer.x += 360;
        }

        if (rotationOffsetBuffer.y > 300)
        {
            rotationOffsetBuffer.y -= 360;
        }
        else if (rotationOffsetBuffer.y < -300)
        {
            rotationOffsetBuffer.y += 360;
        }

        if (rotationOffsetBuffer.z > 300)
        {
            rotationOffsetBuffer.z -= 360;
        }
        else if (rotationOffsetBuffer.z < -300)
        {
            rotationOffsetBuffer.z += 360;
        }

        return rotationOffsetBuffer;
    }

    private void moveHPHElements()
    {
        if (type == UIAnchorManager.AnchorType.HEAD && isStaticToObject)
        {
            float newXRotation = anchorObjectTransform.rotation.eulerAngles.x;
            if (newXRotation < oldObjectXRotation)
            {
                oldObjectXRotation = newXRotation;
            }
            else if (newXRotation > oldObjectXRotation + 3)
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    if (elements[i].priority > AnchoredUI.Priority.LOW)
                    {
                        //elements[i].transform. // ins Bild verschieben
                        if (elements[i].shouldMoveInFieldOfView && elements[i].isInteractableUI)
                        {
                            //Move in view
                        }
                    }
                }
            }
        }
    }

    public void setAnchorObjectTransform(Transform anchorPosition)
    {
        if (anchorPosition == null)
        {
            isUsed = false;
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i].moveToFallbackAnchor(UIAnchorManager.getAnchorFallback(this));
            }
        }
        else
        {
            isUsed = true;
            transform.position = anchorPosition.position;
            if (isStaticToObject)
            {
                transform.SetParent(anchorPosition);
                transform.localScale = new Vector3(width / 100, height / 100, width / 100);
                transform.rotation = Quaternion.Euler(anchorPosition.rotation.eulerAngles + relativeRotation);
                if (style == UIAnchorManager.AnchorStyle.RECTANGLE)
                {
                    transform.Translate(new Vector3(0, 0, -distance), Space.Self);
                }
            }
            else
            {
                if (rotatesWithObject)
                {
                    lastRotationState = anchorPosition.rotation.eulerAngles;
                }
            }
            anchorObjectTransform = anchorPosition;
        }
        if (childAnchor)
        {
            childAnchor.setAnchorObjectTransform(anchorPosition);
        }
    }

    public void setDirection(Vector3 direction)
    {
        rotateDirection = direction;
    }

    public UIAnchorManager.AnchorType getType()
    {
        return type;
    }

    public void addElement(AnchoredUI element)
    {
        //Add element to elements
        //For testing UI is placed in the editor
        //Check priority behaviour!
    }

    public void addContainer(UIContainer container)
    {
        //Add container to elements
        //For testing UI is placed in the editor
        //Check priority behaviour!
    }

    public bool expand(UIAnchor anchor)
    {
        if (anchor.minPriority >= minPriority)
        {
            switch (expType)
            {
                case UIAnchorManager.AnchorExpansionType.SWITCH:
                    //Show switch
                    System.Array.Copy(subContainers, subContainers, subContainers.Length);
                    subContainers[subContainers.Length - 1] = anchor;
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = Vector3.zero;
                    anchor.transform.localRotation.Set(0, 0, 0, 0);
                    break;
                case UIAnchorManager.AnchorExpansionType.DIRECTION_TOP:
                    //Expand...
                    //height + new window height
                    //move old elements (new window height / 2) down
                    break;
                case UIAnchorManager.AnchorExpansionType.DIRECTION_LEFT:
                    //width + new window
                    //move old elements (new window width / 2) right
                    break;
                case UIAnchorManager.AnchorExpansionType.DIRECTION_BOTTOM:
                    //height + new window
                    //move old elements (new window height / 2) up
                    break;
                case UIAnchorManager.AnchorExpansionType.DIRECTION_RIGHT:
                    //width + new window
                    //move old elements (new window width / 2) left
                    break;
            }
            return true;
        }
        else
        {
            if (childAnchor && childAnchor.expand(anchor))
            {
                return true;
            }
            return false;
        }
    }

    public float convertRectXToCylinderX(float xPos)
    {
        float canvasWidth = ((RectTransform)transform).rect.width;
        return (xPos + canvasWidth / 2) / canvasWidth * 2 * Mathf.PI;
    }

    public bool activateElementWithID(int ID)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].UIPositionID == ID)
            {
                elements[i].gameObject.SetActive(true);
                return true;
            }
        }

        for (int i = 0; i < subContainers.Length; i++)
        {
            if (!subContainers[i].Equals(this) && subContainers[i].activateElementWithID(ID))
            {
                return true;
            }
        }
        if (childAnchor && childAnchor.activateElementWithID(ID))
        {
            return true;
        }
        return false;
    }

    public void resize(float newX, float newY)
    {
        /*RectTransform rectTransform = ((RectTransform)transform);
        float oldWidth = rectTransform.rect.width;
        float oldHeight = rectTransform.rect.height;
        rectTransform.sizeDelta = new Vector2(newX, newY);
        for(int i = 0; i < elements.Length; i++)
        {
            //Positionierung
            elements[i].resize(new Vector2(oldWidth, oldHeight), new Vector2(newX, newY)); //Nochmal überdenken
        }*/
        transform.localScale = new Vector3(newX, newY, newX);
    }

    public Vector2 getRelativeSize()
    {
        //TODO
        return Vector2.zero;
    }

    public float getDistanceFromObject()
    {
        return distance;
    }

    public Vector3 getAnchorObjectPosition()
    {
        return anchorObjectTransform.position;
    }

    public bool isUsedAsAnchor()
    {
        return isUsed;
    }

    /*IEnumerable addAnchorQueue(int tryCounter)
    {
        while (!UIAnchorManager.addAnchor(this))
        {
            tryCounter++;
            yield return new WaitForSeconds(0.1f);
            if (tryCounter > 10)
            {
                break;
            }
        }
    }*/
}
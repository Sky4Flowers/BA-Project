﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnchor : MonoBehaviour, UIContainer
{
    #region variables
    public bool isActiveAtStart = true;
    public AnchoredUI.Priority minPriority;
    private List<AnchoredUI> elements;
    [SerializeField]
    private UIAnchor childAnchor;
    private List<UIContainer> subContainers;
    [SerializeField]
    private bool isStaticToObject = true;
    [SerializeField]
    [Tooltip("Only used when not static to object")]
    private bool rotatesWithObject = true;
    [Tooltip("Only used when not static to object.\nFor delayed rotation")]
    public float rotationDelayOffset = 0;
    private bool isUsed = false;
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
    private Quaternion lastRotationStateQuat = new Quaternion(0, 0, 0, 1);
    private Vector3 rotationOffsetBuffer = Vector3.zero;
    [SerializeField]
    private UIAnchorManager.AnchorType type;
    [SerializeField]
    private UIAnchorManager.AnchorStyle style;
    [SerializeField]
    private UIAnchorManager.AnchorExpansionType expType;
    public UISwitch switchElement;

    private float oldObjectXRotation = 0;
    #endregion

    void Awake()
    {
        elements = new List<AnchoredUI>(GetComponentsInChildren<AnchoredUI>());
        subContainers = new List<UIContainer>(GetComponentsInChildren<UIContainer>());

        if (type == UIAnchorManager.AnchorType.HEAD)
        {
            if (isStaticToObject)
            {
                height = height * distance * 6 / 5;
                width = width * distance * 6 / 5;
                distance *= -1;
            }
        }

        foreach (AnchoredUI element in elements)
        {
            element.setAnchor(this);
            if (element.isFallback())
            {
                element.gameObject.SetActive(false);
            }
        }
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
            move();
            if (elements == null)
            {
                Debug.Log(gameObject.name + " no elements found");
                return;
            }
            foreach (AnchoredUI element in elements)
            {
                if (!element.shouldBeDeformed)
                {
                    RectTransform elementTrans = (RectTransform)element.transform;
                    element.setLocal3DPosition(new Vector3(
                        Mathf.Sin(convertRectXToCylinderX(elementTrans.anchoredPosition.x)) * distance * 100,
                        elementTrans.anchoredPosition.y,
                        Mathf.Cos(convertRectXToCylinderX(elementTrans.anchoredPosition.x)) * distance * 100
                        ) - transform.forward * distance * 100
                    );
                    Vector3 headPos = UIAnchorManager.getHeadPosition();
                    elementTrans.LookAt(new Vector3(headPos.x, elementTrans.position.y, headPos.z));
                }
            }
        }
        if (subContainers == null)
        {
            Debug.Log(gameObject.name + " no subcontainers found");
            return;
        }
        foreach (UIContainer container in subContainers)
        {
            if (!container.isAnchor())
            {
                container.setupCylinderElements();
            }
        }
    }

    private void move()
    {
        transform.position = anchorObjectTransform.position + transform.forward * distance;
    }

    private void rotate()
    {
        if (style == UIAnchorManager.AnchorStyle.CYLINDER)
        {
            if (type != UIAnchorManager.AnchorType.HEAD)
            {
                foreach (AnchoredUI element in elements)
                {
                    element.transform.LookAt(UIAnchorManager.getHeadPosition());
                }
            }
        }
        else
        {
            if (type == UIAnchorManager.AnchorType.HEAD && rotatesWithObject)
            {
                if (anchorObjectTransform.rotation.Equals(lastRotationStateQuat))
                {
                    return;
                }

                Vector3 deltaRotation = calculateEulerRotation(lastRotationStateQuat, anchorObjectTransform.rotation);

                rotationOffsetBuffer.x += deltaRotation.x;
                rotationOffsetBuffer.y += deltaRotation.y;
                rotationOffsetBuffer.z = deltaRotation.z;

                lastRotationStateQuat = anchorObjectTransform.rotation;

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
                transform.RotateAround(anchorObjectTransform.position, Vector3.up, rotationY);
            }
            else
            {
                transform.LookAt(UIAnchorManager.getHeadPosition());
            }
        }
    }

    private Vector3 calculateEulerRotation(Quaternion from, Quaternion to)
    {
        float x = to.x;
        float y = to.y;
        float z = to.z;
        float w = to.w;
        float yFactor = Mathf.Atan2(2 * y * w - 2 * x * z, 1 - 2 * y * y - 2 * z * z);
        float xFactor = Mathf.Atan2(2 * x * w - 2 * y * z, 1 - 2 * x * x - 2 * z * z);
        float zFactor = Mathf.Asin(2 * x * y + 2 * z * w);

        x = from.x;
        y = from.y;
        z = from.z;
        w = from.w;
        yFactor -= Mathf.Atan2(2 * y * w - 2 * x * z, 1 - 2 * y * y - 2 * z * z);
        xFactor -= Mathf.Atan2(2 * x * w - 2 * y * z, 1 - 2 * x * x - 2 * z * z);
        zFactor -= Mathf.Asin(2 * x * y + 2 * z * w);
        //from: https://answers.unity.com/questions/416169/finding-pitchrollyaw-from-quaternions.html

        return (new Vector3(xFactor, yFactor, zFactor) * 180 / Mathf.PI);
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
        else if (rotationOffsetBuffer.y > 150)
        {
            rotationOffsetBuffer.y -= 180;
        }
        else if (rotationOffsetBuffer.y < -300)
        {
            rotationOffsetBuffer.y += 360;
        }
        else if (rotationOffsetBuffer.y < -150)
        {
            rotationOffsetBuffer.y += 180;
        }

        if (rotationOffsetBuffer.z > 300)
        {
            rotationOffsetBuffer.z -= 360;
        }
        else if (rotationOffsetBuffer.z > 150)
        {
            rotationOffsetBuffer.z -= 180;
        }
        else if (rotationOffsetBuffer.z < -300)
        {
            rotationOffsetBuffer.z += 360;
        }
        else if (rotationOffsetBuffer.z < -150)
        {
            rotationOffsetBuffer.z += 180;
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
                for (int i = 0; i < elements.Count; i++)
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
            bool shouldExpand = false;
            UIAnchor newAnchor = UIAnchorManager.getAnchorFallback(this);
            for (int i = 0; i < elements.Count; i++)
            {
                if (!elements[i].moveToFallbackAnchor(newAnchor)){
                    shouldExpand = true;
                }
            }
            if (shouldExpand)
            {
                newAnchor.expand(this);
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
                transform.localScale = new Vector3(width / 100, height / 100, width / 100);
                if (rotatesWithObject)
                {
                    if (type == UIAnchorManager.AnchorType.HEAD)
                    {
                        transform.rotation = anchorPosition.rotation;
                    }
                    lastRotationState = anchorPosition.rotation.eulerAngles;
                    lastRotationStateQuat = anchorPosition.rotation;
                }
            }
            anchorObjectTransform = anchorPosition;
        }
        if (childAnchor)
        {
            childAnchor.setAnchorObjectTransform(anchorPosition);
        }

        setupCylinderElements();
    }

    public UIAnchorManager.AnchorType getType()
    {
        return type;
    }

    public bool addElement(AnchoredUI element)
    {
        foreach (UIContainer container in subContainers){
            if(!container.Equals(this) && container.addElement(element))
            {
                return true;
            }
        }
        elements.Add(element);
        return true;
    }

    public void addContainer(UIContainer container)
    {
        subContainers.Add(container);
        //TODO set position
    }

    public bool expand(UIAnchor anchor)
    {
        if (anchor.minPriority >= minPriority)
        {
            subContainers.Add(anchor);
            switch (expType)
            {
                case UIAnchorManager.AnchorExpansionType.SWITCH:
                    if (switchElement)
                    {
                        switchElement.gameObject.SetActive(true);
                        switchElement.setSelection(0, anchor.gameObject);
                    }
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = Vector3.zero;
                    anchor.transform.localRotation.Set(0, 0, 0, 0);
                    anchor.gameObject.SetActive(false);
                    break;
                case UIAnchorManager.AnchorExpansionType.DIRECTION_TOP:
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = new Vector3(0, transform.localScale.y / 2f + anchor.transform.localScale.y / 2f, 0);
                    anchor.transform.localRotation.Set(0, 0, 0, 0);
                    break;
                case UIAnchorManager.AnchorExpansionType.DIRECTION_LEFT:
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = new Vector3(-transform.localScale.x / 2f - anchor.transform.localScale.x / 2f, 0, 0);
                    anchor.transform.localRotation.Set(0, 0, 0, 0);
                    break;
                case UIAnchorManager.AnchorExpansionType.DIRECTION_BOTTOM:
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = new Vector3(0, -transform.localScale.y / 2f - anchor.transform.localScale.y / 2f, 0);
                    anchor.transform.localRotation.Set(0, 0, 0, 0);
                    break;
                case UIAnchorManager.AnchorExpansionType.DIRECTION_RIGHT:
                    anchor.transform.parent = transform;
                    anchor.transform.localPosition = new Vector3(transform.localScale.x / 2f + anchor.transform.localScale.x / 2f, 0, 0);
                    anchor.transform.localRotation.Set(0, 0, 0, 0);
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
        if (!isUsed)
        {
            return false;
        }

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].UIPositionID == ID)
            {
                elements[i].gameObject.SetActive(true);
                return true;
            }
        }

        for (int i = 0; i < subContainers.Count; i++)
        {
            if (!subContainers[i].Equals(this) && subContainers[i].activateElementWithID(ID))
            {
                /*if (expType == UIAnchorManager.AnchorExpansionType.SWITCH && subContainers[i].isAnchor())
                {
                    
                }*/
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
        return Vector2.one;
    }

    public bool isAnchor()
    {
        return true;
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
}
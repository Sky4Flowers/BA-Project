using System.Collections;
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
    private bool isUsed = false;
    private Vector3 rotateDirection;
    private Transform anchorObjectTransform;
    [SerializeField]
    [Tooltip("Distance to anchor object")]
    private float distance;
    private float height;
    private float width;
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
        //StartCoroutine("addAnchorQueue", 0);
        elements = GetComponentsInChildren<AnchoredUI>();
        subContainers = GetComponentsInChildren<UIContainer>();
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
        foreach (AnchoredUI element in elements)
        {
            element.setAnchor(this);
            if (style == UIAnchorManager.AnchorStyle.CYLINDER)
            {
                RectTransform elementTrans = (RectTransform)element.transform;
                elementTrans.localPosition = new Vector3(Mathf.Sin(convertRectXToCylinderX(elementTrans.anchoredPosition.x)) * distance, elementTrans.anchoredPosition.y, Mathf.Cos(convertRectXToCylinderX(elementTrans.anchoredPosition.x)) * distance);
                elementTrans.LookAt(new Vector3(transform.position.x, elementTrans.position.y, transform.position.z));
            }
        }
        /*foreach (UIContainer container in subContainers)
        {
            container.setupCylinderElements();
        }*/
    }

    private void move()
    {
        transform.position = anchorObjectTransform.position;
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
            transform.LookAt(UIAnchorManager.getHeadPosition());
        }
    }

    private void moveHPHElements()
    {
        //TODO transfer into method
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
                        if (elements[i].shouldMoveInFieldOfView && elements[i].isActiveUI)
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
            //TODO inform children and call searchAnchorFallback
        }
        else
        {
            isUsed = true;
            transform.position = anchorPosition.position;
            if (isStaticToObject)
            {
                transform.SetParent(anchorPosition);
            }
            transform.localPosition = new Vector3(0, 0, distance);
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
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
                    anchor.transform.localRotation.Set(0,0,0,0);
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
            if (subContainers[i].activateElementWithID(ID))
            {
                return true;
            }
        }
        return false;
    }

    public void resize(float newX, float newY)
    {
        RectTransform rectTransform = ((RectTransform)transform);
        float oldWidth = rectTransform.rect.width;
        float oldHeight = rectTransform.rect.height;
        rectTransform.sizeDelta = new Vector2(newX, newY);
        for(int i = 0; i < elements.Length; i++)
        {
            //Positionierung
            elements[i].resize(new Vector2(oldWidth, oldHeight), new Vector2(newX, newY)); //Nochmal überdenken
        }
    }

    public Vector2 getRelativeSize()
    {
        //TODO
        return Vector2.zero;
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
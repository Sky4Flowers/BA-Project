using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnchor : MonoBehaviour
{
    public AnchoredUI.Priority minPriority;
    private AnchoredUI[] elements;
    private List<UIAnchor> childAnchors = new List<UIAnchor>();
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

    // Use this for initialization
    void Start()
    {
        StartCoroutine("addAnchorQueue", 0);
        elements = GetComponentsInChildren<AnchoredUI>();
        setupElements();
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
            //TODO transfer into method
            if(type == UIAnchorManager.AnchorType.HEAD && isStaticToObject)
            {
                float newXRotation = anchorObjectTransform.rotation.eulerAngles.x;
                if(newXRotation < oldObjectXRotation)
                {
                    oldObjectXRotation = newXRotation;
                }else if(newXRotation > oldObjectXRotation + 3){
                    for (int i = 0; i < elements.Length; i++)
                    {
                        if (elements[i].priority > AnchoredUI.Priority.LOW)
                        {
                            //elements[i].transform. // ins Bild verschieben
                            if(elements[i].shouldMoveInFieldOfView && elements[i].isActiveUI)
                            {
                                //Move in view
                            }
                        }
                    }
                }
            }
        }
    }

    private void setupElements()
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
            anchorObjectTransform = anchorPosition;
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

    public bool expand(UIAnchor anchor)
    {
        if (anchor.minPriority >= minPriority)
        {
            switch (expType)
            {
                case UIAnchorManager.AnchorExpansionType.SWITCH:
                    //Show switch
                    //Move elements to this anchor 
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
            for (int i = 0; i < childAnchors.Count; i++)
            {
                if (childAnchors[i].expand(anchor))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public float convertRectXToCylinderX(float xPos)
    {
        float canvasWidth = ((RectTransform)transform).rect.width;
        return (xPos + canvasWidth / 2) / canvasWidth * 2 * Mathf.PI;
    }

    public bool activateUIWithID(int ID)
    {
        for (int i = 0; i < elements.Length; i++)
        {
            if (elements[i].UIPositionID == ID)
            {
                elements[i].gameObject.SetActive(true);
                return true;
            }
        }

        for (int i = 0; i < childAnchors.Count; i++)
        {
            if (childAnchors[i].activateUIWithID(ID))
            {
                return true;
            }
        }
        return false;
    }

    IEnumerable addAnchorQueue(int tryCounter)
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
    }
}
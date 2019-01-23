using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnchor : MonoBehaviour
{

    public AnchoredUI.Priority minPriority;
    private AnchoredUI[] elements;
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
        }
        else
        {
            isUsed = true;
            transform.position = anchorPosition.position;
            if (!isStaticToObject)
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

    public float convertRectXToCylinderX(float xPos)
    {
        float canvasWidth = ((RectTransform)transform).rect.width;
        return (xPos + canvasWidth / 2) / canvasWidth * 2 * Mathf.PI;
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
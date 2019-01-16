using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnchor : MonoBehaviour {

    private AnchoredUI[] elements;
    [SerializeField]
    private bool shouldRotate = false;
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
    void Start() {
        StartCoroutine("addAnchorQueue", 0);
        elements = GetComponentsInChildren<AnchoredUI>();
    }

    // Update is called once per frame
    void Update() {
        if (isUsed)
        {
            move();
        }
    }

    private void move()
    {
        if (shouldRotate)
        {
            transform.position = anchorObjectTransform.position;
        }
    }

    public void setAnchorObjectTransform(Transform anchorPosition)
    {
        if(anchorPosition == null)
        {
            isUsed = false;
        }
        else
        {
            isUsed = true;
            if (shouldRotate)
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
    }

    IEnumerable addAnchorQueue(int tryCounter)
    {
        while (!UIAnchorManager.addAnchor(this))
        {
            tryCounter++;
            yield return new WaitForSeconds(0.1f);
            if(tryCounter > 10)
            {
                break;
            }
        }
    }
}
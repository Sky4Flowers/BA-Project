using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableUI : MonoBehaviour, ISelectable
{
    public GameObject content;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void select()
    {
        if(InputComponent.selectedObject == null)
        {
            if(content != null)
            {
                content = InputComponent.setSelectedObject(content);
            }
        }
        else
        {
            if(content == null)
            {
                content = InputComponent.setSelectedObject(null);
                content.transform.SetParent(transform);
                content.transform.localPosition = Vector3.zero;
                content.transform.localScale = Vector3.one * 10;
            }
            else
            {
                content = InputComponent.setSelectedObject(content);
                content.transform.parent = transform;
                content.transform.position = Vector3.zero;
            }
        }
    }
}
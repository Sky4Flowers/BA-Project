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
                InputComponent.setSelectedObject(content);
                content.SetActive(false);
            }
        }
        else
        {
            if(content == null)
            {
                content = Instantiate(InputComponent.selectedObject);
                content.transform.parent = transform;
                content.transform.position = Vector3.zero;
                content.transform.localScale = Vector3.one * 10;
            }
            else
            {
                /*GameObject obj = content;
                content = Instantiate(InputComponent.selectedObject);
                content.transform.parent = transform;
                content.transform.position = Vector3.zero;

                InputComponent.setSelectedObject(obj);
                obj.SetActive(false);*/
            }
        }
    }
}
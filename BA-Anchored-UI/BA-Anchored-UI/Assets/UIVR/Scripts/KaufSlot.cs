using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaufSlot : MonoBehaviour, ISelectable{

    public GameObject content;
    // Use this for initialization
    void Start () {
        content = Instantiate(content);
        content.transform.SetParent(transform);
        content.transform.localPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void select()
    {
        InputComponent.setSelectedObject(content);
    }
}

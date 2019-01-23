using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AnchoredUI : MonoBehaviour {

    /// <summary>
    /// High:       Should always be shown
    /// - Medium:   Should always be available
    /// - Low:      Doesn't have to always be available
    /// - None:     Could be removed
    /// </summary>
    public enum Priority{
        HIGH,
        MEDIUM,
        LOW,
        NONE
    };

    private UIAnchor anchor;
    public Priority priority = Priority.HIGH;
    public bool isActiveUI = false;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void setAnchor(UIAnchor anchor)
    {
        this.anchor = anchor;
        if(anchor.getType() == UIAnchorManager.AnchorType.HEAD) //TODO Typ abfragen
        {
            try
            {
                createCurvedMesh();
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            
        }
    }

    public void searchAnchorFallback(UIAnchor anchor)
    {
        this.anchor = UIAnchorManager.getAnchorFallback(anchor, priority);
        if(this.anchor == null)
        {
            gameObject.SetActive(false);
        }
    }

    public void createCurvedMesh()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        
        //Calculate rangepoints
        RectTransform rect = ((RectTransform)transform);
        float lowerX = anchor.convertRectXToCylinderX(rect.anchoredPosition.x - rect.sizeDelta.x);
        float higherX = anchor.convertRectXToCylinderX(rect.anchoredPosition.x + rect.sizeDelta.x);

        //Project vertices on cylinder
        for(int i = 0; i < vertices.Length; i++)
        {
            //Debug.Log("Old" + vertices[i] * 100);
            float circlePos = lowerX + vertices[i].x * 10 * (higherX - lowerX);
            vertices[i].x = Mathf.Sin(circlePos);
            vertices[i].z = Mathf.Cos(circlePos);
        }
        mesh.vertices = vertices;
    }

    public void createCurvedText()
    {
        //TODO
    }
}
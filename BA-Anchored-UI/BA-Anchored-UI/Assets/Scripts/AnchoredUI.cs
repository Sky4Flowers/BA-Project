using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public void setAnchor(UIAnchor anchor)
    {
        this.anchor = anchor;
    }

    public void searchAnchorFallback(UIAnchor anchor)
    {
        this.anchor = UIAnchorManager.getAnchorFallback(anchor, priority);
        if(this.anchor == null)
        {
            gameObject.SetActive(false);
        }
    }
}
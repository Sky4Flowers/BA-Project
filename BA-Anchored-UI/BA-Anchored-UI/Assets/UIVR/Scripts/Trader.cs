using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour, ISelectable {

    public AnchoredUI[] tradeItems = new AnchoredUI[0];

	// Use this for initialization
	void Start () {
        unselect();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void select()
    {
        for(int i= 0; i < tradeItems.Length; i++)
        {
            tradeItems[i].gameObject.SetActive(true);
        }
    }

    public void unselect()
    {
        for (int i = 0; i < tradeItems.Length; i++)
        {
            tradeItems[i].gameObject.SetActive(false);
        }
    }
}

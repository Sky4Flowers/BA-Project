﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitch : MonoBehaviour, ISelectable {

    public GameObject selection1;
    public GameObject selection2;

    private int selectedIndex = 0;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void select()
    {
        if (selectedIndex == 0)
        {
            selectedIndex = 1;
            selection1.SetActive(false);
            selection2.SetActive(true);
        }
        else
        {
            selectedIndex = 0;
            selection1.SetActive(true);
            selection2.SetActive(false);
        }
    }

    public void deactivateAll()
    {
        selection1.SetActive(false);
        selection2.SetActive(false);
    }

    public void reactivateSelected()
    {
        if(selectedIndex == 0)
        {
            selection1.SetActive(true);
            selection2.SetActive(false);
        }
        else
        {
            selection1.SetActive(false);
            selection2.SetActive(true);
        }
    }
}
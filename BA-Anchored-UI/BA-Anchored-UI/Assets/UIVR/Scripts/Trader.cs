using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trader : MonoBehaviour, ISelectable {

    public AnchoredUI[] tradeItems = new AnchoredUI[0];
    public AnchoredUI vermoegenL;
    public AnchoredUI vermoegenR;
    public InputComponent input;

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

        if (input.leftHandIsShown)
        {
            vermoegenL.gameObject.SetActive(true);
            vermoegenR.gameObject.SetActive(false);
        }
        else
        {
            if (input.rightHandIsShown)
            {
                vermoegenL.gameObject.SetActive(false);
                vermoegenR.gameObject.SetActive(true);
            }
        }
    }

    public void unselect()
    {
        for (int i = 0; i < tradeItems.Length; i++)
        {
            tradeItems[i].gameObject.SetActive(false);
        }
        if (input.leftHandIsShown)
        {
            vermoegenL.gameObject.SetActive(false);
            vermoegenR.gameObject.SetActive(true);
        }
        else
        {
            if (input.rightHandIsShown)
            {
                vermoegenL.gameObject.SetActive(true);
                vermoegenR.gameObject.SetActive(false);
            }
        }
    }
}

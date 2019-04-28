using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnlyShowTextOnHover : MonoBehaviour
{
	public Text TextToHide;
	
    void Start()
    {
		//TextToHide.gameObject.SetActive(false);
	}

    public void MouseOverEnter(BaseEventData data)
    {
	    //TextToHide.gameObject.SetActive(true);
    }

    public void MouseOverExit(BaseEventData data)
    {
		//TextToHide.gameObject.SetActive(false);
	}

	void Update()
    {
		

	}
}

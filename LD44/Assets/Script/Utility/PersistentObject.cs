using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentObject : MonoBehaviour
{
	private bool _originalInstance = false; 

	private void Awake()
	{
		//If a gameobject with this name exists already, destroy myself.
		if (!_originalInstance && GameObject.Find(this.name) != this.gameObject)
		{
			Destroy(this.gameObject);
			return;
		}

		_originalInstance = true;
		DontDestroyOnLoad(this.gameObject);
	}
}

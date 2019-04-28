using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentObject : MonoBehaviour
{
	private bool _originalInstance = false;

	private void Update()
	{
		var sceneName = SceneManager.GetActiveScene().name;
		if (sceneName == "MenuScene" || (!_originalInstance && GameObject.Find(this.name) != this.gameObject))
		{
			Destroy(this.gameObject);
			return;
		}
	}

	private void Awake()
	{
		//If a gameobject with this name exists already, destroy myself.
		if ((!_originalInstance && GameObject.Find(this.name) != this.gameObject))
		{
			Destroy(this.gameObject);
			return;
		}

		_originalInstance = true;
		DontDestroyOnLoad(this.gameObject);
	}
}

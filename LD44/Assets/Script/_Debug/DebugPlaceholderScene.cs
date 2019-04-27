using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugPlaceholderScene : MonoBehaviour
{
	public string NextSceneName;

	private float _switchTimer;

    void Start()
    {
        
    }

    
    void Update()
    {
	    _switchTimer += Time.deltaTime;

	    if (_switchTimer > 2.0f)
	    {
		    SceneManager.LoadScene(NextSceneName);
		}

    }
}

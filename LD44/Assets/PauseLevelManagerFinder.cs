using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseLevelManagerFinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PauseButtonPressed()
    {
		GameObject.FindObjectOfType<LevelManager>()?.PauseUnpause();
    }

    public void ExitButtonPressed()
    {
	    Time.timeScale = 1;

		SceneManager.LoadScene("MenuScene");
	}
}

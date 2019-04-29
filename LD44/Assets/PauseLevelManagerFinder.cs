using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseLevelManagerFinder : MonoBehaviour
{
    public void PauseButtonPressed()
    {
		FindObjectOfType<LevelManager>()?.Unpause();
    }

    public void ExitButtonPressed()
    {
	    Time.timeScale = 1;

		SceneManager.LoadScene("MenuScene");
	}
}

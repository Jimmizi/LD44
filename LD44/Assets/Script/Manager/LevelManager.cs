using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameObject = UnityEngine.GameObject;

public class LevelManager : MonoBehaviour
{
    public GameObject mainCanvas;
    public GameObject gameOverCanvas;
    public GameObject pauseMenuCanvas;

	public AudioClip MusicToPlay;

    public bool paused;
    private float _timerBetweenNullChecks;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
	    if (mainCanvas == null)
	    {
		    mainCanvas = GameObject.Find("MainCanvas_NEW");
	    }

	    if (gameOverCanvas == null)
	    {
		    gameOverCanvas = GameObject.Find("GameOverCanvas");
	    }

	    if (pauseMenuCanvas == null)
	    {
		    pauseMenuCanvas = GameObject.Find("PauseMenuCanvas");
	    }

		SetupHUD();
    }

    void Update()
    {
	    _timerBetweenNullChecks += Time.deltaTime;

	    if (_timerBetweenNullChecks < 1.0f)
	    {
		    return;
	    }

	    _timerBetweenNullChecks = 0.0f;

		if (mainCanvas == null)
	    {
		    mainCanvas = GameObject.Find("MainCanvas_NEW");
	    }

	    if (gameOverCanvas == null)
	    {
		    gameOverCanvas = GameObject.FindWithTag("GameOverCanvas");
	    }

	    if (pauseMenuCanvas == null)
	    {
		    pauseMenuCanvas = GameObject.FindWithTag("PauseMenuCanvas");
	    }
	}

    void SetupHUD()
    {
	    mainCanvas?.SetActive(true);
	    gameOverCanvas?.SetActive(false);
	    pauseMenuCanvas?.SetActive(false);
	}

	void Start()
    {
        GameManager.levelManager = this;
		
        SceneManager.sceneLoaded += OnSceneLoaded;
        SetupHUD();
    }

    public void GameOver()
    {
        mainCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);

        gameOverCanvas.transform.GetChild(0).gameObject.SetActive(true);

		//Time.timeScale = 0;
	}

    public void Pause()
    {
	    if (mainCanvas == null)
	    {
		    return;
	    }
        mainCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(true);

        pauseMenuCanvas.transform.GetChild(0).gameObject.SetActive(true);


		Time.timeScale = 0;

        paused = true;
    }

    public void Unpause()
    {
        mainCanvas.SetActive(true);
        pauseMenuCanvas.SetActive(false);

        Time.timeScale = 1;

        paused = false;
    }

    public void PauseUnpause()
    {
        if (paused)
            Unpause();
        else
            Pause();
    }
}
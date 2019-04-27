using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameObject mainCanvas;
    private GameObject gameOverCanvas;
    private GameObject pauseMenuCanvas;

    public bool paused;

    void Start()
    {
        GameManager.levelManager = this;

        mainCanvas = GameObject.FindWithTag("MainUICanvas");
        gameOverCanvas = GameObject.FindWithTag("GameOverCanvas");
        pauseMenuCanvas = GameObject.FindWithTag("PauseMenuCanvas");

        mainCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(false);
    }

    public void GameOver()
    {
        mainCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);

        Time.timeScale = 0;
    }

    public void Pause()
    {
        mainCanvas.SetActive(false);
        pauseMenuCanvas.SetActive(true);

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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GameObject gameOverCanvas;
    private GameObject mainCanvas;

    void Start()
    {
        GameManager.levelManager = this;

        mainCanvas = GameObject.FindWithTag("MainUICanvas");
        gameOverCanvas = GameObject.FindWithTag("GameOverCanvas");

        mainCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
    }

    public void GameOver()
    {
        mainCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);

        Time.timeScale = 0;
    }
}
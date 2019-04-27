using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/// <summary>
/// Handles round intro/outro and updates during the middle of level gameplay
/// </summary>

public class LevelManager : MonoBehaviour
{
	private const string LEVEL_COMPLETE_SCENE = "BetweenStagesScene";
	public enum LevelState
	{
		Init,		// Make all the entities and get the stage ready
		Update,		// The update during gameplay, checking if we meet succession criteria
		StageOver,	// Displays the result of the stage, any states, nice fade out before we're killing everything
		Shutdown	// Done with the stage, delete everything and go to the next scene
	}

	public List<GameObject> EnemyList = new List<GameObject>();
	private List<GameObject> _currentEnemies = new List<GameObject>();

	// On a range of 1 - 10
	public int Difficulty = 5;

	private LevelState _currentState;
	private SpawnpointFinder _spawnerRef = null;

    
    void Start()
    {
		_spawnerRef = GameObject.FindGameObjectWithTag("Manager_Spawnpoints")?.GetComponent<SpawnpointFinder>();

	}
	
	void Update()
    {
        switch(_currentState)
		{
			case LevelState.Init:
			{
				StateInit();
				break;
			}
			case LevelState.Update:
			{
				StateUpdate();
				break;
			}
			case LevelState.StageOver:
			{
				StateStageOver();
				break;
			}
			case LevelState.Shutdown:
			{
				StateShutdown();
				break;
			}
		}
    }


	void SpawnEnemyIn()
	{
		Vector2 vPosition = _spawnerRef.GetSpawnPoint();

		var tempEnemy = (GameObject)Instantiate(EnemyList[Random.Range(0, EnemyList.Count)], vPosition, new Quaternion());

		_currentEnemies.Add(tempEnemy);
	}

	void StateInit()
	{
		if (EnemyList.Count > 0)
		{
			for (var i = 0; i < Difficulty; i++)
			{
				SpawnEnemyIn();
			}
		}

		_currentState = LevelState.Update;
	}

	void StateUpdate()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.F1))
		{
			Debug.Log("Debug skipping stage.");
			_currentState = LevelState.StageOver;
		}
		
#endif
	}

	void StateStageOver()
	{
		//TODO: Fade out
		_currentState = LevelState.Shutdown;
	}

	void StateShutdown()
	{
		SceneManager.LoadScene(LEVEL_COMPLETE_SCENE);
	}
}

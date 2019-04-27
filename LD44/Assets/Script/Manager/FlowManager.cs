using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Handles round intro/outro and updates during the middle of level gameplay, then the exit to further scenes
/// </summary>

public class FlowManager : MonoBehaviour
{
	private const string LEVEL_COMPLETE_SCENE = "BetweenStagesScene";
	private const float OBJECTIVE_TEXT_ROUND_START_DURATION_TIME = 10.0f;
	private const float OBJECTIVE_TEXT_ROUND_START_FADE_OUT_TIME = 2.0f;


	public enum LevelState
	{
		Init,		// Make all the entities and get the stage ready
		Placement,	// Stage that is active until the player spawns into the world
		PlayerInit,	// We have our placement, start the stage!
		Update,		// The update during gameplay, checking if we meet succession criteria
		StageOver,	// Displays the result of the stage, any states, nice fade out before we're killing everything
		Shutdown	// Done with the stage, delete everything and go to the next scene
	}

	#region Engine facing config

	/// <summary>
	/// List of enemies that can be spawned in for this stage.
	/// </summary>
	public List<GameObject> EnemyList = new List<GameObject>();

	/// <summary>
	/// The prefab for visualisation of where the spawn drop in point will be
	/// </summary>
	public GameObject SpawnInTargetPrefab = null;

	/// <summary>
	/// The current player gameobject, this will also hold their stats so far
	/// </summary>
	public GameObject CurrentPlayerGameObject;

	/// <summary>
	/// The gameobject to spawn in for moving the camera about when deciding where you want to spawn in
	/// </summary>
	public GameObject DummyControllerForPlacementCamera;

	/// <summary>
	/// Text for what is currently happening in the flow, what the player needs to do
	/// </summary>
	public Text GameObjectiveText;

	#endregion

	#region Tuning

	// On a range of 1 - 10
	public int Difficulty = 5;

	#endregion



	#region Spawned Entities

	private List<GameObject> _currentEnemies = new List<GameObject>();
	private GameObject _currentSpawnTarget = null;
	private GameObject _currentPlayer = null;
	private GameObject _currentDummyPlayer = null;

	#endregion

	#region References

	private Camera _mainCameraRef = null;
	private SpawnpointFinder _spawnerRef = null;

	#endregion

	private Vector2 _playerSpawnPosition;
	private float _objectiveTextToGameplayDuration;

	private LevelState _currentState;
	

    
    void Start()
    {
	    _spawnerRef = GameObject.FindGameObjectWithTag("Manager_Spawnpoints")?.GetComponent<SpawnpointFinder>();
		_mainCameraRef = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();

		Debug.Assert(_spawnerRef != null, "Did not have a spawner in the level.");
		Debug.Assert(_mainCameraRef != null, "Did not have a camera in the level.");

		Debug.Assert(DummyControllerForPlacementCamera != null, "Invalid dummy controller.");
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
			case LevelState.Placement:
			{
				StatePlacement();
				break;
			}
			case LevelState.PlayerInit:
			{
				StatePlayerInit();
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
			//TODO actual difficulty scaling
			for (var i = 0; i < Difficulty; i++)
			{
				SpawnEnemyIn();
			}
		}

		//Spawn in the spawn position target
		if (SpawnInTargetPrefab)
		{
			_currentSpawnTarget = (GameObject) Instantiate(SpawnInTargetPrefab);
			_currentSpawnTarget.GetComponent<SpriteRenderer>().sortingOrder = 10;
		}

		//Create an invisible dummy player to hack movement around the map to pick a spawn placement
		if (DummyControllerForPlacementCamera)
		{
			var dummySpawnPos = new Vector3(_mainCameraRef.transform.position.x, _mainCameraRef.transform.position.y, 0.0f);
			_currentDummyPlayer = (GameObject)Instantiate(DummyControllerForPlacementCamera, dummySpawnPos, new Quaternion());

			// Assign the camera to follow around this invisible dummy player
			AssignCameraToFollow(_currentDummyPlayer);
		}

		GameObjectiveText.text = "Pick a spawn point.";

		_currentState = LevelState.Placement;
	}
	
	void StatePlacement()
	{
		//Keep the spawn target tracked to the mouse cursor
		if (_currentSpawnTarget)
		{
			var targetPosition = _mainCameraRef?.ScreenToWorldPoint(Input.mousePosition) ?? Vector3.zero;
			targetPosition.z = 0;

			_currentSpawnTarget.transform.position = targetPosition;
		}

		// Player has clicked for spawn placement
		if (Input.GetMouseButtonDown(0) && CurrentPlayerGameObject)
		{
			//TODO Valid position check
			
			//Take the current position of the target for where the player spawns
			_playerSpawnPosition = _currentSpawnTarget.transform.position;

			//Unassign the camera
			AssignCameraToFollow(null);

			if (_currentSpawnTarget)
			{
				Destroy(_currentSpawnTarget);
			}
			
			if (_currentDummyPlayer)
			{
				Destroy(_currentDummyPlayer);
			}

			_currentState = LevelState.PlayerInit;
		}
	}

	void StatePlayerInit()
	{
		_currentPlayer = (GameObject) Instantiate(CurrentPlayerGameObject, _playerSpawnPosition, new Quaternion());

		// Assign the camera to follow the actual player now
		AssignCameraToFollow(_currentPlayer, true);

		//TODO Extra player init here (HUD, extra bodies)

		GameObjectiveText.text = "Survive.";
		_objectiveTextToGameplayDuration = OBJECTIVE_TEXT_ROUND_START_DURATION_TIME;

		_currentState = LevelState.Update;
	}

	void StateUpdate()
	{
		//Keep the objective text only for so long and then fade it out
		if (_objectiveTextToGameplayDuration > 0.0f)
		{
			_objectiveTextToGameplayDuration -= Time.deltaTime;

			if (_objectiveTextToGameplayDuration <= OBJECTIVE_TEXT_ROUND_START_FADE_OUT_TIME)
			{
				SetObjectiveTextAlpha();
			}
		}
		else
		{
			SetObjectiveTextAlpha(0.0f);
		}




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
		//TODO hand off to the stats system
		SceneManager.LoadScene(LEVEL_COMPLETE_SCENE);
	}


	#region Utility

	void AssignCameraToFollow(GameObject newTarget, bool intoGameplay = false)
	{
		if (_mainCameraRef.gameObject.GetComponent<SimpleCameraLerp>())
		{
			_mainCameraRef.gameObject.GetComponent<SimpleCameraLerp>().LerpTarget = newTarget?.transform;

			//Half camera speed for spawn selection, back to normal on gameplay
			_mainCameraRef.gameObject.GetComponent<SimpleCameraLerp>().LerpSpeed *= intoGameplay ? 2.0f : 0.5f;

		}
	}

	void SetObjectiveTextAlpha(float alphaOverride = -1.0f)
	{
		var tempColor = GameObjectiveText.color;
		tempColor.a = alphaOverride != -1.0f ? alphaOverride : 1.0f * _objectiveTextToGameplayDuration.Normalise(0.0f, OBJECTIVE_TEXT_ROUND_START_FADE_OUT_TIME);
		GameObjectiveText.color = tempColor;
	}

	#endregion
}

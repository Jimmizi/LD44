using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
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
	private const string LEVEL_FAILED_SCENE = "MenuScene";

	private const float OBJECTIVE_TEXT_ROUND_START_DURATION_TIME = 10.0f;
	private const float OBJECTIVE_TEXT_ROUND_START_FADE_OUT_TIME = 2.0f;


	public enum LevelState
	{
		Init,		// Make all the entities and get the stage ready
		Placement,	// Stage that is active until the player spawns into the world
		PlayerInit,	// We have our placement, start the stage!
		Update,		// The update during gameplay, checking if we meet succession criteria
		StageOver,	// Displays the result of the stage, any states, nice fade out before we're killing everything
		GameOver,	// We failed and lost the game!
		Shutdown	// Done with the stage, delete everything and go to the next scene
		
	}

	public bool DebugStraightToGameplay;

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

	public Text RoundTimerText;

	#endregion

	#region Tuning

	// On a range of 1 - 10 - This Gets added onto GameManager.Difficulty
	public int Difficulty = 1;

	public float NeutralEnemiesModifier = 1.0f;
	public float HostileEnemiesModifier = 0.5f;

	public int RoundTimer = 60;

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
	private FriendlyNPCManager _npcManagerRef = null;

	#endregion

	private Vector2 _playerSpawnPosition;
	private float _objectiveTextToGameplayDuration;
	private float _timeSinceRoundStart;
	private LevelState _currentState;
	private float _evaluateRoundOverTimer;

	private float _gameOverTimer;

	public bool IsRoundOver()
	{
		return _currentState >= LevelState.StageOver;
	}

    
    void Start()
    {
	    _spawnerRef = GameObject.FindGameObjectWithTag("Manager_Spawnpoints")?.GetComponent<SpawnpointFinder>();
		_mainCameraRef = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
		_npcManagerRef = GetComponent<FriendlyNPCManager>();

		RoundTimerText.gameObject.SetActive(false);

		Debug.Assert(_spawnerRef != null, "Did not have a spawner in the level.");
		Debug.Assert(_mainCameraRef != null, "Did not have a camera in the level.");
		Debug.Assert(_npcManagerRef != null, "Did not have a FriendlyNPCManager");

		Debug.Assert(DummyControllerForPlacementCamera != null, "Invalid dummy controller.");

		Debug.Log("Difficulty This Round is " + GameManager.Difficulty.ToString());
		GameManager.Difficulty += Difficulty;

		// Just apply this back so we can see it in the editor easily
		Difficulty = GameManager.Difficulty;

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
			case LevelState.GameOver:
			{
				StateGameOver();
				break;
			}
			case LevelState.Shutdown:
			{
				StateShutdown();
				break;
			}
		}
    }


	void SpawnEnemyIn(bool isNeutral = false)
	{
		var vPosition = _spawnerRef.GetSpawnPoint();
		var tempEnemy = (GameObject)Instantiate(EnemyList[Random.Range(0, EnemyList.Count)], vPosition, new Quaternion());

		var tempStats = tempEnemy.GetComponent<ActorStats>();

		if (tempStats)
		{
			tempStats.Neutral = isNeutral;
			tempStats.SetupDifficulty(GameManager.Difficulty);
		}
		
		_currentEnemies.Add(tempEnemy);
	}

	void StateInit()
	{
		if (EnemyList.Count > 0)
		{
			var NumHostiles = (int) (GameManager.Difficulty * HostileEnemiesModifier);
			var NumFriendlies = (int) (GameManager.Difficulty * NeutralEnemiesModifier);

			if (NumHostiles == 0 && NumFriendlies == 0)
			{
				NumHostiles = 1;
			}

			for (var i = 0; i < NumFriendlies; i++)
			{
				SpawnEnemyIn(true);
			}
			for (var i = 0; i < NumHostiles; i++)
			{
				SpawnEnemyIn();
			}
		}

		if (DebugStraightToGameplay)
		{
			_currentState = LevelState.PlayerInit;
			return;
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
		GameObjectiveText.gameObject.transform.parent.gameObject.SetActive(true);

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

			var tempPathfinder = GameObject.FindObjectOfType<PathfinderManager>();
			if (tempPathfinder == null || tempPathfinder.IsPointWithinPlayableArea(_currentSpawnTarget.transform.position))
			{
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
	}

	void StatePlayerInit()
	{
		_currentPlayer = (GameObject) Instantiate(CurrentPlayerGameObject, _playerSpawnPosition, new Quaternion());
		_currentPlayer.GetComponent<ActorStats>()?.ApplyPlayerStats();

		// Assign the camera to follow the actual player now
		AssignCameraToFollow(_currentPlayer, true);

		//TODO Extra player init here (HUD, extra bodies)

		GameObjectiveText.text = "Survive.";

		for (int i = 0; i < RoundTimerText.gameObject.transform.parent.childCount; i++)
		{
			RoundTimerText.gameObject.transform.parent.GetChild(i).gameObject.SetActive(true);
		}

		_objectiveTextToGameplayDuration = OBJECTIVE_TEXT_ROUND_START_DURATION_TIME;

		_npcManagerRef.SetButtonActive();
		_npcManagerRef.SetStartSpawningFriendlies(_playerSpawnPosition);

		_currentState = LevelState.Update;

		foreach (var enemy in _currentEnemies)
		{
			enemy.GetComponent<AIController>()?.RespondToPlayerSpawned();
		}
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

		_timeSinceRoundStart += Time.deltaTime;

		if (_timeSinceRoundStart >= RoundTimer)
		{
			_timeSinceRoundStart = RoundTimer;
			_currentState = LevelState.StageOver;
		}

		RoundTimerText.text = (RoundTimer - (int) _timeSinceRoundStart).ToString();

		_evaluateRoundOverTimer -= Time.deltaTime;
		if (_evaluateRoundOverTimer <= 0.0f)
		{
			_evaluateRoundOverTimer = 1.5f;

			var allActors = GameObject.FindObjectsOfType<ActorStats>();

			// All Friendlies Dead
			if (allActors.Where(x => x.Infected).ToArray().Length == 0)
			{
				_currentState = LevelState.GameOver;
				_gameOverTimer = 0.0f;
				return;
			}

			//All Enemies Dead
			if (allActors.Where(x => !x.Infected).ToArray().Length == 0)
			{
				_currentState = LevelState.StageOver;
				return;
			}
		}


#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.F1))
		{
			Debug.Log("Debug skipping stage.");
			_currentState = LevelState.StageOver;
		}

		if (Input.GetKeyDown(KeyCode.F2))
		{
			var allActors = GameObject.FindObjectsOfType<ActorStats>().Where(x => x.gameObject.GetComponent<PlayerController>()).ToArray();

			if (allActors.Length > 0)
			{
				allActors[0].gameObject.AddComponent<KillActor>();
			}

		}

#endif
	}

	void StateStageOver()
	{
		//TODO: Fade out
		_currentState = LevelState.Shutdown;
	}

	void StateGameOver()
	{
		if (_gameOverTimer == 0.0f)
		{
			GameObject.FindObjectOfType<LevelManager>()?.GameOver();
		}

		_gameOverTimer += Time.deltaTime;

		if (_gameOverTimer >= 4.0f)
		{
			SceneManager.LoadScene(LEVEL_FAILED_SCENE);
		}
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

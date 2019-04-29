using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Data;
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

	private const float OBJECTIVE_TEXT_ROUND_START_DURATION_TIME = 6.0f;
	private const float OBJECTIVE_TEXT_ROUND_START_FADE_OUT_TIME = 3.0f;


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

	private struct ObjectiveTextStruct
	{
		public string scenePresentIn;
		public string text;
	}

	public bool DebugStraightToGameplay;
	public bool ThisIsTheTitleMenu;

	#region Engine facing config
	
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

	#region Tuning - per round

	public enum EnemyType
	{
		Neutral,
		Hostile,
		Sweeper
	}
	
	[Serializable]
	public struct SpawnStats
	{
		[SerializeField]
		public GameObject Prefab;

		[SerializeField]
		public EnemyType Type;

		/// <summary>
		/// The base chance to spawn this in
		/// </summary>
		[SerializeField]
		public int BaseSpawnChance;

		/// <summary>
		/// Extra difficulty levels above the DifficultyLevelToSpawn level add onto the chance to spawn
		/// </summary>
		[SerializeField]
		public int SpawnIncreasePerDifficultyLevel;

		/// <summary>
		/// The difficulty level of the stage needed before we start trying to spawn things in
		/// </summary>
		[SerializeField]
		public int DifficultyLevelToSpawn;

		/// <summary>
		/// How many times to try and spawn
		/// </summary>
		[SerializeField]
		public int BaseTimesToTryAndSpawn;

		/// <summary>
		/// How many times to try and spawn
		/// </summary>
		[SerializeField]
		public float TimesToTrySpawnIncreasePerDifficultyLevel;

		/// <summary>
		/// If a chance spawns one, don't try to spawn any more
		/// </summary>
		[SerializeField]
		public bool OnlySpawnOnce;

		public bool SpawnAtLeastOnce;
	}

	[SerializeField]
	public List<SpawnStats> EnemyList = new List<SpawnStats>();


	public int RoundTimer = 60;
	public float DifficultyMod = 0.5f;

	// On a range of 1 - 10 - This Gets added onto GameManager.Difficulty
	public int DifficultyIncreaseAfterRound = 1;

	public string NextScene = "";
	public int NextSceneAtDifficultyLevel = 5;
	

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
	private bool _doingFadeIn = true;
	private bool _doingFadeOut = false;
	private bool _startedFailFade = false;
	
	private static bool _tutorialPassed = false;
	private static bool _randomiseObjectiveText = false;

	private TransitionFade _fader;

	public bool IsRoundOver()
	{
		return _currentState >= LevelState.StageOver;
	}

	private static bool _listSetUp;
	static List<ObjectiveTextStruct> _objectiveTextList = new List<ObjectiveTextStruct>();

	string[,] _objectiveTextALL = new string[,]
	{
		{ "Take what is yours.", "Sacrifice the minions.", "Dominate.", "Survive. Prosper.", "Multiply."},
		{ "Seek. Destroy.", "Find and take.", "No cell can hide.", "You are unstoppable.", "You are immortal."},
		{ "Rule. Conquer.", "Take. Take.", "More. More.", "Everything is yours.", "There is only you."},
		{ "No remorse.", "No pity.", "No thought.", "No feelings.", "Only you."},
	};

	void SetupObjectiveTextList()
	{
		if (_listSetUp)
		{
			return;
		}

		const int sceneCount = 4;
		const int objCount = 5;
		var sceneName = new string[] {"FirstLevel", "SecondLevel", "ThirdLevel", "FourthLevel"};
		
		for (int scene = 0; scene < sceneCount; scene++)
		{

			//4 scenes times 5 difficulties per scene
			for (int i = 0; i < objCount; i++)
			{
				var tempOT = new ObjectiveTextStruct();
				tempOT.scenePresentIn = sceneName[scene];
				tempOT.text = _objectiveTextALL[scene, i];

				_objectiveTextList.Add(tempOT);
			}
		}

		_listSetUp = true;
	}

    void Start()
    {
	    SetupObjectiveTextList();

		_spawnerRef = GameObject.FindGameObjectWithTag("Manager_Spawnpoints")?.GetComponent<SpawnpointFinder>();
		_mainCameraRef = GameObject.FindGameObjectWithTag("MainCamera")?.GetComponent<Camera>();
		_npcManagerRef = GetComponent<FriendlyNPCManager>();
		_fader = GameObject.FindObjectOfType<TransitionFade>();

		RoundTimerText?.gameObject.SetActive(false);

		if (!_tutorialPassed)
		{
			GameObject.FindGameObjectWithTag("TempUpgradeCanvas")?.SetActive(false);
		}

		Debug.Assert(_spawnerRef != null, "Did not have a spawner in the level.");
		Debug.Assert(_mainCameraRef != null, "Did not have a camera in the level.");
		Debug.Assert(_npcManagerRef != null, "Did not have a FriendlyNPCManager");

		Debug.Assert(DummyControllerForPlacementCamera != null, "Invalid dummy controller.");
		
		Debug.Log("Difficulty This Round is " + GameManager.Difficulty.ToString());
    }
	
	void Update()
    {
	    if (_fader)
	    {
		    if (_doingFadeIn && !_doingFadeOut)
		    {
			    _doingFadeIn = !_fader.DoFadeIn(2.0f);
		    }
		    if (_doingFadeOut && !_doingFadeIn)
		    {
			    _doingFadeOut = !_fader.DoFadeOut(2.0f);
		    }
		}



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


	void TryToSpawnEnemy(SpawnStats data)
	{
		if (data.DifficultyLevelToSpawn > GameManager.Difficulty)
		{
			Debug.Log("TryToSpawnEnemy - " + data.Type.ToString() + " cannot spawn this round, difficulty is too low: " + data.DifficultyLevelToSpawn.ToString() + " > " +  GameManager.Difficulty.ToString());
			return;
		}

		var levelsAboveBaseDifficulty = (GameManager.Difficulty - data.DifficultyLevelToSpawn) + ActorStats.MapRotationCount;

		var extraSpawnChance = data.SpawnIncreasePerDifficultyLevel * levelsAboveBaseDifficulty * ActorStats.MapRotationCount;
		var extraSpawnTries = Mathf.Floor(data.TimesToTrySpawnIncreasePerDifficultyLevel * levelsAboveBaseDifficulty);

		if (ActorStats.MapRotationCount > 1)
		{
			extraSpawnChance *= 2;
		}

		var spawnChance = data.BaseSpawnChance + extraSpawnChance;
		var timesToTrySpawn = (data.BaseTimesToTryAndSpawn + extraSpawnTries * ActorStats.MapRotationCount);

		if (data.Type == EnemyType.Hostile)
		{
			timesToTrySpawn += GameManager.InfectedCellsCount / 3;
		}

		if (data.Type == EnemyType.Sweeper)
		{
			timesToTrySpawn += GameManager.InfectedCellsCount / 10;
		}

		if (data.Type == EnemyType.Neutral)
		{
			spawnChance /= ActorStats.MapRotationCount;
		}

		Debug.Log("TryToSpawnEnemy - " + data.Type.ToString() + " spawn chance: " + spawnChance.ToString() + "%, times to try: " + timesToTrySpawn.ToString());

		var autoSpawn = data.SpawnAtLeastOnce;

		for (var i = 0; i < timesToTrySpawn; i++)
		{
			var spawnChanceRandom = Random.Range(0.0f, 100.0f);

			if (spawnChanceRandom >= spawnChance && !autoSpawn)
			{
				Debug.Log("TryToSpawnEnemy - " + data.Type.ToString() + " try " + i.ToString() + " failed. Got: " + spawnChanceRandom.ToString() + " but chance was " + spawnChance.ToString());
				continue;
			}

			autoSpawn = false;


			Debug.Log("TryToSpawnEnemy - " + data.Type.ToString() + " try " + i.ToString() + " succeeded!");
			
			var tempEnemy = (GameObject)Instantiate(data.Prefab, _spawnerRef.GetSpawnPoint(), new Quaternion());
			
			var tempStats = tempEnemy.GetComponent<ActorStats>();
			if (tempStats)
			{
				
				DataController dataController = DataController.GetInstance();
				if (dataController)
				{
					dataController.initActorFromType(tempStats, data.Type);
				}
				tempStats.SetupDifficulty(GameManager.Difficulty * ActorStats.MapRotationCount, DifficultyMod * ActorStats.MapRotationCount);
			}

			_currentEnemies.Add(tempEnemy);

			if (data.OnlySpawnOnce)
			{
				Debug.Log("TryToSpawnEnemy - " + data.Type.ToString() + " not spawning any more of these.");
				break;
			}
		}
	}

	void HandleEnemySpawning()
	{
		foreach (var enemy in EnemyList)
		{
			TryToSpawnEnemy(enemy);
		}
	}

	void StateInit()
	{
		HandleEnemySpawning();

		if (ThisIsTheTitleMenu)
		{
			_currentState = LevelState.Update;
			return;
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

		GameObjectiveText.text = "Pick the infection point";
		GameObjectiveText.gameObject.transform.parent.gameObject.SetActive(true);

		_currentState = LevelState.Placement;
	}
	
	void StatePlacement()
	{
	    Cursor.visible = false;

		if (GameObject.FindObjectOfType<LevelManager>())
		{
			if (GameObject.FindObjectOfType<LevelManager>().paused)
			{
				return;
			}
		}

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

			    Cursor.visible = true;
            }	    
        }	    
    }

	void StatePlayerInit()
	{
		_currentPlayer = (GameObject) Instantiate(CurrentPlayerGameObject, _playerSpawnPosition, new Quaternion());
		_currentPlayer.GetComponent<ActorStats>()?.ApplyPlayerStats();

		// Assign the camera to follow the actual player now
		AssignCameraToFollow(_currentPlayer, true);

		if (_objectiveTextList.Count > 0)
		{
			if (!_randomiseObjectiveText)
			{
				var textsForLevel = _objectiveTextList.Where(x => x.scenePresentIn == SceneManager.GetActiveScene().name).ToArray();
				
				if ((GameManager.Difficulty - 1) < textsForLevel.Length)
				{
					GameObjectiveText.text = textsForLevel[(GameManager.Difficulty - 1)].text;
				}
				else
				{
					GameObjectiveText.text = textsForLevel[Random.Range(0, textsForLevel.Length)].text;
				}
			}
			else
			{
				var randomScene = Random.Range(0, 4);
				var randomText = Random.Range(0, 5);
				GameObjectiveText.text = _objectiveTextALL[randomScene, randomText];
			}
		}
		
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
		if (ThisIsTheTitleMenu)
		{
			return;
		}

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
#endif
	}

	void StateStageOver()
	{
		_tutorialPassed = true;

		//TODO: Fade out
		_currentState = LevelState.Shutdown;
		_doingFadeOut = true;
		_doingFadeIn = false;

		//TODO need to reset this on new stage start
		GameManager.Difficulty += DifficultyIncreaseAfterRound;
	}

	void StateGameOver()
	{
		if (_gameOverTimer == 0.0f)
		{
			GameObject.FindObjectOfType<LevelManager>()?.GameOver();
		}

		if (!_startedFailFade && _gameOverTimer > 0.5f)
		{
			_startedFailFade = true;
			_doingFadeOut = true;
		}

		_gameOverTimer += Time.deltaTime;
		if (_gameOverTimer >= 4.0f)
		{
			var tempLevelManagerPersObj = GameObject.FindObjectOfType<LevelManager>()?.GetComponent<PersistentObject>();
			if (tempLevelManagerPersObj)
			{
				//NOTE Persistent manager now will delete that object if found within the menu scene
				//Delete the persistent at game over so that it won't transfer back to the main menu
				//Destroy(tempLevelManagerPersObj);
			}
			
			SceneManager.LoadScene(LEVEL_FAILED_SCENE);
		}
	}

	void StateShutdown()
	{
		if (_doingFadeOut)
		{
			return;
		}

		if (GameManager.Difficulty >= NextSceneAtDifficultyLevel)
		{
			//Next level, reset difficulty
			UpgradeNextScene.NextSceneToUse = NextScene;
			GameManager.Difficulty = 1;

			if (UpgradeNextScene.NextSceneToUse == "FirstLevel")
			{
				_randomiseObjectiveText = true;
				ActorStats.MapRotationCount++;
			}
		}
		else
		{
			UpgradeNextScene.NextSceneToUse = SceneManager.GetActiveScene().name;
		}
		
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

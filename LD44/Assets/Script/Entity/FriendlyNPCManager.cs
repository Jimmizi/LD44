using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;

/// <summary>
/// Script that manages the NPCs friendly to the player
/// </summary>

public class FriendlyNPCManager : MonoBehaviour
{
	public bool OnMenu_SpawnImmediately = false;
	public GameObject FriendlyNPCPrefab;
	public List<GameObject> FriendliesList = new List<GameObject>();

	//public Text FriendlyAttackTypeText;
	public Text FriendliesLeftText;

	private ActionManager.AttackType _aiPreferredAttack;
	private int _friendliesLeftToSpawn;
	private int _failedAttempts = 0;
	private Vector2 _pointToSpawnAround;

	public void SwitchPreferredAttackType()
	{
		//if (_aiPreferredAttack == ActionManager.AttackType.InfectAttempt)
		//{
		//	_aiPreferredAttack = ActionManager.AttackType.Lethal;
		//}
		//else
		//{
		//	_aiPreferredAttack = ActionManager.AttackType.InfectAttempt;
		//}

		//foreach (var friend in FriendliesList)
		//{
		//	friend.GetComponent<AIController>()?.SetDesiredAttackType(_aiPreferredAttack);
		//}
		
		
	}

	public void SetButtonActive()
	{
		//FriendlyAttackTypeText.gameObject.transform.parent.gameObject.SetActive(true);
	}

	public void SetStartSpawningFriendlies(Vector2 spawnPoint)
	{
		if (GameManager.InfectedCellsCount == 0)
		{
			//_friendliesLeftToSpawn = 1; Start out with just yourself
		}
		else
		{
			_friendliesLeftToSpawn = GameManager.InfectedCellsCount;

			if (!OnMenu_SpawnImmediately)
			{
				GameManager.InfectedCellsCount = 0;
			}
		}
		_pointToSpawnAround = spawnPoint;
	}

	public void SpawnSingleFriendly(Vector2 spawnPoint)
	{
		_friendliesLeftToSpawn++;
		_pointToSpawnAround = spawnPoint;
	}

	void Start()
    {
	    if (FriendliesLeftText)
	    {
		    FriendliesLeftText.text = GameManager.InfectedCellsCount.ToString();
	    }

		if (OnMenu_SpawnImmediately)
		{
			SetStartSpawningFriendlies(Vector2.zero);
		}
	}


	void Update()
	{
		if (FriendliesLeftText)
		{
			FriendliesLeftText.text = GameManager.InfectedCellsCount.ToString();
		}

		if (_friendliesLeftToSpawn <= 0)
		{
			_friendliesLeftToSpawn = 0;
			return;
		}

		var tempPathfinder = GameObject.FindObjectOfType<PathfinderManager>();
		if (tempPathfinder == null)
		{
			return;
		}

		var rangeBound = 0.5f + (0.25f * _failedAttempts);
		var spawnedSuccessfully = false;

		for (var i = 0; i < 5; i++)
		{
			var tempRandomSpawnPoint = _pointToSpawnAround + new Vector2(Random.Range(-rangeBound, rangeBound), Random.Range(-rangeBound, rangeBound));

			if (tempPathfinder.IsPointWithinPlayableArea(tempRandomSpawnPoint))
			{
				//TODO Nicer spawn in with particles/sound, perhaps a fade in
				var tempFriendly = (GameObject) Instantiate(FriendlyNPCPrefab, tempRandomSpawnPoint, new Quaternion());

				tempFriendly.GetComponent<ActorStats>()?.ApplyFriendlyStats();

				var tempController = tempFriendly.GetComponent<AIController>();

				if (tempController)
				{
					tempController.SetDesiredAttackType(_aiPreferredAttack);
					tempController.CurrentTask = AIController.AITask.AttackTarget;
				}
				
				FriendliesList.Add(tempFriendly);
				spawnedSuccessfully = true;

				_friendliesLeftToSpawn--;

				if (!OnMenu_SpawnImmediately)
				{
					GameManager.InfectedCellsCount++;
				}

				break;
			}
		}

		if (!spawnedSuccessfully)
		{
			_failedAttempts++;
		}
	}
}


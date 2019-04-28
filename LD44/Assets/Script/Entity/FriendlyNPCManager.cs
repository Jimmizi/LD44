using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script that manages the NPCs friendly to the player
/// </summary>

public class FriendlyNPCManager : MonoBehaviour
{

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
		if (_aiPreferredAttack == ActionManager.AttackType.InfectAttempt)
		{
			_aiPreferredAttack = ActionManager.AttackType.Lethal;
		}
		else
		{
			_aiPreferredAttack = ActionManager.AttackType.InfectAttempt;
		}

		foreach (var friend in FriendliesList)
		{
			friend.GetComponent<AIController>()?.SetDesiredAttackType(_aiPreferredAttack);
		}
		
		
	}

	public void SetButtonActive()
	{
		//FriendlyAttackTypeText.gameObject.transform.parent.gameObject.SetActive(true);
	}

	public void SetStartSpawningFriendlies(Vector2 spawnPoint)
	{
		
		_friendliesLeftToSpawn = GameManager.InfectedCellsCount;
		_pointToSpawnAround = spawnPoint;
	}

	void Start()
    {
		//FriendlyAttackTypeText.gameObject.transform.parent.gameObject.SetActive(false);
		FriendliesLeftText.text = GameManager.InfectedCellsCount.ToString();
	}


	void Update()
	{
		// Toggle between attack modes
		if (Input.GetKeyDown(KeyCode.E))
		{
			SwitchPreferredAttackType();

			//if (FriendlyAttackTypeText)
			//{
			//	FriendlyAttackTypeText.text = "(E) " + (_aiPreferredAttack == ActionManager.AttackType.Lethal ? "Friendlies Prefer Killing" : "Friendlies Prefer Infecting");
			//}
		}

		if (FriendliesLeftText)
		{
			FriendliesLeftText.text = GameManager.InfectedCellsCount.ToString();
		}

		if (_friendliesLeftToSpawn <= 0)
		{
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
			}
		}

		if (!spawnedSuccessfully)
		{
			_failedAttempts++;
		}
	}
}


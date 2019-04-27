using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script that manages the NPCs friendly to the player
/// </summary>

public class FriendlyNPCManager : MonoBehaviour
{

	public GameObject FriendlyNPCPrefab;
	public Text PreferredTypeText;
	public List<GameObject> FriendliesList = new List<GameObject>();
	
	private ActionManager.AttackType _aiPreferredAttack;
	private int _friendliesLeftToSpawn;
	private int _failedAttempts = 0;
	private Vector2 _pointToSpawnAround;

	public void SwitchPreferredAttackType()
	{
		if (_aiPreferredAttack == ActionManager.AttackType.InfectAttempt)
		{
			_aiPreferredAttack = ActionManager.AttackType.Lethal;
			PreferredTypeText.text = "Friendlies go for kills.";
		}
		else
		{
			_aiPreferredAttack = ActionManager.AttackType.InfectAttempt;
			PreferredTypeText.text = "Friendlies go for infecting.";
		}

		foreach (var friend in FriendliesList)
		{
			friend.GetComponent<AIController>()?.SetDesiredAttackType(_aiPreferredAttack);
		}
		
		
	}

	public void SetButtonActive()
	{
		PreferredTypeText.gameObject.transform.parent.gameObject.SetActive(true);
	}

	public void SetStartSpawningFriendlies(Vector2 spawnPoint)
	{
		_friendliesLeftToSpawn = 5;
		_pointToSpawnAround = spawnPoint;
	}

	void Start()
    {
		PreferredTypeText.text = "Friendlies go for kills.";
		PreferredTypeText.gameObject.transform.parent.gameObject.SetActive(false);
    }


	void Update()
	{
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


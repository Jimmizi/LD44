using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackResolver : MonoBehaviour
{
	public ActionManager.cbAttackResolution CallerResultCallback = null;
	public bool ReadyForQuery;


	private void ProcessAttackDone(ActionManager.AttackResult queryResult = ActionManager.AttackResult.InvalidTarget)
	{
		CallerResultCallback(queryResult);
		Destroy(gameObject);
	}
	
	void Update()
	{
		if (!ReadyForQuery)
		{
			return;
		}

		var thisCollider = GetComponent<Collider2D>();
		var queryResult = ActionManager.AttackResult.InvalidTarget;

		if (!thisCollider)
		{
			ProcessAttackDone();
			return;
		}

		//get all that is colliding with the query object
		var contactsList = new Collider2D[10];
		var numContacts = thisCollider.GetContacts(contactsList);

		var myGameStats = GetComponent<GameStats>();

		if (numContacts <= 0)
		{
			ProcessAttackDone();
			return;
		}

		foreach (var other in contactsList)
		{
			//Reached the end of found objects
			if (other == null)
			{
				break;
			}

			//TODO Picking a target depending on distance (limited by attack range)
			//TODO Multiple targets possibly if an attack allows for that

			Debug.Log("Collided with: " + other.name);

			//If the other doesn't have gamestats it's not something we can interact with
			if (other.GetComponent<GameStats>())
			{
				switch (ResolveAttack(myGameStats, other.GetComponent<GameStats>()))
				{
					case ActionManager.AttackResult.InvalidTarget:
						break;
					case ActionManager.AttackResult.VictimDeath:
					{
						//TODO this should call something on the victim so they can spawn stuff, play death sounds
						Destroy(other.gameObject);
						break;
					}
					case ActionManager.AttackResult.VictimInfected:
						break;
					case ActionManager.AttackResult.VictimOkay:
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		ProcessAttackDone();
	}

	private ActionManager.AttackResult ResolveAttack(GameStats attackerStats, GameStats victimStats)
	{
		//TODO evaluate health left and all that
		return ActionManager.AttackResult.VictimDeath;
	}

	void OnDrawGizmos()
	{

	}
}

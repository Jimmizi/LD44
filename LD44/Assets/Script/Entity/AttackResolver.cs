using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackResolver : MonoBehaviour
{
	public ActionManager.cbAttackResolution CallerResultCallback = null;
	public ActionManager.cbTargetAttemptingToInfect CallerInfectionTarget = null;
	public bool ReadyForQuery;
	public ActionManager.AttackType CurrentAttackType = ActionManager.AttackType.Lethal;
	public ActorStats CallerStats;

	public GameObject SpecificTarget = null;
	public GameObject CallingGameObject = null;

	ActionManager.AttackResult _queryResult = ActionManager.AttackResult.InvalidTarget;

	private void ProcessAttackDone(ActionManager.AttackResult queryResult = ActionManager.AttackResult.InvalidTarget)
	{
		CallerResultCallback(_queryResult);
		Destroy(gameObject);
	}
	
	void Update()
	{
		if (!ReadyForQuery)
		{
			return;
		}

		var thisCollider = GetComponent<Collider2D>();

		if (!thisCollider)
		{
			ProcessAttackDone();
			return;
		}
		
		if (SpecificTarget != null)
		{
			if (SpecificTarget.GetComponent<Collider2D>())
			{
				Debug.Log("Attacking specific target.");
				ProcessAttack(SpecificTarget.GetComponent<Collider2D>());
			}

			ProcessAttackDone();
			return;
		}

		//get all that is colliding with the query object
		var contactsList = new Collider2D[10];
		var numContacts = thisCollider.GetContacts(contactsList);
		
		if (numContacts <= 0)
		{
			ProcessAttackDone();
			return;
		}

		foreach (var other in contactsList)
		{
			//Reached the end of found objects
			if (other == null || CallingGameObject == null || other == CallingGameObject.GetComponent<Collider2D>())
			{
				break;
			}

			//If the invoker of the attack is not being infected, they cannot attack
			if (CallerStats.BeingInfectedBy != null)
			{
				break;
			}
			
			//TODO Picking a target depending on distance (limited by attack range)
			//TODO Multiple targets possibly if an attack allows for that

			Debug.Log("Collided with: " + other.name);

			ProcessAttack(other);
		}

		ProcessAttackDone();
	}

	private void ProcessAttack(Collider2D other)
	{
		ActorStats othersStats = null;

		othersStats = other.GetComponent<ActorStats>();

		//If the other doesn't have gamestats it's not something we can interact with
		if (othersStats == null)
		{
			return;
		}

		//Don't attack the same type
		if (CallerStats.Infected == othersStats.Infected)
		{
			return;
		}

		//Can't (or don't want to) kill a cell in the process of being infected.
		if (othersStats.BeingInfectedBy != null && !othersStats.Infected)
		{
			//But only if we're not the one infecting them
			if (othersStats.BeingInfectedBy != CallingGameObject)
			{
				return;
			}
		}



		_queryResult = ResolveAttack(CallerStats, othersStats);

		switch (_queryResult)
		{
			case ActionManager.AttackResult.InvalidTarget:
				break;
			case ActionManager.AttackResult.VictimDeath:
			{
				other.gameObject.AddComponent<KillActor>();
				break;
			}
			case ActionManager.AttackResult.VictimInfected:
			{
				other.gameObject.AddComponent<InfectActor>();
				break;
			}
			case ActionManager.AttackResult.VictimOkay:
				break;
			case ActionManager.AttackResult.InfectionInProgress:
				//Tell the other character they're being infected by us
				//NOTE: No longer how infection works
				//othersStats.BeingInfectedBy = CallingGameObject;
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		
	}

	private ActionManager.AttackResult ResolveAttack(ActorStats attackerStats, ActorStats victimStats)
	{
		//var justInfected = false;


		//switch (CurrentAttackType)
		//{
		//	case ActionManager.AttackType.Lethal:
		//	{
		//		victimStats.Health -= attackerStats.Damage;

		//		//var victimActions = victimStats.GetComponent<ActionManager>();

		//		//if (victimActions)
		//		//{
		//		//	//If the victim is infecting another, they are vulnerable because of it 
		//		//	//Could be just and increase of damage if needed
		//		//	instaKill = victimActions.CurrentAttack == ActionManager.AttackType.Infect;
		//		//}

		//		break;
		//	}
		//		//NOTE: No longer how infection works
		//		//case ActionManager.AttackType.InfectAttempt:
		//		//{
		//		//	CallerInfectionTarget(victimStats.gameObject);
		//		//	return ActionManager.AttackResult.InfectionInProgress;
		//		//}
		//		//case ActionManager.AttackType.Infect:
		//		//{
		//		//	victimStats.Infected = true;
		//		//	justInfected = true;
		//		//	break;
		//		//}
		//}

		victimStats.Health -= attackerStats.Damage;

		if (victimStats.Health <= 0.0f)
		{
			//TODO Chance of infection/cloning

			return ActionManager.AttackResult.VictimDeath;
		}
		//else if (justInfected)
		//{
		//	return ActionManager.AttackResult.VictimInfected;
		//}
		
		
		return ActionManager.AttackResult.VictimOkay;
	}

	void OnDrawGizmos()
	{

	}
}

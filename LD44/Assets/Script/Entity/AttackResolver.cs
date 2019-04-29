using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class AttackResolver : MonoBehaviour
{
	public ActionManager.cbAttackResolution CallerResultCallback = null;
	public ActionManager.cbTargetAttemptingToInfect CallerInfectionTarget = null;
	public bool ReadyForQuery;
	public ActionManager.AttackType CurrentAttackType = ActionManager.AttackType.Lethal;
	public ActorStats CallerStats;

	public GameObject AoeEffectsPrefab;

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

		if (CallerStats.UsesAoeAttack)
		{
			Instantiate(AoeEffectsPrefab, CallingGameObject.transform.position, new Quaternion());
		}
		
		if (SpecificTarget != null)
		{
			if (SpecificTarget.GetComponent<Collider2D>())
			{
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

		if (other.GetComponent<InfectActor>() || other.GetComponent<KillActor>())
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
		var infectionChance = UnityEngine.Random.Range(0.0f, 100.0f);
		var cloningChance = UnityEngine.Random.Range(0.0f, 100.0f);
		var infectedVictim = (infectionChance <= attackerStats.InfectionChance);
		
		victimStats.Health -= attackerStats.Damage;

		if (victimStats.Health <= 0.0f)
		{
			if (infectedVictim)
			{
				//If infecting on the last hit instead, give the health back
				victimStats.Health += attackerStats.Damage;
			}

			//Infected attackers have the chance to clone the victim on death
			if (attackerStats.Infected)
			{
				if (cloningChance <= attackerStats.CloningChance)
				{
					GameObject.FindObjectOfType<FriendlyNPCManager>().SpawnSingleFriendly(victimStats.gameObject.transform.position);
				}
			}
			
			return infectedVictim ? ActionManager.AttackResult.VictimInfected : ActionManager.AttackResult.VictimDeath;
		}
		
		return infectedVictim ? ActionManager.AttackResult.VictimInfected : ActionManager.AttackResult.VictimOkay;
	}

	void OnDrawGizmos()
	{

	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour
{
	public enum ActionType
	{
		Idle,			//Not performing an action, just moving about via the controller

		Attack,			//Perform an attack action
		Dash			//Fast dash in a direction
	}

	public enum AttackType
	{
		Lethal,			//Attack a target looking to kill them
		InfectAttempt,	//Attack a target looking to spread infection, this is the first stage that starts attempting to infect the target
		Infect			//We've attempted and have succeeded in infecting them
	}

	public enum AttackResult
	{
		InvalidTarget,	//i.e. terrain, nothing

		VictimDeath,
		VictimInfected,
		VictimOkay,
		InfectionInProgress
	}

	/// <summary>
	/// The object we spawn process an attack
	/// </summary>
	public GameObject AttackQueryPrefab = null;

	/// <summary>
	/// A toggle between wanting to kill or infect
	/// </summary>
	public AttackType CurrentAttack = AttackType.Lethal;

	public Vector2 TargetLocationForAction;

	private ActionType _currentAction;
	private bool _awaitingResult;
	private float _actionDelay;
	private GameObject _attemptingToInfectTarget = null;

	public delegate void cbAttackResolution(AttackResult eResult);
	public delegate void cbTargetAttemptingToInfect(GameObject other);


	private ActorMovement _moverRef = null;
	private ActorStats _statsRef = null;
	
	public bool CanAttack
	{
		get
		{
			return _currentAction == ActionType.Idle && _actionDelay <= 0.0f;
		}
	}

	public bool DoAction(ActionType newAction)
	{
		if (newAction == ActionType.Idle || _currentAction != ActionType.Idle)
		{
			return false;
		}

		_currentAction = newAction;
		_awaitingResult = false;
		_actionDelay = _statsRef.AttackSpeed;
		
		return true;

	}

	public void ToggleAttackMode()
	{
		//NOTE: We no longer have dual attack modes
		//can't do another action if waiting for something to get back, or waiting for the infect attack.
		if (!_awaitingResult && CurrentAttack != AttackType.Infect)
		{
			//CurrentAttack = CurrentAttack == AttackType.InfectAttempt ? AttackType.Lethal : AttackType.InfectAttempt;
		}
	}
	
	void Start()
	{
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");

		_statsRef = GetComponent<ActorStats>();
		Debug.Assert(_statsRef != null, "Didn't manage to find a ActorStats.");
	}
	
    void Update()
    {
		if (_awaitingResult)
	    {
		    return;
	    }

	    if (_actionDelay > 0.0f)
	    {
		    _actionDelay -= Time.deltaTime;
		   
	    }

		switch (_currentAction)
		{
			case ActionType.Attack:
			{
				DoAttack();
				break;
			}
		}
	}

    public void GetActionResult(AttackResult eResult)
    {
	    _currentAction = ActionType.Idle;
	    _attemptingToInfectTarget = null;
		_awaitingResult = false;
		TargetLocationForAction = Vector2.zero;
		
    }

    private Vector2 GetNearestPosition()
    {
	    var allActors = GameObject.FindObjectsOfType<ActorStats>();
	    var enemiesToTarget = new List<GameObject>();

	    // Weed out all the same type as me
	    foreach (var actor in allActors)
	    {

			if (!actor.Active) continue;

		    //If the actor we're looking at doesn't have the same stat of infection as me, they are a target
		    if (actor.Infected != _statsRef.Infected)
		    {
			    enemiesToTarget.Add(actor.gameObject);
		    }
	    }

	    if (enemiesToTarget.Count == 0)
	    {
		    return Vector2.zero;
	    }

	    foreach (var enemy in enemiesToTarget)
	    {
		    var distance = ((Vector2)enemy.transform.position - (Vector2)this.transform.position).magnitude;

		    if (distance < _statsRef.AttackRange)
		    {
			    return enemy.transform.position;

		    }
		}

	    return Vector2.zero;
	}

	//TODO Make the attack update to the updated position of the target

	void DoAttack()
    {
		Vector2 attackSpawnPos;

		if (TargetLocationForAction != Vector2.zero)
		{
			attackSpawnPos = TargetLocationForAction;
		}
		else if (_attemptingToInfectTarget)
		{
			attackSpawnPos = _attemptingToInfectTarget.transform.position;
		}
		else
		{
			attackSpawnPos = GetNearestPosition(); 
		}

		//NOTE: No longer how infection works
		//if (CurrentAttack == AttackType.Infect)
		//{
		//	//If we were about to infect a target and they no longer exist, bail out.
		//	//	Others won't be able to attack or target something being infected, but do this just in case
		//	if (_attemptingToInfectTarget == null)
		//	{
		//		_currentAction = ActionType.Idle;
		//		_awaitingResult = false;
		//		return;
		//	}
		//}

		if (attackSpawnPos == Vector2.zero)
		{
			_currentAction = ActionType.Idle;
			_awaitingResult = false;
			return;
		}

		var attackSize = 1.0f;

		if (_statsRef.UsesAoeAttack)
		{
			attackSpawnPos = this.transform.position;
			attackSize = 3.0f;
		}

		var tempQuery = (GameObject) Instantiate(AttackQueryPrefab, attackSpawnPos, new Quaternion());
		var tempResolver = tempQuery.GetComponent<AttackResolver>();

		tempQuery.transform.localScale *= attackSize;

		if (tempResolver)
		{
			tempResolver.CallerResultCallback = GetActionResult;
			//tempResolver.CallerInfectionTarget = other => _attemptingToInfectTarget = other;
			tempResolver.SpecificTarget = _attemptingToInfectTarget;
			tempResolver.CallingGameObject = this.gameObject;
			tempResolver.CallerStats = _statsRef;

			//tempResolver.CurrentAttackType = CurrentAttack;
			tempResolver.ReadyForQuery = true;
			_awaitingResult = true;
		}
		else
		{
			//Should never hit this
			_currentAction = ActionType.Idle;
			_awaitingResult = false;
		}
	}

    void DoDash()
    {
	    
    }
}

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
	public AttackType CurrentAttack = AttackType.InfectAttempt;

	public Vector2 TargetLocationForAction;

	private ActionType _currentAction;
	private bool _awaitingResult;
	private float _actionDelay;
	private GameObject _attemptingToInfectTarget = null;

	public delegate void cbAttackResolution(AttackResult eResult);
	public delegate void cbTargetAttemptingToInfect(GameObject other);


	private ActorMovement _moverRef = null;
	private ActorStats _statsRef = null;
	
	public bool DoingAnAction
	{
		get
		{
			return _currentAction != ActionType.Idle;
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
		//can't do another action if waiting for something to get back, or waiting for the infect attack.
		if (!_awaitingResult && CurrentAttack != AttackType.Infect)
		{
			CurrentAttack = CurrentAttack == AttackType.InfectAttempt ? AttackType.Lethal : AttackType.InfectAttempt;
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
		    return;
	    }

		switch (_currentAction)
		{
			case ActionType.Attack:
			{
				DoAttack();
				break;
			}

			case ActionType.Dash:
			{
				DoDash();
				break;
			}
		}
	}

    public void GetActionResult(AttackResult eResult)
    {
	    switch (eResult)
	    {
		    case AttackResult.InvalidTarget:
			    break;
		    case AttackResult.VictimDeath:
			    break;
		    case AttackResult.VictimInfected:
			    break;
		    case AttackResult.VictimOkay:
			    break;
		    case AttackResult.InfectionInProgress:
		    {
				//TODO Add charge/eating animations to make it visual that an infection is happening
				
			    _actionDelay = _statsRef.InfectionSpeed;
			    CurrentAttack = AttackType.Infect;
			    _awaitingResult = false;

			    return;
		    }
		    default:
			    throw new ArgumentOutOfRangeException(nameof(eResult), eResult, null);
	    }
	    
		Debug.Log("Action result received");
	    _currentAction = ActionType.Idle;
	    _attemptingToInfectTarget = null;
		_awaitingResult = false;
		TargetLocationForAction = Vector2.zero;


		if (CurrentAttack == AttackType.Infect)
	    {
		    CurrentAttack = AttackType.InfectAttempt;
	    }
    }

    private Vector2 GetNearestPosition()
    {
	    var allActors = GameObject.FindObjectsOfType<ActorStats>();
	    var enemiesToTarget = new List<GameObject>();

	    // Weed out all the same type as me
	    foreach (var actor in allActors)
	    {
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
	    else if (enemiesToTarget.Count == 1)
	    {
		    return enemiesToTarget[0].transform.position;
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

	void DoAttack()
    {
		//TODO stats for range and size of attack

		//TODO improve where the bounds of the attack are
		Vector2 attackSpawnPos;

		if (TargetLocationForAction != Vector2.zero)
		{
			attackSpawnPos = TargetLocationForAction;
		}
		else
		{
			attackSpawnPos = GetNearestPosition(); //(Vector2)transform.position + new Vector2(transform.localScale.x * _moverRef.DirectionFacing, 0.0f);
		}

		if (attackSpawnPos == Vector2.zero)
		{
			_currentAction = ActionType.Idle;
			_awaitingResult = false;
			return;
		}

		var tempQuery = (GameObject) Instantiate(AttackQueryPrefab, attackSpawnPos, new Quaternion());
		var tempResolver = tempQuery.GetComponent<AttackResolver>();

		if (tempResolver)
		{
			tempResolver.CallerResultCallback = GetActionResult;
			tempResolver.CallerInfectionTarget = other => _attemptingToInfectTarget = other;
			tempResolver.SpecificTarget = _attemptingToInfectTarget;
			tempResolver.CallingGameObject = this.gameObject;
			tempResolver.CallerStats = _statsRef;

			tempResolver.CurrentAttackType = CurrentAttack;
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

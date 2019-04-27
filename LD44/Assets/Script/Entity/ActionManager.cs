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
		Lethal, //Attack a target looking to kill them
		Infect  //Attack a target looking to spread infection - slower than trying to kill (riskier)
	}

	public enum AttackResult
	{
		InvalidTarget,	//i.e. terrain, nothing

		VictimDeath,
		VictimInfected,
		VictimOkay
	}

	/// <summary>
	/// The object we spawn process an attack
	/// </summary>
	public GameObject AttackQueryPrefab = null;

	/// <summary>
	/// A toggle between wanting to kill or infect
	/// </summary>
	public AttackType CurrentAttack;

	private ActionType _currentAction;
	private bool _awaitingResult;

	public delegate void cbAttackResolution(AttackResult eResult);

	private ActorMovement _moverRef = null;

	public bool DoAction(ActionType newAction)
	{
		if (_currentAction != ActionType.Idle)
		{
			return false;
		}

		_currentAction = newAction;
		_awaitingResult = false;
		return true;

	}

	public void ToggleAttackMode()
	{
		CurrentAttack = CurrentAttack == AttackType.Infect ? AttackType.Lethal : AttackType.Infect;
	}
	
	void Start()
    {
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");
	}
	
    void Update()
    {
	    if (_awaitingResult)
	    {
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
		Debug.Log("Action result received");
	    _currentAction = ActionType.Idle;
	    _awaitingResult = false;
    }

    void DoAttack()
    {
		//TODO stats for range and size of attack

		//TODO improve where the bounds of the attack are
		var attackSpawnPos = (Vector2)transform.position + new Vector2(transform.localScale.x * _moverRef.DirectionFacing, 0.0f);
		var tempQuery = (GameObject) Instantiate(AttackQueryPrefab, attackSpawnPos, new Quaternion());

		var tempResolver = tempQuery.GetComponent<AttackResolver>();

		if (tempResolver)
		{
			tempResolver.CallerResultCallback = GetActionResult;
		}

		tempResolver.ReadyForQuery = true;
		_awaitingResult = true;
		

    }

    void DoDash()
    {

    }
}

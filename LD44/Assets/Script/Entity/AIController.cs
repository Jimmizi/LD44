using System;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Control the actor via code, and a direction calculated for the AI to move in
/// </summary>

[RequireComponent(typeof(ActorMovement))]
[RequireComponent(typeof(ActionManager))]

public class AIController : MonoBehaviour
{
	public enum AITask
	{
		Wander,			//Randomly around the map
		AttackTarget,	//Go to a target and start using attack actions
		DefendTarget,	//Stick around a target
		Flee,			//Flee from a target
		Panic			//Erratic wander
	}

	private AITask _currentTask = AITask.AttackTarget;

	private ActorMovement _moverRef = null;
	private ActionManager _actionRef = null;
	private PathfinderManager _pathfinder = null;
	private ActorStats _statsRef = null;

	// Pathing Vars
	List<Node> _currentPath = new List<Node>();
	private int _currentNode;
	private float _endOfPathWaitTime;
	private bool _waitingOnPathResult;

	//Target vars
	private GameObject _taskTarget = null;
	private Vector2 _originalTargetPlace;

	private Vector2 _currentDirection;

	public bool ShouldRunFaster()
	{
		return _currentTask == AITask.Panic;
	}

    void Start()
    {
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");

		_actionRef = GetComponent<ActionManager>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActionManager.");

		_pathfinder = GameObject.FindObjectOfType<PathfinderManager>();
		Debug.Assert(_pathfinder != null, "Didn't manage to find a PathfinderManager.");

		_statsRef = GetComponent<ActorStats>();
		Debug.Assert(_statsRef != null, "Didn't manage to find a ActorStats.");
	}


    void Update()
    {
	    //If we're not infected and we're on the process of being so, stop all movement
	    if (_statsRef.BeingInfectedBy != null && !_statsRef.Infected)
	    {
			//TODO Play "being infected" anim or particle effects and/or sound
			_moverRef.Direction = Vector2.zero;

			return;
	    }

		switch (_currentTask)
		{
			case AITask.Wander:
			case AITask.Panic:
			{
				ProcessTaskWander();
				break;
			}
			case AITask.AttackTarget:
			{
				ProcessTaskAttackTarget();
				break;
			}
			case AITask.DefendTarget:
				break;
			case AITask.Flee:
				break;
			
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

    public void cbPathResult(List<Node> path)
    {
	    if (path != null && path.Count > 0)
	    {
		    _currentPath = path;
	    }

		_waitingOnPathResult = false;

    }

    private void ProcessTaskAttackTarget()
    {
		//TODO Proper selection than just the player as a target
		if (_taskTarget == null)
		{
			_taskTarget = GameObject.FindGameObjectWithTag("Player");
		}

		if (_taskTarget == null)
		{
			_moverRef.Direction = Vector2.zero;
			return;
		}

		if (_actionRef.DoingAnAction)
		{
			_moverRef.Direction = Vector2.zero;
			return;
		}

		//If we're close enough, attack the target
		if (((Vector2) _taskTarget.transform.position - (Vector2)this.transform.position).magnitude < 0.4f)
		{
			_actionRef.DoAction(ActionManager.ActionType.Attack);
			_actionRef.TargetLocationForAction = _taskTarget.transform.position;
		}
		else if (_currentPath == null || _currentPath.Count == 0)
		{
			if (!_waitingOnPathResult)
			{
				_pathfinder.RequestPathfind(this.transform.position, _taskTarget.transform.position, cbPathResult);
				_originalTargetPlace = _taskTarget.transform.position;
				_waitingOnPathResult = true;
			}
		}
		//We've done our current one
		else if (_currentNode >= _currentPath.Count)
		{
			_currentPath.Clear();
			_currentNode = 0;
			
		}
		//Recalculate if the target has moved 
		else if (((Vector2)_taskTarget.transform.position - _originalTargetPlace).magnitude >= 0.3f)
		{
			_currentPath.Clear();
			_currentNode = 0;

			_moverRef.Direction /= 5;
		}
		else
		{
			//Otherwise travel in the direction of the next node
			var direction = _currentPath[_currentNode].position - transform.position;

			var normal = transform.forward;
			Vector3.OrthoNormalize(ref normal, ref direction);

			_moverRef.Direction = direction;

			if ((transform.position - _currentPath[_currentNode].position).magnitude <= 0.25f)
			{
				_currentNode++;

				if (_currentNode >= _currentPath.Count)
				{
					_moverRef.Direction = Vector2.zero;
				}
			}
		}
	}
	
	private void ProcessTaskWander()
    {
		//Random wait at path end
	    if (_endOfPathWaitTime > 0.0f)
	    {
		    _endOfPathWaitTime -= Time.deltaTime;
		    return;
	    }

		//Find a new path
	    if (_currentPath == null || _currentPath.Count == 0)
	    {
		    if (!_waitingOnPathResult)
		    {
			    _pathfinder.RequestPathfind(this.transform.position, _pathfinder.GetRandomPointInBounds(), cbPathResult);
			    _waitingOnPathResult = true;
		    }
	    }
		//We've done our current one
	    else if(_currentNode >= _currentPath.Count)
	    {
		    _currentPath = null;
		    _currentNode = 0;

		    _endOfPathWaitTime = Random.Range(0.5f, 3.0f);
		}
	    else
	    {
			//Otherwise travel in the direction of the next node
		    var direction = _currentPath[_currentNode].position - transform.position;

			var normal = transform.forward;
			Vector3.OrthoNormalize(ref normal, ref direction);
			
			_moverRef.Direction = direction;

			if ((transform.position - _currentPath[_currentNode].position).magnitude <= 0.25f)
			{
				_currentNode++;

				if (_currentNode >= _currentPath.Count)
				{
					_moverRef.Direction = Vector2.zero;
				}
			}
		}
	}
}

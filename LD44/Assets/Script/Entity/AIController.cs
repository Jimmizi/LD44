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

	private AITask _currentTask;

	private ActorMovement _moverRef = null;
	private ActionManager _actionRef = null;
	private PathfinderManager _pathfinder = null;

	// Pathing Vars
	List<Node> _currentPath = new List<Node>();
	private int _currentNode;
	private float _endOfPathWaitTime;
	private bool _waitingOnPathResult;

	private Vector2 _currentDirection;
	

    void Start()
    {
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");

		_actionRef = GetComponent<ActionManager>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActionManager.");

		_pathfinder = GameObject.FindObjectOfType<PathfinderManager>();
		Debug.Assert(_pathfinder != null, "Didn't manage to find a PathfinderManager.");
	}


    void Update()
    {
		switch (_currentTask)
		{
			case AITask.Wander:
			{
				ProcessTaskWander();
				break;
			}
			case AITask.AttackTarget:
				break;
			case AITask.DefendTarget:
				break;
			case AITask.Flee:
				break;
			case AITask.Panic:
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
	
	private void ProcessTaskWander()
    {
	    if (_endOfPathWaitTime > 0.0f)
	    {
		    _endOfPathWaitTime -= Time.deltaTime;
		    return;
	    }

	    if (_currentPath == null || _currentPath.Count == 0)
	    {
		    if (!_waitingOnPathResult)
		    {
			    _pathfinder.RequestPathfind(this.transform.position, _pathfinder.GetRandomPointInBounds(), cbPathResult);
			    _waitingOnPathResult = true;
		    }
	    }
	    else if(_currentNode >= _currentPath.Count)
	    {
		    _currentPath = null;
		    _currentNode = 0;

		    _endOfPathWaitTime = Random.Range(0.5f, 3.0f);
		}
	    else
	    {
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

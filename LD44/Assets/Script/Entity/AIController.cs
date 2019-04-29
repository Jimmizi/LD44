using Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Control the actor via code, and a direction calculated for the AI to move in
/// </summary>

[RequireComponent(typeof(ActorMovement))]
[RequireComponent(typeof(ActionManager))]

public class AIController : MonoBehaviour
{

	private const float REACHED_NODE_DISTANCE = 0.25f;
	private const float REEVALUATE_DIST_FOR_TARGET_HAVING_MOVED = 0.3f;


	public enum AITask
	{
		Wander,			//Randomly around the map
		AttackTarget,	//Go to a target and start using attack actions
		DefendTarget,	//Stick around a target
		Flee,			//Flee from a target
		Panic			//Erratic wander
	}

	public AITask CurrentTask = AITask.Wander;

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
	private float _timeSinceLastAttackAction;
	private int _targetFindAttempts;

	private Vector2 _currentDirection;

	public bool ShouldRunFaster()
	{
		return CurrentTask == AITask.Panic || CurrentTask == AITask.Flee;
	}

	public void SetDesiredAttackType(ActionManager.AttackType newType)
	{
		//TODO perhaps redo this?
		//Not caring about a proper way to do this right now, just randomise whether the character takes this new type

		//30% of your bodies will ignore your change attack priority order //TODO too confusing for player?
		if (Random.Range(0, 101) < 30)
		{
			return;
		}

		if (GetComponent<ActionManager>())
		{
			GetComponent<ActionManager>().CurrentAttack = newType;
		}
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


    void FixedUpdate()
    {
	    //If we're not infected and we're on the process of being so, stop all movement
	    if (_statsRef.BeingInfectedBy != null && !_statsRef.Infected)
	    {
			//TODO Play "being infected" anim or particle effects and/or sound
			_moverRef.Direction = Vector2.zero;

			return;
	    }
		
		switch (CurrentTask)
		{
			case AITask.Wander:
			case AITask.Panic:
			case AITask.Flee:
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
			
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

    public void RespondToInfected()
    {
	    CurrentTask = AITask.AttackTarget;
	    ClearTarget();
    }

    public float GetDistFromNodeToEndOfPath(int node)
    {
	    var runningDist = 0f;

	    for (int i = node; node < _currentPath.Count-1; i++)
	    {
		    runningDist += ((Vector2) _currentPath[i].position - (Vector2) _currentPath[i + 1].position).magnitude;
	    }

	    return runningDist;
    }

    public int GetNextNodeNearestToTarget()
    {
	    var myDistances = new List<float>();
	    var targetDistances = new List<float>();
	    var nodeDistancesToEnd = new List<float>();

		var myClosestIndex = 0;
	    var myClosestDist = 99999f;
	    var myDistToEnd = 0.0f;

	    var targetClosestIndex = 0;
	    var targetClosestDist = 99999f;

		for (int i = 0; i < _currentPath.Count; i++)
	    {
		    myDistances.Add(((Vector2)_currentPath[i].position - (Vector2) this.transform.position).magnitude);
		    //nodeDistancesToEnd.Add(GetDistFromNodeToEndOfPath(i));
		    //targetDistances.Add(((Vector2)_currentPath[i].position - (Vector2)_currentPath[_currentPath.Count-1].position).magnitude);
	    }
		
		for (int i = 0; i < myDistances.Count; i++)
		{
			if (myDistances[i] < myClosestDist)
			{
				myClosestDist = myDistances[i];
				myClosestIndex = i;
			}
		}

		//Could find a nice way to find the next node via distance, but just
		//	get the closest node and start going to the next one
		if (myClosestIndex + 1 < _currentPath.Count)
		{
			myClosestIndex++;
		}

		if (myClosestIndex < 0 || !_pathfinder.IsPointWithinPlayableArea(_currentPath[myClosestIndex].position))
		{
			myClosestIndex = 0;
		}

		return Math.Max(0, myClosestIndex);
		//myDistToEnd = GetDistFromNodeToEndOfPath(myClosestIndex);
    }

    public void RespondToPlayerSpawned()
    {
	    if (_statsRef.Neutral)
	    {
		    CurrentTask = Random.Range(0, 101) < 50 ? AITask.Panic : AITask.Flee;
		    _currentPath.Clear();
		    _endOfPathWaitTime = 0.0f;

		    _currentNode = 0;
	    }
	    else
	    {
			CurrentTask = AITask.AttackTarget;
	    }
    }

    public void cbPathResult(List<Node> path)
    {
	    if (!this)
		    return;

	    if (path != null && path.Count > 0)
	    {
		    _currentPath.Clear();
		    _currentPath = path;
		    _currentNode = GetNextNodeNearestToTarget();
	    }
	    
		_waitingOnPathResult = false;

    }

    private GameObject FindSuitableTargetToAttack()
    {
	    var allActors = GameObject.FindObjectsOfType<ActorStats>();
		var enemiesToTarget = new List<GameObject>();

		// Weed out all the same type as me
		foreach (var actor in allActors)
		{
			//If the actor we're looking at doesn't have the same stat of infection as me, they are a target
			if (actor.Infected == _statsRef.Infected)
			{
				continue;
			}

			//Don't pick an actor in the process of being infected
			if (actor.BeingInfectedBy != null && !actor.Infected)
			{
				continue;
			}

			if (actor.GetComponent<KillActor>())
			{
				continue;
			}

			enemiesToTarget.Add(actor.gameObject);
		}

		//Exit out if no enemies, if only one just use that
		if (enemiesToTarget.Count == 0)
		{
			return null;
		}
		else if (enemiesToTarget.Count == 1)
		{
			return enemiesToTarget[0];
		}

	    var randomChance = Random.Range(0, 101);

		//Random chance between random enemy, picking the closest, and picking the furthest
		if (randomChance >= 0 && randomChance < 25)
		{
			return enemiesToTarget[Random.Range(0, enemiesToTarget.Count)];
		}

		float lowestScore = 9999.9f;
		int lowestScoreIndex = -1;

		float highestScore = 0.0f;
		int highestScoreIndex = -1;

		//Get the furthest and closest
		for (var i = 0; i < enemiesToTarget.Count; i++)
		{
			var distance = ((Vector2)enemiesToTarget[i].transform.position - (Vector2)this.transform.position).magnitude;

			if (distance < lowestScore)
			{
				lowestScore = distance;
				lowestScoreIndex = i;
			}

			if (distance > highestScore)
			{
				highestScore = distance;
				highestScoreIndex = i;
			}
		}
		
		//random chances to pick the furthest or closest
		if (highestScoreIndex != -1 && randomChance >= 25 && randomChance < 25)
		{
			return enemiesToTarget[highestScoreIndex];
		}
		else if(lowestScoreIndex != -1)
		{
			return enemiesToTarget[lowestScoreIndex];
		}

		//Else select a random one as a fallback
		return enemiesToTarget[Random.Range(0, enemiesToTarget.Count)];
	}

    private void ClearTarget()
    {
	    _actionRef.TargetLocationForAction = Vector2.zero;
	    _currentPath.Clear();
	    _endOfPathWaitTime = 0.0f;
	    _originalTargetPlace = Vector2.zero;
	    _moverRef.Direction = Vector2.zero;
		_currentNode = 0;
	    _timeSinceLastAttackAction = 0;
	    _targetFindAttempts = 0;
		_taskTarget = null;
    }

    private void ProcessTaskAttackTarget()
	{
	    _moverRef.Direction = Vector2.zero;

		if (_taskTarget == null)
		{
			_taskTarget = FindSuitableTargetToAttack();
			_moverRef.Direction = Vector2.zero;

			if (_taskTarget == null)
			{
				_targetFindAttempts++;

				if (_targetFindAttempts >= 4)
				{
					CurrentTask = AITask.Wander;
					_targetFindAttempts = 0;
				}
				
				return;
			}

			_targetFindAttempts = 0;
		}
		
		//Retarget if our target has begun being infected, and not by me
		if (!_waitingOnPathResult && _statsRef.BeingInfectedBy != null && !_statsRef.Infected)
		{
			if (_statsRef.BeingInfectedBy != this.gameObject)
			{
				ClearTarget();
				return;
			}
		}

		if (_taskTarget.GetComponent<KillActor>())
		{
			ClearTarget();
			return;
		}

		//TODO Maybe retarget if we see something else is right next to us

		//Retarget if the target is now infected
		if (!_waitingOnPathResult && _statsRef.Infected && _taskTarget.GetComponent<ActorStats>().Infected)
		{
			ClearTarget();
			return;
		}
		
		_timeSinceLastAttackAction += Time.deltaTime;
		if (_timeSinceLastAttackAction >= 7.5f)
		{
			if (((Vector2)_taskTarget.transform.position - (Vector2)this.transform.position).magnitude > _statsRef.AttackRange * 2.0f)
			{
				ClearTarget();
				return;
			}
		}

		Vector2 customMovetoPoint = Vector2.zero;

		if (_statsRef.UsesAoeAttack)
		{
			if (_actionRef.CanAttack)
			{
				_actionRef.DoAction(ActionManager.ActionType.Attack);
				_actionRef.TargetLocationForAction = this.transform.position;
				_timeSinceLastAttackAction = 0.0f;
			}
		}
		//If we're close enough, attack the target
		else if (((Vector2) _taskTarget.transform.position - (Vector2)this.transform.position).magnitude < _statsRef.AttackRange)
		{
			if (_actionRef.CanAttack)
			{
				_actionRef.DoAction(ActionManager.ActionType.Attack);
				_actionRef.TargetLocationForAction = _taskTarget.transform.position;
				_timeSinceLastAttackAction = 0.0f;
			}
			//TODO maybe reintroduce this - atm enemies can just sit inside their target
			//customMovetoPoint = _taskTarget.transform.position;
		}
		
		if (_currentPath == null || _currentPath.Count == 0)
		{
			_moverRef.Direction = Vector2.zero;
			if (!_waitingOnPathResult)
			{
				if(_pathfinder.RequestPathfind(this.transform.position, _taskTarget.transform.position, this, cbPathResult))
				{ 
					_originalTargetPlace = _taskTarget.transform.position;
					_waitingOnPathResult = true;
				}
			}
		}
		//We've done our current one
		else if (_currentNode >= _currentPath.Count)
		{
			_currentPath.Clear();
			_currentNode = 0;
		}
		//Recalculate if the target has moved 
		else if (((Vector2)_taskTarget.transform.position - _originalTargetPlace).magnitude >= REEVALUATE_DIST_FOR_TARGET_HAVING_MOVED)
		{
			if (!_waitingOnPathResult)
			{
				if(_pathfinder.RequestPathfind(this.transform.position, _taskTarget.transform.position, this, cbPathResult))
				{ 
					_originalTargetPlace = _taskTarget.transform.position;
					_waitingOnPathResult = true;
				}
			}
		}
		else
		{
			//Otherwise travel in the direction of the next node
			var direction = (Vector2)_currentPath[_currentNode].position - (Vector2)transform.position;
			var orthDir = new Vector3(direction.x, direction.y, 0);

			var normal = transform.forward;
			Vector3.OrthoNormalize(ref normal, ref orthDir);

			_moverRef.Direction = orthDir;

			if ((transform.position - _currentPath[_currentNode].position).magnitude <= REACHED_NODE_DISTANCE)
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
		    _moverRef.Direction = Vector2.zero;
			return;
	    }

		//Find a new path
	    if (_currentPath == null || _currentPath.Count == 0)
	    {
		    if (!_waitingOnPathResult)
		    {
			    _waitingOnPathResult = _pathfinder.RequestPathfind(this.transform.position, _pathfinder.GetRandomPointInBounds(CurrentTask != AITask.Wander), this, cbPathResult);
			    _moverRef.Direction = Vector2.zero;
			}
	    }
		//We've done our current one
	    else if(_currentNode >= _currentPath.Count)
	    {
		    _currentPath.Clear();
		    _currentNode = 0;
		    _moverRef.Direction = Vector2.zero;

			if (CurrentTask == AITask.Wander)
		    {
			    _endOfPathWaitTime = Random.Range(0.5f, 3.0f);
		    }
	    }
	    else
	    {
			//Otherwise travel in the direction of the next node
		    var direction = _currentPath[_currentNode].position - transform.position;

			var normal = transform.forward;
			Vector3.OrthoNormalize(ref normal, ref direction);
			
			_moverRef.Direction = direction;

			if ((transform.position - _currentPath[_currentNode].position).magnitude <= REACHED_NODE_DISTANCE)
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

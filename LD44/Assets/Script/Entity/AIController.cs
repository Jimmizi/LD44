using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the actor via code, and a direction calculated for the AI to move in
/// </summary>

[RequireComponent(typeof(ActorMovement))]
[RequireComponent(typeof(ActionManager))]

public class AIController : MonoBehaviour
{
	private ActorMovement _moverRef = null;
	private ActionManager _actionRef = null;

	private Vector2 _currentDirection;
	private float _testTimer;

    void Start()
    {
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");

		_actionRef = GetComponent<ActionManager>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActionManager.");
	}


    void Update()
    {
	    return;

		/// Something to just randomly move about until we get to a route planning stage
        if(_testTimer > 0.5f)
		{
			_testTimer = 0.0f;
			_currentDirection = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
		}

		_moverRef.Direction = _currentDirection;
		_testTimer += Time.deltaTime;
	}
}

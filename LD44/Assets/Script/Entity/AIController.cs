using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the actor via code, and a direction calculated for the AI to move in
/// </summary>

[RequireComponent(typeof(ActorMovement))]
public class AIController : MonoBehaviour
{
	private ActorMovement _moverRef = null;

	private Vector2 _currentDirection;
	private float _testTimer;

    void Start()
    {
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");
	}


    void Update()
    {
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

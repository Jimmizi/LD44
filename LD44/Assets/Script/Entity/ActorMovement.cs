using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Takes a directional input from a controller and applies that to the actor
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class ActorMovement : MonoBehaviour
{
	public Vector2 Direction;
	public int DirectionFacing = 1; //-1 for Left, 1 for right
	

	private Rigidbody2D _rigidbodyRef = null;
	private ActionManager _actionManager = null;
	private AIController _aiController = null;
	private ActorStats _statsRef = null;

    void Start()
    {
		_rigidbodyRef = GetComponent<Rigidbody2D>();
		Debug.Assert(_rigidbodyRef != null, "Didn't manage to find a rigidbody.");

		_actionManager = GetComponent<ActionManager>();
		Debug.Assert(_actionManager != null, "Didn't manage to find a ActionManager.");

		_statsRef = GetComponent<ActorStats>();
		Debug.Assert(_statsRef != null, "Didn't manage to find a ActorStats.");

		_aiController = GetComponent<AIController>();

		if (_rigidbodyRef)
		{
			_rigidbodyRef.freezeRotation = true;
			_rigidbodyRef.drag = 10.0f;
			_rigidbodyRef.gravityScale = 0.0f;
		}
	}
	
    void FixedUpdate()
    {
	    if (_actionManager?.CurrentAttack == ActionManager.AttackType.Infect)
	    {
		    return;
	    }

	    var tempSpeed = _statsRef.MovementSpeed;
		

		_rigidbodyRef.velocity = Direction * tempSpeed;

		if (Direction.x != 0.0f)
		{
			DirectionFacing = Direction.x < 0.0f ? -1 : 1;
		}
	}
}

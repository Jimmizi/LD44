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
	public float Speed = 2.0f;

	private Rigidbody2D _rigidbodyRef = null;


    void Start()
    {
		_rigidbodyRef = GetComponent<Rigidbody2D>();
		Debug.Assert(_rigidbodyRef != null, "Didn't manage to find a rigidbody.");

		if(_rigidbodyRef)
		{
			_rigidbodyRef.freezeRotation = true;
			_rigidbodyRef.drag = 10.0f;
			_rigidbodyRef.gravityScale = 0.0f;
		}
	}
	
    void FixedUpdate()
    {
		_rigidbodyRef.velocity = Direction * Speed;

		if (Direction.x != 0.0f)
		{
			DirectionFacing = Direction.x < 0.0f ? -1 : 1;
		}
	}
}

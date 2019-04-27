using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the actor via physical input
/// </summary>

[RequireComponent(typeof(ActorMovement))]
public class PlayerController : MonoBehaviour
{
	private Vector2 _playerInput;
	private ActorMovement _moverRef = null;

	// Start is called before the first frame update
	void Start()
    {
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");


	}
	
    void Update()
    {
        _playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		_moverRef.Direction = _playerInput;
	}
}

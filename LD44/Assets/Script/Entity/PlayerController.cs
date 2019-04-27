using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Control the actor via physical input
/// </summary>

[RequireComponent(typeof(ActorMovement))]
[RequireComponent(typeof(ActionManager))]
public class PlayerController : MonoBehaviour
{
	private Vector2 _playerInput;
	private ActorMovement _moverRef = null;
	private ActionManager _actionRef = null;

	// Start is called before the first frame update
	void Start()
    {
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");

		_actionRef = GetComponent<ActionManager>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActionManager.");
	}
	
    void Update()
    {
        _playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		_moverRef.Direction = _playerInput;

		// Toggle between attack modes
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			//_actionRef.ToggleAttackMode();
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			//TODO Player attack should find the nearest enemy, not just a attack on a adjacent square
			_actionRef.DoAction(ActionManager.ActionType.Attack);
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			//_actionRef.DoAction(ActionManager.ActionType.Dash);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	private FlowManager _flowRef = null;

	public Text PlayerAttackTypeText;

	// Start is called before the first frame update
	void Start()
    {
		_moverRef = GetComponent<ActorMovement>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActorMovement.");

		_actionRef = GetComponent<ActionManager>();
		Debug.Assert(_moverRef != null, "Didn't manage to find a ActionManager.");

		//PlayerAttackTypeText = GameObject.FindGameObjectWithTag("PlayerAttackType")?.GetComponent<Text>();
		//Debug.Assert(_moverRef != null, "Didn't manage to find a ActionManager.");

		_flowRef = GameObject.FindGameObjectWithTag("FlowManager")?.GetComponent<FlowManager>();
		Debug.Assert(_flowRef != null, "Didn't manage to find a FlowManager.");
	}
	
    void Update()
    {
	    if (_flowRef && _flowRef.IsRoundOver())
	    {
		    _moverRef.Direction = Vector2.zero;

			return;
	    }

        _playerInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		_moverRef.Direction = _playerInput;

		//NOTE: No longer how infection works
		// Toggle between attack modes
		//if (Input.GetKeyDown(KeyCode.Q))
		//{
		//	_actionRef.ToggleAttackMode();

		//	if (PlayerAttackTypeText)
		//	{
		//		PlayerAttackTypeText.text = "(Q) " + (_actionRef.CurrentAttack == ActionManager.AttackType.Lethal ? "Player Attacks Are Lethal" : "Player Attacks Are Infectious");
		//	}
		//}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			_actionRef.DoAction(ActionManager.ActionType.Attack);
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			//_actionRef.DoAction(ActionManager.ActionType.Dash);
		}
	}
}

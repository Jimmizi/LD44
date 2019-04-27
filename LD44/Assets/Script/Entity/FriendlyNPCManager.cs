using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script that manages the NPCs friendly to the player
/// </summary>

public class FriendlyNPCManager : MonoBehaviour
{


	public Text PreferredTypeText;
	
	
	private ActionManager.AttackType _aiPreferredAttack;

	public void SwitchPreferredAttackType()
	{
		if (_aiPreferredAttack == ActionManager.AttackType.InfectAttempt)
		{
			_aiPreferredAttack = ActionManager.AttackType.Lethal;
			PreferredTypeText.text = "Friendlies go for kills.";
		}
		else
		{
			_aiPreferredAttack = ActionManager.AttackType.InfectAttempt;
			PreferredTypeText.text = "Friendlies go for infecting.";
		}


	}

	public void SetButtonActive()
	{
		PreferredTypeText.gameObject.transform.parent.gameObject.SetActive(true);
	}

	void Start()
    {
		PreferredTypeText.text = "Friendlies go for kills.";
		PreferredTypeText.gameObject.transform.parent.gameObject.SetActive(false);
    }


    void Update()
    {
        
    }


}

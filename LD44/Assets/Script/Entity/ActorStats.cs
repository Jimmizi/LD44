using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

public class ActorStats : MonoBehaviour
{

	/// <summary>
	/// Denotes whether this actor participates in simulation.
	/// </summary>
	public bool Active = true;

	#region Tuning - Per actor

	/// <summary>
	/// How much health does this character have left?
	/// </summary>
	public int Health = 100;

	/// <summary>
	/// How much damage does this character do to another?
	/// </summary>
	public int Damage = 20;

	/// <summary>
	/// How fast does the character move?
	/// </summary>
	public float MovementSpeed = 0.5f;

	/// <summary>
	/// How fast does this character attack? How little of a delay between attack input and the attack happening is there?
	/// </summary>
	public float AttackSpeed = 0.25f;

	/// <summary>
	/// How far away can this character attack from?
	/// </summary>
	public float AttackRange = 0.425f; //a good default melee range

	
	//NOTE: No longer how infection works
	//public float InfectionSpeed = 4.5f;

	/// <summary>
	/// The chance per hit of infecting a target
	/// Range: 0.0f - 100.0f
	/// </summary>
	public float InfectionChance = 90.0f;

	/// <summary>
	/// The chance on killing a target that they clone
	/// Range: 0.0f - 100.0f
	/// </summary>
	public float CloningChance = 5.0f;

	/// <summary>
	/// Is this character infected?
	/// </summary>
	public bool Infected;

	/// <summary>
	/// Is this character neutral? (being neutral means they won't attack)
	/// </summary>
	public bool Neutral;

	/// <summary>
	/// The character is currently being infected by another character
	/// </summary>
	public GameObject BeingInfectedBy;

	/// <summary>
	/// Does this character do a bigger area of affect attack
	/// </summary>
	public bool UsesAoeAttack;


	public int ExtraHealthIntroducedAtDifficultyLevel = 3;
	public int ExtraHealthPerDifficultyStep = 10;

	public int ExtraDamageIntroducedAtDifficultyLevel = 2;
	public int ExtraDamagePerDifficultyStep = 5;

	public int ExtraAttackSpeedIntroducedAtDifficultyLevel = 4;
	public float ExtraAttackSpeedPerDifficultyStep = 0.025f;

	//public int ExtraMovementSpeedIntroducedAtDifficultyLevel = 2;
	//public float ExtraMovementSpeedPerDifficultyStep = 0.05f;

	#endregion

	/// <summary>
	/// Difficulty setup for enemies to the player
	/// </summary>
	/// <param name="difficultyLevel"></param>
	public void SetupDifficulty(int difficultyLevel, float modifier = 1.0f)
	{
		int extraHealth = 0, extraDamage = 0;
		float extraSpeed = 0.0f, extraMovement = 0.0f;

		//Neutrals only get half of the extra stats
		if (difficultyLevel >= ExtraHealthIntroducedAtDifficultyLevel)
		{
			extraHealth = (ExtraHealthPerDifficultyStep / (Neutral ? 2 : 1)) * (difficultyLevel - (ExtraHealthIntroducedAtDifficultyLevel - 1));
			extraHealth = (int)(extraHealth * modifier);
		}

		if (difficultyLevel >= ExtraDamageIntroducedAtDifficultyLevel)
		{
			extraDamage = (ExtraDamagePerDifficultyStep / (Neutral ? 2 : 1)) * (difficultyLevel - (ExtraDamageIntroducedAtDifficultyLevel - 1));
			extraDamage = (int)(extraDamage * modifier);
		}

		if (difficultyLevel >= ExtraAttackSpeedIntroducedAtDifficultyLevel)
		{
			extraSpeed = (ExtraAttackSpeedPerDifficultyStep / (Neutral ? 2 : 1)) * (difficultyLevel - (ExtraAttackSpeedIntroducedAtDifficultyLevel - 1));
			extraSpeed *= modifier;
		}

		//if (difficultyLevel >= ExtraMovementSpeedIntroducedAtDifficultyLevel)
		//{
			//extraMovement = (EXTRA_MOVEMENT_SPEED_PER_DIFFICULTY_STEP / (Neutral ? 2 : 1)) * (difficultyLevel - (EXTRA_MOVEMENT_SPEED_INTRODUCED_AT_DIFFICULTY_LEVEL - 1));
			//extraMovement *= modifier;
		//}

		Health += extraHealth;

		Damage += extraDamage;

		AttackSpeed -= extraSpeed;
		AttackSpeed = Mathf.Max(0.05f, AttackSpeed);

		//MovementSpeed += extraMovement;
	}

	public void ApplyPlayerStats()
	{
		DataController dataController = DataController.GetInstance();
		if (dataController != null)
		{
			dataController.initActorFromType(this, 4);	
		}
		
	    UpgradeEffectManager.ApplyPermanentUpgrades(this);
    }

	public void ApplyFriendlyStats()
	{
		DataController dataController = DataController.GetInstance();
		if (dataController != null)
		{
			dataController.initActorFromType(this, 3);	
		}
		
	    UpgradeEffectManager.ApplyPermanentUpgrades(this);
    }
}
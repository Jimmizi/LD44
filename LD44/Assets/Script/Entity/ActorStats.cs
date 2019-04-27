using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorStats : MonoBehaviour
{
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

	/// <summary>
	/// How fast can this character infect another (In seconds)?
	/// </summary>
	public float InfectionSpeed = 4.5f;

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

	private const int EXTRA_HEALTH_INTRODUCED_AT_DIFFICULTY_LEVEL = 3;
	private const int EXTRA_HEALTH_PER_DIFFICULTY_STEP = 10;

	private const int EXTRA_DAMAGE_INTRODUCED_AT_DIFFICULTY_LEVEL = 2;
	private const int EXTRA_DAMAGE_PER_DIFFICULTY_STEP = 5;

	private const int EXTRA_ATTACK_SPEED_INTRODUCED_AT_DIFFICULTY_LEVEL = 4;
	private const float EXTRA_ATTACK_SPEED_PER_DIFFICULTY_STEP = 0.025f;

	private const int EXTRA_MOVEMENT_SPEED_INTRODUCED_AT_DIFFICULTY_LEVEL = 2;
	private const float EXTRA_MOVEMENT_SPEED_PER_DIFFICULTY_STEP = 0.1f;

	/// <summary>
	/// Difficulty setup for enemies to the player
	/// </summary>
	/// <param name="difficultyLevel"></param>
	public void SetupDifficulty(int difficultyLevel)
	{
		//Neutrals only get half of the extra stats
		if (difficultyLevel >= EXTRA_HEALTH_INTRODUCED_AT_DIFFICULTY_LEVEL)
		{
			Health += (EXTRA_HEALTH_PER_DIFFICULTY_STEP / (Neutral ? 2 : 1)) * (EXTRA_HEALTH_INTRODUCED_AT_DIFFICULTY_LEVEL - (difficultyLevel - 1));
		}

		if (difficultyLevel >= EXTRA_DAMAGE_INTRODUCED_AT_DIFFICULTY_LEVEL)
		{
			Damage += (EXTRA_DAMAGE_PER_DIFFICULTY_STEP / (Neutral ? 2 : 1)) * (EXTRA_DAMAGE_INTRODUCED_AT_DIFFICULTY_LEVEL - (difficultyLevel - 1));
		}

		if (difficultyLevel >= EXTRA_ATTACK_SPEED_INTRODUCED_AT_DIFFICULTY_LEVEL)
		{
			AttackSpeed -= (EXTRA_ATTACK_SPEED_PER_DIFFICULTY_STEP / (Neutral ? 2 : 1)) * (EXTRA_ATTACK_SPEED_INTRODUCED_AT_DIFFICULTY_LEVEL - (difficultyLevel - 1));
			AttackSpeed = Mathf.Max(0.05f, AttackSpeed);
		}

		if (difficultyLevel >= EXTRA_MOVEMENT_SPEED_INTRODUCED_AT_DIFFICULTY_LEVEL)
		{
			MovementSpeed += (EXTRA_MOVEMENT_SPEED_PER_DIFFICULTY_STEP / (Neutral ? 2 : 1)) * (EXTRA_MOVEMENT_SPEED_INTRODUCED_AT_DIFFICULTY_LEVEL - (difficultyLevel - 1));
		}
	}

}

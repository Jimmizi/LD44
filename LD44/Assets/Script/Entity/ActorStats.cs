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
	/// How fast does this character attack? How little of a delay between attack input and the attack happening is there?
	/// </summary>
	public float AttackSpeed = 0.25f;

	/// <summary>
	/// How fast can this character infect another (In seconds)?
	/// </summary>
	public float InfectionSpeed = 2.5f;

	/// <summary>
	/// Is this character infected?
	/// </summary>
	public bool Infected;

	/// <summary>
	/// The character is currently being infected by another character
	/// </summary>
	public GameObject BeingInfectedBy;
	
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adding this onto the actor will infect them, transitioning them to the players side
/// </summary>

public class InfectActor : MonoBehaviour
{
	private bool _done = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		//TODO Add to player stats
		//TODO change sprite under like a particle effects hide
		//TODO Play wololololo sound?

		//just a test 
		this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Test32x32_Friendly");

		var tempStats = this.GetComponent<ActorStats>();
		if (tempStats)
		{
			tempStats.Infected = true;
			tempStats.Neutral = false;
			tempStats.ApplyFriendlyStats();
		}

		var tempController = this.GetComponent<AIController>();
		if (tempController)
		{
			tempController.RespondToInfected();
		}

		

		if (_done)
		{
			//Once done just destroy this script
			GameManager.InfectedCellsCount++;
			Destroy(this);
		}
    }
}

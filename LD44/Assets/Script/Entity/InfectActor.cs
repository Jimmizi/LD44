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

		if (_done)
		{
			//Once done just destroy this script
			Destroy(this);
		}
    }
}

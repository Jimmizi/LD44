using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Adding this component to an actor kills it
/// </summary>

public class KillActor : MonoBehaviour
{
	private bool _done = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    if (_done)
	    {
			//TODO Before this point, we will want to spawn other entities, particles, sounds
			Destroy(gameObject);
	    }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


			if (GetComponent<PlayerController>())
			{
				var allActors = GameObject.FindObjectsOfType<ActorStats>().Where(x => x.Infected).ToArray();

				if (allActors.Length > 0)
				{
					var newPlayerActor = allActors[Random.Range(0, allActors.Length)];

					if (newPlayerActor.gameObject.GetComponent<AIController>())
					{
						Destroy(newPlayerActor.gameObject.GetComponent<AIController>());
					}

					newPlayerActor.gameObject.AddComponent<PlayerController>();
					newPlayerActor.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Test32x32");

					Camera.main.gameObject.GetComponent<SimpleCameraLerp>().LerpTarget = newPlayerActor.gameObject.transform;
				}

			}

			Destroy(gameObject);
	    }
    }
}

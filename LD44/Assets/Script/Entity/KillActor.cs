using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fx;

/// <summary>
/// Adding this component to an actor kills it
/// </summary>

public class KillActor : MonoBehaviour
{
	private bool _done = true;
    private bool _triggered = false;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Kill()
    {
	    Destroy(gameObject);
    }

    private void TriggerVisuals()
    {
	    if (_triggered)
	    {
		    return;
	    }
	    
	    foreach (Component component in gameObject.GetComponents<Component>())
	    {
		    ActorMovement av = component as ActorMovement;
		    ActionManager am = component as ActionManager;
		    PlayerController pc = component as PlayerController;
		    ActorStats ac = component as ActorStats;
		    AIController ai = component as AIController;
				
		    if (av != null) av.enabled = false;
		    if (am != null) am.enabled = false;
		    if (pc != null) pc.enabled = false;
		    if (ac != null)
		    {
			    ac.enabled = false;
			    ac.Active = false;
		    }
		    if (ai != null) ai.enabled = false;
				
	    }
			
	    Collider2D cl = gameObject.GetComponent<Collider2D>();
	    if (cl != null) Destroy(cl);
	    
	    DeceaseFx fx = gameObject.GetComponent<DeceaseFx>();
	    if (fx != null)
	    {
		    fx.Trigger(Kill);
	    }
	    else
	    {
		    Kill();
	    }

	    _triggered = true;

    }

    // Update is called once per frame
    void Update()
    {
	    if (_done)
	    {
			//TODO Before this point, we will want to spawn other entities, particles, sounds


			if (GetComponent<PlayerController>())
			{
				var allActors = GameObject.FindObjectsOfType<ActorStats>().Where(x => x.Infected && !x.GetComponent<PlayerController>()).ToArray();

				if (allActors.Length > 0)
				{
					var newPlayerActor = allActors[Random.Range(0, allActors.Length)];

					if (newPlayerActor.gameObject.GetComponent<AIController>())
					{
						Destroy(newPlayerActor.gameObject.GetComponent<AIController>());
					}

					newPlayerActor.gameObject.AddComponent<PlayerController>();
					newPlayerActor.gameObject.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/Test32x32");
					newPlayerActor.GetComponent<ActorStats>()?.ApplyPlayerStats();

					Camera.main.gameObject.GetComponent<SimpleCameraLerp>().LerpTarget = newPlayerActor.gameObject.transform;
				}
				else
				{
				    GameManager.levelManager.GameOver();
				}
			}

	        if (GetComponent<ActorStats>().Infected)
	            GameManager.InfectedCellDies();

			//Destroy(gameObject);
			TriggerVisuals();
	    }
    }
}

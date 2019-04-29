using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{

    [System.Serializable]
    public struct TriggerObject
    {
        public GameObject samplerObject;
    }
    
    public TriggerObject[] triggers;

    public void Trigger()
    {
        if (triggers == null) return;

        foreach (TriggerObject triggerObject in triggers)
        {
            Sampler sampler = triggerObject.samplerObject.GetComponent<Sampler>();
            if (sampler != null)
            {
                sampler.CueWithPitch(Random.Range(0.8f, 1.8f));
            }
        }
    }
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCameraLerp : MonoBehaviour
{
	public Transform LerpTarget;
	public float LerpSpeed = 5.0f;

	private float _originalPositionZ;

    void Start()
    {
	    if (LerpTarget == null)
	    {
		    LerpTarget = GameObject.FindGameObjectWithTag("Player")?.transform;
	    }

	    _originalPositionZ = this.transform.position.z;
    }
	
    void FixedUpdate()
    {
        if(LerpTarget)
        {
	        var newPosition = Vector3.Lerp(this.transform.position, LerpTarget.position, Time.deltaTime * LerpSpeed);
	        newPosition.z = _originalPositionZ;

			this.transform.position = newPosition;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyMe : MonoBehaviour
{
	public float DestroyAfterSeconds = 1.0f;

	private float _timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	    _timer += Time.deltaTime;
	    if (_timer >= DestroyAfterSeconds)
	    {
			Destroy(gameObject);
	    }

    }
}

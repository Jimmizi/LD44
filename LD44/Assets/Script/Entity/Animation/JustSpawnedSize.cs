using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Increases the size of the just spawned npc
/// </summary>

public class JustSpawnedSize : MonoBehaviour
{
	private Vector3 _originalScale = Vector3.zero;
	private Vector3 _currentScale  = Vector3.zero;

	private float _scaleTimer;

    void Start()
    {
	    _originalScale = this.transform.localScale;
		transform.localScale = Vector3.zero;
    }
	
    void FixedUpdate()
    {
	    _scaleTimer += Time.deltaTime;
		
		_currentScale.x = Mathf.Lerp(_currentScale.x, _originalScale.x, Time.deltaTime * 2.0f);
		_currentScale.y = Mathf.Lerp(_currentScale.y, _originalScale.y, Time.deltaTime * 2.0f);
		_currentScale.z = Mathf.Lerp(_currentScale.z, _originalScale.z, Time.deltaTime * 2.0f);

		transform.localScale = _currentScale;

		if (_scaleTimer >= 2.0f)
	    {
		    transform.localScale = _originalScale;
			Destroy(this);
	    }
    }
}

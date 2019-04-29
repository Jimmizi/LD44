using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillAoeAnim : MonoBehaviour
{
	private Animator _animatorRef = null;
	private SpriteRenderer _renderRef = null;

	private bool _startedDestroy;
	private float _destroyTimer;

	void Start()
    {
		_animatorRef = GetComponent<Animator>();
		Debug.Assert(_animatorRef != null, "Didn't manage to find a Animator.");

		_renderRef = GetComponent<SpriteRenderer>();
		Debug.Assert(_renderRef != null, "Didn't manage to find a SpriteRenderer.");
	}
	
    void Update()
    {
	    if (_startedDestroy)
	    {
		    _destroyTimer += Time.deltaTime;

		    var tempColor = _renderRef.color;
		    var tempScale = transform.localScale;
		   
		    tempColor.a -= Time.deltaTime;
		    tempScale += new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime) * 0.15f;

		    if (tempColor.a < 0)
		    {
			    tempColor.a = 0;
		    }
		    
		    transform.localScale = tempScale;
		    _renderRef.color = tempColor;

			if (_destroyTimer > 1.0f)
		    {
				Destroy(this.gameObject);
		    }
	    }
	    else
	    {
		    var currentClipInfo = _animatorRef?.GetCurrentAnimatorClipInfo(0);

		    if (currentClipInfo != null && currentClipInfo[0].clip.name == "AOEAttackIdle")
		    {
			    _startedDestroy = true;
		    }
	    }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class UIStatusText : MonoBehaviour
{
	private float _timer;
	private Text _textRef;
	private Vector3 _worldPosSpawn;

	// Start is called before the first frame update
    void Start()
    {
	    _textRef = GetComponent<Text>();
	    _worldPosSpawn = this.transform.position;

	    _textRef.color = _textRef.text == "+1" ? Color.green : Color.red;

	}

    private void SetPositionFromWorld(Vector3 worldPos)
    {
	    var onScreenPos = Camera.main.WorldToScreenPoint(worldPos);

		//TODO Move it if offscreen

	    transform.position = onScreenPos;

    }

    // Update is called once per frame
    void Update()
    {

	    _timer += Time.deltaTime;

	    var tempColor = _textRef.color;
	    tempColor.a -= Time.deltaTime / 2.5f;
	    _textRef.color = tempColor;


	    var tempPos = _worldPosSpawn;
		tempPos.y += Time.deltaTime * 0.25f;
		_worldPosSpawn = tempPos;

		SetPositionFromWorld(_worldPosSpawn);

		if (_timer >= 2.5f)
	    {
			Destroy(this.gameObject);
	    }
    }
}

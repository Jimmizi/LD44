using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionFade : MonoBehaviour
{
	private float _timer;
	private float _currentAlpha;
	private SpriteRenderer _sprite;

	private bool _doingTransition;

	void Start()
	{
		_sprite = GetComponent<SpriteRenderer>();
	}

	private void SetAlpha(float alpha)
	{
		var tempCol = _sprite.color;
		tempCol.a = alpha;
		_sprite.color = tempCol;
	}

	public bool DoFadeIn(float time)
	{
		if (!_doingTransition)
		{
			_timer = 0.0f;
			_currentAlpha = 1.0f;
			_doingTransition = true;
		}
		else
		{
			_timer += Time.deltaTime;

			_currentAlpha = Mathf.Lerp(_currentAlpha, 0.0f, Time.deltaTime * Mathf.Max(2.0f, time));
			SetAlpha(_currentAlpha);

			if (_timer >= time)
			{
				_doingTransition = false;
				SetAlpha(0.0f);

				return true;
			}
		}

		return false;
	}

	public bool DoFadeOut(float time)
	{
		if (!_doingTransition)
		{
			_timer = 0.0f;
			_currentAlpha = 0.0f;
			_doingTransition = true;
		}
		else
		{
			_timer += Time.deltaTime;

			_currentAlpha = Mathf.Lerp(_currentAlpha, 1.0f, Time.deltaTime * Mathf.Max(2.0f, time));
			SetAlpha(_currentAlpha);

			if (_timer >= time)
			{
				_doingTransition = false;
				SetAlpha(1.0f);

				return true;
			}
		}

		return false;
	}

}

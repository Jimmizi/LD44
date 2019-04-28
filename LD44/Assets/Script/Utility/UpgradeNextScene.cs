using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeNextScene : MonoBehaviour
{
	public static string NextSceneToUse = "";
	private bool _doingFadeIn = true;
	private bool _doingFadeOut = false;
	private bool _startedExitFade = false;
	private TransitionFade _fader;

	// Start is called before the first frame update
	void Start()
    {
		_fader = GameObject.FindObjectOfType<TransitionFade>();
	}

    // Update is called once per frame
    void Update()
    {
		if (_fader)
		{
			if (_doingFadeIn && !_doingFadeOut)
			{
				_doingFadeIn = !_fader.DoFadeIn(2.0f);
			}
			if (_doingFadeOut && !_doingFadeIn)
			{
				_doingFadeOut = !_fader.DoFadeOut(2.0f);
			}
		}

		if (_startedExitFade)
		{
			if (!_doingFadeOut)
			{
				if (NextSceneToUse == "")
				{
					SceneManager.LoadScene("FirstLevel");
				}
				else
				{
					SceneManager.LoadScene(NextSceneToUse);
				}
			}
		}
	}

    public void LoadNextScene()
    {
	    if (!_startedExitFade && !_doingFadeIn)
	    {
		    GameObject.Find("MainCanvas")?.SetActive(false);
		    _startedExitFade = true;
		    _doingFadeOut = true;
		}
	    
	}
}

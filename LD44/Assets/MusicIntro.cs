using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicIntro : MonoBehaviour
{
	public AudioClip MusicIntroToPlay;
	public AudioClip MusicToPlayAfterIntro;

	private AudioSource _asRef = null;

	private float _killTimer = 0.0f;
	private bool _killingBGM;

    // Start is called before the first frame update
    void Start()
    {
	    _asRef = GetComponent<AudioSource>();
	    _asRef.clip = MusicToPlayAfterIntro;
	    _asRef.loop = true;

		_asRef.PlayOneShot(MusicIntroToPlay);
	    _asRef.PlayDelayed(MusicIntroToPlay.length);
    }

    public void KillBGM()
    {
	    _killingBGM = true;

    }

    // Update is called once per frame
    void Update()
    {
	    if (_killingBGM)
	    {
		    _killTimer += Time.deltaTime;

		    _asRef.volume -= Time.deltaTime / 2;
		    if (_killTimer >= 2.0f)
		    {
			    _killingBGM = false;
			    _asRef.volume = 0.0f;
		    }

	    }
    }
}

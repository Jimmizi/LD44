using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicIntro : MonoBehaviour
{
	public AudioClip MusicIntroToPlay;
	public AudioClip MusicToPlayAfterIntro;

	private AudioSource _asRef = null;

    // Start is called before the first frame update
    void Start()
    {
	    _asRef = GetComponent<AudioSource>();
	    _asRef.clip = MusicToPlayAfterIntro;

	    _asRef.PlayOneShot(MusicIntroToPlay);
	    _asRef.PlayDelayed(MusicIntroToPlay.length);

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}

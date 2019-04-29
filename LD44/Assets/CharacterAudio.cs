using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	public AudioClip AttackSound;
	public AudioClip DeathSound;
	public List<AudioClip> MoveSounds = new List<AudioClip>();

	private AudioSource _asRef = null;

    // Start is called before the first frame update
    void Start()
    {
	    _asRef = GetComponent<AudioSource>();
	    if (_asRef)
	    {
		    _asRef.loop = true;
	    }
    }

    public void StopMoveSound()
    {
		_asRef?.Stop();
    }

    public void PlayDeathSound()
    {
	    _asRef?.PlayOneShot(DeathSound);
	}

    public void StartMoveSound()
	{
		if (_asRef)
		{
			_asRef.clip = MoveSounds[Random.Range(0, MoveSounds.Count)];
		}

	    _asRef?.Play();
	}

    public void PlayAttackSound()
    {
	    _asRef?.PlayOneShot(AttackSound);
    }
}

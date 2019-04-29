using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAudio : MonoBehaviour
{
	public AudioClip AttackSound;
	public AudioClip MoveSound;

	private AudioSource _asRef = null;

    // Start is called before the first frame update
    void Start()
    {
	    _asRef = GetComponent<AudioSource>();

    }

    public void PlayAttackSound()
    {
	    _asRef?.PlayOneShot(AttackSound);
    }
}

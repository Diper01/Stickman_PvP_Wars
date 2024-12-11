using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {
    public AudioClip ClipJump;
    public AudioClip ClipDie;
    public AudioClip ClipPistol;
    public AudioClip ClipAutomaton;
    public AudioClip ClipRocket;
    public AudioClip ClipFlame;
    public AudioClip ClipLaser;
    public AudioClip ClipMine;
    
    [SerializeField] AudioSource source;

    public void PlayClip(AudioClip clip) {
        if (GameOptions.Sound)        
            source.PlayOneShot(clip);        
    }
}

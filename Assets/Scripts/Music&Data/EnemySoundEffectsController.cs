using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundEffectsController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    private void PlayerClipPlay(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    
    private void OnEnable()
    { 
        SoundEvents.PlayerCharacterEffects += PlayerClipPlay;
    }

    private void OnDisable()
    {
        SoundEvents.PlayerCharacterEffects -= PlayerClipPlay;
    }
}

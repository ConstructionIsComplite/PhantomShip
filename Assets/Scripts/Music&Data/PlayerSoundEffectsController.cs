using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffectsController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private TimeManager timeManager;

    private void PlayerClipPlay(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    private void Update()
    {
        audioSource.pitch = timeManager.CurrentTimeScale;
    }
    
    private void OnEnable()
    {
        SoundEvents.PlayerCharacterEffects += PlayerClipPlay;
    }

    private void OnDisable()
    {
        SoundEvents.PlayerCharacterEffects -= PlayerClipPlay;
    }

    private void OnValidate()
    {
        timeManager = FindObjectOfType<TimeManager>();
    }
}

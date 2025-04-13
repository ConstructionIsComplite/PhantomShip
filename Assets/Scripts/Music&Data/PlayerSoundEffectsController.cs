using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundEffectsController : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    private void EnemyClipPlay(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    
    private void OnEnable()
    {
        SoundEvents.EnemyEffects += EnemyClipPlay;
    }

    private void OnDisable()
    {
        SoundEvents.EnemyEffects -= EnemyClipPlay;
    }
}

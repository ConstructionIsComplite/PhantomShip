using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundEvents : MonoBehaviour
{
    public static event Action<AudioClip> PlayerCharacterEffects;
    public static event Action<AudioClip> EnemyEffects;

    public static void OnPlayerCharacterEffect(AudioClip clip)
    {
        PlayerCharacterEffects?.Invoke(clip);
    }

    public static void OnEnemyEffects(AudioClip clip)
    {
        EnemyEffects?.Invoke(clip);
    }
    
    
}

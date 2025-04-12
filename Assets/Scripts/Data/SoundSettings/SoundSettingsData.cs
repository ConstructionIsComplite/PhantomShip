using System.Diagnostics.CodeAnalysis;
using UnityEngine;


[CreateAssetMenu(fileName = "SoundSettingsData", menuName = "ScriptableObjects/SoundSettingsData")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SoundSettingsData : ScriptableObject
{
    [Range(0f, 1f)]
    public float musicVolume;
    [Range(0f, 1f)]
    public float effectsVolume;
    [Range(0f, 1f)] 
    public float voiceVolume;

    public void ChangeMusicVolume(float value)
    {
        musicVolume = value;
    }

    public void ChangeEffectsVolume(float value)
    {
        effectsVolume = value;
    }

    public void ChangeVoiceVolume(float value)
    {
        voiceVolume = value;
    }
}


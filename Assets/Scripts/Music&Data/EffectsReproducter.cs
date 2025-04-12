using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsReproducter : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_audioSource;
    [SerializeField]
    private AudioClip m_audioClip;
    
    [SerializeField]
    private SoundSettingsData m_soundSettings;
    

    private void Start()
    {
        m_audioSource.clip = m_audioClip;
        m_audioSource.Play();
    }

    private void Update()
    {
        m_audioSource.volume = 0f;//m_soundSettings.effectsVolume;
        if (m_audioSource.isPlaying == false)
        {
            m_audioSource.Play();
        }
    }
}

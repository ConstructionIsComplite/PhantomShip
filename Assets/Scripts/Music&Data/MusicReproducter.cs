using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MusicReproducter : MonoBehaviour
{
    [SerializeField]
    private AudioSource m_audioSource;
    [SerializeField]
    private SoundSettingsData m_soundSettings;
    [SerializeField]
    private MusicList m_musicList;
    [SerializeField]
    private TimeManager m_timeManager;
    
    private List<AudioClip> m_localMusicList = new();
    
    private float m_startTime;
    private float m_currentTime;
    private float m_lengthOfCurrentTrack;
    private float m_endPoint;
    private float m_smoothStep = 0.05f;

    private float m_updateValue;
    
    private bool m_isRandomPlaying = true;
    private bool m_isMiddlePartPlaying;

    private void Start()
    {
        m_endPoint = m_soundSettings.musicVolume;
        
        FillLocalMusicList();
    }

    private void Update()
    {
        MusicPlayWithFading();
        m_audioSource.pitch = m_timeManager.CurrentTimeScale;
    }

    private void MusicPlayWithFading()
    {
        m_audioSource.volume += (m_endPoint - m_audioSource.volume) * m_smoothStep;
        
        if (!m_audioSource.isPlaying)
        {
            if (m_isRandomPlaying)
            {
                SetRandomClip();
            }

            m_audioSource.Play();
            IncreaseVolume();
        }

        if (IsVolumeEqualsSettingsVolume())
        {
            m_isMiddlePartPlaying = true;
        }

        if (m_isMiddlePartPlaying)
        {
            m_audioSource.volume = m_soundSettings.musicVolume;
        }
        
        m_currentTime = Time.time;
        
        if (IsTrackEnding())
        {
            m_isRandomPlaying = true;
            FadeVolume();
        }
    }
    
    public void SetRandomClip()
    {
        if (m_localMusicList.Count == 0)
        {
            FillLocalMusicList();
        }
        
        var index = (int)(Mathf.Floor(Random.Range(0, m_localMusicList.Count)));
        
        var clip = m_localMusicList[index];
        m_audioSource.clip = clip;
        m_lengthOfCurrentTrack = m_audioSource.clip.length;
        m_localMusicList.RemoveAt(index);
    }

    public void RandomMusicStop()
    {
        m_audioSource.Stop();
    }

    private void FadeVolume()
    {
        m_isMiddlePartPlaying = false;
        m_endPoint = 0f;
    }

    private void IncreaseVolume()
    {
        m_isMiddlePartPlaying = false;
        m_endPoint = m_soundSettings.musicVolume;
        m_startTime = Time.time;
    }

    private void FillLocalMusicList()
    {
        foreach (var track in m_musicList.clips)
        {
            m_localMusicList.Add(track);
        }
    }
    
    private bool IsVolumeEqualsSettingsVolume()
    {
        return Mathf.Abs(m_audioSource.volume - m_soundSettings.musicVolume) < 0.01f;
    }

    private bool IsTrackEnding()
    {
        float remainingTime = m_lengthOfCurrentTrack - (m_currentTime - m_startTime);
        return remainingTime <= 5f;
    }

    private void OnValidate()
    { 
        m_timeManager = FindObjectOfType<TimeManager>();
    }
}

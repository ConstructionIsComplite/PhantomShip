using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField][Range(0.01f, 1f)] float initialTimeScale = 1f;
    [SerializeField][Range(0f, 0.1f)] float slowDownRate = 0.01f;
    [SerializeField][Range(0.01f, 1f)] float minTimeScale = 0.1f;

    private float currentTimeScale;

    public event Action<float> OnTimeScaleChanged; // Событие изменения времени

    public float InitialTimeScale => initialTimeScale;
    public float MinTimeScale => minTimeScale;
    public float CurrentTimeScale => currentTimeScale;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeTime();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeTime()
    {
        currentTimeScale = initialTimeScale;
        UpdateTimeScale();
    }

    void Update()
    {
        if (currentTimeScale > minTimeScale)
        {
            currentTimeScale -= slowDownRate * Time.unscaledDeltaTime;
            currentTimeScale = Mathf.Clamp(currentTimeScale, minTimeScale, initialTimeScale);
            UpdateTimeScale();
        }
    }

    void UpdateTimeScale()
    {
        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = 0.02f * currentTimeScale;
        OnTimeScaleChanged?.Invoke(currentTimeScale); // Вызов события
    }

    public void ResetTimeScale()
    {
        currentTimeScale = initialTimeScale;
        UpdateTimeScale();
    }

    // Для ручного управления при необходимости
    public void SetTimeScale(float newScale)
    {
        currentTimeScale = Mathf.Clamp(newScale, minTimeScale, initialTimeScale);
        UpdateTimeScale();
    }
}

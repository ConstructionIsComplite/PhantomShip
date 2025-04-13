using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [SerializeField][Range(0.01f, 1f)] float initialTimeScale = 1f;
    [SerializeField][Range(0f, 0.1f)] float slowDownRate = 0.01f;
    [SerializeField][Range(0.01f, 1f)] float minTimeScale = 0.1f;

    private bool isTimeStopped = false;
    private float currentTimeScale;
    private float savedTimeScale;

    public event Action<float> OnTimeScaleChanged; // ������� ��������� �������

    public bool IsTimeStopped => isTimeStopped;
    public float InitialTimeScale => initialTimeScale;
    public float MinTimeScale => minTimeScale;
    public float CurrentTimeScale => currentTimeScale;
    private bool isSlowDownPaused = false;

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
        if (isTimeStopped || isSlowDownPaused) return; // ���������� ��������������
        if (currentTimeScale > minTimeScale)
        {
            currentTimeScale -= slowDownRate * Time.unscaledDeltaTime;
            currentTimeScale = Mathf.Clamp(currentTimeScale, minTimeScale, initialTimeScale);
            UpdateTimeScale();

            // �������� �� ����������� �����
            if (currentTimeScale <= minTimeScale)
            {
                GameManager.Instance.TriggerGameOver("TIME FLOW STOPPED");
            }
        }
    }

    void UpdateTimeScale()
    {
        Time.timeScale = currentTimeScale;
        Time.fixedDeltaTime = 0.02f * currentTimeScale;
        OnTimeScaleChanged?.Invoke(currentTimeScale); // ����� �������
    }

    public void RestorePreviousTimeScale()
    {
        currentTimeScale = Mathf.Clamp(currentTimeScale, minTimeScale, initialTimeScale);
        UpdateTimeScale();
    }

    public void ResetTimeScale()
    {
        currentTimeScale = initialTimeScale;
        isTimeStopped = false; // ���� ���� ���� ��������� �������
        UpdateTimeScale();
    }

    // ��� ������� ���������� ��� �������������
    public void SetTimeScale(float newScale)
    {
        if (isTimeStopped) return; // ���������� ��������� �� ����� ���������
        currentTimeScale = Mathf.Clamp(newScale, minTimeScale, initialTimeScale);
        UpdateTimeScale();
    }

    public void StopTime()
    {
        savedTimeScale = currentTimeScale; // ��������� ������� ��������
        isTimeStopped = true;
        currentTimeScale = 0f;
        UpdateTimeScale();
    }

    public void RestoreTime()
    {
        isTimeStopped = false;
        currentTimeScale = savedTimeScale; // ��������������� ����������� ��������
        UpdateTimeScale();
    }

    public void PauseSlowDown(bool pause) => isSlowDownPaused = pause;
}

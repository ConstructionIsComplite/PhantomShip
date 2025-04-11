using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    [Header("Time Settings")]
    [Range(0.01f, 1f)]
    [SerializeField] float globalTimeScale = 1f;

    public float TimeScale => globalTimeScale;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        Time.timeScale = globalTimeScale;
        Time.fixedDeltaTime = 0.02f * globalTimeScale;
    }

    public void SetTimeScale(float newScale)
    {
        globalTimeScale = Mathf.Clamp(newScale, 0.01f, 1f);
    }
}

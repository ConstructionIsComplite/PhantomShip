using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TimeScaleUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Slider timeScaleSlider;
    [SerializeField] TMP_Text timeScaleText; // Изменено на TMP_Text

    void Start()
    {
        if (!timeScaleSlider) timeScaleSlider = GetComponent<Slider>();

        timeScaleSlider.minValue = TimeManager.Instance.MinTimeScale;
        timeScaleSlider.maxValue = TimeManager.Instance.InitialTimeScale;

        TimeManager.Instance.OnTimeScaleChanged += UpdateUI;
        UpdateUI(TimeManager.Instance.CurrentTimeScale);
    }

    void UpdateUI(float currentScale)
    {
        timeScaleSlider.value = currentScale;

        if (timeScaleText)
        {
            timeScaleText.text = $"TIME FLOW: {currentScale * 100:F0}%";
            timeScaleText.fontStyle = currentScale <= 0.3f ?
                FontStyles.Bold | FontStyles.Italic :
                FontStyles.Normal;
        }
    }

    void OnDestroy()
    {
        if (TimeManager.Instance)
            TimeManager.Instance.OnTimeScaleChanged -= UpdateUI;
    }
}
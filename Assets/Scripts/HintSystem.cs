using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.Events;

[System.Serializable]
public class HintEvent : UnityEvent<GameObject> { }

public class HintSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject hintPrefab;
    [SerializeField] private Vector3 hintOffset = new Vector3(0, 2f, 0);
    [SerializeField] private Vector3 hintRotation = Vector3.zero;
    [SerializeField] private float checkInterval = 0.3f;
    [SerializeField] private float interactionRadius = 3f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private bool showHintAutomatically = true;

    [Header("Appearance")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float pulseSpeed = 1f;
    [SerializeField] private float pulseIntensity = 0.2f;

    [Header("Events")]
    public HintEvent OnHintShow;
    public HintEvent OnHintHide;
    public UnityEvent OnInteraction;

    private GameObject currentHint;
    private Transform player;
    private CanvasGroup hintCanvasGroup;
    private Coroutine checkRoutine;
    private bool isInRange;

    [Header("Текст подсказки")]
    [SerializeField] private string hintText = "Нажмите E"; // Текст по умолчанию

    [Header("Настройки префаба")]
    [SerializeField] private TMP_FontAsset hintFont; // Шрифт
    [SerializeField] private Color hintColor = Color.white; // Цвет текста

    private TextMeshProUGUI hintTextComponent; // Ссылка на компонент текста

    private void Awake()
    {
        if (hintPrefab == null)
        {
            Debug.LogError("Hint prefab is not assigned!", this);
            enabled = false;
        }
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        checkRoutine = StartCoroutine(CheckDistanceRoutine());
    }

    private IEnumerator CheckDistanceRoutine()
    {
        while (true)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            bool shouldShow = distance <= interactionRadius;

            if (shouldShow != isInRange)
            {
                isInRange = shouldShow;
                HandleHintVisibility(shouldShow);
            }

            if (isInRange && Input.GetKeyDown(interactionKey))
            {
                OnInteraction?.Invoke();
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void HandleHintVisibility(bool show)
    {
        if (show)
        {
            CreateHint();
            StartCoroutine(FadeHint(0, 1));
            if (showHintAutomatically) ShowHint();
        }
        else
        {
            StartCoroutine(FadeHint(1, 0, true));
        }
    }

    private void CreateHint()
    {
        if (currentHint != null) return;

        currentHint = Instantiate(hintPrefab, transform.position + hintOffset, Quaternion.Euler(hintRotation));
        currentHint.transform.SetParent(transform);
        hintTextComponent = currentHint.GetComponentInChildren<TextMeshProUGUI>();
        if (hintTextComponent != null)
        {
            hintTextComponent.text = hintText;
            hintTextComponent.font = hintFont;
            hintTextComponent.color = hintColor;
        }
        hintCanvasGroup = currentHint.AddComponent<CanvasGroup>();

        OnHintShow?.Invoke(currentHint);
        StartCoroutine(PulseAnimation());
    }

    private IEnumerator FadeHint(float from, float to, bool destroyAfter = false)
    {
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            if (hintCanvasGroup != null)
            {
                hintCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
                elapsed += Time.deltaTime;
            }
            yield return null;
        }

        if (destroyAfter && currentHint != null)
        {
            OnHintHide?.Invoke(currentHint);
            Destroy(currentHint);
            currentHint = null;
        }
    }

    private IEnumerator PulseAnimation()
    {
        RectTransform rectTransform = currentHint.GetComponent<RectTransform>();
        Vector3 originalScale = rectTransform.localScale;

        while (currentHint != null)
        {
            float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1) * pulseIntensity;
            rectTransform.localScale = originalScale * (1 + pulse);
            yield return null;
        }
    }

    public void ShowHint() => StartCoroutine(FadeHint(0, 1));
    public void HideHint() => StartCoroutine(FadeHint(1, 0, true));

    private void OnDisable()
    {
        if (checkRoutine != null) StopCoroutine(checkRoutine);
        if (currentHint != null) Destroy(currentHint);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
        Gizmos.DrawIcon(transform.position + hintOffset, "HintIcon.png", true);
    }
#endif
}
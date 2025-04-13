using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI Settings")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Button continueButton;
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private bool freezeTimeDuringDialogue = true;
    [SerializeField] private GameObject[] uiElementsToHide;

    [Header("Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    private Queue<string> sentences = new Queue<string>();
    private Coroutine typingCoroutine;
    private float preDialogueTimeScale; // Сохраняем масштаб времени перед диалогом
    private List<GameObject> hiddenUIElements = new List<GameObject>(); // Список скрытых элементов
    private bool isTyping = false;
    private bool dialogueActive = false;
    public bool IsDialogueActive => dialogueActive;

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
            return;
        }

        continueButton.onClick.AddListener(NextSentence);
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogueActive) return;
        TimeManager.Instance.StopTime(); // Полная остановка времени
        /*
        OnDialogueStart?.Invoke();
        dialogueActive = true;
        
        // Сохраняем текущее время и скрываем UI
        preDialogueTimeScale = TimeManager.Instance.CurrentTimeScale;
        */
        HideNonDialogueUI();

        if (freezeTimeDuringDialogue)
            TimeManager.Instance.StopTime();

        sentences.Clear();
        foreach (string sentence in dialogue.sentences)
            sentences.Enqueue(sentence);

        dialoguePanel.SetActive(true);
        NextSentence();
    }

    private void HideNonDialogueUI()
    {
        foreach (var uiElement in uiElementsToHide)
        {
            if (uiElement.activeInHierarchy)
            {
                hiddenUIElements.Add(uiElement);
                uiElement.SetActive(false);
            }
        }
    }

    private void ShowHiddenUI()
    {
        foreach (var uiElement in hiddenUIElements)
        {
            uiElement.SetActive(true);
        }
        hiddenUIElements.Clear();
    }

    public void NextSentence()
    {
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = sentences.Peek();
            isTyping = false;
            return;
        }

        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        typingCoroutine = StartCoroutine(TypeSentence(sentence));
    }

    private IEnumerator TypeSentence(string sentence)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        isTyping = false;
    }

    private void EndDialogue()
    {
        TimeManager.Instance.RestoreTime();
        dialogueActive = false;
        dialoguePanel.SetActive(false);
        ShowHiddenUI();
        //TimeManager.Instance.ResetTimeScale(); // Восстанавливаем время через TimeManager
        OnDialogueEnd?.Invoke();
    }

    public void SkipDialogue()
    {
        if (!dialogueActive) return;
        StopAllCoroutines();
        EndDialogue();
    }

    void Update()
    {
        if (dialogueActive && Input.GetKeyDown(KeyCode.Space))
            NextSentence();
    }
}
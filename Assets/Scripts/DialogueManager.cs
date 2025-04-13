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
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private GameObject[] uiElementsToHide;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;

    [Header("Events")]
    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    private Queue<Dialogue.DialogueLine> dialogueLines = new Queue<Dialogue.DialogueLine>();
    private Coroutine currentRoutine;
    private List<GameObject> hiddenUIElements = new List<GameObject>();
    private bool isDialogueActive;
    private float preDialogueTimeScale;
    private Dialogue currentDialogue;

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

    public void StartDialogue(Dialogue dialogue)
    {
        if (isDialogueActive) return;
        currentDialogue = dialogue;
        preDialogueTimeScale = TimeManager.Instance.CurrentTimeScale;
        dialogueLines.Clear();

        if (dialogue.freezeTime)
        {
            TimeManager.Instance.StopTime();
            HideNonDialogueUI();
        }
        else
        {
            // —охран€ем текущее замедление, но не останавливаем его
            TimeManager.Instance.PauseSlowDown(false);
        }

        foreach (var line in dialogue.lines)
        {
            dialogueLines.Enqueue(line);
        }

        preDialogueTimeScale = TimeManager.Instance.CurrentTimeScale;

        if (dialogue.freezeTime)
        {
            TimeManager.Instance.StopTime();
            HideNonDialogueUI();
        }

        if (dialogue.pauseGameplay)
        {
            GameManager.Instance.PausePlayerControls(true);
        }

        isDialogueActive = true;
        OnDialogueStart?.Invoke();
        StartCoroutine(ProcessDialogue());

    }

    private IEnumerator ProcessDialogue()
    {
        while (dialogueLines.Count > 0)
        {
            var line = dialogueLines.Dequeue();

            if (line.voiceClip != null)
            {
                audioSource.PlayOneShot(line.voiceClip);
            }

            if (!line.audioOnly)
            {
                dialoguePanel.SetActive(true);
                yield return StartCoroutine(TypeText(line.sentence));
            }

            if (line.displayDuration > 0)
            {
                yield return new WaitForSecondsRealtime(line.displayDuration);
            }
            else
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
            }

            dialogueText.text = "";
            dialoguePanel.SetActive(false);
        }

        EndDialogue();
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
    }

    private void HideNonDialogueUI()
    {
        foreach (var uiElement in uiElementsToHide)
        {
            if (uiElement.activeSelf)
            {
                hiddenUIElements.Add(uiElement);
                uiElement.SetActive(false);
            }
        }
    }

    private void EndDialogue()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialoguePanel.SetActive(false);

        TimeManager.Instance.RestoreTime();

        ShowHiddenUI();


        if (GameManager.Instance != null)
        {
            GameManager.Instance.PausePlayerControls(false);
        }

        if (currentDialogue != null && currentDialogue.freezeTime)
        {
            TimeManager.Instance.RestoreTime();
        }

        isDialogueActive = false;
        OnDialogueEnd?.Invoke();
        currentDialogue = null; // —бросить текущий диалог
        TimeManager.Instance.PauseSlowDown(false);
    }

    private void ShowHiddenUI()
    {
        foreach (var uiElement in hiddenUIElements)
        {
            uiElement.SetActive(true);
        }
        hiddenUIElements.Clear();
    }
}
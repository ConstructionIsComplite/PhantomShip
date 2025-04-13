using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DialogueTriggerCondition : UnityEngine.Events.UnityEvent<bool> { }

public class DialogueTrigger : MonoBehaviour
{
    public enum TriggerType { OnEnter, OnInteract, Manual }

    [Header("Dialogue Settings")]
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private TriggerType triggerType = TriggerType.OnEnter;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactionRadius = 3f;

    [Header("Conditions")]
    [SerializeField] private DialogueTriggerCondition triggerCondition;

    [Header("Events")]
    public UnityEvent OnTriggerActivated;
    public UnityEvent OnTriggerCompleted;

    private Transform player;
    private bool canActivate = false;
    private bool hasBeenTriggered = false;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (triggerType != TriggerType.OnInteract) return;

        float distance = Vector3.Distance(transform.position, player.position);
        canActivate = distance <= interactionRadius;

        if (canActivate && Input.GetKeyDown(interactKey))
            TryActivateTrigger();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerType == TriggerType.OnEnter && other.CompareTag("Player"))
            TryActivateTrigger();
    }

    public void TryActivateTrigger()
    {
        if (hasBeenTriggered) return;

        if (triggerCondition != null)
        {
            bool conditionMet = true;
            triggerCondition.Invoke(conditionMet);
            if (!conditionMet) return;
        }

        DialogueManager.Instance.StartDialogue(dialogue);
        hasBeenTriggered = true;
        OnTriggerActivated?.Invoke();
        DialogueManager.Instance.OnDialogueEnd.AddListener(() => OnTriggerCompleted?.Invoke());
    }

    public void SetDialogue(Dialogue newDialogue) => dialogue = newDialogue;
    public void SetTriggerType(TriggerType type) => triggerType = type;

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (triggerType == TriggerType.OnInteract)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, interactionRadius);
        }
    }
#endif
}
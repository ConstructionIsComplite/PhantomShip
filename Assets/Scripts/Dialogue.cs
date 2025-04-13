using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        [TextArea(3, 10)]
        public string sentence;
        public AudioClip voiceClip;
        public Sprite characterPortrait;

        [Header("Timing Settings")]
        public float displayDuration = 0f; // 0 = ждать ввода

        [Header("Display Options")]
        public bool audioOnly = false;
    }

    [Header("Global Dialogue Settings")]
    public bool pauseGameplay = true;
    public bool freezeTime = true;

    public DialogueLine[] lines;
}
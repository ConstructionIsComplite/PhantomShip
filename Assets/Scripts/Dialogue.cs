using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue System/Dialogue")]
public class Dialogue : ScriptableObject
{
    [TextArea(3, 10)]
    public string[] sentences;
    public AudioClip[] voiceClips;
    public Sprite[] characterPortraits;
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicList", menuName = "ScriptableObjects/MusicList")]
public class MusicList : ScriptableObject
{
    public List<AudioClip> clips = new();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "Sequence", menuName = "Sequence", order = 0)]
public class Sequence : ScriptableObject
{
    [Tooltip("The song this sequence is created for")]
    public AudioClip Song = null;

    [Tooltip("Should the song be looped or not")]
    public bool Loop = true;

    [Tooltip("The Bpm of the song used in this sequence")]
    public float Bpm = 120f;

    [Tooltip("The offset of the first beat within the song")]
    public float SongOffset = 0f;

    [Tooltip("The beatnumbers of the events")]
    public List<float> EventPositions = null;

    public void ReorderEvents()
    {
        EventPositions.Sort();
    }

    public void AddEvent(float beatNumber)
    {
        EventPositions.Add(beatNumber);
        ReorderEvents();
    }
}
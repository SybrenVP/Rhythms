using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythms
{

    [CreateAssetMenu(fileName = "NewAudioData", menuName = "Rhythms/Create Audio Data")]
    public class AudioData : ScriptableObject
    {
        [Tooltip("The song this sequence is created for")]
        public AudioClip Song = null;

        [Tooltip("Should the song be looped or not")]
        public bool Loop = true;

        [Tooltip("The Bpm of the song used in this sequence")]
        public float Bpm = 120f;

        [Tooltip("The offset of the first beat within the song")]
        public float SongOffset = 0f;

        public float SecPerBeat
        {
            get  => 60f / Bpm;
        }

        public float BeatPerSec
        {
            get => Bpm / 60f;
        }

        public int AmountBeatsInSong
        {
            get => (int)(BeatPerSec * Song.length);
        }
    }
}
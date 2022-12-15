﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythms
{
    [CreateAssetMenu(fileName = "NewSequence", menuName = "Rhythms/Create Sequence", order = 0)]
    [System.Serializable]
    public class RhythmSequence : ScriptableObject
    {
        public string Name = "Sequence";

        public AudioData Audio = null;

        //Has multiple tracks, tracks contain rhythm states
        public List<RhythmTrack> Tracks = new List<RhythmTrack>();

        public SequenceVariables Variables;

        public void OnEnable()
        {
            if (!Audio)
                Audio = CreateInstance<AudioData>();
        }

        public void Start()
        {
            foreach (RhythmTrack track in Tracks)
            {
                track.Start();
            }
        }

        public void OnUpdate(int currentBeat)
        {
            foreach (RhythmTrack track in Tracks)
            {
                track.OnUpdate(currentBeat);
            }
        }

        public void OnBeatUpdate(int currentBeat)
        {
            //We'll check if each active track has a state on the current beat (HasKey), if it does, call BeatUpdate on the state
            foreach (RhythmTrack track in Tracks)
            {
                track.OnUpdate(currentBeat);
            }
        }
    }
}
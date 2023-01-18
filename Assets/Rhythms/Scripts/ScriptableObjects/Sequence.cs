using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    [CreateAssetMenu(fileName = "NewSequence", menuName = "Rhythms/Create Sequence", order = 0)]
    [System.Serializable]
    public class Sequence : ScriptableObject
    {
        public string Name = "Sequence";

        public AudioData Audio = null;

        //Has multiple tracks, tracks contain rhythm states
        public List<Track> Tracks = new List<Track>();
        public List<DataConnection> DataConnections = new List<DataConnection>();

        public SequenceVariables Variables;

        public void OnEnable()
        {
            if (!Audio)
                Audio = CreateInstance<AudioData>();

            foreach (DataConnection connection in DataConnections)
            {
                connection.Start();
            }
        }

        public void OnUpdate(int currentBeat, float beatOffset)
        {
            foreach (Track track in Tracks)
            {
                track.OnUpdate(currentBeat, beatOffset, Audio);
            }
        }

        public void OnBeatUpdate(int currentBeat)
        {
            //We'll check if each active track has a state on the current beat (HasKey), if it does, call BeatUpdate on the state
            foreach (Track track in Tracks)
            {
                track.OnBeatUpdate(currentBeat);
            }
        }
    }
}
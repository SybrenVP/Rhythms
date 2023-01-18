using UnityEngine;
using System.Collections;

namespace Rhythm
{
    [System.Serializable]
    public abstract class Action : ScriptableObject
    {
        [HideInInspector]
        public bool Enabled = true;

#if UNITY_EDITOR

        [HideInInspector]
        public bool Foldout = true;

#endif

        protected AudioData _audioData;
        protected int _beat;
        protected int _lengthInBeats;

        public abstract void Start();

        public void SetAudioData(AudioData data)
        {
            _audioData = data;
        }

        public void SetStateInformation(int beat, int length)
        {
            _beat = beat;
            _lengthInBeats = length;
        }

        public abstract void BeatUpdate();

        public abstract void Update();

        public abstract void Exit();
    }
}
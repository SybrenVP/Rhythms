using UnityEngine;
using System.Collections;

namespace Rhythms
{

    [System.Serializable]
    public abstract class RhythmAction : ScriptableObject
    {
        [HideInInspector]
        public bool Enabled = true;

#if UNITY_EDITOR

        [HideInInspector]
        public bool Foldout = true;

#endif

        public abstract void OnTimelineActivate();

        public abstract void OnTimelineDisable();

        public abstract void Start();

        public abstract void BeatUpdate();

        public abstract void Update();

        public abstract void Exit();
    }
}
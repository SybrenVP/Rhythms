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

        public abstract void Raise();
    }
}
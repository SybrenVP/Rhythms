using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythms
{
    [System.Serializable]
    public class RhythmState : ScriptableObject
    {
        public List<RhythmAction> Actions = new List<RhythmAction>();

        public void Start()
        {
            foreach (RhythmAction action in Actions)
            {
                if (action.Enabled)
                    action.Raise();
            }
        }
    }
}
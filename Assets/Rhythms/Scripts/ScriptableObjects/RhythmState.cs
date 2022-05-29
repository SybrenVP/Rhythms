using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythms
{
    [System.Serializable]
    public class RhythmState : ScriptableObject
    {
        public List<RhythmAction> Actions = new List<RhythmAction>();

        public bool Active = false;

        //If the length is 0, it will only play Start
        public int LengthInBeats = 1;

        public void Start()
        {
            foreach (RhythmAction action in Actions)
            {
                if (action.Enabled)
                    action.Start();
            }
        }

        public void OnBeatUpdate()
        {
            if (!Active)
            {
                Start();
                if (LengthInBeats == 0)
                    return;

                Active = true;
            }

            if (Active)
            {
                foreach (RhythmAction action in Actions)
                {
                    if (action.Enabled)
                        action.BeatUpdate();
                }
            }
        }

        public void OnUpdate()
        {
            if (Active)
            {
                foreach (RhythmAction action in Actions)
                {
                    if (action.Enabled)
                        action.Update();
                }
            }
        }

        public void ExitState()
        {
            if (Active)
            {
                foreach (RhythmAction action in Actions)
                {
                    if (action.Enabled)
                        action.Exit();
                }
                Active = false;
            }
        }
    }
}
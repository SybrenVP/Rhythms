using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    [System.Serializable]
    public class State : ScriptableObject
    {
        public List<Action> Actions = new List<Action>();

        public bool Active = false;

        //If the length is 0, it will only play Start
        public int LengthInBeats = 1;

        public void StartState(int beat, AudioData data)
        {
            foreach (Action action in Actions)
            {
                if (action.Enabled)
                {
                    action.SetAudioData(data);
                    action.SetStateInformation(beat, LengthInBeats);

                    action.Start();
                }
            }
        }

        public void OnBeatUpdate()
        {
            if (Active)
            {
                foreach (Action action in Actions)
                {
                    if (action.Enabled)
                        action.BeatUpdate();
                }
            }
        }

        public void OnUpdate(int currentBeat, AudioData data)
        {
            if (!Active)
            {
                StartState(currentBeat, data);
                if (LengthInBeats == 0)
                    return;

                Active = true;
            }

            if (Active)
            {
                foreach (Action action in Actions)
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
                foreach (Action action in Actions)
                {
                    if (action.Enabled)
                        action.Exit();
                }
                Active = false;
            }
        }
    }
}
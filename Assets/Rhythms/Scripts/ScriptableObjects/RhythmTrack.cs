using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythms
{
    [System.Serializable]
    public class IntStateDictionary : SerializableDictionary<int, RhythmState> { }

    [System.Serializable]
    public class RhythmTrack : ScriptableObject
    {
        public bool ActiveOnStart = false;

        //int is a beat number, each beat can contain a state, to have multiple states on the same beat, you need to use multiple tracks. This needs to be a serializable dictionary
        public IntStateDictionary States = new IntStateDictionary();

        [HideInInspector, SerializeField] public bool Active = true;

        //Allows us to disable and enable tracks at runtime, kind of like making a choice in a story game
        public void SetActive(bool active)
        {
            Active = active;
        }

        public void Start()
        {
            if (Active)
            {
                foreach (var state in States)
                {
                    state.Value.OnTimelineActivate();
                }
            }
        }

        public void OnUpdate(int currentBeat)
        {
            if (Active)
            {
                if (States.ContainsKey(currentBeat))
                {
                    States[currentBeat].OnUpdate();
                }
            }
        }

        public void OnBeatUpdate(int currentBeat)
        {
            if (Active)
            {
                if (States.ContainsKey(currentBeat - 1)) //Check the previous beat to exit it if necessary
                {
                    //The currentbeat could be the same state as the previous beat in which case we do not need to exit it yet. 
                    if (States.ContainsKey(currentBeat) && !States[currentBeat].Active)
                        States[currentBeat - 1].ExitState();
                }

                if (States.ContainsKey(currentBeat))
                {
                    States[currentBeat].OnBeatUpdate();
                }

                
            }
        }

        #region Accessors

        public int GetBeatForState(RhythmState state)
        {
            int lowestBeat = int.MaxValue;
            bool found = false;
            foreach (int key in States.Keys)
            {
                if (States[key] == state && key < lowestBeat)
                {
                    lowestBeat = key;
                    found = true;
                }
            }

            if (!found)
                lowestBeat = -1;

            return lowestBeat;
        }

        #endregion
    }
}
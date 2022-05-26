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

        [HideInInspector, SerializeField] public bool Active = false;

        //Allows us to disable and enable tracks at runtime, kind of like making a choice in a story game
        public void SetActive(bool active)
        {
            Active = active;
        }

        public void OnUpdate(int beatNumber)
        {
            if (Active)
            {
                if (States.ContainsKey(beatNumber))
                {
                    States[beatNumber].Start(); 
                }

                //TODO: Some states are longer than 1 beat, meaning they get to update multiple times
            }
        }

        #region Accessors

        public int GetBeatForState(RhythmState state)
        {
            foreach (int key in States.Keys)
            {
                if (States[key] == state)
                    return key;
            }

            return -1;
        }

        #endregion
    }
}
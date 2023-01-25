using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    [System.Serializable]
    public class IntStateDictionary : SerializableDictionary<int, State> { }

    [System.Serializable]
    public class Track : ScriptableObject
    {
        public bool ActiveOnStart = false;

        //int is a beat number, each beat can contain a state, to have multiple states on the same beat, you need to use multiple tracks. This needs to be a serializable dictionary
        public IntStateDictionary States = new IntStateDictionary();

        [HideInInspector, SerializeField] public bool Active = true;


        public void OnUpdate(int currentBeat, float beatOffset, AudioData data)
        {
            if (Active)
            {
                if (beatOffset > 0.5f)
                {
                    currentBeat += 1;

                    if (States.ContainsKey(currentBeat - 1)) //Check the previous beat to exit it if necessary
                    {
                        //The currentbeat could be the same state as the previous beat in which case we do not need to exit it yet. 
                        if ((States.ContainsKey(currentBeat) && !States[currentBeat].Active) || !States.ContainsKey(currentBeat))
                            States[currentBeat - 1].ExitState();
                    }
                }

                if (States.ContainsKey(currentBeat))
                {
                    States[currentBeat].OnUpdate(currentBeat, data);
                }
            }
        }

        public void OnBeatUpdate(int currentBeat)
        {
            if (Active)
            {
                if (States.ContainsKey(currentBeat))
                {
                    States[currentBeat].OnBeatUpdate();
                }
            }
        }

        #region Accessors

        public int GetBeatForState(State state)
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
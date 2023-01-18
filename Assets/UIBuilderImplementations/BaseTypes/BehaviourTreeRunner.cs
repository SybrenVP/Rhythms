using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This would be a Rhythm Controller
public class BehaviourTreeRunner : MonoBehaviour
{
    public Track Track;

    void Start()
    {
        Track = Track.Clone();
        //TODO: Tree.Bind(); //This is used by the blackboard to connect to all nodes
    }

    void Update()
    {
        Track.Update();
    }
}

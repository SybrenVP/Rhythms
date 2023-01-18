using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Translated to the Rhythm editor these would be 'States'
public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running,
        Failure,
        Success
    }

    [HideInInspector] public State ActiveState = State.Running;
    [HideInInspector] public bool Started = false;
    [HideInInspector] public string Guid;

    [HideInInspector] public Vector2 Position;

    [HideInInspector] public Blackboard Blackboard;

    [TextArea] public string Description;

    public State Update()
    {
        if (!Started)
        {
            OnStart();
            Started = true;
        }

        ActiveState = OnUpdate();

        if (ActiveState == State.Failure || ActiveState == State.Success)
        {
            OnStop();
            Started = false;
        }

        return ActiveState;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }
    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}

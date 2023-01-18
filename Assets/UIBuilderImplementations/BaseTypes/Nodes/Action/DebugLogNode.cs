using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugLogNode : ActionNode
{
    public string Message;

    protected override void OnStart()
    {
        Debug.Log($"START :: {Message}");
    }

    protected override void OnStop()
    {
        Debug.Log($"STOP :: {Message}");
    }

    protected override State OnUpdate()
    {
        Debug.Log($"UPDATE :: {Message}");
        return State.Success;
    }
}

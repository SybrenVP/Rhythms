using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequencerNode : CompositeNode
{
    private int _current;

    protected override void OnStart()
    {
        _current = 0;
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        Node child = Children[_current];
        switch (child.Update())
        {
            case State.Running:
                return State.Running;

            case State.Failure:
                return State.Failure;

            case State.Success:
                _current++;
                break;
        }

        return _current == Children.Count ? State.Success : State.Running;
    }
}

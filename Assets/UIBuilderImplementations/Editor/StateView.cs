using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StateView : VisualElement
{
    public Action<StateView> OnStateSelected;

    public BehaviourTree State;

    public StateView(BehaviourTree state)
    {
        State = state;
    }
}

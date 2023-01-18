using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

//This has no place in the rhythm editor I think
public abstract class DecoratorNode : Node
{
    [HideInInspector] public Node Child;

    public override Node Clone()
    {
        DecoratorNode node = Instantiate(this);
        node.Child = Child.Clone();
        return node;
    }
}

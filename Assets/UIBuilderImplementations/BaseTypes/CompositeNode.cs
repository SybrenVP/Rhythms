using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This has no place in the Rhythm editor I think
public abstract class CompositeNode : Node
{
    [HideInInspector] public List<Node> Children = new List<Node>();

    public override Node Clone()
    {
        CompositeNode node = Instantiate(this);
        node.Children = Children.ConvertAll(c => c.Clone());
        return node;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;

    public Node Node;
    public Port Input;
    public Port Output;

    public NodeView(Node node) : base("Assets/UIBuilderImplementations/NodeView.uxml")
    {
        this.Node = node;
        this.title = Node.name;
        this.viewDataKey = Node.Guid;

        style.left = Node.Position.x;
        style.top = Node.Position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("Description");
        descriptionLabel.bindingPath = "Description";
        descriptionLabel.Bind(new SerializedObject(Node));
    }

    private void SetupClasses()
    {
        if (Node is ActionNode)
        {
            AddToClassList("action");
        }
        else if (Node is CompositeNode)
        {
            AddToClassList("composite");
        }
        else if (Node is DecoratorNode)
        {
            AddToClassList("decorator");
        }
        else if (Node is RootNode)
        {
            AddToClassList("root");
        }
    }

    private void CreateOutputPorts()
    {
        if (Node is ActionNode)
        {
        }
        else if (Node is CompositeNode)
        {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        }
        else if (Node is DecoratorNode)
        {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        else if (Node is RootNode)
        {
            Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }

        if (Output != null)
        {
            Output.portName = "";
            Output.style.flexDirection = FlexDirection.ColumnReverse;
            Output.style.fontSize = 1;
            outputContainer.Add(Output);
        }
    }

    private void CreateInputPorts()
    {
        if (Node is ActionNode)
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (Node is CompositeNode)
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (Node is DecoratorNode)
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }
        else if (Node is RootNode)
        {
            
        }

        if (Input != null)
        {
            Input.portName = "";
            Input.style.flexDirection = FlexDirection.Column;
            Input.style.fontSize = 1;
            inputContainer.Add(Input);
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(Node, "Behaviour Tree (Move Node)");
        Node.Position.x = newPos.xMin;
        Node.Position.y = newPos.yMin;
        EditorUtility.SetDirty(Node);
    }

    public override void OnSelected()
    {
        base.OnSelected();

        if (OnNodeSelected != null)
            OnNodeSelected.Invoke(this);
    }

    public void SortChildren()
    {
        CompositeNode composite = Node as CompositeNode;
        if (composite)
            composite.Children.Sort(SortByHorizontalPosition);
    }

    private int SortByHorizontalPosition(Node lhs, Node rhs)
    {
        return lhs.Position.x < rhs.Position.x ? -1 : 1;
    }

    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");

        if (Application.isPlaying)
        {
            switch (Node.ActiveState)
            {
                case Node.State.Running:
                    if (Node.Started)
                        AddToClassList("running");
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
}
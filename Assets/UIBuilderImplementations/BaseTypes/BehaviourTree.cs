using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//Translated to the Rhythm editor, these would be Sequences
[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node RootNode;
    public Node.State TreeState = Node.State.Running;

    public List<Node> Nodes = new List<Node>();

    public Blackboard Blackboard = new Blackboard();

    public int Beat = -1;
    [HideInInspector] public Vector2 Position;

    public Node.State Update()
    {
        if (RootNode.ActiveState == Node.State.Running)
            TreeState = RootNode.Update();

        return TreeState;
    }

    public void Traverse(Node node, System.Action<Node> visiter)
    {
        if (node)
        {
            visiter.Invoke(node);
            List<Node> children = GetChildren(node);
            children.ForEach((n) => Traverse(n, visiter));
        }
    }

    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.RootNode = tree.RootNode.Clone();
        tree.Nodes = new List<Node>();
        Traverse(tree.RootNode, (n) =>
        {
            tree.Nodes.Add(n);
        });
        return tree;
    }

    public void Bind()
    {
        Traverse(RootNode, n =>
        {
            n.Blackboard = Blackboard;
        });
    }

#if UNITY_EDITOR
    public Node CreateNode(System.Type type)
    {
        Node node = ScriptableObject.CreateInstance(type) as Node;
        node.name = type.Name;
        node.Guid = GUID.Generate().ToString();

        Undo.RecordObject(this, "Behaviour Tree (Create Node)");
        Nodes.Add(node);

        if (!Application.isPlaying)
            AssetDatabase.AddObjectToAsset(node, this);
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (Create Node)");

        AssetDatabase.SaveAssets();
        return node;
    }

    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behaviour Tree (Delete Node)");
        Nodes.Remove(node);

        //AssetDatabase.RemoveObjectFromAsset(node);
        Undo.DestroyObjectImmediate(node);

        AssetDatabase.SaveAssets();
    }

    public void AddChild(Node parent, Node child)
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (Connect Nodes)");
            rootNode.Child = child;
            EditorUtility.SetDirty(rootNode);
        }

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (Connect Nodes)");
            decorator.Child = child;
            EditorUtility.SetDirty(decorator);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (Connect Nodes)");
            composite.Children.Add(child);
            EditorUtility.SetDirty(composite);
        }
    }

    public void RemoveChild(Node parent, Node child)
    {
        RootNode rootNode = parent as RootNode;
        if (rootNode)
        {
            Undo.RecordObject(rootNode, "Behaviour Tree (Disconnect Nodes)");
            rootNode.Child = null;
            EditorUtility.SetDirty(rootNode);
        }

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator)
        {
            Undo.RecordObject(decorator, "Behaviour Tree (Disconnect Nodes)");
            decorator.Child = null;
            EditorUtility.SetDirty(decorator);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            Undo.RecordObject(composite, "Behaviour Tree (Disconnect Nodes)");
            composite.Children.Remove(child);
            EditorUtility.SetDirty(composite);
        }
    }

    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();

        RootNode rootNode = parent as RootNode;
        if (rootNode && rootNode.Child != null)
        {
            children.Add(rootNode.Child);
        }

        DecoratorNode decorator = parent as DecoratorNode;
        if (decorator && decorator.Child != null)
        {
            children.Add(decorator.Child);
        }

        CompositeNode composite = parent as CompositeNode;
        if (composite)
        {
            return composite.Children;
        }

        return children;
    }
#endif
}

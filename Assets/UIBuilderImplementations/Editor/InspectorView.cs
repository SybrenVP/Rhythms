using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using System.Runtime.ConstrainedExecution;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, InspectorView.UxmlTraits> { }

    private Editor _editor;

    public InspectorView()
    {

    }

    internal void UpdateSelected(NodeView nodeView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(_editor);
        _editor = Editor.CreateEditor(nodeView.Node);
        IMGUIContainer container = new IMGUIContainer(() => 
        { 
            if (_editor.target)
                _editor.OnInspectorGUI(); 
        });
        Add(container);
    }

    internal void UpdateSelected(StateView stateView)
    {
        Clear();

        UnityEngine.Object.DestroyImmediate(_editor);
        _editor = Editor.CreateEditor(stateView.State);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (_editor.target)
                _editor.OnInspectorGUI();
        });
        Add(container);
    }
}

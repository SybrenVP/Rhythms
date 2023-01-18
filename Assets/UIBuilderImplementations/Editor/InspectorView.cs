using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;

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
}

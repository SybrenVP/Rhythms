using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using static RhythmEditor.Toolbar;

[Overlay(typeof(BehaviourTreeEditor), _id, "Rhythms Toolbar")]
public class ToolbarView : IMGUIOverlay, IAccessContainerWindow
{
    private const string _id = "toolbar-overlay-rhythms";

    public enum EToolType
    {
        Select,
        Move,
        Resize
    }

    private GUIContent _moveStateContent = null;
    private GUIContent _resizeStateContent = null;
    private GUIContent _selectStateContent = null;

    private GUIContent[] _stateToolbarContent = null;

    private EToolType _selectedTool = EToolType.Select;
    private EToolType _previousTool = EToolType.Select;

    private static readonly float BUTTON_WIDTH = 45f;
    public static readonly float BUTTON_VERTICAL_OFFSET = 3f;
    public static readonly float BUTTON_HORIZONTAL_OFFSET = 10f;

    EditorWindow IAccessContainerWindow.containerWindow { get; set; }

    public ToolbarView()
    {
        //State tool bar content
        _moveStateContent = EditorGUIUtility.IconContent("d_Grid.MoveTool");
        _resizeStateContent = EditorGUIUtility.IconContent("ScaleTool On");
        _selectStateContent = EditorGUIUtility.IconContent("d_RectTool");

        _stateToolbarContent = new GUIContent[] { _selectStateContent, _moveStateContent, _resizeStateContent };
    }

    public override void OnGUI()
    {
        GUILayout.Space(BUTTON_VERTICAL_OFFSET);
        GUILayout.BeginHorizontal();
        GUILayout.Space(BUTTON_HORIZONTAL_OFFSET);

        EToolType newTool = (EToolType)GUILayout.Toolbar((int)_selectedTool, _stateToolbarContent, EditorStyles.toolbarButton, GUILayout.Width(_stateToolbarContent.Length * BUTTON_WIDTH));

        if (newTool != _selectedTool)
        {
            _previousTool = _selectedTool;
            _selectedTool = newTool;
            if (containerWindow is BehaviourTreeEditor editor)
                editor.ToolChange(_previousTool, _selectedTool);
        }

        GUILayout.Space(BUTTON_HORIZONTAL_OFFSET);
        GUILayout.EndHorizontal();
        GUILayout.Space(BUTTON_VERTICAL_OFFSET);
    }
}

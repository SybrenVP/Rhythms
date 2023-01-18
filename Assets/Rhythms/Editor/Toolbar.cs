using UnityEngine;
using UnityEditor;

namespace RhythmEditor
{

    public class Toolbar
    {
        public enum ToolType
        {
            Select,
            ViewMove,
            Move,
            Resize
        }

        public delegate void ToolbarToolChangeEvent(ToolType newState);
        public ToolbarToolChangeEvent OnToolChange;

        public Rect View;

        public bool Play = false;

        public static readonly float BUTTON_VERTICAL_OFFSET = 8f;
        public static readonly float BUTTON_HORIZONTAL_OFFSET = 10f;

        private RhythmSequenceEditor _editor;
        
        private GUIContent _playToggleContent = null;
        private GUIContent _pauseToggleContent = null;
        private GUIContent _stopButtonContent = null;

        private GUIContent _viewMoveContent = null;
        private GUIContent _moveStateContent = null;
        private GUIContent _resizeStateContent = null;
        private GUIContent _selectStateContent = null;

        private GUIContent[] _stateToolbarContent = null;

        private GUIContent _addTrackButtonContent = null;

        private ToolType _selectedTool = ToolType.Select;
        private ToolType _nextTool = ToolType.Select;

        private Color _backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);

        private static readonly float BUTTON_WIDTH = 45f;

        public Toolbar(RhythmSequenceEditor editor)
        {
            _editor = editor;

            //Editor controls
            _playToggleContent = EditorGUIUtility.IconContent("d_Animation.Play");
            _pauseToggleContent = EditorGUIUtility.IconContent("d_PauseButton");
            _stopButtonContent = EditorGUIUtility.IconContent("PreMatQuad");

            //State tool bar content
            _viewMoveContent = EditorGUIUtility.IconContent("d_ViewToolMove");
            _moveStateContent = EditorGUIUtility.IconContent("d_Grid.MoveTool");
            _resizeStateContent = EditorGUIUtility.IconContent("ScaleTool On");
            _selectStateContent = EditorGUIUtility.IconContent("d_RectTool");

            _stateToolbarContent = new GUIContent[] { _selectStateContent, _viewMoveContent, _moveStateContent, _resizeStateContent  };

            //Track tool bar content
            _addTrackButtonContent = EditorGUIUtility.IconContent("CreateAddNew");
        }

        public void OnGUI()
        {
            EditorGUI.DrawRect(View, _backgroundColor);

            GUILayout.BeginArea(View);
            {
                GUILayout.Space(BUTTON_VERTICAL_OFFSET);

                EditorGUILayout.BeginHorizontal();
                {
                    DrawStateToolbar();

                    DrawActionStackToolbar();

                    DrawTimelineToolbar();

                    DrawSoundControl();
                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(BUTTON_VERTICAL_OFFSET);
            }
            GUILayout.EndArea();
        }

        public void Update()
        {
            if (_selectedTool != _nextTool)
            {
                _selectedTool = _nextTool;

                OnToolChange?.Invoke(_selectedTool);
            }
        }

        private void DrawSoundControl()
        {
            GUILayout.Space(BUTTON_HORIZONTAL_OFFSET);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {

                if (GUILayout.Button(_stopButtonContent, EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                {
                    Play = false;
                }

                Play = GUILayout.Toggle(Play, Play ? _pauseToggleContent : _playToggleContent, "ToolbarButton", GUILayout.Width(BUTTON_WIDTH));
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(BUTTON_HORIZONTAL_OFFSET);
        }

        private void DrawStateToolbar()
        {
            GUILayout.Space(BUTTON_HORIZONTAL_OFFSET);

            _nextTool = (ToolType)GUILayout.Toolbar((int)_selectedTool, _stateToolbarContent, EditorStyles.toolbarButton, GUILayout.Width(_stateToolbarContent.Length * BUTTON_WIDTH));

            GUILayout.Space(BUTTON_HORIZONTAL_OFFSET);
        }

        private void DrawActionStackToolbar()
        {
            GUILayout.Space(BUTTON_HORIZONTAL_OFFSET);

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                EditorGUI.BeginDisabledGroup(!_editor.HasUndoChanges());
                if (GUILayout.Button("Undo", EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                {
                    _editor.UndoChange();
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!_editor.HasRedoChanges());
                if (GUILayout.Button("Redo", EditorStyles.toolbarButton, GUILayout.Width(BUTTON_WIDTH)))
                {
                    _editor.RedoChange();
                }
                EditorGUI.EndDisabledGroup();
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(BUTTON_HORIZONTAL_OFFSET);
        }

        private void DrawTimelineToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button(_addTrackButtonContent, "ToolbarButton", GUILayout.Width(BUTTON_WIDTH)))
                {
                    _editor.Timeline.AddTrack();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
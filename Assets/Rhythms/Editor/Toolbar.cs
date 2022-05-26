using UnityEngine;
using UnityEditor;

namespace Rhythms_Editor
{

    public class Toolbar
    {
        private enum ToolType
        {
            ViewMove,
            Move,
            Resize,
            MultiSelect
        }


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
        private GUIContent _multiSelectStateContent = null;

        private GUIContent[] _stateToolbarContent = null;

        private GUIContent _addTrackButtonContent = null;

        private ToolType _selectedTool = ToolType.ViewMove;
        private ToolType _nextTool = ToolType.ViewMove;

        private Color _backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);

        private static string _errorMessage = "";
        private static float _errorMessageEndTime = 0f;

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
            _multiSelectStateContent = EditorGUIUtility.IconContent("d_RectTool");

            _stateToolbarContent = new GUIContent[] { _viewMoveContent, _moveStateContent, _resizeStateContent, _multiSelectStateContent };

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
                _editor.DisableCurrentInputState();

                _selectedTool = _nextTool;
            }

            switch (_selectedTool)
            {
                case ToolType.ViewMove:
                    //Grant movement on the track timeline 
                    _editor.AcceptTimelineMovement();
                    break;
                case ToolType.Move:
                    //Grant State movement capabilities
                    _editor.AcceptStateMovement();
                    break;
                case ToolType.Resize:
                    //Grant State resize capabilities
                    break;
                case ToolType.MultiSelect:
                    //Grant State multiselect capabilities
                    break;
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

        private void DrawTimelineToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button(_addTrackButtonContent, "ToolbarButton", GUILayout.Width(BUTTON_WIDTH)))
                {
                    _editor.AddTrack();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void SetErrorMessage(string message, float time)
        {
            _errorMessage = message;
            _errorMessageEndTime = Time.realtimeSinceStartup + time;
        }
    }
}
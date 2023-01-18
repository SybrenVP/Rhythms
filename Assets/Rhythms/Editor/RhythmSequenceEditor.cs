using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace RhythmEditor
{
    /// <summary>
    /// The main power source of the rhythms engine. This is the editor window for the rhythm editor, combines the timeline views with inspector and toolbars. 
    /// </summary>
    public class RhythmSequenceEditor : EditorWindow
    {
        public Rhythm.Sequence ActiveSequence = null;

        public Rect Rect_EditorWindow;

        #region Timelines

        public TimelineGUI Timeline = null;

        #endregion

        #region Inspector

        public SequenceInspector Inspector;

        public Vector2 ScrollPosition_Inspector = Vector2.zero;

        #endregion

        #region Toolbar

        public Toolbar Tools;

        #endregion

        #region States
        
        public Rhythm.State SelectedState { get => _inputController?.SelectedState?.State; }

        #endregion
        
        private RhythmSequenceEditorInputController _inputController = null;
        private EditorActionStack _actionStack = null;

        private Rhythm.RhythmController _controller = null;

        private bool _refresh = false;

        [MenuItem("Window/Rhythms/Open editor")]
        public static RhythmSequenceEditor OpenWindow()
        {
            var editorWindow = GetWindow<RhythmSequenceEditor>("Rhythms");

            editorWindow.minSize = new Vector2(TrackGUI.MINWIDTH + SequenceInspector.MINWIDTH, TrackGUI.MINHEIGHT);
            editorWindow.wantsMouseMove = true;

            return editorWindow;
        }

        public void OpenSequenceFromController(Rhythm.RhythmController controller, Rhythm.Sequence sequence) 
        {
            ActiveSequence = sequence;
            _controller = controller;

            Timeline = null;

            if (ActiveSequence == null) 
                ActiveSequence = ScriptableObject.CreateInstance<Rhythm.Sequence>();

            Rect_EditorWindow = new Rect(Vector2.zero, position.size);

            InitializeToolbar();

            Timeline = new TimelineGUI();
            Timeline.Initialize(ActiveSequence, this, new Rect(0f, Tools.View.height, position.width - SequenceInspector.MINWIDTH, position.height - Tools.View.height));

            InitializeSequenceInspector();

            InitializeInputController();

            InitializeActionStack();

            InitializeDataConnections();
        }

        private void InitializeSequenceInspector()
        {
            float inspectorWidth = SequenceInspector.MINWIDTH;

            Inspector = new SequenceInspector(ActiveSequence, this);
            Inspector.View = new Rect(Timeline.View.width, Tools.View.height, inspectorWidth, position.height - Tools.View.height);
        }

        private void InitializeToolbar()
        {
            Tools = new Toolbar(this);
            Tools.View = new Rect(0f, 0f, position.width, EditorGUIUtility.singleLineHeight + Toolbar.BUTTON_VERTICAL_OFFSET * 2f);
        }

        private void InitializeInputController()
        {
            _inputController = new RhythmSequenceEditorInputController();
            _inputController.Init(Tools, this);
        }

        private void InitializeActionStack()
        {
            _actionStack = new EditorActionStack();
        }

        private void InitializeDataConnections()
        {
            //TODO: Loop through the data connections, search for a state in which one of the actions has a variable equal to the output and input variable. Set the connection node, connected node to the correct value
            foreach (DataConnection connection in ActiveSequence.DataConnections)
            {
                foreach (Rhythm.Track track in ActiveSequence.Tracks)
                {
                    foreach (KeyValuePair<int, Rhythm.State> state in track.States)
                    {
                        foreach (Rhythm.Action action in state.Value.Actions)
                        {

                        }
                    }
                }
            }
        }

        protected void OnEnable()
        {
            Rhythm.RhythmController controller = null;
            if (Selection.activeGameObject != null)
            {
                controller = Selection.activeGameObject.GetComponent<Rhythm.RhythmController>();
                if (controller == _controller)
                    return;
            }
            else
            {
                controller = _controller;
            }

            if (controller)
            {
                OpenSequenceFromController(controller, controller.ActiveSequence);
                EditorApplication.update += OnTryRepaint;
            }
            else
            {
                Close();
            }

            Debug.Log("data connections: " + ActiveSequence.DataConnections.Count);
        }

        private void LoadSequence()
        {
            if (Selection.activeGameObject)
            {
                _controller = Selection.activeGameObject.GetComponent<Rhythm.RhythmController>();
                if (_controller)
                {
                    OpenSequenceFromController(_controller, _controller.ActiveSequence);
                }
            }
        }

        protected void OnDisable()
        {
            SaveSequence();

            EditorApplication.update -= OnTryRepaint;
        }

        public void Refresh()
        {
            _refresh = true;
        }

        protected void OnTryRepaint()
        { 
            if (!_refresh)
                return;

            _refresh = false;
            Repaint();
        }

        private void Update()
        {
            Tools.Update();
        }

        protected void OnGUI()
        {
            if (!ActiveSequence)
                return;

            Timeline.OnGUI();
            Timeline.HandleInput();

            #region Inspector

            Inspector.OnGUI();

            #endregion

            Tools.OnGUI();

            _inputController.Update();
        }

        #region Undo/Redo

        public void RecordChange(RhythmToolAction action)
        {
            _actionStack.Record(action);
        }

        public void UndoChange()
        {
            _actionStack.Undo();
        }

        public void RedoChange()
        {
            _actionStack.Redo();
        }

        public bool HasUndoChanges()
        {
            return _actionStack.HasUndoChanges();
        }

        public bool HasRedoChanges()
        {
            return _actionStack.HasRedoChanges();
        }

        #endregion

        #region DataConnections

        public void CreateDataConnection()
        {

        }

        public void DestroyDataConnection()
        {

        }

        #endregion

        public void SaveSequence()
        {
            if (_controller) //Meaning we opened this from a controller (only this is supported at this time, in the future I also want to add scriptable object support from the assets folder)
            {
                _controller.ActiveSequence = ActiveSequence;
                EditorUtility.SetDirty(ActiveSequence);
                EditorUtility.SetDirty(_controller);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }


}
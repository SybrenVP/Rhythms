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

        public List<TrackTimeline> Timelines = new List<TrackTimeline>();

        public Vector2 ScrollPosition_Timelines = Vector2.zero;
        public Rect Rect_TimelinesEditorView;
        public Rect Rect_TimelinesEditorFull;

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

            editorWindow.minSize = new Vector2(TrackTimeline.MINWIDTH + SequenceInspector.MINWIDTH, TrackTimeline.MINHEIGHT);
            editorWindow.wantsMouseMove = true;

            return editorWindow;
        }

        public void OpenSequenceFromController(Rhythm.RhythmController controller, Rhythm.Sequence sequence) 
        {
            ActiveSequence = sequence;
            _controller = controller;

            Timelines.Clear(); 

            if (ActiveSequence == null)
                ActiveSequence = ScriptableObject.CreateInstance<Rhythm.Sequence>();

            Rect_EditorWindow = new Rect(Vector2.zero, position.size);

            InitializeToolbar();

            InitializeTrackTimelines();

            InitializeSequenceInspector();

            InitializeInputController();

            InitializeActionStack();
        }

        private void InitializeTrackTimelines()
        {
            Vector2 size = DefineWidthHeightOfTimeline();

            //We differentiate between Full and View. Full is the entire object, View is the portion visible by the scroll rect
            Rect_TimelinesEditorFull = new Rect(Vector2.zero, new Vector2(size.x, size.y * ActiveSequence.Tracks.Count));
            Rect_TimelinesEditorView = new Rect(Vector2.zero, new Vector2(Rect_TimelinesEditorFull.width, position.height));
            Rect_TimelinesEditorView.y = EditorGUIUtility.singleLineHeight;

            for (int i = 0; i < ActiveSequence.Tracks.Count; i++)
            {
                if (ActiveSequence.Tracks[i] == null)
                {
                    Debug.LogWarning("Corrupt sequence, destroying tracks :("); //Temporary null catch, this will only be called if serialization is failing
                    ActiveSequence.Tracks.Clear();
                    Timelines.Clear();

                    return;
                }

                Color trackBackground = i % 2 == 1 ? new Color(0.30f, 0.30f, 0.30f, 1f) : new Color(0.20f, 0.20f, 0.20f, 1f);

                CreateTimeline(size.y, size.x, ActiveSequence.Tracks[i]);
            }
        }

        private Vector2 DefineWidthHeightOfTimeline()
        {
            Vector2 size = Vector2.zero;

            size.x = position.width - (Inspector != null ? Inspector.View.width : SequenceInspector.MINWIDTH);
            size.y = position.height / ActiveSequence.Tracks.Count;
            size.y = Mathf.Max(size.y, TrackTimeline.MINHEIGHT);

            return size;
        }

        private void CreateTimeline(float height, float width, Rhythm.Track track)
        {
            Color trackBackground = Timelines.Count % 2 == 1 ? new Color(0.30f, 0.30f, 0.30f, 1f) : new Color(0.20f, 0.20f, 0.20f, 1f);

            Timelines.Add(new TrackTimeline(track, ActiveSequence.Audio, this, new Rect(0f, height * Timelines.Count, width, height), trackBackground));
        }

        private void UpdateTimelines()
        {
            Vector2 size = DefineWidthHeightOfTimeline();

            for (int i = 0; i < ActiveSequence.Tracks.Count; i++)
            {
                Color trackBackground = i % 2 == 1 ? new Color(0.30f, 0.30f, 0.30f, 1f) : new Color(0.20f, 0.20f, 0.20f, 1f);

                Timelines[i].SetView(size, i);
                Timelines[i].SetBackground(trackBackground);
            }

            Rect_TimelinesEditorFull = new Rect(Vector2.zero, new Vector2(size.x, size.y * ActiveSequence.Tracks.Count));
        }

        private void InitializeSequenceInspector()
        {
            float inspectorWidth = SequenceInspector.MINWIDTH;

            Inspector = new SequenceInspector(ActiveSequence, this);
            Inspector.View = new Rect(Rect_TimelinesEditorView.width, Tools.View.height, inspectorWidth, position.height - Tools.View.height);
        }

        private void InitializeToolbar()
        {
            Tools = new Toolbar(this);
            Tools.View = new Rect(position.x, 0f, position.width, EditorGUIUtility.singleLineHeight + Toolbar.BUTTON_VERTICAL_OFFSET * 2f);
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

        protected void OnEnable()
        {
            var controller = Selection.activeGameObject.GetComponent<Rhythm.RhythmController>();
            if (controller)
            {
                OpenSequenceFromController(controller, controller.ActiveSequence);
                EditorApplication.update += OnTryRepaint;
            }
            else
            {
                Close();
            }

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

            foreach (TrackTimeline timeline in Timelines)
            {
                timeline.Destroy();
            }

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

            #region Timelines

            ScrollPosition_Timelines = GUI.BeginScrollView(Rect_TimelinesEditorView, ScrollPosition_Timelines, Rect_TimelinesEditorFull, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);

            foreach (TrackTimeline timeline in Timelines)
            { 
                timeline.OnGUI();
            }

            //Draw states on the timeline here
            foreach (TrackTimeline timeline in Timelines)
            {
                timeline.OnStateGUI();
            }

            foreach (TrackTimeline timeline in Timelines)
            {
                timeline.OnStateGhostGUI();
            }

            GUI.EndScrollView(true);

            #endregion

            #region Inspector

            Inspector.OnGUI();

            #endregion

            Tools.OnGUI();

            _inputController.Update();
        }

        #region Timeline Control

        public void AddTrack()
        {
            Rhythm.Track newTrack = ScriptableObject.CreateInstance<Rhythm.Track>();
            ActiveSequence.Tracks.Add(newTrack);

            SaveSequence();

            Vector2 size = DefineWidthHeightOfTimeline();
            CreateTimeline(size.y, size.x, newTrack);

            UpdateTimelines();
        }

        public void RemoveTrack(object track)
        {
            int trackIndex = ActiveSequence.Tracks.IndexOf((Rhythm.Track)track);
            if (trackIndex < 0)
            {
                Debug.LogWarning("Found a track that is not present in the Created Tracks. Clearing all corrupted data."); //Temporary null catch, this will only be called if serialization is failing
                ActiveSequence.Tracks.Clear();
                Timelines.Clear();
            }
            else
            {
                ActiveSequence.Tracks.RemoveAt(trackIndex);
                Timelines.RemoveAt(trackIndex);
            }

            SaveSequence();

            UpdateTimelines();
        }


        #endregion

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

        public void SaveSequence()
        {
            if (_controller) //Meaning we opened this from a controller (only this is supported at this time, in the future I also want to add scriptable object support from the assets folder)
            {
                _controller.ActiveSequence = ActiveSequence;
                EditorUtility.SetDirty(_controller);
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }


}
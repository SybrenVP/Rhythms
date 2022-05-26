using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Rhythms_Editor
{
    /// <summary>
    /// The main power source of the rhythms engine. This is the editor window for the rhythm editor, combines the timeline views with inspector and toolbars. 
    /// </summary>
    public class RhythmSequenceEditor : EditorWindow
    {
        public Rhythms.RhythmSequence ActiveSequence = null;

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

        private StateDrawer _selectedState = null;
        public Rhythms.RhythmState SelectedState { get => _selectedState?.State; }

        #endregion

        #region InputState

        private enum InputState
        {
            ControlTimeline,
            MoveState,
            ResizeState,
            SelectStates,
        }

        private InputState _currentInputState = InputState.ControlTimeline;
        private bool _inputStateActive = false;

        private TrackTimeline _owningTimelineOfInput = null;
        private Vector2 _lastMousePos = Vector2.zero;

        #endregion

        private Rhythms.RhythmController _controller = null;

        private bool _refresh = false;
        private Vector2 _resizeLastMousePos = Vector2.zero;

        [MenuItem("Window/Rhythms/Open editor")]
        public static RhythmSequenceEditor OpenWindow()
        {
            var editorWindow = GetWindow<RhythmSequenceEditor>("Rhythms");

            editorWindow.minSize = new Vector2(TrackTimeline.MINWIDTH + SequenceInspector.MINWIDTH, TrackTimeline.MINHEIGHT);
            editorWindow.wantsMouseMove = true;

            return editorWindow;
        }

        public void OpenSequenceFromController(Rhythms.RhythmController controller, Rhythms.RhythmSequence sequence) 
        {
            ActiveSequence = sequence;
            _controller = controller;

            Timelines.Clear(); 

            if (ActiveSequence == null)
                ActiveSequence = ScriptableObject.CreateInstance<Rhythms.RhythmSequence>();

            Rect_EditorWindow = new Rect(Vector2.zero, position.size);

            InitializeToolbar();

            InitializeTrackTimelines();

            InitializeSequenceInspector();
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

        private void CreateTimeline(float height, float width, Rhythms.RhythmTrack track)
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

        protected void OnEnable()
        {
            var controller = Selection.activeGameObject.GetComponent<Rhythms.RhythmController>();
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
                _controller = Selection.activeGameObject.GetComponent<Rhythms.RhythmController>();
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

            GUI.EndScrollView(true);

            #endregion

            #region Inspector

            Inspector.OnGUI();

            #endregion

            Tools.OnGUI();

            HandleInput();
        }

        protected void HandleInput()
        {            
            switch (_currentInputState)
            {
                case InputState.ControlTimeline:
                    HandleTimelineControlInput();
                    break;
                case InputState.MoveState:
                    HandleStateMovementInput();
                    break;
            }
        }

        #region Timeline Control

        private void HandleTimelineControlInput()
        {
            Event e = Event.current;

            int controlID = GUIUtility.GetControlID(_currentInputState.GetHashCode(), FocusType.Passive);
            int hotControl = GUIUtility.hotControl;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (!_inputStateActive)
                        {
                            //get the owning timeline of this input position
                            _owningTimelineOfInput = TrackTimeline.FindOwningTimeline(this, e.mousePosition);
                            _inputStateActive = true;

                            _lastMousePos = e.mousePosition;

                            GUIUtility.hotControl = controlID;
                            e.Use();
                        }
                    }
                    break;

                case EventType.MouseUp:
                    if (hotControl == controlID)
                    {
                        if (_inputStateActive)
                        {
                            _owningTimelineOfInput = null;
                            _inputStateActive = false;
                        }

                        GUIUtility.hotControl = 0;
                        e.Use();
                    }

                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (_inputStateActive)
                        {
                            Vector2 diff = e.mousePosition - _lastMousePos;
                            _owningTimelineOfInput.Scroll(-diff.x);

                            _lastMousePos = e.mousePosition;
                            _refresh = true;
                        }
                    }
                    break;
            }
        }

        public void AddTrack()
        {
            Rhythms.RhythmTrack newTrack = ScriptableObject.CreateInstance<Rhythms.RhythmTrack>();
            ActiveSequence.Tracks.Add(newTrack);

            SaveSequence();

            Vector2 size = DefineWidthHeightOfTimeline();
            CreateTimeline(size.y, size.x, newTrack);

            UpdateTimelines();
        }

        public void RemoveTrack(object track)
        {
            int trackIndex = ActiveSequence.Tracks.IndexOf((Rhythms.RhythmTrack)track);
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

        public void AcceptTimelineMovement()
        {
            _currentInputState = InputState.ControlTimeline;
        }


        #endregion

        #region State Control

        public void AcceptStateMovement()
        {
            _currentInputState = InputState.MoveState;
        }

        private void HandleStateMovementInput()
        {
            Event e = Event.current;

            int controlID = GUIUtility.GetControlID(_currentInputState.GetHashCode(), FocusType.Passive);
            int hotControl = GUIUtility.hotControl;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (!_inputStateActive)
                        {
                            //Get the owning timeline
                            _owningTimelineOfInput = TrackTimeline.FindOwningTimeline(this, e.mousePosition);
                            //Convert the mouse pos to the beat pos on this timeline
                            int beat = _owningTimelineOfInput.GetBeatForPosition(e.mousePosition);
                            //Get the state responsible for this beat
                            StateDrawer stateDrawer = _owningTimelineOfInput.GetStateForBeat(beat);
                            //Check if the click was within the View of the stateDrawer
                            if (stateDrawer != null && stateDrawer.View.Contains(e.mousePosition))
                            {
                                _inputStateActive = true;
                                _lastMousePos = e.mousePosition;

                                _selectedState = stateDrawer;

                                GUIUtility.hotControl = controlID;
                                e.Use();
                            }
                        }
                    }
                    break;

                case EventType.MouseUp:
                    //if (hotControl == controlID)
                    {
                        if (_inputStateActive)
                        {
                            _selectedState.SetBeat(_owningTimelineOfInput.GetBeatForPosition(e.mousePosition));
                            if (!_owningTimelineOfInput.MoveStateTo(_selectedState.State, _selectedState.Beat))
                            {
                                _selectedState.SetBeat(_owningTimelineOfInput.Track.GetBeatForState(_selectedState.State));
                            }
                            _owningTimelineOfInput = null;
                            _inputStateActive = false;
                        }

                        GUIUtility.hotControl = 0;
                        e.Use();
                    }

                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (_inputStateActive)
                        {
                            bool currentTimeline = _owningTimelineOfInput.View.Contains(e.mousePosition);
                            if (!currentTimeline)
                            {
                                //Get the timeline it belongs to now and cache the old timeline
                                TrackTimeline newTimeline = TrackTimeline.FindOwningTimeline(_owningTimelineOfInput, e.mousePosition);
                                if (newTimeline != null)
                                {
                                    _owningTimelineOfInput.DeleteState(_selectedState.Beat);

                                    _owningTimelineOfInput = newTimeline;
                                    _selectedState.OwningTimeline = _owningTimelineOfInput;

                                    _owningTimelineOfInput.AcceptState(_selectedState);
                                    _selectedState.SetView();
                                }
                            }

                            _selectedState.SetBeat(_owningTimelineOfInput.GetBeatForPosition(e.mousePosition));
                            _owningTimelineOfInput.MoveStateTo(_selectedState.State, _selectedState.Beat);

                            _lastMousePos = e.mousePosition;

                            _refresh = true;
                        }
                    }
                    break;
            }
        }

        #endregion

        public void DisableCurrentInputState()
        {
            _inputStateActive = false;
            _owningTimelineOfInput = null;
        }

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
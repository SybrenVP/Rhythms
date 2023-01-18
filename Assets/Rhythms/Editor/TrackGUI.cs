using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PlasticPipe.PlasticProtocol.Messages;

namespace RhythmEditor
{
    public class TrackGUI
    {
        //Main editor window
        private RhythmSequenceEditor _sequenceEditorWindow = null;

        //Data
        private Rhythm.Sequence _sequence = null;
        private Rhythm.Track _track = null;

        //Owner of this GUI
        private TimelineGUI _timelineGUI = null;

        //GUI owned by this class
        private List<StateDrawer> _stateDrawers = new List<StateDrawer>();

        #region Variables :: Size

        public Rect BaseRect;
        public Rect View { get; private set; }

        public Vector2 Size;
        public Vector2 Position;

        #endregion

        #region Variables :: Consts

        private Color _backgroundColor = Color.grey;
        private readonly Color _shadowColor = Color.black;

        public static readonly float MINHEIGHT = 250f;
        public static readonly float MINWIDTH = 500f;

        public static readonly float WIDTHPERSEC = 50f;

        public static readonly int SHADOWWIDTH = 7;

        #endregion

        #region ContextMenu

        private static readonly int CONTEXTMENU_CONTROLID = "ContextMenu".GetHashCode();
        private GUIContent _contextMenuTitleContent;

        private readonly string[] _contextMenuItems =
        {
            "New State", "New Variable"
        };

        #endregion

        #region Initialize

        public TrackGUI(Rhythm.Track track, Rhythm.Sequence seq, TimelineGUI timeline, RhythmSequenceEditor editor)
        {
            _sequenceEditorWindow = editor;
            _timelineGUI = timeline;
            _sequence = seq;
            _track = track;

            InitializeStateGUIList();

            _contextMenuTitleContent = new GUIContent("Track");
        }

        private void InitializeStateGUIList()
        {
            List<Rhythm.State> createdStates = new List<Rhythm.State>();

            //Track.States is an IntState Dictionary
            foreach (KeyValuePair<int, Rhythm.State> state in _track.States)
            {
                if (state.Value == null)
                {
                    Debug.LogError("Serialization failed and a null state was saved");
                    _track.States.Clear();
                    _stateDrawers.Clear();
                    return;
                }

                if (createdStates.Contains(state.Value))
                    continue;

                int beat = _track.GetBeatForState(state.Value);
                if (beat >= 0)
                {
                    createdStates.Add(state.Value);
                    AddStateGUI(state.Value, beat);
                }
            }
        }

        private void AddStateGUI(Rhythm.State state, int beat)
        {
            StateDrawer newStateDrawer = new StateDrawer(state, beat, this, _timelineGUI, _sequenceEditorWindow);

            RefreshStatePositionAndSize(newStateDrawer, beat, state.LengthInBeats);

            _stateDrawers.Add(newStateDrawer);
        }

        public void RefreshStatePositionAndSize(StateDrawer drawer, int beat, int length)
        {
            drawer.SetPositionAndSize(new Vector2(_timelineGUI.GetPositionForBeat(beat), BaseRect.y), new Vector2(TimelineGUI.WIDTH_PER_BEAT * length, BaseRect.height));

            Rect calculatedView = drawer.BaseRect;
            calculatedView.position -= View.position - BaseRect.position;

            drawer.SetView(calculatedView);
        }

        #endregion

        #region WindowDrawing

        public void DrawBackground()
        {
            EditorGUI.DrawRect(BaseRect, _backgroundColor);
            Utility.DrawShadowRect(BaseRect, new Inset(0f, 0f, 0f, 0f), 2, _shadowColor);
        }

        public void OnGUI()
        {
            
        }

        public void OnStateGUI()
        {
            int cachedCount = _stateDrawers.Count;
            foreach (StateDrawer stateDrawer in _stateDrawers)
            {
                stateDrawer.OnGUI();
                if (_stateDrawers.Count != cachedCount)
                    break;
            }
        }

        public void OnStateGhostGUI()
        {
            int cachedCount = _stateDrawers.Count;
            foreach (StateDrawer stateDrawer in _stateDrawers)
            {
                stateDrawer.OnGhostGUI();
                if (_stateDrawers.Count != cachedCount)
                    break;
            }
        }

        public void HandleInput()
        {
            Event e = Event.current;

            int controlID = GUIUtility.GetControlID(CONTEXTMENU_CONTROLID, FocusType.Passive);

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:

                    if (e.button == 1)
                    {
                        bool canOpen = View.Contains(e.mousePosition);
                        if (!canOpen)
                            return;

                        CreateContextMenu(e.mousePosition);                       

                        e.Use();
                    }

                    break;
            }
        }

        #endregion  

        #region ContextMenu

        private void CreateContextMenu(Vector2 pos)
        {
            GenericMenu menu = new GenericMenu();
            
            int overlappingBeat = _timelineGUI.GetBeatForPosition(pos);

            if (_track.States.ContainsKey(overlappingBeat))
            {
                menu.AddItem(new GUIContent("Delete State"), false, RemoveState, overlappingBeat);
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("New State"), false, CreateState, pos);
            menu.AddItem(new GUIContent("Clear All States"), false, ClearAllStates);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Remove Track"), false, _timelineGUI.RemoveTrack, _track);

            menu.ShowAsContext();
        }

        private void CreateState(object mousePosition)
        {
            //Get the beat for this mouse position
            Vector2 mousePos = (Vector2)mousePosition;

            int beatNumber = _timelineGUI.GetBeatForPosition(mousePos);
            if (_track.States.ContainsKey(beatNumber))
            {
                Debug.Log("There's already a state on this beat");
                return;
            }

            Rhythm.State newState = (Rhythm.State)ScriptableObject.CreateInstance(typeof(Rhythm.State));
            _track.States.Add(beatNumber, newState);
            
            SetDirty();

            AddStateGUI(newState, beatNumber);
        }

        private void ClearAllStates()
        {
            _track.States.Clear();
            _stateDrawers.Clear();

            SetDirty();
        }

        public void RemoveState(object beat)
        {
            if (_track.States.ContainsKey((int)beat))
            {
                Rhythm.State state = _track.States[(int)beat];
                StateDrawer drawerToRemove = _stateDrawers.Find(drawer => drawer.State == state);

                for (int i = 0; i < state.LengthInBeats; i++)
                {
                    _track.States.Remove(drawerToRemove.Beat + i);
                }

                SetDirty();

                _stateDrawers.Remove(drawerToRemove);
            }
        }

        #endregion

        public void AcceptState(StateDrawer stateDrawer)
        {
            MoveStateTo(stateDrawer.State, stateDrawer.Beat);

            SetDirty();

            _stateDrawers.Add(stateDrawer);
        }

        public bool MoveStateTo(Rhythm.State state, int newBeatPos)
        {
            //Check if the new position is free

            if (!IsFree(newBeatPos, state))
                return false;

            //Clear the old positions

            List<int> oldStatePositions = GetBeatsForState(state);
            foreach (int oldBeat in oldStatePositions)
            {
                _track.States.Remove(oldBeat);
            }

            //Fill the new positions

            for (int beat = newBeatPos; beat < newBeatPos + state.LengthInBeats; beat++)
            {
                if (_track.States.ContainsKey(beat) && _track.States[beat] == state)
                    continue;
                else
                    _track.States.Add(beat, state);
            }

            SetDirty();

            return true;
        }

        #region Helpers

        public bool IsFree(int beat, Rhythm.State state)
        {
            for (int i = beat; i < beat + state.LengthInBeats; i++)
            {
                if (_track.States.ContainsKey(i) && _track.States[i] != state)
                {
                    return false;
                }
            }

            return true;
        }

        private List<int> GetBeatsForState(Rhythm.State state)
        {
            List<int> beatResults = new List<int>();
            foreach (var beatState in _track.States)
            {
                if (beatState.Value == state)
                    beatResults.Add(beatState.Key);
            }

            return beatResults;
        }

        public StateDrawer GetStateForBeat(int beat)
        {
            if (_track.States.ContainsKey(beat))
            {
                return _stateDrawers.Find(drawer => drawer.State == _track.States[beat]);
            }
            return null;
        }

        #endregion

        public void SetPositionAndSize(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;

            BaseRect = new Rect(position, size);
            View = new Rect(position, size);

            foreach (StateDrawer drawer in _stateDrawers)
                RefreshStatePositionAndSize(drawer, drawer.Beat, drawer.State.LengthInBeats);
        }

        public void SetView(Rect view)
        {
            View = view;
        }

        public void SetBackground(Color bg)
        {
            _backgroundColor = bg;
        }

        private void SetDirty()
        {
            _sequenceEditorWindow.SaveSequence();
        }
    }
}
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Rhythms_Editor
{


    public class TrackTimeline
    {
        public Rhythms.RhythmTrack Track = null;
        public Rhythms.AudioData Audio = null;

        public static Texture2D AudioWaveform;

        public Vector2 ScrollPosition = Vector2.zero;

        public Rect View;
        public Rect Full;

        public float WidthPerSec = 0f;
        public float WidthPerBeat = 0f;

        #region States

        private List<StateDrawer> _stateDrawers = new List<StateDrawer>();

        private Rect _autoScrollLeft;
        private Rect _autoScrollRight;

        #endregion

        #region Colors

        public Color BackgroundColor = Color.grey;
        public Color ForegroundColor = new Color(0.8f, 0.2f, 0.2f, 1f);
        public Color BeatColor = new Color(0f, 0f, 0f, 0.5f);
        public Color ShadowColor = Color.black;

        #endregion

        #region ContextMenu

        private static readonly int CONTEXTMENU_CONTROLID = "ContextMenu".GetHashCode();
        private GUIContent _contextMenuTitleContent;

        private readonly string[] _contextMenuItems =
        {
            "New State", "New Variable"
        };

        #endregion
        
        private RhythmSequenceEditor _editor;

        #region Read only

        public static readonly float MINHEIGHT = 250f;
        public static readonly float MINWIDTH = 500f;

        public static readonly float WIDTHPERSEC = 50f;

        public static readonly int SHADOWWIDTH = 7;

        private static readonly int AUTOSCROLLWIDTH = 25;

        private static readonly int SPACING_WIDTH = 50;

        #endregion

        #region Init

        public TrackTimeline(Rhythms.RhythmTrack track, Rhythms.AudioData audioData, RhythmSequenceEditor editor, Rect reservedView, Color background)
        {
            _editor = editor;

            Track = track;
            Audio = audioData;

            View = reservedView;
            Full = new Rect(Vector2.zero, new Vector2(WIDTHPERSEC * Audio.Song.length + 2 * SPACING_WIDTH, View.height));

            _autoScrollLeft = new Rect(0f, View.y, AUTOSCROLLWIDTH, View.height);
            _autoScrollRight = new Rect(View.width - AUTOSCROLLWIDTH, View.y, AUTOSCROLLWIDTH, View.height);

            BackgroundColor = background;

            GetWaveForm();

            WidthPerSec = (Full.width - 2 * SPACING_WIDTH) / Audio.Song.length;
            WidthPerBeat = WidthPerSec / Audio.BeatPerSec;

            InitStateDrawers();

            _contextMenuTitleContent = new GUIContent("Track");
        }

        private void InitStateDrawers()
        {
            List<Rhythms.RhythmState> createdStates = new List<Rhythms.RhythmState>();
            foreach (KeyValuePair<int, Rhythms.RhythmState> state in Track.States)
            {
                if (state.Value == null)
                {
                    Debug.LogWarning("Corrupt sequence, destroying states for Track " + _editor.Timelines.IndexOf(this) + " :("); //Temporary null catch, this will only be called if serialization is failing
                    Track.States.Clear();
                    _stateDrawers.Clear();
                    return;
                }

                if (createdStates.Contains(state.Value))
                    continue;

                int beat = Track.GetBeatForState(state.Value);
                if (beat > 0)
                {
                    createdStates.Add(state.Value);
                    AddStateDrawer(state.Value, beat);
                }
            }
        }

        private void AddStateDrawer(Rhythms.RhythmState state, int beat)
        {
            StateDrawer newStateDrawer = new StateDrawer(state, beat, this, _editor);
            _stateDrawers.Add(newStateDrawer);
        }

        private void GetWaveForm()
        {
            //if (!AudioWaveform)
            //{
            //    int width = (int)Full.width;
            //    int height = (int)Full.height;
            //
            //    AudioWaveform = Utility.GetWaveformTextureFromAudioClip(Audio.Song, width, height, ForegroundColor, BackgroundColor);
            //}

            //Full.width = AudioWaveform.width;
            //Full.height = AudioWaveform.height;
        }

        public void LoadPreviousState()
        {

        }

        #endregion

        #region WindowDrawing

        public void OnGUI()
        {
            #region Background visuals

            EditorGUI.DrawRect(View, BackgroundColor);

            Rect beatRect = new Rect(Audio.SongOffset * WidthPerSec + SPACING_WIDTH, 0, 0, View.height);
            Rect beatLabelRect = new Rect(Audio.SongOffset * WidthPerSec + 5f + SPACING_WIDTH, View.height - 40f, 100f, 20f);

            Utility.DrawShadowRect(View, new Inset(0f, 13f, 0f, 13f), SHADOWWIDTH, ShadowColor);

            ScrollPosition = GUI.BeginScrollView(View, ScrollPosition, Full, GUI.skin.horizontalScrollbar, GUIStyle.none);

            //GUI.DrawTexture(Full, AudioWaveform);
            
            for (int i = 0; i < Audio.AmountBeatsInSong; i++)
            {
                if (i % 4 == 0)
                {
                    beatRect.width = 2f;

                    EditorGUI.LabelField(beatLabelRect, i.ToString());
                }
                else
                {
                    beatRect.width = 1f;
                }

                EditorGUI.DrawRect(beatRect, BeatColor);

                beatRect.x += WidthPerBeat;
                beatLabelRect.x += WidthPerBeat;
            }

            Rect fullEnd = new Rect(Full.width - SPACING_WIDTH, Full.y, SPACING_WIDTH, Full.height);
            Utility.DrawShadowRect(fullEnd, new Inset(0f, 0f, -10f, -10f), 5, Color.red);

            GUI.EndScrollView(false);

            #endregion

            #region Debug

            //EditorGUI.DrawRect(_autoScrollLeft, Color.red);
            //EditorGUI.DrawRect(_autoScrollRight, Color.red);

            #endregion

            HandleInput();
        }

        public void OnStateGUI()
        {
            #region States

            int cachedCount = _stateDrawers.Count;
            foreach (StateDrawer stateDrawer in _stateDrawers)
            {
                stateDrawer.OnGUI();
                if (_stateDrawers.Count != cachedCount)
                    break;
            }

            #endregion
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

        private void HandleInput()
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
            
            int overlappingBeat = GetBeatForPosition(pos);

            if (Track.States.ContainsKey(overlappingBeat))
            {
                menu.AddItem(new GUIContent("Delete State"), false, RemoveState, overlappingBeat);
                menu.AddSeparator("");
            }

            menu.AddItem(new GUIContent("New State"), false, CreateState, pos);
            menu.AddItem(new GUIContent("Clear All States"), false, ClearAllStates);
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Remove Track"), false, _editor.RemoveTrack, Track);


            menu.ShowAsContext();
        }

        private void CreateState(object mousePosition)
        {
            //Get the beat for this mouse position
            Vector2 mousePos = (Vector2)mousePosition;

            int beatNumber = GetBeatForPosition(mousePos);
            if (Track.States.ContainsKey(beatNumber))
            {
                Debug.Log("There's already a state on this beat");
                return;
            }

            Rhythms.RhythmState newState = (Rhythms.RhythmState)ScriptableObject.CreateInstance(typeof(Rhythms.RhythmState));
            Track.States.Add(beatNumber, newState);
            SetDirty();

            AddStateDrawer(newState, beatNumber);
        }

        private void ClearAllStates()
        {
            Track.States.Clear();
            _stateDrawers.Clear();

            SetDirty();
        }

        public void RemoveState(object beat)
        {
            if (Track.States.ContainsKey((int)beat))
            {
                Rhythms.RhythmState state = Track.States[(int)beat];
                StateDrawer drawerToRemove = _stateDrawers.Find(drawer => drawer.State == state);

                for (int i = 0; i < state.LengthInBeats; i++)
                {
                    Track.States.Remove(drawerToRemove.Beat + i);
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

        public bool MoveStateTo(Rhythms.RhythmState state, int newBeatPos)
        {
            //Check if the new position is free

            if (!IsFree(newBeatPos, state))
                return false;

            //Clear the old positions

            List<int> oldStatePositions = GetBeatsForState(state);
            foreach (int oldBeat in oldStatePositions)
            {
                Track.States.Remove(oldBeat);
            }

            //Fill the new positions

            for (int beat = newBeatPos; beat < newBeatPos + state.LengthInBeats; beat++)
            {
                if (Track.States.ContainsKey(beat) && Track.States[beat] == state)
                    continue;
                else
                    Track.States.Add(beat, state);
            }

            SetDirty();

            return true;
        }

        #region Autoscrolling

        public Rect GetLeftAutoScrollRect()
        {
            return _autoScrollLeft;
        }

        public Rect GetRightAutoScrollRect()
        {
            return _autoScrollRight;
        }

        public void ScrollLeft()
        {
            if (ScrollPosition.x > 0)
                ScrollPosition.x -= WidthPerBeat;
        }

        public void ScrollRight()
        {
            if (ScrollPosition.x < Full.width - WidthPerBeat)
                ScrollPosition.x += WidthPerBeat;
        }

        public void Scroll(float diff)
        {
            ScrollPosition.x += diff;
        }

        #endregion

        #region Helpers

        public bool IsFree(int beat, Rhythms.RhythmState state)
        {
            for (int i = beat; i < beat + state.LengthInBeats; i++)
            {
                if (Track.States.ContainsKey(i) && Track.States[i] != state)
                {
                    return false;
                }
            }

            return true;
        }

        public int GetBeatForPosition(Vector2 pos)
        {
            int result = -1;

            result = (int)((pos.x + ScrollPosition.x) / WidthPerBeat);

            return result;
        }

        private List<int> GetBeatsForState(Rhythms.RhythmState state)
        {
            List<int> beatResults = new List<int>();
            foreach (var beatState in Track.States)
            {
                if (beatState.Value == state)
                    beatResults.Add(beatState.Key);
            }

            return beatResults;
        }

        public float GetXForBeat(int beat)
        {
            return (WidthPerBeat * beat) - ScrollPosition.x;
        }

        public StateDrawer GetStateForBeat(int beat)
        {
            if (Track.States.ContainsKey(beat))
            {
                return _stateDrawers.Find(drawer => drawer.State == Track.States[beat]);
            }
            return null;
        }

        #endregion

        public void SetView(Vector2 size, int id)
        {
            View.y = size.y * id;
            View.width = size.x;
            View.height = size.y;

            _autoScrollLeft = new Rect(0f, View.y, AUTOSCROLLWIDTH, View.height);
            _autoScrollRight = new Rect(View.x - AUTOSCROLLWIDTH, View.y, AUTOSCROLLWIDTH, View.height);

            Full = new Rect(Vector2.zero, new Vector2(WIDTHPERSEC * Audio.Song.length + 2 * SPACING_WIDTH, View.height));
        }

        public void SetBackground(Color bg)
        {
            BackgroundColor = bg;
        }

        //Finds the timeline with which pos overlaps
        public static TrackTimeline FindOwningTimeline(TrackTimeline currentOwner, Vector2 pos)
        {
            return FindOwningTimeline(currentOwner._editor, pos);
        }

        public static TrackTimeline FindOwningTimeline(RhythmSequenceEditor editor, Vector2 pos)
        {
            foreach (TrackTimeline timeline in editor.Timelines)
            {
                if (timeline.View.Contains(pos))
                    return timeline;
            }

            return null;
        }

        private void SetDirty()
        {
            EditorUtility.SetDirty(_editor.ActiveSequence);
            _editor.SaveSequence();
        }

        public void Destroy()
        {
            Object.DestroyImmediate(AudioWaveform);
        }
    }
}
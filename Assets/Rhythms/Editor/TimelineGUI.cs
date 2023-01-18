using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This manages the visual aspect of all of the Tracks together. The scroll window and makes sure the TrackGUIs communicate when they want repaint or other
/// </summary>
/// 
namespace RhythmEditor
{
    public class TimelineGUI
    {
        //Main editor window
        private RhythmSequenceEditor _sequenceEditorWindow;

        //Data
        private Rhythm.Sequence _sequence;

        //GUI owned by this class
        public List<TrackGUI> TrackGUIList = new List<TrackGUI>();

        #region Variables :: Size

        public Rect View;
        public Vector2 FullSize;
        private Rect _fullRect;
        public Vector2 TrackViewSize;

        #endregion

        #region Variables :: Consts

        private readonly Color _oddTrackBackground = new Color(0.3f, 0.3f, 0.3f, 1f);
        private readonly Color _evenTrackBackground = new Color(0.2f, 0.2f, 0.2f, 1f);

        private readonly Color _beatLineColor = new Color(0f, 0f, 0f, 0.5f);

        public const float WIDTH_PER_BEAT = 25f;

        private const int WIDTH_SPACING = 50;

        #endregion

        #region Variables :: Scroll

        private Vector2 _scrollPosition = Vector2.zero;

        #endregion

        #region Initialize

        public void Initialize(Rhythm.Sequence seq, RhythmSequenceEditor editor, Rect viewRect)
        {
            _sequence = seq;
            _sequenceEditorWindow = editor;

            //Decide width and height 
            Resize(viewRect);

            InitializeTrackGUIList();
        }

        private void InitializeTrackGUIList()
        {
            for (int i = 0; i < _sequence.Tracks.Count; i++)
            {
                if (_sequence.Tracks[i] == null)
                {
                    Debug.LogError("Corrupt sequence, destroying tracks :(");
                    _sequence.Tracks.Clear();
                    return;
                }

                TrackGUI trackGUI = null;
                Color trackBackground = i % 2 == 1 ? _oddTrackBackground : _evenTrackBackground;

                if (TrackGUIList.Count <= i)
                {
                    trackGUI = new TrackGUI(_sequence.Tracks[i], _sequence, this, _sequenceEditorWindow);
                    TrackGUIList.Add(trackGUI);
                }
                else
                {
                    trackGUI = TrackGUIList[i];
                }

                trackGUI.SetBackground(trackBackground);
                trackGUI.SetPositionAndSize(new Vector2(0f, TrackViewSize.y * i), new Vector2(FullSize.x, TrackViewSize.y));
            }
        }

        #endregion

        #region Size

        //This is to calculate the view, the visible UI space
        //The viewRect should be width of the window - inspector width, height of the window - toolbar height
        public void Resize(Rect viewRect)
        {
            View = viewRect;
            FullSize = CalculateFullSize();
            _fullRect = new Rect(Vector2.zero, FullSize);
            TrackViewSize = CalculateTrackSize();
        }

        private Vector2 CalculateFullSize()
        {
            Vector2 fullSize = Vector2.zero;

            //The width of the song
            fullSize.x = WIDTH_PER_BEAT * _sequence.Audio.AmountBeatsInSong + 2 * WIDTH_SPACING;

            //The height of all tracks
            fullSize.y = Mathf.Max(View.height, _sequence.Tracks.Count * TrackGUI.MINHEIGHT);

            return fullSize;
        }

        private Vector2 CalculateTrackSize()
        {
            Vector2 trackSize = Vector2.zero;

            //The width of a track
            trackSize.x = FullSize.x;

            //The height of a track
            trackSize.y = FullSize.y / _sequence.Tracks.Count;

            return trackSize;
        }

        #endregion

        #region Update

        public void OnGUI()
        {
            _scrollPosition = GUI.BeginScrollView(View, _scrollPosition, _fullRect, true, true);

            //First we draw the background while defining the new view space of the tracks by considering the scroll position
            foreach (TrackGUI trackGUI in TrackGUIList)
            {
                Rect calculatedView = trackGUI.BaseRect;
                calculatedView.position = new Vector2(_scrollPosition.x + calculatedView.position.x, calculatedView.position.y - _scrollPosition.y);

                trackGUI.SetView(calculatedView);
                trackGUI.DrawBackground();
            }

            //On top of the background, we can draw lines per beat
            Rect beatRect = new Rect(_sequence.Audio.SongOffsetInBeats * WIDTH_PER_BEAT + WIDTH_SPACING, 0, 0, FullSize.y);
            Rect beatLabelRect = new Rect(_sequence.Audio.SongOffsetInBeats * WIDTH_PER_BEAT + 5f + WIDTH_SPACING, View.height - 40f + _scrollPosition.y, 100f, 20f);
            
            for (int i = 0; i < _sequence.Audio.AmountBeatsInSong; i++)
            {
                if (i % 4 == 0) //Every 4 beats, we want to add a label stating the beat count
                {
                    beatRect.width = 2f;

                    EditorGUI.LabelField(beatLabelRect, i.ToString());
                }
                else
                {
                    beatRect.width = 1f;
                }

                EditorGUI.DrawRect(beatRect, _beatLineColor);

                beatRect.x += WIDTH_PER_BEAT;
                beatLabelRect.x += WIDTH_PER_BEAT;
            }

            //Draw a rect to indicate the end of the audio 
            Rect fullEnd = new Rect(FullSize.x - WIDTH_SPACING, 0f, WIDTH_SPACING, FullSize.y);
            Utility.DrawShadowRect(fullEnd, new Inset(0f, 0f, -10f, -10f), 5, Color.red);

            //Next up is just the plain GUI
            foreach (TrackGUI trackGUI in TrackGUIList)
            {
                trackGUI.OnGUI();
            }

            //Next up is the states
            foreach (TrackGUI timeline in TrackGUIList)
            {
                timeline.OnStateGUI();
            }

            //Next up is the ghost version of states
            foreach (TrackGUI timeline in TrackGUIList)
            {
                timeline.OnStateGhostGUI();
            }

            GUI.EndScrollView(true);
        }

        public void HandleInput()
        {
            foreach (TrackGUI trackGUI in TrackGUIList)
            {
                trackGUI.HandleInput();
            }
        }

        public void Update()
        {

        }

        #endregion

        #region Accessors

        //TODO: Change this to be listening to the toolbar buttons
        public void AddTrack()
        {
            Rhythm.Track newTrack = ScriptableObject.CreateInstance<Rhythm.Track>();
            _sequence.Tracks.Add(newTrack);

            _sequenceEditorWindow.SaveSequence();

            Resize(View);
            InitializeTrackGUIList();
        }

        public void RemoveTrack(object track)
        {
            int trackIndex = _sequence.Tracks.IndexOf(track as Rhythm.Track);
            if (trackIndex >= 0)
            {
                _sequence.Tracks.RemoveAt(trackIndex);
                TrackGUIList.RemoveAt(trackIndex);

                _sequenceEditorWindow.SaveSequence();

                Resize(View);
                InitializeTrackGUIList();
            }
        }
        
        public static TrackGUI FindOwningTrackGUI(TimelineGUI timelineGUI, Vector2 pos)
        {
            foreach (TrackGUI trackGUI in timelineGUI.TrackGUIList)
            {
                //Rect calculatedView = trackGUI.BaseRect;
                //calculatedView.position -= timelineGUI._scrollPosition;

                if (trackGUI.View.Contains(pos))
                    return trackGUI;
            }

            return null;
        }

        public int GetBeatForPosition(Vector2 pos)
        {
            int result = -1;

            result = (int)(((pos.x + _scrollPosition.x) - WIDTH_SPACING + 0.5f * WIDTH_PER_BEAT) / WIDTH_PER_BEAT);
            Debug.Log(result);
            return result;
        }

        public float GetPositionForBeat(int beat)
        {
            return (WIDTH_PER_BEAT * beat - _scrollPosition.x - WIDTH_PER_BEAT * 0.5f) + WIDTH_SPACING;
        }

        #endregion

    }
}
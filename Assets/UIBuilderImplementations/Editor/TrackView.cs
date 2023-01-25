using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TrackView : VisualElement
{
    public Action<StateView> OnStateSelected;
    private Track _track;

    #region ScrollView

    private ScrollView _scrollView;
    private Scroller _horizontalScroller;
    private Scroller _verticalScroller;

    private VisualElement _scrollViewContent;
    private IMGUIContainer _timelineTime;

    #endregion

    private List<StateView> _stateViews = new List<StateView>();

    private VisualElement _timelineTimeStampVisual;

    private StateSelector _stateSelector;

    //private VisualElement _selectionRect;
    //private Vector2 _start;
    //private Vector2 _end;

    private const float WIDTH_PER_BEAT = 40f;
    private const float LEFT_SPACING = 10f;
    private const float RIGHT_SPACING = 20f;

    private const float TOP_SPACING = 10f;
    public const float HEIGHT_PER_TRACK = 100f;
    private const float BOTTOM_SPACING = 20f;

    private GUIStyle _beatLabelStyle = GUIStyle.none;

    public new class UxmlFactory : UxmlFactory<TrackView, TrackView.UxmlTraits> { }
    public TrackView()
    {
        this.AddManipulator(new ContextualMenuManipulator((evt) =>
        {
            Vector2 localPos = this.WorldToLocal(evt.mousePosition);

            int beat = ConvertPositionToBeat(localPos);
            int trackId = ConvertPositionToTrackId(localPos);
            
            List<BehaviourTree> clickedStates = GetStates(beat, trackId);
            if (clickedStates.Count > 0)
            {
                evt.menu.AppendAction("Remove State", (a) => RemoveStates(clickedStates));
                evt.menu.AppendSeparator();
                //evt.menu.AppendAction("Copy", (a) => CopyState(clickedStates));
                //evt.menu.AppendAction("Cut", (a) => CutState(clickedStates));
                //evt.menu.AppendAction("Paste", (a) => PasteState(clickedStates));
            }
            else
                evt.menu.AppendAction("Add State", (a) => CreateState(beat, trackId));
        }));

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(_track);
        AssetDatabase.SaveAssets();
    }

    public void PopulateView(Track track)
    {
        _track = track;

        ScrollView scrollView = this.Q<ScrollView>(); 
        if (scrollView != null)
        {
            _scrollView = scrollView;
            _horizontalScroller = scrollView.horizontalScroller;
            _verticalScroller = scrollView.verticalScroller;

            _scrollViewContent = _scrollView.Q<VisualElement>("scrollview-content");
            _scrollViewContent.style.width = WIDTH_PER_BEAT * _track.AudioData.AmountBeatsInSong + LEFT_SPACING + RIGHT_SPACING;
            DefineScrollContentHeight();
        }

        CreateBackground();

        _timelineTimeStampVisual = this.Q<VisualElement>("DraggerVisual");

        //Create state views
        foreach (BehaviourTree state in _track.States)
        {
            CreateStateView(state);
        }

        _stateSelector = new StateSelector(this, _scrollViewContent);
    }

    private void CreateBackground()
    {
        Color beatColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        Color extendedBeatColor = beatColor * 0.3f;
        extendedBeatColor.a = 1f;

        _beatLabelStyle.fontSize = 11;
        _beatLabelStyle.normal.textColor = beatColor;

        _timelineTime = this.Q<IMGUIContainer>();
        _timelineTime.style.width = WIDTH_PER_BEAT * _track.AudioData.AmountBeatsInSong + LEFT_SPACING + RIGHT_SPACING;
        _timelineTime.onGUIHandler = () =>
        {
            Event e = Event.current;
            if (e.type == EventType.Repaint)
            {
                Rect beatRect = new Rect(_track.AudioData.SongOffsetInBeats * WIDTH_PER_BEAT + LEFT_SPACING, 0, 1f, 20f);
                Rect beatLabelRect = new Rect(_track.AudioData.SongOffsetInBeats * WIDTH_PER_BEAT + 5f + LEFT_SPACING, 2f, 100f, 20f);

                for (int i = 0; i < _track.AudioData.AmountBeatsInSong; i++)
                {
                    if (i % 4 == 0) //Every 4 beats, we want to add a label stating the beat count
                    {
                        beatRect.height = 15f;
                        beatRect.y = 8f;

                        EditorGUI.LabelField(beatLabelRect, i.ToString(), _beatLabelStyle);
                    }
                    else
                    {
                        beatRect.height = 10f;
                        beatRect.y = 13f;
                    }

                    EditorGUI.DrawRect(beatRect, beatColor);

                    //Move beatRect and draw the rest
                    beatRect.y = beatRect.height + beatRect.y;
                    beatRect.height = resolvedStyle.height - beatRect.y;

                    EditorGUI.DrawRect(beatRect, extendedBeatColor);


                    beatRect.x += WIDTH_PER_BEAT;
                    beatLabelRect.x += WIDTH_PER_BEAT;
                }
            }
        };
    }

    #region Selection

    public List<StateView> GetRectBasedStateViews(Rect rect)
    {
        List<StateView> stateViews = new List<StateView>();
        foreach (StateView state in _stateViews)
        {
            if (state.layout.Overlaps(rect))
            {
                stateViews.Add(state);
            }
        }

        return stateViews;
    }

    public void DisableSelection()
    {
        _stateSelector.DisableSelection();
    }

    public void EnableSelection()
    {
        _stateSelector.EnableSelection();
    }

    #endregion

    #region State Move

    public void DisableMove()
    {
        foreach (StateView stateView in _stateViews)
            stateView.DisableMove();
    }

    public void EnableMove()
    {
        foreach (StateView stateView in _stateViews)
            stateView.EnableMove();
    }

    public void TrackVerticalScroll(Vector2 pos, Vector2 prevPos)
    {
        float deltaY = pos.y - prevPos.y;
        _verticalScroller.value += deltaY * 2f;
    }

    #endregion

    public int ConvertPositionToBeat(Vector2 pos)
    {
        int result = -1;

        result = (int)(((pos.x + _horizontalScroller.value + 0.5f * WIDTH_PER_BEAT) - LEFT_SPACING) / WIDTH_PER_BEAT);

        return result;
    }

    public float ConvertBeatToPosition(int beat)
    {
        return WIDTH_PER_BEAT * beat + LEFT_SPACING - (0.5f * WIDTH_PER_BEAT);
    }

    public int ConvertPositionToTrackId(Vector2 pos)
    {
        int result = -1;

        result = (int)((pos.y - TOP_SPACING) / HEIGHT_PER_TRACK);

        return result;
    }

    public float ConvertTrackIdToPosition(int trackId)
    {
        return HEIGHT_PER_TRACK * trackId + TOP_SPACING;
    }

    public bool IsWithinExpandArea(Vector2 pos)
    {
        return pos.y > _scrollViewContent.layout.height - BOTTOM_SPACING;
    }

    public void Expand()
    {
        _track.AmountOfTracks++;

        DefineScrollContentHeight();
    }

    public void TryShrink()
    {
        if (_track.AmountOfTracks > 1)
        {
            foreach (BehaviourTree state in _track.States)
            {
                int trackId = ConvertPositionToTrackId(state.Position);
                if (trackId == _track.AmountOfTracks - 1)
                    return;
            }

            _track.AmountOfTracks--;
            Debug.Log("Amount of tracks: " + _track.AmountOfTracks);
            DefineScrollContentHeight();
        }
    }

    private void DefineScrollContentHeight()
    {
        _scrollViewContent.style.height = (HEIGHT_PER_TRACK * _track.AmountOfTracks) + TOP_SPACING + BOTTOM_SPACING;
    }

    private List<BehaviourTree> GetStates(int beat, int trackId)
    {
        List<BehaviourTree> states = new List<BehaviourTree>();

        foreach (StateView state in _stateViews)
        {
            if (state.State.Beat == beat && Mathf.Abs(state.style.top.value.value - ConvertTrackIdToPosition(trackId)) < Mathf.Epsilon)
            {
                states.Add(state.State);
            }
        }

        return states;
    }

    private void RemoveStates(List<BehaviourTree> states)
    {
        foreach (StateView stateView in _stateViews)
        {
            if (states.Contains(stateView.State))
            {
                _track.DeleteState(stateView.State);
                _scrollViewContent.Remove(stateView);
            }
        }

        _stateViews.RemoveAll(view => states.Contains(view.State));

        OnStateSelected?.Invoke(null);
    }

    private void CreateState(int beat, int trackId)
    {
        BehaviourTree state = _track.CreateState(beat);
        state.Position = new Vector2(ConvertBeatToPosition(beat), ConvertTrackIdToPosition(trackId));
        CreateStateView(state);

        //TODO: _selectionRect.BringToFront();
    }

    private void CreateStateView(BehaviourTree state)
    {
        StateView stateView = new StateView(state, this);
        _stateViews.Add(stateView);
        stateView.OnStateSelected = OnStateSelected;
        _scrollViewContent.Add(stateView);
    }
}

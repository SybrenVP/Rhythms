using GluonGui.Dialog;
using Rhythm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TrackView : VisualElement
{
    public Action<StateView> OnStateSelected;
    private Track _track;

    private ScrollView _scrollView;
    private Scroller _verticalScroller;
    private Scroller _horizontalScroller;

    private VisualElement _scrollViewContent;

    private const float WIDTH_PER_BEAT = 40f;
    private const float LEFT_SPACING = 10f;
    private const float RIGHT_SPACING = 20f;

    private GUIStyle _beatLabelStyle = GUIStyle.none;

    public new class UxmlFactory : UxmlFactory<TrackView, TrackView.UxmlTraits> { }
    public TrackView()
    {
        this.AddManipulator(new ContextualMenuManipulator((evt) =>
        {
            ConvertPositionToBeat(evt.mousePosition);
            //evt.menu.AppendAction("Add State", (a) => CreateState(ConvertPositionToBeat(evt.mousePosition)));
        }));
    }

    public void PopulateView(Track track)
    {
        _track = track;

        //Create state views
        foreach (BehaviourTree state in _track.States)
        {
            CreateStateView(state);
        }

        ScrollView scrollView = this.Q<ScrollView>();
        if (scrollView != null)
        {
            _scrollView = scrollView;
            _verticalScroller = scrollView.verticalScroller;
            _horizontalScroller = scrollView.horizontalScroller;

            _scrollViewContent = _scrollView.Q<VisualElement>("scrollview-content");
            _scrollViewContent.style.width = WIDTH_PER_BEAT * _track.AudioData.AmountBeatsInSong + LEFT_SPACING + RIGHT_SPACING;
        }

        CreateBackground();
    }

    private void CreateBackground()
    {
        Color beatColor = new Color(0.8f, 0.8f, 0.8f, 1f);
        Color extendedBeatColor = beatColor * 0.3f;
        extendedBeatColor.a = 1f;

        _beatLabelStyle.fontSize = 11;
        _beatLabelStyle.normal.textColor = beatColor;

        IMGUIContainer beatRectContainer = this.Q<IMGUIContainer>();
        beatRectContainer.style.width = WIDTH_PER_BEAT * _track.AudioData.AmountBeatsInSong + LEFT_SPACING + RIGHT_SPACING;
        beatRectContainer.onGUIHandler = () =>
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
        };
    }

    private int ConvertPositionToBeat(Vector2 pos)
    {
        int result = -1;

        pos = this.WorldToLocal(pos);
        result = (int)(((pos.x + _horizontalScroller.value + 0.5f * WIDTH_PER_BEAT) - LEFT_SPACING) / WIDTH_PER_BEAT);

        Debug.Log(result);

        return result;
    }

    private float ConvertBeatToPosition(int beat)
    {
        return WIDTH_PER_BEAT * beat;

    }

    private void CreateState(int beat)
    {
        BehaviourTree state = _track.CreateState(beat);
        CreateStateView(state);
    }

    private void CreateStateView(BehaviourTree state)
    {
        StateView stateView = new StateView(state);
        stateView.OnStateSelected = OnStateSelected;
        Add(stateView);
    }

    private void CreateBeatView(float xPos)
    {
        Vector2 startPos = new Vector2(20, 50);
        Vector2 endPos = new Vector2(50f, 50f);

        BeatView beatView = new BeatView(startPos, endPos, 10f);
        Add(beatView);
    }
}

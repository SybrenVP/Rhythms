//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

///// <summary>
///// Visualizing the background of the rythm editor
///// </summary>
//public class TimelineViewer
//{
//    #region Public 

//    public static Rect TimelineRect;

//    public int AmtRows = 4;
//    public float Offset = 200f;

//    public static float HEIGHT_PER_ROW = 0f;

//    #endregion

//    #region Protected

//    protected SequenceEditor _sequenceEditor;

//    protected int _amtTimelineControls = 0;
//    protected Vector2 _timelineControlButtonSize = new Vector2(50f, 50f);



//    #endregion

//    public void Init(SequenceEditor editor)
//    {
//        _sequenceEditor = editor;
//    }

//    public void DrawWithinScroll(Rect scrollViewableRect)
//    {
//        Sequence seq = _sequenceEditor.LoadedSequence;
//        LoadedSequenceInformation info = _sequenceEditor.Info;

//        //Define the height per Row
//        HEIGHT_PER_ROW = (_sequenceEditor.position.height - _sequenceEditor.Inspector.Height) / AmtRows;

//        //Create the label rect and the rect per beat
//        Rect beatLabelRect = new Rect(seq.SongOffset * info.WidthPerSec + 5f + Offset, _sequenceEditor.position.height - _sequenceEditor.Inspector.Height - _timelineControlButtonSize.y, _timelineControlButtonSize.x, _timelineControlButtonSize.y);
//        Rect beatRect = new Rect(seq.SongOffset * info.WidthPerSec + Offset, 0, 0, _sequenceEditor.position.height - _sequenceEditor.Inspector.Height);

//        //Start drawing every beatline
//        for (int i = 0; i < info.AmtBeatsInSong; i++)
//        {
//            beatRect.x += info.WidthPerBeat * i;
//            if (!scrollViewableRect.Contains(beatRect.center)) //Only draw beats which are visible
//                continue;

//            beatRect.width = 1f;

//            if (i % 4 == 0) //Every 4th beat draw a thicker line and add a label of which beat it is
//            {
//                beatRect.width = 2f;
//                beatLabelRect.x += info.WidthPerBeat * i;

//                EditorGUI.LabelField(beatLabelRect, i.ToString());

//                beatLabelRect.x = seq.SongOffset * info.WidthPerSec + 5f + Offset;
//            }

//            EditorGUI.DrawRect(beatRect, Color.black); //Draw the beat line
//            beatRect.x = seq.SongOffset * info.WidthPerSec + Offset;
//        }
//    }

//    public void DrawGUI()
//    {
//        for (int i = 0; i < AmtRows; i++) //Draw every row line
//        {
//            EditorGUI.DrawRect(new Rect(0f, HEIGHT_PER_ROW * (i + 1), _sequenceEditor.position.width, 1f), Color.gray);
//        }

//        _amtTimelineControls = 0;

//        if (DrawTimelineControl(new GUIContent("Inspector")))
//            ToggleInspector();

//        if (DrawTimelineControl(new GUIContent("Play")))
//            StartPlaying();

//        if (DrawTimelineControl(new GUIContent("Stop")))
//            StopPlaying();

//        if (DrawTimelineControl(new GUIContent("Settings")))
//            OpenSettings();
//    }

//    public bool DrawTimelineControl(GUIContent content)
//    {
//        bool value;

//        value = GUI.Button(new Rect(_amtTimelineControls * _timelineControlButtonSize.x, 0f, _timelineControlButtonSize.x, _timelineControlButtonSize.y), content);

//        _amtTimelineControls++;
//        return value;
//    }

//    public void ToggleInspector()
//    {
//        _sequenceEditor.Inspector.Toggle();
//    }

//    public void StartPlaying()
//    {
//        _sequenceEditor.StartPlaying();
//    }

//    public void StopPlaying()
//    {
//        _sequenceEditor.StopPlaying();
//    }

//    public void OpenSettings()
//    {

//    }
//}

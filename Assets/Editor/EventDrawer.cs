//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class EventDrawer
//{
//    #region Protected

//    protected SequenceEditor _sequenceEditor;

//    #endregion

//    public void Init(SequenceEditor editor)
//    {
//        _sequenceEditor = editor;
//    }

//    public void DrawWithinScroll(Rect scrollViewableRect)
//    {
//        Sequence seq = _sequenceEditor.LoadedSequence;
//        for (int i = 0; i < seq.Events.Count; i++)
//        {
//            if (scrollViewableRect.Contains(seq.Events[i].Editor_Rect.position))
//                DrawEvent(seq.Events[i]);

//            if (seq.Events[i].Response != null)
//                DrawConnection(seq.Events[i]);
//        }
//    }

//    public void UpdateEventPositions()
//    {
//        foreach (SequenceEvent_Object seqEv in _sequenceEditor.LoadedSequence.Events)
//        {
//            seqEv.Editor_Rect = new Rect(_sequenceEditor.Info.WidthPerBeat * seqEv.BeatPos + _sequenceEditor.LoadedSequence.SongOffset * _sequenceEditor.Info.WidthPerSec + _sequenceEditor.TimelineViewer.Offset, TimelineViewer.HEIGHT_PER_ROW * seqEv.PlacedOnLine + TimelineViewer.HEIGHT_PER_ROW / 4f, _sequenceEditor.Info.WidthPerBeat * seqEv.BaseEvent.LengthInBeats, TimelineViewer.HEIGHT_PER_ROW / 2f);
//        }
//    }

//    protected void DrawEvent(SequenceEvent_Object ev)
//    {
//        Handles.BeginGUI();

//        Handles.DrawSolidRectangleWithOutline(ev.Editor_Rect, Color.gray, Color.black);

//        Handles.EndGUI();

//        GUILayout.BeginArea(ev.Editor_Rect);

//        GUI.contentColor = Color.black;

//        GUILayout.Label(ev.BeatPos.ToString());
//        GUILayout.Label(ev.BaseEvent.Type.ToString());
//        GUILayout.Label(ev.BaseEvent.name.Substring(ev.BaseEvent.Type == SequenceEventType.Call ? 5 : 3));

//        GUI.contentColor = Color.white;

//        GUILayout.EndArea();
//    }

//    protected void DrawConnection(SequenceEvent_Object ev)
//    {
//        Handles.BeginGUI();

//        foreach (SequenceEvent_Object re in ev.Response)
//            Handles.DrawLine(ev.Editor_Rect.center, re.Editor_Rect.center);

//        Handles.EndGUI();
//    }
//}

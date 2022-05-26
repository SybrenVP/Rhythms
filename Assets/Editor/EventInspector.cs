//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class EventInspector
//{
//    protected SequenceEditor _sequenceEditor = null;

//    public float Height = 0f;

//    protected bool _openInspector = false;

//    public void Init(SequenceEditor editor)
//    {
//        _sequenceEditor = editor;
//    }

//    public void Toggle()
//    {
//        _openInspector = !_openInspector;
//        Height = _openInspector ? 200f : 0f;
//        TimelineViewer.HEIGHT_PER_ROW = (_sequenceEditor.position.height - Height) / 4f;

//        _sequenceEditor.EventDrawer.UpdateEventPositions();
//    }
//}

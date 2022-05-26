////NEW SEQUENCE EDITOR

//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;
//using System.Linq;

//public enum SequenceEditorState
//{
//    Default,
//    Playing,
//    MovingEvents,
//    Selecting
//}

//public class SequenceEditor : EditorWindow
//{
//    #region Editors

//    public EventInspector Inspector = null;
//    public TimelineViewer TimelineViewer = null;
//    public EventDrawer EventDrawer = null;

//    public LoadedSequenceInformation Info = null;

//    #endregion

//    #region Sequence information

//    public Sequence LoadedSequence = null;

//    protected List<SequenceEvent_Object> _selectedEvents = new List<SequenceEvent_Object>();

//    protected float _progress = 0f;
//    protected float _startedPlayingTime = 0f;

//    #endregion

//    #region Timeline information

//    protected SequenceEditorState _currentState = SequenceEditorState.Default;

//    protected float _progressBeforePlaying = 0f;
//    protected bool _changedPlayingToCurrent = false;

//    #endregion

//    #region Inspector information

//    protected bool _openInspector = false;

//    protected Rect _inspectorRect = new Rect();

//    #region Folderstructure

//    protected Dictionary<SequenceEvent, string> _allCallEvents = null;
//    protected Dictionary<string, bool> _folderFoldout = new Dictionary<string, bool>();
//    protected Vector2 _folderScrollPos = Vector2.zero;

//    #endregion

//    #endregion

//    #region Rects

//    Rect _progressRect = new Rect();
//    Rect _progressBeatRect = new Rect();

//    Rect _scrollViewableRect = new Rect();

//    Rect _selectionRect = new Rect();

//    #endregion

//    #region Editor information

//    protected Rect _lastWindowSize;

//    protected Vector2 _scrollPosition = Vector2.zero;

//    #endregion

//    [MenuItem("Window/SequenceEditor")]
//    public static void OpenWindow()
//    {
//        SequenceEditor window = FindObjectOfType<SequenceEditor>();
//        if (window != null)
//            DestroyImmediate(window);

//        window = CreateInstance<SequenceEditor>();

//        window.Initiate();

//        window.Show();

//        window.Info = new LoadedSequenceInformation();
//        window.Info.WidthPerSec = 50f;

//        window.OpenSequence(Selection.activeObject as Sequence);

//        window.RefreshEvents();
//    }

//    protected void Initiate()
//    {
//        TimelineViewer = new TimelineViewer();

//    }

//    public void OnFocus()
//    {
//        RefreshEvents();
//    }

//    protected void Update()
//    {
//        if (LoadedSequence == null)
//            return;

//        #region Progress

//        if (_currentState == SequenceEditorState.Playing && !EditorApplication.isPlaying)
//        {
//            _progress = AudioUtility.GetClipPosition(LoadedSequence.Song);
            
//            Repaint();
//        }
//        else if (EditorApplication.isPlaying)
//        {
//            //Update progress during playtime here
//            RhythmController rhythm = FindObjectOfType<RhythmController>();
//            if (rhythm.LoadedSequence != null && rhythm.LoadedSequence != LoadedSequence)
//            {
//                OpenSequence(rhythm.LoadedSequence);
//            }

//            if (rhythm.MusicSource.isPlaying)
//            {
//                _progress = rhythm.SongPosition;
//                Repaint();
//            }
//        }

//        #endregion

//        #region Selection

//        if (_currentState == SequenceEditorState.Selecting)
//            Repaint();

//        #endregion

//        if (_lastWindowSize != position)
//            OpenSequence(LoadedSequence);
//    }

//    protected void OnGUI()
//    {
//        if (LoadedSequence == null)
//            return;

//        #region Input

//        var e = Event.current;

//        switch (e.type)
//        {
//            case EventType.MouseDown:
//                if (e.button == 0)
//                {
//                    if (_currentState == SequenceEditorState.Default)
//                    {
//                        if (_scrollViewableRect.Contains(e.mousePosition))
//                        {
//                            _currentState = SequenceEditorState.Selecting;
//                            _selectionRect.size = Vector2.zero;
//                            _selectionRect.position = e.mousePosition;
//                        }

//                        foreach (SequenceEvent_Object seqEv in LoadedSequence.Events)
//                        {
//                            Rect properSeqEVRect = new Rect(seqEv.Editor_Rect);
//                            properSeqEVRect.x -= _scrollPosition.x;
//                            if (properSeqEVRect.Contains(e.mousePosition) && _selectedEvents.Contains(seqEv))
//                            {
//                                _currentState = SequenceEditorState.MovingEvents;
//                                break;
//                            }
//                            else if(properSeqEVRect.Contains(e.mousePosition))
//                            {
//                                StartDrag(seqEv);
//                                break;
//                            }
//                        }
//                    }
//                }
//                break;
//            case EventType.MouseUp:
//                if (_currentState == SequenceEditorState.MovingEvents && _selectedEvents.Count > 0)
//                {
//                    foreach (SequenceEvent_Object seqEv in _selectedEvents)
//                    {
//                        Vector2 timelinePos = TranslateToTimelinePos(seqEv.Editor_Rect);
//                        seqEv.BeatPos = timelinePos.x;
//                        seqEv.PlacedOnLine = (int)timelinePos.y;
//                        seqEv.Editor_Rect.x = Info.WidthPerBeat * seqEv.BeatPos + LoadedSequence.SongOffset * Info.WidthPerSec + TimelineViewer.Offset;
//                        seqEv.Editor_Rect.y = TimelineViewer.HEIGHT_PER_ROW * seqEv.PlacedOnLine + TimelineViewer.HEIGHT_PER_ROW / 4f;
//                    }

//                    _currentState = SequenceEditorState.Default;
//                    Repaint();
//                }
//                else if (_currentState == SequenceEditorState.Selecting)
//                {
//                    _selectedEvents.Clear();
//                    foreach (var ev in LoadedSequence.Events)
//                    {
//                        Vector2 rectCenter = ev.Editor_Rect.center;
//                        rectCenter.x -= _scrollPosition.x;
//                        if (_selectionRect.Contains(rectCenter))
//                            _selectedEvents.Add(ev);
//                    }
//                    _currentState = SequenceEditorState.Default;
//                    Repaint();
//                }
//                break;
//            case EventType.MouseDrag:
//                if (e.button == 0)
//                {
//                    if (_currentState == SequenceEditorState.MovingEvents)
//                    {
//                        foreach (SequenceEvent_Object seqEv in _selectedEvents)
//                        {
//                            seqEv.Editor_Rect.position += e.delta;
//                        }
//                        Repaint();
//                    }
//                    else if(_currentState == SequenceEditorState.Selecting)
//                    {
//                        _selectionRect.width = e.mousePosition.x - _selectionRect.position.x;
//                        _selectionRect.height = e.mousePosition.y - _selectionRect.position.y;
//                    }
//                }
//                else if(e.button == 2)
//                {
//                    _scrollPosition -= e.delta;
//                    Repaint();
//                }
//                break;
//            case EventType.ScrollWheel:
//                if (TimelineViewer.TimelineRect.Contains(e.mousePosition))
//                {
//                    Info.WidthPerSec -= e.delta.y;
//                    if (Info.WidthPerSec * LoadedSequence.Song.length < position.width)
//                        Info.WidthPerSec = position.width / LoadedSequence.Song.length;
//                    Info.WidthPerBeat = Info.WidthPerSec / (LoadedSequence.Bpm / 60f);

//                    //Update the rect of the events
//                    EventDrawer.UpdateEventPositions();
//                    Repaint();
//                }
//                break;
//            case EventType.KeyUp:
//                switch (e.keyCode)
//                {
//                    case KeyCode.D:
//                        foreach (SequenceEvent_Object seqEv in _selectedEvents)
//                        {
//                            List<SequenceEvent_Object> otherEv = new List<SequenceEvent_Object>();

//                            if (seqEv.BaseEvent.Type == SequenceEventType.Response)
//                            {
//                                SequenceEvent_Object temp = LoadedSequence.Events.Find(callEv => callEv.Response.Contains(seqEv));
//                                if (temp != null)
//                                {
//                                    otherEv.Add(temp);
//                                    otherEv.AddRange(temp.Response);
//                                }

//                            }
//                            else
//                            {
//                                otherEv.AddRange(seqEv.Response);
//                                otherEv.Add(seqEv);
//                            }

//                            if (otherEv.Count > 0)
//                                LoadedSequence.Events.RemoveAll(ev => otherEv.Contains(ev));
//                        }
//                        Repaint();
//                        break;
//                }
//                break;
//        }

//        #endregion

//        #region Timeline
        
//        _scrollPosition = GUI.BeginScrollView(TimelineViewer.TimelineRect, _scrollPosition, _scrollViewableRect, true, false);

//        if (_currentState == SequenceEditorState.Playing)
//            _scrollPosition.x = _progress * Info.WidthPerSec;
//        else if (_changedPlayingToCurrent)
//        {
//            _changedPlayingToCurrent = false;
//            _scrollPosition.x = _progress * Info.WidthPerSec;
//        }
//        else
//            _progress = _scrollPosition.x / Info.WidthPerSec;

//        TimelineViewer.DrawWithinScroll(_scrollViewableRect);

//        EventDrawer.DrawWithinScroll(_scrollViewableRect);

//        _progressRect = new Rect(_progress * Info.WidthPerSec + LoadedSequence.SongOffset * Info.WidthPerSec - 2f + TimelineViewer.Offset, 0f, 5f, position.height - Inspector.Height);
//        _progressBeatRect = new Rect(_progress * Info.WidthPerSec + LoadedSequence.SongOffset * Info.WidthPerSec + 10f + TimelineViewer.Offset, 10f, 50f, 50f);

//        EditorGUI.DrawRect(_progressRect, Color.cyan);
//        EditorGUI.LabelField(_progressBeatRect, (_progress * (LoadedSequence.Bpm / 60f)).ToString("F1"), EditorStyles.boldLabel);

//        GUI.EndScrollView(false);

//        TimelineViewer.DrawGUI();

//        if(_currentState == SequenceEditorState.Selecting)
//            EditorGUI.DrawRect(_selectionRect, new Color(0.9f, 0.9f, 0.9f, 0.2f));

//        #endregion

//        #region Bottom placed inspector
    
//        if(_openInspector)
//        {
//            GUILayout.BeginArea(new Rect(0f, position.size.y - Inspector.Height, position.width / 3f, Inspector.Height));
//            _folderScrollPos = GUILayout.BeginScrollView(_folderScrollPos, false, true);

//            //Based on in which folder we found the events, we create our very own folder structure on the left
//            string prevValue = "";
//            foreach(KeyValuePair<SequenceEvent, string> kvp in _allCallEvents)
//            {
//                if(prevValue != kvp.Value)
//                {
//                    if (!_folderFoldout.ContainsKey(kvp.Value))
//                        _folderFoldout.Add(kvp.Value, false);

//                    if(EditorGUI.indentLevel > 0)
//                        EditorGUI.indentLevel -= 2;
//                    _folderFoldout[kvp.Value] = EditorGUILayout.Foldout(_folderFoldout[kvp.Value], kvp.Value, EditorStyles.foldoutHeader);
//                    prevValue = kvp.Value;
//                    EditorGUI.indentLevel += 2;
//                }

//                if (_folderFoldout[kvp.Value])
//                {
//                    EditorGUILayout.LabelField(kvp.Key.name);
                    
//                    if (e.type == EventType.MouseDown && GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
//                        CreateAndDragEvent(kvp.Key);
//                }
//            }

//            GUILayout.EndScrollView();

//            //From there you can select and drag an event / mini-game

//            GUILayout.EndArea();

//            GUILayout.BeginArea(new Rect(position.width / 3f, position.size.y - Inspector.Height, 2 * position.width / 3f, Inspector.Height));

//            //On the right you will see the general information of the selected sequence
//            EditorGUILayout.LabelField("Progress in seconds: " + _progress.ToString("F1"));
//            EditorGUILayout.LabelField("Progress in beats: " + (_progress * (LoadedSequence.Bpm / 60f)).ToString("F1"));

//            GUILayout.EndArea();
//        }

//        #endregion
//    }

//    protected void OnSelectionChange()
//    {
//        if (Selection.activeObject is Sequence)
//            OpenSequence(Selection.activeObject as Sequence);
//    }

//    public void OpenSequence(Sequence seq)
//    {
//        TimelineViewer = new TimelineViewer();
//        TimelineViewer.Init(this);

//        EventDrawer = new EventDrawer();
//        EventDrawer.Init(this);

//        Inspector = new EventInspector();
//        Inspector.Init(this);

//        LoadedSequence = seq;

//        _currentState = SequenceEditorState.Default;

//        if (Info == null)
//            Info = new LoadedSequenceInformation();
//        Info.Init(LoadedSequence);

//        _lastWindowSize = position;

//        #region Init rects

//        TimelineViewer.TimelineRect = new Rect(0f, 0f, position.width, position.height - Inspector.Height);
//        _inspectorRect = new Rect(0f, position.height - Inspector.Height, position.width, Inspector.Height);

//        _scrollViewableRect = new Rect(Vector2.zero, new Vector2(Info.WidthPerSec * LoadedSequence.Song.length + TimelineViewer.Offset, position.size.y - Inspector.Height - 50f));

//        #endregion

//        Repaint();
//    }

//    #region Timeline

//    protected void DrawEvent(SequenceEvent_Object seqEv)
//    {
//        Handles.BeginGUI();
//        if (_selectedEvents.Contains(seqEv))
//            Handles.DrawSolidRectangleWithOutline(seqEv.Editor_Rect, Color.white, Color.black);
//        else
//            Handles.DrawSolidRectangleWithOutline(seqEv.Editor_Rect, Color.gray, Color.black);
        
//        Handles.EndGUI();

//        GUILayout.BeginArea(seqEv.Editor_Rect);

//        GUI.contentColor = Color.black;
//        GUILayout.Label(seqEv.BeatPos.ToString());
//        GUILayout.Label(seqEv.BaseEvent.Type.ToString());
//        GUILayout.Label(seqEv.BaseEvent.name.Substring(seqEv.BaseEvent.Type == SequenceEventType.Call ? 5 : 3));
//        GUI.contentColor = Color.white;

//        GUILayout.EndArea();
//    }

//    protected void DrawConnection(SequenceEvent_Object seqEv)
//    {   
//        Handles.BeginGUI();
        
//        foreach(SequenceEvent_Object responseEv in seqEv.Response)
//            Handles.DrawLine(seqEv.Editor_Rect.center, responseEv.Editor_Rect.center);
    
//        Handles.EndGUI();
//    }

//    protected Vector2 TranslateToTimelinePos(Rect editorPos)
//    {
//        Vector2 result = new Vector2();

//        result.x = (int)((editorPos.x - TimelineViewer.Offset) / Info.WidthPerBeat);
//        result.x = Mathf.Clamp(result.x, 0f, Info.AmtBeatsInSong);
//        result.y = (int)(editorPos.center.y / TimelineViewer.HEIGHT_PER_ROW);
//        result.y = Mathf.Clamp(result.y, 0, 3);

//        return result;
//    }

//    #endregion

//    #region Timeline controls

//    public void StartPlaying()
//    {
//        AudioUtility.StopAllClips();

//        //Calculate the startSample
//        int startSample = 0;
//        float percentage = _progress / LoadedSequence.Song.length;
//        int amtSamples = LoadedSequence.Song.samples;
//        startSample = (int)(percentage * amtSamples);

//        _progressBeforePlaying = _progress;

//        _currentState = SequenceEditorState.Playing;
//        AudioUtility.PlayClip(LoadedSequence.Song, startSample, false);
//        AudioUtility.SetClipSamplePosition(LoadedSequence.Song, startSample);
//    }

//    public void StopPlaying()
//    {
//        AudioUtility.StopClip(LoadedSequence.Song);
//        _currentState = SequenceEditorState.Default;
//        _progress = _progressBeforePlaying;
//        _changedPlayingToCurrent = true;
//    }

//    #endregion

//    #region Inspector controls

//    protected void CreateAndDragEvent(SequenceEvent value)
//    {
//        _selectedEvents.Clear();

//        SequenceEvent_Object newEvent = new SequenceEvent_Object(value);

//        newEvent.Editor_Rect = new Rect(Event.current.mousePosition.x, position.height - Inspector.Height + Event.current.mousePosition.y, Info.WidthPerBeat * value.LengthInBeats, TimelineViewer.HEIGHT_PER_ROW / 2f);

//        _selectedEvents.Add(newEvent);
//        LoadedSequence.Events.Add(newEvent);

//        if(newEvent.BaseEvent.ResponseEvent != null && newEvent.BaseEvent.ResponseEvent.Count > 0)
//        {
//            for(int i = 0; i < newEvent.BaseEvent.ResponseEvent.Count; i++) 
//            {
//                SequenceEvent_Object responseEvent = new SequenceEvent_Object(newEvent.BaseEvent.ResponseEvent[i]);

//                responseEvent.Editor_Rect = new Rect(Event.current.mousePosition.x + newEvent.Editor_Rect.width, position.height - Inspector.Height + Event.current.mousePosition.y, Info.WidthPerBeat * responseEvent.BaseEvent.LengthInBeats, TimelineViewer.HEIGHT_PER_ROW / 2f);
//                if(i > 0)
//                    responseEvent.Editor_Rect.x = newEvent.Response[i - 1].Editor_Rect.x + newEvent.Response[i - 1].Editor_Rect.width;

//                newEvent.Response.Add(responseEvent);

//                _selectedEvents.Add(responseEvent);
//                LoadedSequence.Events.Add(responseEvent);
//            }
//        }

//        _currentState = SequenceEditorState.MovingEvents;

//        Repaint();
//    }

//    protected void StartDrag(SequenceEvent_Object seqEv)
//    {
//        _selectedEvents.Add(seqEv);
//        _currentState = SequenceEditorState.MovingEvents;
//    }

//    #endregion

//    #region Helpers

//    protected void RefreshEvents()
//    {
//        _folderFoldout.Clear();
//        _allCallEvents = SequenceUtilities.LoadAllSequenceEvents();
//    }

//    #endregion
//}
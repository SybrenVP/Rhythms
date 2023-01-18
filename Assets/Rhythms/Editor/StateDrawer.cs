using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using static UnityEditor.PlayerSettings;
using UnityEditor.Graphs;
using static UnityEngine.EventSystems.StandaloneInputModule;

namespace RhythmEditor
{
    public class StateDrawer
    {
        //Main editor window
        private RhythmSequenceEditor _sequenceEditorWindow;

        //Data
        public Rhythm.State State = null;

        //GUI owned by this class
        private RhythmBeatDragBox _box_1 = null;
        private RhythmBeatDragBox _box_2 = null;

        private StateDrawer _ghost = null;

        //Owner of this GUI
        public TrackGUI TrackGUI = null;
        private TimelineGUI _timelineGUI = null;

        public int Beat = -1;
        public int LengthInBeats = 1;

        #region Variables :: Size

        public Rect BaseRect;
        public Rect View { get; private set; }

        public Vector2 Size;
        public Vector2 Position;

        #endregion

        #region Variables :: Consts

        private readonly float STATE_SIZE_PERCENTAGE = 0.75f;
        private readonly float STATE_MIN_HEIGHT = 50f;
        private readonly float NODE_HEIGHT = 10f;

        private Color _backgroundColor = Color.black;
        private readonly Color _shadowColor = Color.grey;

        #endregion

        #region Variables :: Variable Nodes

        private Dictionary<int, Dictionary<ConnectionNode, string>> _outputConnectionNodes = new Dictionary<int, Dictionary<ConnectionNode, string>>();
        private Dictionary<int, Dictionary<ConnectionNode, string>> _inputConnectionNodes = new Dictionary<int, Dictionary<ConnectionNode, string>>();
        private int _nodeCount = 0;

        private GUISkin _rightAlignSkin;

        #endregion

        #region Initialize

        public StateDrawer(Rhythm.State state, int beat, TrackGUI trackGUI, TimelineGUI timeline, RhythmSequenceEditor editor)
        {
            _sequenceEditorWindow = editor;
            State = state;
            _timelineGUI = timeline;
            TrackGUI = trackGUI;

            Beat = beat;
            LengthInBeats = state.LengthInBeats;

            //CreateConnectionNodes();
        }

        #endregion

        //private void CreateConnectionNodes()
        //{
        //    _nodeCount = 0;
        //    for (int i = 0; i < _state.Actions.Count; i++)
        //    {
        //        //Create serializedObject and prepare for drawing the inspectors
        //        var so = new SerializedObject(_state.Actions[i]);
        //        so.Update();
        //
        //        SerializedProperty it = so.GetIterator();
        //        it.NextVisible(true);
        //
        //        while (it.NextVisible(false))
        //        {
        //            Rhythm.OutputAttribute outputAttribute = it.GetAttributes<Rhythm.OutputAttribute>();
        //            if (outputAttribute != null)
        //            {
        //                if (!_outputConnectionNodes.ContainsKey(i))
        //                {
        //                    _outputConnectionNodes.Add(i, new Dictionary<ConnectionNode, string>());
        //                }
        //
        //                _outputConnectionNodes[i].Add(new ConnectionNode(this, EConnectionType.Output, _sequenceEditorWindow), it.propertyPath);
        //                _nodeCount++;
        //            }
        //
        //            Rhythm.InputAttribute inputAttribute = it.GetAttributes<Rhythm.InputAttribute>();
        //            if (inputAttribute != null)
        //            {
        //                if (!_inputConnectionNodes.ContainsKey(i))
        //                {
        //                    _inputConnectionNodes.Add(i, new Dictionary<ConnectionNode, string>());
        //                }
        //
        //                _inputConnectionNodes[i].Add(new ConnectionNode(this, EConnectionType.Input, _sequenceEditorWindow), it.propertyPath);
        //                _nodeCount++;
        //            }
        //        }
        //    }
        //}
        
        public void OnGUI()
        {
            EditorGUI.DrawRect(View, _backgroundColor);
            Utility.DrawShadowRect(View, new Inset(), 5, _shadowColor);

            //Create a box with the size of a connection node, on the first node position possible on this state
            //Rect nextConnectionNode = new Rect(View.x, View.y + 5f, EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight);
            //
            //for (int i = 0; i < _state.Actions.Count; i++)
            //{
            //    if (_inputConnectionNodes.ContainsKey(i) || _outputConnectionNodes.ContainsKey(i))
            //    {
            //        //Create the rect for the name of the action containing the next few variable nodes
            //        //The rect should be as wide as the state drawer and as high as a label (with some added offsets to keep it readable
            //        Rect actionNameRect = nextConnectionNode;
            //        actionNameRect.x = View.x; 
            //        actionNameRect.width = View.width;
            //        actionNameRect.height += 5f;
            //        GUILayout.BeginArea(actionNameRect, EditorStyles.helpBox);
            //
            //        GUI.skin.label.alignment = TextAnchor.MiddleCenter; //Align in the middle of the helpbox
            //        GUILayout.Label(_state.Actions[i].GetType().Name);
            //        GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            //
            //        GUILayout.EndArea();
            //
            //        nextConnectionNode.y = actionNameRect.y + actionNameRect.height; //Move the next node rect to the bottom of the action name
            //    }
            //
            //    if (_inputConnectionNodes.ContainsKey(i))
            //    {
            //        //For an input node we put the connection at half the offset inside of the state
            //        nextConnectionNode.x = View.x + EditorGUIUtility.singleLineHeight * 0.5f;
            //        foreach (KeyValuePair<ConnectionNode, string> inputNode in _inputConnectionNodes[i])
            //        {
            //            inputNode.Key.Draw(nextConnectionNode);
            //
            //            Rect nodeNameRect = nextConnectionNode;
            //            nodeNameRect.x = nextConnectionNode.x + EditorGUIUtility.singleLineHeight + 5f;
            //            nodeNameRect.width = View.width;
            //
            //            GUILayout.BeginArea(nodeNameRect);
            //
            //            GUILayout.Label(inputNode.Value);
            //
            //            GUILayout.EndArea();
            //
            //            nextConnectionNode.y += EditorGUIUtility.singleLineHeight;
            //        }
            //    }
            //
            //    if (_outputConnectionNodes.ContainsKey(i))
            //    {
            //        nextConnectionNode.x = View.x + View.width - EditorGUIUtility.singleLineHeight * 1.5f;
            //        foreach (KeyValuePair<ConnectionNode, string> outputNode in _outputConnectionNodes[i])
            //        {
            //            outputNode.Key.Draw(nextConnectionNode);
            //
            //            Rect nodeNameRect = nextConnectionNode;
            //            nodeNameRect.x = View.x;
            //            nodeNameRect.width = View.width - EditorGUIUtility.singleLineHeight - (5f * 2f);
            //
            //            GUILayout.BeginArea(nodeNameRect);
            //
            //            GUI.skin.label.alignment = TextAnchor.MiddleRight;
            //            GUILayout.Label(outputNode.Value);
            //            GUI.skin.label.alignment = TextAnchor.MiddleLeft;
            //
            //            GUILayout.EndArea();
            //
            //            nextConnectionNode.y += EditorGUIUtility.singleLineHeight;
            //        }
            //    }
            //
            //    nextConnectionNode.y += EditorGUIUtility.singleLineHeight;
            //}
        }

        public void OnGhostGUI()
        {
            if (_ghost != null)
                _ghost.OnGUI();

            if (_box_1 != null)
                _box_1.Draw();
            if (_box_2 != null)
                _box_2.Draw();
        }

        public void SetBeat(int beat)
        {
            if (beat == Beat)
                return;
            
            Beat = beat;

            TrackGUI.RefreshStatePositionAndSize(this, Beat, State.LengthInBeats);
        }

        public void SetView(Rect view)
        { 
            View = view;
        }

        public void SetPositionAndSize(Vector2 pos, Vector2 size)
        {
            Position = pos;
            Size = size;

            BaseRect = new Rect(Position, Size);
            View = new Rect(Position, Size);
        }

        public ConnectionNode GetConnectionNodeForPosition(Vector2 pos, EConnectionType type)
        {
            if (type == EConnectionType.Output)
            {
                foreach (Dictionary<ConnectionNode, string> propertyToConnectionNodeList in _outputConnectionNodes.Values)
                {
                    foreach (ConnectionNode connectionNode in propertyToConnectionNodeList.Keys)
                    {
                        if (connectionNode.View.Contains(pos))
                            return connectionNode;
                    }
                }
            }
            else
            {
                foreach (Dictionary<ConnectionNode, string> propertyToConnectionNodeList in _inputConnectionNodes.Values)
                {
                    foreach (ConnectionNode connectionNode in propertyToConnectionNodeList.Keys)
                    {
                        if (connectionNode.View.Contains(pos))
                            return connectionNode;
                    }
                }
            }

            return null;
        }

        public SerializedProperty GetPropertyForConnectionNode(ConnectionNode value)
        {
            foreach (KeyValuePair<int, Dictionary<ConnectionNode, string>> propertyToConnectionNodeList in _outputConnectionNodes)
            {
                if (propertyToConnectionNodeList.Value.ContainsKey(value))
                    return GetProperty(propertyToConnectionNodeList.Key, propertyToConnectionNodeList.Value[value]);
            }

            foreach (KeyValuePair<int, Dictionary<ConnectionNode, string>> propertyToConnectionNodeList in _inputConnectionNodes)
            {
                if (propertyToConnectionNodeList.Value.ContainsKey(value))
                    return GetProperty(propertyToConnectionNodeList.Key, propertyToConnectionNodeList.Value[value]);
            }

            return null;
        }

        public SerializedProperty GetProperty(int actionid, string path)
        {
            SerializedObject so = new SerializedObject(State.Actions[actionid]);
            return so.FindProperty(path);
        }

        #region Resize Handles

        public void CreateBoxHandles()
        {
            CreateGhost();

            if (_box_1 == null)
            {
                _box_1 = RhythmBeatDragBox.Create(Beat, _timelineGUI, new Vector2(10f, 10f), OnBoxHandleMoved, OnBoxHandleApplied);
                Debug.Log("created first box handle");
            }

            if (State.LengthInBeats > 0 && _box_2 == null)
            {
                _box_2 = RhythmBeatDragBox.Create(Beat + State.LengthInBeats, _timelineGUI, new Vector2(10f, 10f), OnBoxHandleMoved, OnBoxHandleApplied);
                Debug.Log("created second box handle");
            }
        }

        public void DeleteBoxHandles()
        {
            if (_box_1 != null)
            {
                _box_1 = null;
            }

            if (_box_2 != null)
            {
                _box_2 = null;
            }

            if (_ghost != null)
            {
                _ghost = null;
            }
        }

        private void OnBoxHandleMoved(int oldBeat, int newBeat)
        {
            if (oldBeat == _ghost.Beat) //First box handle moved
            {
                _ghost.Beat = newBeat;

                //also need to update Length in Beats
                _ghost.LengthInBeats += (oldBeat - newBeat);
            }

            if (oldBeat == _ghost.Beat + _ghost.LengthInBeats) //Second box handle moved
            {
                _ghost.LengthInBeats += (newBeat - oldBeat);
            }

            Debug.Log(Beat + ", " + State.LengthInBeats);

            TrackGUI.RefreshStatePositionAndSize(this, Beat, State.LengthInBeats);
        }

        private void OnBoxHandleApplied()
        {
            RhythmToolStateMoveAction newChange = new RhythmToolStateMoveAction(this, TrackGUI, _ghost.TrackGUI, Beat, _ghost.Beat, LengthInBeats, _ghost.LengthInBeats);
            _sequenceEditorWindow.RecordChange(newChange);

            ApplyGhost(false);
        }

        #endregion

        //This region defines a ghost copy of this state drawer, BUT it holds only the relevant information we can later copy into this drawer
        #region Ghost Handler 

        public void CreateGhost()
        {
            if (_ghost != null)
            {
                Debug.LogWarning("This drawer already has a ghost active");
                return;
            }

            _ghost = new StateDrawer(State, Beat, TrackGUI, _timelineGUI, _sequenceEditorWindow);
            _ghost._backgroundColor.a *= .5f;
        }

        public void MoveGhost(TrackGUI timeline, int beat)
        {
            if (_ghost != null)
            {
                _ghost.TrackGUI = timeline;
                _ghost.Beat = beat;

                TrackGUI.RefreshStatePositionAndSize(_ghost, beat, State.LengthInBeats);
            }
        }

        public void ApplyGhost(bool destroyGhost = true)
        {
            if (_ghost != null)
            {
                //Check if the position the ghost is occupying is free for another state to move to
                if (!_ghost.TrackGUI.IsFree(_ghost.Beat, State))
                {
                    //Open error prompt
                    if (EditorUtility.DisplayDialog("Error placing state", "Position on timeline is not free for the state you are about to move"/*, "Create new timeline"*/, "Cancel"))
                    {
                        //TODO: Create another track
                    }
                    //else
                    DeleteBoxHandles();
                    _ghost = null;
                    return;
                }

                if (_ghost.TrackGUI != TrackGUI)
                {
                    TrackGUI.RemoveState(Beat);
                    TrackGUI = _ghost.TrackGUI;

                    TrackGUI.AcceptState(this);
                    TrackGUI.RefreshStatePositionAndSize(this, Beat, State.LengthInBeats);
                }

                SetBeat(_ghost.Beat);
                LengthInBeats = _ghost.LengthInBeats;
                State.LengthInBeats = _ghost.LengthInBeats;
                _ghost.TrackGUI.MoveStateTo(State, _ghost.Beat);

                if (destroyGhost)
                    _ghost = null;
            }
        }

        public StateDrawer GetGhost()
        {
            return _ghost;
        }


        #endregion
    }
}
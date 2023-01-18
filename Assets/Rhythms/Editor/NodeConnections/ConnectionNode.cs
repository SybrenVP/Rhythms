using Rhythm;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RhythmEditor
{
    public enum EConnectionType
    {
        Output,
        Input
    }

    public class ConnectionNode
    {
        //This script is a visual representation of the in- and output variables
        public StateDrawer OwningStateDrawer;
        public Rect View;

        public ConnectionNode ConnectedNode;

        private Rect _localRect;
        private EConnectionType _connectionType;

        private bool _dragActive = false;

        private RhythmSequenceEditor _editor;

        public ConnectionNode(StateDrawer owningStateDrawer, EConnectionType type, RhythmSequenceEditor editor)
        {
            OwningStateDrawer = owningStateDrawer;
            _connectionType = type;
            _localRect = OwningStateDrawer.View;

            _editor = editor;
        }

        public void Draw(Rect drawRect)
        {
            View = drawRect;
            _localRect = drawRect;

            EditorGUI.DrawRect(drawRect, Color.white);
            Utility.DrawShadowRect(drawRect, new Inset(), 2, Color.grey);

            Input(Event.current, drawRect);

            if (_dragActive || ConnectedNode != null)
            {
                Vector2 start = drawRect.center;
                Vector2 end = Event.current.mousePosition;
                if (!_dragActive)
                    end = ConnectedNode._localRect.center;

                Vector2 controlPointOffset = end - start;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(start, end, start + controlPointOffset, end - controlPointOffset, Color.white, null, 2f);
            }
        }

        private void Input(Event e, Rect drawRect)
        {
            int controlID = GUIUtility.GetControlID("ResizeDrag".GetHashCode(), FocusType.Passive);
            int hotControl = GUIUtility.hotControl;

            switch (e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (!_dragActive && drawRect.Contains(e.mousePosition))
                        {
                            _dragActive = true;
                            e.Use();
                        }
                    }

                    break;

                case EventType.MouseUp:
                    if (e.button == 0)
                    {
                        if (_dragActive)
                        {
                            _dragActive = false;

                            //Check if overlapping with another connection node
                            //This might actually be very complicated as we would need to ask all states in all timelines if we are overlapping with one of their connection nodes

                            //First get the overlapping timeline
                            bool setConnectedNode = false;
                            TrackGUI timeline = TimelineGUI.FindOwningTrackGUI(_editor.Timeline, e.mousePosition);
                            if (timeline != null)
                            {
                                int beat = _editor.Timeline.GetBeatForPosition(e.mousePosition);
                                StateDrawer connectionState = timeline.GetStateForBeat(beat);
                                if (connectionState != null)
                                {
                                    ConnectionNode connectedNode = connectionState.GetConnectionNodeForPosition(e.mousePosition, _connectionType == EConnectionType.Input ? EConnectionType.Output : EConnectionType.Input);
                                    if (connectedNode != null)
                                    {
                                        ConnectedNode = connectedNode;

                                        SerializedProperty prop = OwningStateDrawer.GetPropertyForConnectionNode(this);
                                        SerializedProperty soProp = prop.FindPropertyRelative("Variable");
                                        SerializedProperty valueTypeProp = prop.FindPropertyRelative("Type");

                                        SerializedProperty connectedProp = ConnectedNode.OwningStateDrawer.GetPropertyForConnectionNode(ConnectedNode);
                                        SerializedProperty connectedSoProp = connectedProp.FindPropertyRelative("Variable");
                                        SerializedProperty connectedValueTypeProp = prop.FindPropertyRelative("Type");

                                        if ((VariableType)valueTypeProp.enumValueIndex == (VariableType)connectedValueTypeProp.enumValueIndex)
                                        {
                                            Rhythm.R_VariableSO variableSO = soProp.objectReferenceValue as Rhythm.R_VariableSO;
                                            if (variableSO == null)
                                            {
                                                variableSO = _editor.ActiveSequence.Variables.CreateNewVariable((VariableType)valueTypeProp.enumValueIndex);
                                                int loopCount = 0;
                                                while (_editor.ActiveSequence.Variables.DoesNameExist(variableSO.Type, variableSO.name, variableSO))
                                                {
                                                    loopCount++;
                                                    variableSO.name = "New " + variableSO.Type + " " + loopCount.ToString();
                                                    variableSO.TempName = variableSO.name;
                                                }

                                                _editor.SaveSequence();

                                                soProp.objectReferenceValue = variableSO;
                                                prop.serializedObject.ApplyModifiedProperties();
                                                Debug.Log("Created variable SO for " + prop.displayName);
                                            }

                                            Rhythm.R_VariableSO connectedVariableSO = connectedSoProp.objectReferenceValue as Rhythm.R_VariableSO;
                                            if (connectedVariableSO == null)
                                            {
                                                connectedVariableSO = _editor.ActiveSequence.Variables.CreateNewVariable((VariableType)valueTypeProp.enumValueIndex);

                                                int loopCount = 0;
                                                while (_editor.ActiveSequence.Variables.DoesNameExist(connectedVariableSO.Type, connectedVariableSO.name, connectedVariableSO))
                                                {
                                                    loopCount++;
                                                    connectedVariableSO.name = "New " + connectedVariableSO.Type + " " + loopCount.ToString();
                                                    connectedVariableSO.TempName = connectedVariableSO.name;
                                                }

                                                _editor.SaveSequence();

                                                connectedSoProp.objectReferenceValue = connectedVariableSO;
                                                connectedProp.serializedObject.ApplyModifiedProperties();
                                                Debug.Log("Created variable SO for " + connectedProp.displayName);
                                            }

                                            DataConnection variableDataConnection = ScriptableObject.CreateInstance<DataConnection>();

                                            variableDataConnection.Output.Variable = connectedNode._connectionType == EConnectionType.Output ? connectedVariableSO : variableSO;
                                            variableDataConnection.Input.Variable = connectedNode._connectionType == EConnectionType.Output ? variableSO : connectedVariableSO;

                                            _editor.ActiveSequence.DataConnections.Add(variableDataConnection);

                                            _editor.SaveSequence();

                                            setConnectedNode = true;
                                        }
                                        else
                                        {
                                            Debug.LogWarning("Variables are not the same type, connection not possible");
                                            _editor.Refresh();
                                        }
                                    }
                                }
                            }

                            if (!setConnectedNode && ConnectedNode != null)
                            {
                                _editor.DestroyDataConnection();

                                ConnectedNode = null;
                            }
                        }
                    }

                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (_dragActive)
                        {
                            _editor.Refresh();
                        }
                    }

                    break;


            }
        }

    }
}
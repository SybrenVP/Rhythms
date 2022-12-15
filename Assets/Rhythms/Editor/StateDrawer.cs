using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace RhythmEditor
{
    public class StateDrawer
    {
        public TrackTimeline OwningTimeline = null;

        public Rhythm.State State = null;
        public int Beat = -1;
        public int LengthInBeats = 1;

        public Rect View;

        private readonly float STATE_SIZE_PERCENTAGE = 0.75f;

        private RhythmBeatDragBox _box_1 = null;
        private RhythmBeatDragBox _box_2 = null;

        private StateDrawer _ghost = null;

        private RhythmSequenceEditor _editor;

        private Color _backgroundColor = Color.black;
        private Color _shadowColor = Color.grey;

        private Dictionary<int, Dictionary<string, ConnectionNode>> _outputConnectionNodes = new Dictionary<int, Dictionary<string, ConnectionNode>>();
        private Dictionary<int, Dictionary<string, ConnectionNode>> _inputConnectionNodes = new Dictionary<int, Dictionary<string, ConnectionNode>>();

        public StateDrawer(Rhythm.State state, int beat, TrackTimeline owner, RhythmSequenceEditor editor)
        {
            State = state;
            Beat = beat;
            LengthInBeats = state.LengthInBeats;

            OwningTimeline = owner;

            _editor = editor;

            SetView();
        }

        public void OnGUI()
        {
            SetView();

            EditorGUI.DrawRect(View, _backgroundColor);
            Utility.DrawShadowRect(View, new Inset(), 5, _shadowColor);

            GUILayout.BeginArea(View);

            //GUILayout.Label(State.Actions.Count.ToString());
            GUILayout.Label(State.LengthInBeats.ToString());
            GUILayout.EndArea();

            for(int i = 0; i < State.Actions.Count; i++)
            {
                //Create serializedObject and prepare for drawing the inspectors
                var so = new SerializedObject(State.Actions[i]);
                so.Update();

                SerializedProperty it = so.GetIterator();
                it.NextVisible(true);

                while (it.NextVisible(false))
                {
                    Rhythm.OutputAttribute outputAttribute = it.GetAttributes<Rhythm.OutputAttribute>();
                    if (outputAttribute != null)
                    {
                        if (!_outputConnectionNodes.ContainsKey(i))
                            _outputConnectionNodes.Add(i, new Dictionary<string, ConnectionNode>());

                        if (!_outputConnectionNodes[i].ContainsKey(it.propertyPath))
                            _outputConnectionNodes[i].Add(it.propertyPath, new ConnectionNode(this, EConnectionType.Output));

                        _outputConnectionNodes[i][it.propertyPath].Draw();
                    }

                    Rhythm.InputAttribute inputAttribute = it.GetAttributes<Rhythm.InputAttribute>();
                    if (inputAttribute != null)
                    {
                        if (!_inputConnectionNodes.ContainsKey(i))
                            _inputConnectionNodes.Add(i, new Dictionary<string, ConnectionNode>());

                        if (!_inputConnectionNodes[i].ContainsKey(it.propertyPath))
                            _inputConnectionNodes[i].Add(it.propertyPath, new ConnectionNode(this, EConnectionType.Input));

                        _inputConnectionNodes[i][it.propertyPath].Draw();
                    }
                }
            }

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

            SetView();
        }

        public void SetView()
        {
            Rect owningView = OwningTimeline.View;

            View = new Rect(OwningTimeline.GetXForBeat(Beat), owningView.y + (1 - STATE_SIZE_PERCENTAGE) * 0.5f * owningView.height, OwningTimeline.WidthPerBeat * LengthInBeats, STATE_SIZE_PERCENTAGE * owningView.height);
        }

        #region Resize Handles

        public void CreateBoxHandles()
        {
            CreateGhost();

            if (_box_1 == null)
            {
                _box_1 = RhythmBeatDragBox.Create(Beat, OwningTimeline, new Vector2(10f, 10f), OnBoxHandleMoved, OnBoxHandleApplied);
                Debug.Log("created first box handle");
            }

            if (State.LengthInBeats > 0 && _box_2 == null)
            {
                _box_2 = RhythmBeatDragBox.Create(Beat + State.LengthInBeats, OwningTimeline, new Vector2(10f, 10f), OnBoxHandleMoved, OnBoxHandleApplied);
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

            SetView();
        }

        private void OnBoxHandleApplied()
        {
            RhythmToolStateMoveAction newChange = new RhythmToolStateMoveAction(this, OwningTimeline, _ghost.OwningTimeline, Beat, _ghost.Beat, LengthInBeats, _ghost.LengthInBeats);
            _editor.RecordChange(newChange);

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

            _ghost = new StateDrawer(State, Beat, OwningTimeline, _editor);
            _ghost._backgroundColor.a *= .5f;
        }

        public void MoveGhost(TrackTimeline timeline, int beat)
        {
            if (_ghost != null)
            {
                _ghost.OwningTimeline = timeline;
                _ghost.Beat = beat;

                _ghost.SetView();
            }
        }

        public void ApplyGhost(bool destroyGhost = true)
        {
            if (_ghost != null)
            {
                //Check if the position the ghost is occupying is free for another state to move to
                if (!_ghost.OwningTimeline.IsFree(_ghost.Beat, State))
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

                if (_ghost.OwningTimeline != OwningTimeline)
                {
                    OwningTimeline.RemoveState(Beat);
                    OwningTimeline = _ghost.OwningTimeline;

                    OwningTimeline.AcceptState(this);
                    SetView();
                }

                SetBeat(_ghost.Beat);
                LengthInBeats = _ghost.LengthInBeats;
                State.LengthInBeats = _ghost.LengthInBeats;
                _ghost.OwningTimeline.MoveStateTo(State, _ghost.Beat);

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
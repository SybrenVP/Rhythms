using UnityEngine;
using UnityEditor;

namespace Rhythms_Editor
{
    public class StateDrawer
    {
        public TrackTimeline OwningTimeline = null;

        public Rhythms.RhythmState State = null;
        public int Beat = -1;
        public int LengthInBeats = 1;

        public Rect View;

        private readonly float STATE_SIZE_PERCENTAGE = 0.75f;

        private RhythmBeatDragBox _box_1 = null;
        private RhythmBeatDragBox _box_2 = null;

        private StateDrawer _ghost = null;

        private Color _backgroundColor = Color.black;
        private Color _shadowColor = Color.grey;

        public StateDrawer(Rhythms.RhythmState state, int beat, TrackTimeline owner)
        {
            State = state;
            Beat = beat;
            LengthInBeats = state.LengthInBeats;

            OwningTimeline = owner;

            SetView();
        }

        //Keeping this here for future reference, for now this has been disabled since we moved the moving functionality to the editor itself
        
        //public void Update(ref bool refresh)
        //{
        //    if (IsMoving && OwningTimeline.GetLeftAutoScrollRect().Contains(_mousePos))
        //    {
        //        // Scroll left
        //        OwningTimeline.ScrollLeft();
        //        _refresh = true;
        //    }
        //
        //    if (IsMoving && OwningTimeline.GetRightAutoScrollRect().Contains(_mousePos))
        //    {
        //        //Scroll right
        //        OwningTimeline.ScrollRight();
        //        _refresh = true;
        //    }
        //
        //    if (_refresh)
        //        refresh = true;
        //}

        public void OnGUI()
        {
            SetView();

            EditorGUI.DrawRect(View, _backgroundColor);
            Utility.DrawShadowRect(View, new Inset(), 5, _shadowColor);

            GUILayout.BeginArea(View);

            //GUILayout.Label(State.Actions.Count.ToString());
            GUILayout.Label(State.LengthInBeats.ToString());

            GUILayout.EndArea();
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

            _ghost = new StateDrawer(State, Beat, OwningTimeline);
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
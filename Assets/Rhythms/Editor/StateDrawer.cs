using UnityEngine;
using UnityEditor;

namespace Rhythms_Editor
{
    public class StateDrawer
    {
        public TrackTimeline OwningTimeline = null;

        public Rhythms.RhythmState State = null;
        public int Beat = -1;

        public Rect View;

        private readonly float STATE_SIZE_PERCENTAGE = 0.75f;

        private RhythmBeatDragBox _box_1 = null;
        private RhythmBeatDragBox _box_2 = null;

        public StateDrawer(Rhythms.RhythmState state, int beat, TrackTimeline owner)
        {
            State = state;
            Beat = beat;

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

            EditorGUI.DrawRect(View, Color.black);
            Utility.DrawShadowRect(View, new Inset(), 5, Color.grey);

            if (_box_1 != null)
                _box_1.Draw();
            if (_box_2 != null)
                _box_2.Draw();

            GUILayout.BeginArea(View);

            //GUILayout.Label(State.Actions.Count.ToString());
            GUILayout.Label(State.LengthInBeats.ToString());

            GUILayout.EndArea();
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

            View = new Rect(OwningTimeline.GetXForBeat(Beat), owningView.y + (1 - STATE_SIZE_PERCENTAGE) * 0.5f * owningView.height, OwningTimeline.WidthPerBeat * State.LengthInBeats, STATE_SIZE_PERCENTAGE * owningView.height);
        }

        public void CreateBoxHandles()
        {
            if (_box_1 == null)
            {
                _box_1 = RhythmBeatDragBox.Create(Beat, OwningTimeline, new Vector2(10f, 10f), OnBoxHandleChanged);
                Debug.Log("created first box handle");
            }

            if (State.LengthInBeats > 0 && _box_2 == null)
            {
                _box_2 = RhythmBeatDragBox.Create(Beat + State.LengthInBeats, OwningTimeline, new Vector2(10f, 10f), OnBoxHandleChanged);
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
        }

        private void OnBoxHandleChanged(int oldBeat, int newBeat)
        {
            if (oldBeat == Beat) //First box handle moved
            { 
                Beat = newBeat;

                //also need to update Length in Beats
                State.LengthInBeats += (oldBeat - newBeat);
            }

            if (oldBeat == Beat + State.LengthInBeats) //Second box handle moved
            {
                State.LengthInBeats += (newBeat - oldBeat);
            }

            Debug.Log(Beat + ", " + State.LengthInBeats);

            OwningTimeline.MoveStateTo(State, Beat);

            SetView();
        }
    }
}
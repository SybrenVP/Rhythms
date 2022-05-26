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

            GUILayout.BeginArea(View);

            GUILayout.Label(State.Actions.Count.ToString());

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

            View = new Rect(OwningTimeline.GetXForBeat(Beat), owningView.y + (1 - STATE_SIZE_PERCENTAGE) * 0.5f * owningView.height, OwningTimeline.WidthPerBeat, STATE_SIZE_PERCENTAGE * owningView.height);
        }
    }
}
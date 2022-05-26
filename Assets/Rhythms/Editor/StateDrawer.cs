using UnityEngine;
using UnityEditor;

namespace Rhythms_Editor
{

    public class StateDrawer
    {
        public static Rhythms.RhythmState SelectedState = null;
        public static StateDrawer SelectedStateDrawer = null;

        public TrackTimeline OwningTimeline = null;

        public Rhythms.RhythmState State = null;
        public int Beat = -1;

        public bool IsSelected = false;
        public bool IsMoving = false;

        public Rect View;

        private Vector2 _mousePos = Vector2.zero;

        private bool _refresh = false;

        private readonly float STATE_SIZE_PERCENTAGE = 0.75f;
        private readonly float TOOLBAR_BUTTON_WIDTH = 25f;

        public StateDrawer(Rhythms.RhythmState state, int beat, TrackTimeline owner)
        {
            State = state;
            Beat = beat;

            OwningTimeline = owner;

            SetView();
        }

        public void Update(ref bool refresh)
        {
            if (IsMoving && OwningTimeline.GetLeftAutoScrollRect().Contains(_mousePos))
            {
                // Scroll left
                OwningTimeline.ScrollLeft();
                _refresh = true;
            }

            if (IsMoving && OwningTimeline.GetRightAutoScrollRect().Contains(_mousePos))
            {
                //Scroll right
                OwningTimeline.ScrollRight();
                _refresh = true;
            }

            if (_refresh)
                refresh = true;
        }

        public void OnGUI()
        {
            SetView();

            EditorGUI.DrawRect(View, Color.black);
            Utility.DrawShadowRect(View, new Inset(), 5, Color.grey);

            GUILayout.BeginArea(View);

            GUILayout.Label(State.Actions.Count.ToString());

            GUILayout.EndArea();

            HandleInput();
        }

        private void HandleInput()
        {
            Event e = Event.current;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            switch(e.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:

                    if (e.button == 0)
                    {
                        if (IsMoving)
                        {
                            FinishMoveState(e.mousePosition);
                            return;
                        }

                        bool canSelect = View.Contains(e.mousePosition);
                        if (!canSelect)
                        {
                            SelectState(false);
                            return;
                        }

                        SelectState(true);

                        e.Use();
                    }

                    break;

                case EventType.MouseMove:

                    if (IsMoving)
                    {
                        bool currentTimeline = OwningTimeline.View.Contains(e.mousePosition);
                        if (!currentTimeline)
                        {
                            //Get the time line it belongs to now and cache the old timeline
                            TrackTimeline newTimeline = TrackTimeline.FindOwningTimeline(OwningTimeline, e.mousePosition);
                            if (newTimeline != null)
                            {
                                OwningTimeline.DeleteState(Beat);

                                OwningTimeline = newTimeline;

                                OwningTimeline.AcceptState(this);
                                SetView();
                            }
                        }
                        else //Check if we inside the drag rects of the timeline
                        {

                        }

                        SetBeat(OwningTimeline.GetBeatForPosition(e.mousePosition));

                        OwningTimeline.MoveStateTo(State, Beat);

                        _mousePos = e.mousePosition;
                    }

                    break;
            }
        }

        private void FinishMoveState(Vector2 pos)
        {
            SetBeat(OwningTimeline.GetBeatForPosition(pos));

            if (OwningTimeline.MoveStateTo(State, Beat))
                IsMoving = false;
        }

        private void SelectState(bool value)
        {
            IsSelected = value;
            if (IsMoving)
                IsMoving = value;

            SelectedState = value ? State : null;
            SelectedStateDrawer = value ? this : null;

            _refresh = true;
        }

        public void SetBeat(int beat)
        {
            if (beat == Beat)
                return;

            _refresh = true;
            Beat = beat;

            SetView();
        }

        private void SetView()
        {
            Rect owningView = OwningTimeline.View;

            View = new Rect(OwningTimeline.GetXForBeat(Beat), owningView.y + (1 - STATE_SIZE_PERCENTAGE) * 0.5f * owningView.height, OwningTimeline.WidthPerBeat, STATE_SIZE_PERCENTAGE * owningView.height);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythms_Editor
{
    public enum InputState
    {
        Select, //Default selection, allows to click on a state and show it's information in the inspector //TODO: Add multi selection
        ControlTimeline, //Allows to scrub the track timelines
        Move, //Allows the movement of states
        Resize, //Allows resizing of states
    }

    public class RhythmSequenceEditorInputController
    {
        public InputState CurrentState = InputState.Select;

        //We create an object of this class when the editor is opened. 
        //The toolbar dictates how our input is processed
        private Toolbar _toolbar = null;
        private RhythmSequenceEditor _editor = null;

        //Each state has an active state 
        // Select -> Selected, ControlTimeline -> ControllingTimeline, Move -> Moving, Resize -> Resizing
        private bool _inputStateActive = false;

        private TrackTimeline _inputOwningTimeline = null;
        private Vector2 _lastMousePos = Vector2.zero;
        private float _offsetToMousePos = 0f;

        private int _controlId = 0;
        private int _hotControl = 0;

        public StateDrawer SelectedState = null;

        public void Init(Toolbar toolbar, RhythmSequenceEditor editor)
        {
            _editor = editor;

            _toolbar = toolbar;

            //Listen to the change of tool
            _toolbar.OnToolChange += ToolChanged;
        }

        private void ToolChanged(Toolbar.ToolType newTool)
        {
            if (CurrentState == InputState.Resize && _inputStateActive)
            {
                SelectedState?.DeleteBoxHandles();
            }

            switch (newTool)
            {
                case Toolbar.ToolType.Select:
                    CurrentState = InputState.Select;
                    break;
                case Toolbar.ToolType.ViewMove:
                    CurrentState = InputState.ControlTimeline;
                    break;
                case Toolbar.ToolType.Move:
                    CurrentState = InputState.Move;
                    break;
                case Toolbar.ToolType.Resize:
                    CurrentState = InputState.Resize;
                    break;
            }
        }

        public bool Update() //if the update returns true, we need to refresh the editor regularly
        {
            Event e = Event.current;
            _controlId = GUIUtility.GetControlID(CurrentState.GetHashCode(), FocusType.Passive);
            _hotControl = GUIUtility.hotControl;

            switch (CurrentState)
            {
                case InputState.Select:

                    HandleSelect(e);

                    break;

                case InputState.ControlTimeline:

                    HandleTimelineControl(e);

                    break;

                case InputState.Move:

                    if (_inputStateActive)
                        HandleMove(e);
                    else
                    {
                        HandleSelect(e);
                        if (SelectedState != null)
                            SelectedState.CreateGhost();
                    }

                    break;

                case InputState.Resize:

                    if (_inputStateActive)
                        HandleResize(e);
                    else
                    {
                        HandleSelect(e);
                        if (SelectedState != null)
                            SelectedState.CreateGhost();
                    }

                    break;
            }

            return false;
        }

        #region Select

        private void HandleSelect(Event e)
        {
            switch (e.GetTypeForControl(_controlId))
            {
                case EventType.MouseDown:

                    HandleDeselect(e);

                    MouseDownSelect(e);


                    break;
            }
        }

        private void MouseDownSelect(Event e)
        {
            if (e.button == 0) //Left Mouse Button
            {
                //Via the timeline we can find the correct state
                _inputOwningTimeline = TrackTimeline.FindOwningTimeline(_editor, e.mousePosition);

                int beat = _inputOwningTimeline.GetBeatForPosition(e.mousePosition);

                StateDrawer stateDrawer = _inputOwningTimeline.GetStateForBeat(beat);

                if (stateDrawer != null && stateDrawer.View.Contains(e.mousePosition)) //We selected a state
                {
                    _inputStateActive = true; //We are now in the active state 

                    //We need to keep track of the offset between mouse position and selected state, so we can keep the state at the same offset when moving it around
                    _offsetToMousePos = _inputOwningTimeline.GetXForBeat(beat) - _inputOwningTimeline.GetXForBeat(stateDrawer.Beat);

                    GUIUtility.hotControl = _controlId;

                    SelectedState = stateDrawer;
                    e.Use();
                }
            }
        }
        
        private void HandleDeselect(Event e)
        {
            if (e.button == 0 && _inputStateActive) //Left mouse button
            {
                SelectedState = null;
                _inputStateActive = false;
                _editor.Refresh();

                e.Use();
            }
        }

        #endregion

        #region Timeline Control

        private void HandleTimelineControl(Event e)
        {
            switch (e.GetTypeForControl(_controlId))
            {
                case EventType.MouseDown:

                    HandleMouseDownTimeline(e);

                    break;

                case EventType.MouseUp:

                    if (_hotControl == _controlId)
                        HandleMouseUpTimeline(e);

                    break;

                case EventType.MouseDrag:

                    HandleMouseDragTimeline(e);

                    break;
            }
        }

        private void HandleMouseDownTimeline(Event e)
        {
            if (e.button == 0)
            {
                if (!_inputStateActive)
                {
                    _inputOwningTimeline = TrackTimeline.FindOwningTimeline(_editor, e.mousePosition);
                    _inputStateActive = true;

                    _lastMousePos = e.mousePosition;

                    GUIUtility.hotControl = _controlId;
                    e.Use();
                }
            }
        }

        private void HandleMouseUpTimeline(Event e)
        {
            if (_inputStateActive)
            {
                _inputOwningTimeline = null;
                _inputStateActive = false;
            }

            GUIUtility.hotControl = 0;
            e.Use();
        }

        private void HandleMouseDragTimeline(Event e)
        {
            if (e.button == 0)
            {
                if (_inputStateActive)
                {
                    Vector2 diff = e.mousePosition - _lastMousePos;
                    _inputOwningTimeline.Scroll(-diff.x);

                    _lastMousePos = e.mousePosition;
                    _editor.Refresh();
                }
            }
        }

        #endregion

        #region Move

        private void HandleMove(Event e)
        {
            switch (e.GetTypeForControl(_controlId))
            {
                //If this mouseDown event was not claimed by the drag boxes, we should handle input for deselecting
                case EventType.MouseDown:

                    HandleDeselect(e);

                    break;

                case EventType.MouseUp:

                    HandleMouseUpMove(e);

                    break;

                case EventType.MouseDrag:

                    HandleMouseDragMove(e);

                    break;
            }
        }

        private void HandleMouseUpMove(Event e)
        {
            SelectedState.ApplyGhost();

            _inputOwningTimeline = null;
            _inputStateActive = false;
            SelectedState = null;

            GUIUtility.hotControl = 0;
            e.Use();
        }

        private void HandleMouseDragMove(Event e)
        {
            if (e.button == 0)
            {
                //Move to another timeline
                TrackTimeline newTimeline = TrackTimeline.FindOwningTimeline(_editor, e.mousePosition);

                if (newTimeline != null)
                    _inputOwningTimeline = newTimeline;

                int ghostBeat = _inputOwningTimeline.GetBeatForPosition(new Vector2(e.mousePosition.x - _offsetToMousePos, e.mousePosition.y));
                SelectedState.MoveGhost(newTimeline, ghostBeat);

                _editor.Refresh();
            }
        }

        #endregion

        #region Resize

        private void HandleResize(Event e)
        {
            switch (e.GetTypeForControl(_controlId))
            {
                case EventType.MouseDown:

                    if (_inputStateActive && SelectedState != null)
                        SelectedState.DeleteBoxHandles();
                    HandleDeselect(e);

                    break;
            }

            if (_inputStateActive)
            {
                SelectedState?.CreateBoxHandles();
                _editor.Refresh();
            }
        }

        #endregion  
    }
}
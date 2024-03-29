using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmEditor
{

    public abstract class RhythmToolAction
    {
        public abstract void Apply();

        public abstract void Revert();
    }

    public class RhythmToolStateMoveAction : RhythmToolAction
    {
        private StateDrawer _affectedState;

        private TrackGUI _oldTimeline;
        private TrackGUI _newTimeline;

        private int _oldLength;
        private int _newLength;

        private int _oldBeat;
        private int _newBeat;

        public RhythmToolStateMoveAction(StateDrawer state, TrackGUI oldTrack, TrackGUI newTrack, int oldBeat, int newBeat, int oldLength, int newLength)
        {
            _affectedState = state;

            _oldTimeline = oldTrack;
            _newTimeline = newTrack;

            _oldBeat = oldBeat;
            _newBeat = newBeat;

            _oldLength = oldLength;
            _newLength = newLength;
        }

        public override void Apply()
        {
            if (_oldTimeline != _newTimeline)
            {
                _oldTimeline.RemoveState(_oldBeat);
                _affectedState.TrackGUI = _newTimeline;
                _newTimeline.AcceptState(_affectedState);
            }

            _affectedState.SetBeat(_newBeat);
            _affectedState.LengthInBeats = _newLength;
            _affectedState.State.LengthInBeats = _newLength;
            _newTimeline.MoveStateTo(_affectedState.State, _newBeat);

            _newTimeline.RefreshStatePositionAndSize(_affectedState, _newBeat, _newLength);
        }

        public override void Revert()
        {
            if (_newTimeline != _oldTimeline)
            {
                _newTimeline.RemoveState(_newBeat);
                _affectedState.TrackGUI = _oldTimeline;
                _oldTimeline.AcceptState(_affectedState);
            }

            _affectedState.SetBeat(_oldBeat);
            _affectedState.LengthInBeats = _oldLength;
            _affectedState.State.LengthInBeats = _oldLength;
            _oldTimeline.MoveStateTo(_affectedState.State, _oldBeat);

            _newTimeline.RefreshStatePositionAndSize(_affectedState, _oldBeat, _oldLength);
        }
    }
}
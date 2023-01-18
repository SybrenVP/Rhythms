using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;

namespace Rhythm
{
    //Example action
    public class BeatLerp : Action
    {
        public R_Bool UseStateLength = new R_Bool(true);

        [Tooltip("Length in beats")]
        public R_Float Length = new R_Float(1f);

        public R_Vector3 StartPosition;
        public R_Vector3 TargetPosition;

        public R_Vector3 OutPosition;

        public R_Bool UseBeatUpdate;
        public R_Bool WaitForFirstBeatUpdate;

        private float _speed;
        private float _prevDelta = 0f;

        private bool _firstBeatUpdate = false;

        private Vector3 _direction;
        private float _unitsPerBeat;

        private float _lengthInSeconds;

        public override void Start()
        {
            if (!WaitForFirstBeatUpdate.Value)
                _firstBeatUpdate = true;


            _direction = TargetPosition.Value - StartPosition.Value;
            _unitsPerBeat = _direction.magnitude / Length.Value;

            _speed = _unitsPerBeat * 2f;

            _lengthInSeconds = Length.Value * _audioData.SecPerBeat;

            OutPosition.Value = StartPosition.Value;

            Debug.Log("Speed: " + _speed);
        }

        public override void BeatUpdate() 
        {
            if (WaitForFirstBeatUpdate.Value && !_firstBeatUpdate)
                _firstBeatUpdate = true;

            if (UseBeatUpdate.Value)
            {
                OutPosition.Value += _direction.normalized * _unitsPerBeat;
                Debug.Log(Time.time);
            }
        }

        public override void Update()
        {
            if (!_firstBeatUpdate)
                return;

            if (UseBeatUpdate.Value)
                return;

            float delta = _prevDelta / _lengthInSeconds;

            //Debug.Log("Delta: " + delta);

            OutPosition.Value = Vector3.Lerp(StartPosition.Value, TargetPosition.Value, delta);

            //Debug.Log("Out Position: " + OutPosition.Value);
            _prevDelta += Time.deltaTime;

            if (delta > 1f)
            {
                Debug.Log("target reached!");

                Enabled = false;
            }
        }

        public override void Exit() { }
    }
}
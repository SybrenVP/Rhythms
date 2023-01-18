using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    public class ScaleOnInputResult : ListenToInputAction
    {
        public R_GameObject TargetObject;

        public R_Vector3 TargetScale;

        public R_Float LengthInBeats;

        private Vector3 _startScale;
        private float _lengthInSeconds = -1f;
        private float _prevDelta = 0f;

        public override void Start()
        {
            base.Start();

            _startScale = TargetObject.Value.transform.localScale;
        }

        public override void Update()
        {
            base.Update();

            if (InputResult.Value) //Input happened
            {
                float delta = _prevDelta / _lengthInSeconds;

                Vector3 scale = Vector3.Lerp(_startScale, TargetScale.Value, delta);

                TargetObject.Value.transform.localScale = scale;
                _prevDelta += Time.deltaTime;

                if (delta > 1f || TargetObject.Value == null)
                {
                    Enabled = false;
                }
            }
        }
    }
}
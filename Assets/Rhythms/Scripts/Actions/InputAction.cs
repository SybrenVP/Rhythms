using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    public class InputAction : Action
    {
        public KeyCode InputKey = KeyCode.Space;

        [Output]
        public R_Bool Result = new R_Bool(false);

        public R_Float InputOffset;

        private float _inputTime = -1f;
        private float _beatTime = -1f;

        private void KeyDownEvent()
        {
            Result.Value = true;
            _inputTime = Time.time;

            if (_beatTime > 0f)
            {
                CalculateOffset();
            }
        }

        private void KeyUpEvent()
        {
        }

        public override void BeatUpdate()
        {
            _beatTime = Time.time;
            if (_inputTime > 0f)
            {
                CalculateOffset();
            }
        }

        public override void Exit()
        {
            if (_inputTime < 0f)
            {
                Debug.Log("Missed");
                InputOffset.Value = -1f;
            }

            InputManager.Instance.RemoveKeyDown(InputKey, KeyDownEvent);
            InputManager.Instance.RemoveKeyUp(InputKey, KeyUpEvent);
        }

        public override void Start()
        {
            InputManager.Instance.ListenToKeyDown(InputKey, KeyDownEvent);
            InputManager.Instance.ListenToKeyUp(InputKey, KeyUpEvent);
        }

        public override void Update() 
        {
            if (Result.Value && _beatTime > 0f)
            {
                Enabled = false;
                Exit();
            }
        }

        private void CalculateOffset()
        {
            float offset = _inputTime - _beatTime;

            if (offset > Mathf.Abs(_audioData.SecPerBeat))
            {
                InputOffset.Value = -1f;
                Debug.Log("Missed");
                return;
            }

            if (offset < -0.1f)
                Debug.Log("Early");

            if (offset > 0.1f)
                Debug.Log("Late");

            if (offset > -0.1f && offset < 0.1f)
                Debug.Log("Perfect");

            InputOffset.Value = offset;
        }
    }
}
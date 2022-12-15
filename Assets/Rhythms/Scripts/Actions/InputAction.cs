using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythms
{
    public class InputAction : RhythmAction
    {
        public KeyCode InputKey = KeyCode.Space;

        public override void OnTimelineActivate()
        {
            //Listen to the input manager
            InputManager.Instance.ListenToKeyDown(InputKey, KeyDownEvent);
            InputManager.Instance.ListenToKeyUp(InputKey, KeyUpEvent);
        }

        public override void OnTimelineDisable()
        {
            
        }

        private void KeyDownEvent()
        {
            Debug.Log("Key went down");

            //Check if it was within the beat range
        }

        private void KeyUpEvent()
        {
            Debug.Log("Key went up");
        }

        public override void BeatUpdate()
        {
            
        }

        public override void Exit()
        {
            
        }

        public override void Start()
        {
            
        }

        public override void Update()
        {
            
        }
    }
}
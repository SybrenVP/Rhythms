using UnityEngine;
using System.Collections;

namespace Rhythms
{
    //Example action
    public class BeatLerp : RhythmAction
    {
        public R_Bool UseStateLength = new R_Bool(true);

        [Tooltip("Length in beats")]
        public R_Float Length = new R_Float(1f); 

        public override void Start()
        {
            throw new System.NotImplementedException();
        }

        public override void BeatUpdate()
        {
            throw new System.NotImplementedException();
        }

        public override void Update()
        {
            throw new System.NotImplementedException();
        }

        public override void Exit()
        {
            throw new System.NotImplementedException();
        }
    }
}
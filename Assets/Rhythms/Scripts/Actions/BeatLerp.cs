using UnityEngine;
using System.Collections;

namespace Rhythms
{
    public class BeatLerp : RhythmAction
    {
        public R_Bool UseStateLength = new R_Bool(true);

        [Tooltip("Length in beats")]
        public R_Float Length = new R_Float(1f); 

        public override void Raise()
        {
            throw new System.NotImplementedException();
        }
    }
}
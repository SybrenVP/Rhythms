using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    public class MoveGameObject : Action
    {
        public R_GameObject TargetGameObject;
        public R_Vector3 TargetPosition;

        public override void BeatUpdate() { }

        public override void Exit() { }

        public override void Start() { }

        public override void Update() 
        {
            TargetGameObject.Value.transform.position = TargetPosition.Value;
        }
    }
}
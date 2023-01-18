using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    public class InstantiateGameObject : Action
    {
        public R_GameObject TargetPositionObject;

        public R_Vector3 TargetPosition;

        public R_GameObject Prefab;

        public R_GameObject OutputObject;

        public override void Start()
        {
            OutputObject.Value = Instantiate(Prefab.Value, TargetPositionObject.Value == null ? TargetPosition.Value : TargetPositionObject.Value.transform.position, Quaternion.identity);
        }

        public override void BeatUpdate() { }

        public override void Exit() { }

        public override void Update() { }
    }
}
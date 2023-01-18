using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    public class FadeOnInputResult : ListenToInputAction
    {
        public R_GameObject TargetObject;

        public R_Float LengthInBeats;

        private float _lengthInSeconds = -1f;
        private float _prevDelta = 0f;
        private SpriteRenderer[] childSpriteRenderers = null;

        public override void BeatUpdate() { }

        public override void Exit() { }

        public override void Start() 
        {
            _lengthInSeconds = LengthInBeats.Value * _audioData.SecPerBeat;

            childSpriteRenderers = TargetObject.Value.GetComponentsInChildren<SpriteRenderer>();
        }

        public override void Update() 
        { 
            if (InputResult.Value)
            {
                float delta = _prevDelta / _lengthInSeconds;

                float alpha = Mathf.Lerp(1f, 0f, delta);

                foreach (SpriteRenderer spriteRenderer in childSpriteRenderers)
                {
                    Color newColor = spriteRenderer.color;
                    newColor.a = alpha;
                    spriteRenderer.color = newColor;
                }

                _prevDelta += Time.deltaTime;

                if (delta > 1f)
                {
                    Destroy(TargetObject.Value);
                    Enabled = false;
                }
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnSequenceEvent : DoOnSequenceEvent
{
    protected override void OnSequence(float offset)
    {
        Destroy(gameObject);
    }
}

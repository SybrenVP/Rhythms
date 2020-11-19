using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoOnSequenceEvent : MonoBehaviour
{
    protected virtual void Start()
    {
        RhythmController.Instance.SequenceEvent += OnSequence;
    }

    protected abstract void OnSequence(float offset);
}

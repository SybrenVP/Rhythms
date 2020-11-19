using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowOnSequence : DoOnSequenceEvent
{
    protected override void OnSequence(float offset)
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}

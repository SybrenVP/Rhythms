using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSequenceButton : UIButton
{
    [SerializeField] private Sequence _seq = null;

    protected override void OnButtonPress()
    {
        RhythmController.Instance.LoadSequence(_seq);
    }
}

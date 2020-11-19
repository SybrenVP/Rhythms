using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySequenceButton : UIButton
{
    [SerializeField] private Sequence _seq = null;

    protected override void OnButtonPress()
    {
        if (_seq)
            RhythmController.Instance.LoadSequence(_seq, true);
        else
            RhythmController.Instance.StartSequenceMusic();
    }
}

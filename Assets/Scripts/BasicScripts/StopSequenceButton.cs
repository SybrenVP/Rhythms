using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopSequenceButton : UIButton
{
    protected override void OnButtonPress()
    {
        RhythmController.Instance.Stop();
    }
}

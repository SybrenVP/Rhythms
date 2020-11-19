using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : RhythmController
{
    Sequence _loadedSequence = null;

    protected override void Update()
    {
        base.Update();


    }

    public void LoadSequence(Sequence seq)
    {
        _loadedSequence = Instantiate(seq);
    }
}

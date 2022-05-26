using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadedSequenceInformation
{
    public float WidthPerSec = 50f;
    public float WidthPerBeat = 0f;
    public int AmtBeatsInSong = 0;

    //public void Init(Sequence seq)
    //{
    //    float beatPerSec = seq.Bpm / 60f;
    //    WidthPerBeat = WidthPerSec / beatPerSec;
    //
    //    AmtBeatsInSong = (int)(beatPerSec * seq.Song.length);
    //}
}

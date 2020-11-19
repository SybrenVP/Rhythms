using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnBeat : DoOnBeat
{
    public GameObject Target = null;
    public float AmtOfBeatsToReachTarget = 0f;

    public bool StartLerp = false;

    private bool _ongoingLerp = false;
    private int _lerpId = 0;

    void Update()
    {
        if(_ongoingLerp)
        {
            transform.position = BeatLerp.LerpProgressFromId(_lerpId);
        }
    }

    protected void Reached()
    {
        _ongoingLerp = false;
        Debug.Log("Finished lerp at " + Time.time);
    }

    protected override void OnBeat(float beatOffset)
    {
        if (StartLerp && !_ongoingLerp)
        {
            _lerpId = BeatLerp.StartLerp(transform.position, Target.transform.position, AmtOfBeatsToReachTarget, Reached);
            StartLerp = false;
            _ongoingLerp = true;
        }
    }
}

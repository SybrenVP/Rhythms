using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoOnBeat : MonoBehaviour
{
    protected void Start()
    {
        RhythmController.Instance.OnStartMusic += OnStart;
    }

    protected virtual void OnStart()
    {
        RhythmController.Instance.Beat += OnBeat;
        RhythmController.Instance.OnStopMusic += OnStop;

        RhythmController.Instance.OnStartMusic -= OnStart;
    }

    protected virtual void OnStop()
    {
        RhythmController.Instance.Beat -= OnBeat;
        RhythmController.Instance.OnStopMusic -= OnStop;

        RhythmController.Instance.OnStartMusic += OnStart;
    }

    protected abstract void OnBeat(float beatOffset);

    protected void OnDestroy()
    {
        RhythmController.Instance.Beat -= OnBeat;
        RhythmController.Instance.OnStopMusic -= OnStop;
        RhythmController.Instance.OnStartMusic -= OnStart;
    }
}

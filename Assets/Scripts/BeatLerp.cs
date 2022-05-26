using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Do lerps on the beat
/// </summary>
public static class BeatLerp
{
    private static List<BeatLerpObject> _activeLerps = new List<BeatLerpObject>();

    /// <summary>
    /// Starts a lerp within the beatlerp
    /// </summary>
    /// <returns>The id of the lerp</returns>
    public static int StartLerp(Vector3 startPos, Vector3 endPos, float amtBeatsToReachEnd, Action callback)
    {
        //BeatLerpObject newLerp = new BeatLerpObject(_activeLerps.Count, startPos, endPos, RhythmController.Instance.SongPositionInBeats, amtBeatsToReachEnd, callback);
        //_activeLerps.Add(newLerp);
        //
        //return newLerp.Id;

        return 0;
    }

    public static Vector3 LerpProgressFromId(int id)
    {
        //float step = (RhythmController.Instance.SongPositionInBeats - (_activeLerps[id].StartBeat)) / _activeLerps[id].AmtBeatsToReachEnd;
        //
        //Vector3 newPos = Vector3.Lerp(_activeLerps[id].StartPos, _activeLerps[id].EndPos, step);
        //
        //if(Vector3.SqrMagnitude(newPos - _activeLerps[id].EndPos) <= Mathf.Epsilon)
        //{
        //    _activeLerps[id].Callback?.Invoke();
        //    EndLerpId(id);
        //}
        //
        //return newPos;

        return Vector3.zero;
    }

    public static void EndLerpId(int id)
    {
        Debug.Log("Lerp took: " + (Time.time - _activeLerps[id].StartTime));
        _activeLerps.RemoveAt(id);
    }
}

public class BeatLerpObject
{
    public int Id = 0;
    public Vector3 StartPos = Vector3.zero;
    public Vector3 EndPos = Vector3.zero;
    public float StartBeat = 0;
    public float AmtBeatsToReachEnd = 0;
    public Action Callback;

    public float StartTime = 0f;

    public BeatLerpObject(int id, Vector3 startPos, Vector3 endPos, float startBeat, float amtBeatsToReachEnd, Action callback)
    {
        Id = id;
        StartPos = startPos;
        EndPos = endPos;
        StartBeat = startBeat;
        AmtBeatsToReachEnd = amtBeatsToReachEnd;
        Callback = callback;

        StartTime = Time.time;
    }
}

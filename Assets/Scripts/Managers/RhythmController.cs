using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Based on https://www.gamasutra.com/blogs/GrahamTattersall/20190515/342454/Coding_to_the_Beat__Under_the_Hood_of_a_Rhythm_Game_in_Unity.php
[RequireComponent(typeof(AudioSource))]
public class RhythmController : Singleton<RhythmController>
{
    public delegate void MusicEvent();
    public delegate void OnBeat(float beatOffset);
    public MusicEvent OnStopMusic;
    public MusicEvent OnStartMusic;
    public OnBeat Beat;
    public OnBeat SequenceEvent;
    public MusicEvent OnFinishedSequence;

    #region Information accessors

    [Tooltip("Fill in based on the song we're trying to sync to")]
    public float SongBpm = 0f;

    [Tooltip("Number of seconds for each beat")]
    public float SecPerBeat = 0f;

    [Tooltip("Current position in the song, in seconds")]
    public float SongPosition = 0f;

    [Tooltip("Current position in the song, in beats")]
    public float SongPositionInBeats = 0f;

    [Tooltip("The amount of seconds passed since the song started")]
    public float SecondsPassed = 0f;

    [Tooltip("The amount of beats passed")]
    public int BeatsPassed = 0;

    [Tooltip("The offset to the first beat")]
    public float SongOffset = 0f;

    #endregion

    #region Variables

    [Header("Sequence information")]
    public Sequence LoadedSequence = null;

    public bool SeqIsFinished = false;

    [Tooltip("The source that plays the music")]
    [HideInInspector] public AudioSource MusicSource = null;

    private float _lastBeat = 0;
    private int _sequenceIndex = 0;

    #endregion

    #region Debug
#if UNITY_EDITOR
    public bool LogVerbose = true;
#endif
#endregion

    public override void Start()
    {
        base.Start();

        if (MusicSource == null)
            MusicSource = GetComponent<AudioSource>();

        if(MusicSource == null)
        {
            Debug.LogError("No audio source in Rythm Controller!");
            Destroy(gameObject);
        }

        enabled = false;
    }

    protected virtual void Update()
    {
        SongPosition = (float)(AudioSettings.dspTime - SecondsPassed - SongOffset);

        SongPositionInBeats = SongPosition / SecPerBeat;

        BeatsPassed = (int)SongPositionInBeats;

        if (_lastBeat + 1f <= SongPositionInBeats)
        {
            _lastBeat += 1f;
            Beat?.Invoke(SongPositionInBeats - _lastBeat);
            if(LoadedSequence && _sequenceIndex < LoadedSequence.EventPositions.Count && Mathf.Abs(LoadedSequence.EventPositions[_sequenceIndex] - _lastBeat) < Mathf.Epsilon)
            {
                _sequenceIndex++;
                SequenceEvent?.Invoke(SongPositionInBeats - _lastBeat);
                Log("Sequence event invoked");
            }

            if(!SeqIsFinished && _sequenceIndex >= LoadedSequence.EventPositions.Count)
            {
                OnFinishedSequence?.Invoke();
                SeqIsFinished = true;
                Log("Sequence finished");
            }

            Log("Beat invoked with an offset of " + (SongPositionInBeats - _lastBeat));
        }
    }

    public virtual void StartMusic()
    {
        SecPerBeat = 60f / SongBpm;

        _lastBeat = 0;
        _sequenceIndex = 0;
        enabled = true;
        SeqIsFinished = false;

        SecondsPassed = (float)AudioSettings.dspTime;

        OnStartMusic?.Invoke();

        MusicSource.Play();
    }

    public virtual void StartSequenceMusic()
    {
        if (LoadedSequence == null)
        {
            Log("ERROR: No sequence loaded!");
            return;
        }

        MusicSource.clip = LoadedSequence.Song;
        MusicSource.loop = LoadedSequence.Loop;

        StartMusic();
    }

    public virtual void Stop()
    {
        OnStopMusic?.Invoke();
        enabled = false;
        MusicSource.Stop();
    }

    public void Log(string log)
    {
#if UNITY_EDITOR
        if(LogVerbose)
            Debug.Log("RhythmController :: " + log);
#endif
    }

    public void LoadSequence(Sequence seq, bool startImmediately = false)
    {
        LoadedSequence = Instantiate(seq);

        SongBpm = LoadedSequence.Bpm;
        SongOffset = LoadedSequence.SongOffset;

        if (startImmediately)
        {
            StartSequenceMusic();
        }
        else
        {
            Stop();
        }
    }
}

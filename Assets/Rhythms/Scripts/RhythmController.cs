//Based on https://www.gamasutra.com/blogs/GrahamTattersall/20190515/342454/Coding_to_the_Beat__Under_the_Hood_of_a_Rhythm_Game_in_Unity.php
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rhythm
{
    [RequireComponent(typeof(AudioSource))]
    public class RhythmController : MonoBehaviour
    {
        public Sequence ActiveSequence = null;

        private AudioSource _activeSource = null;

        private float _songPositionInSeconds = 0f;
        private float _songPositionInBeats = 0f;
        private int _currentBeatNumber = 0;

        private float _startTime = 0f;
        private int _previousBeat = 0;
        
        private AudioData _activeAudio = null;

        private void Start()
        {
            _activeSource = GetComponent<AudioSource>();

            Load(ActiveSequence);
            Play();
        }

        public void Load(Sequence sequence)
        {
            ActiveSequence = Instantiate(sequence); //TODO: This has to be removed probably
            _activeAudio = ActiveSequence.Audio;

            _activeSource.clip = _activeAudio.Song;
            _activeSource.loop = _activeAudio.Loop;

            _currentBeatNumber = 0;
        }

        public void Play()
        {
            _startTime = Time.time;
            _activeSource.Play();
            ActiveSequence.Start();
        }

        public void Update()
        {
            if (_activeSource.isPlaying)
            {
                _songPositionInSeconds = Time.time - _startTime - _activeAudio.SongOffset;
                _songPositionInBeats = _songPositionInSeconds / _activeAudio.SecPerBeat;

                _currentBeatNumber = (int)_songPositionInBeats;

                UpdateSequence();

                if (_previousBeat < _currentBeatNumber)
                {
                    _previousBeat = _currentBeatNumber;
                }

            }
        }

        public void UpdateSequence()
        {
            if (_previousBeat < _currentBeatNumber)
            {
                ActiveSequence.OnBeatUpdate(_currentBeatNumber);
            }

            ActiveSequence.OnUpdate(_currentBeatNumber);
        }
    }
}
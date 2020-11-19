using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SequenceEditor : EditorWindow
{
    private static Sequence _active = null;
    private static Texture2D _line = null;
    private static Texture2D _tex = null;

    private float _widthPerBeat = 0f;
    private float _songLength = 0f;

    private int _movingEvent = -1;

    private bool _foldOutOpen = true;

    private const float _buttonSize = 30f;
    private const int _amtButtons = 5;
    private const float _sideOffset = 5f;

    private bool _isPlaying = false;
    private float _startTime = 0f;
    private float _progress = 0f;
    private float _widthPerSec = 0f;

    private bool _recording = false;

    private Rect lastWindowSize;

    private Texture2D _addButtonTex = null;
    private Texture2D _playButtonTex = null;
    private Texture2D _pauseButtonTex = null;
    private Texture2D _recordButtonTex = null;
    private Texture2D _settingsButtonTex = null;

    [MenuItem("Window/SequenceEditor")]
    public static void OpenWindow()
    {
        SequenceEditor window = FindObjectOfType<SequenceEditor>();
        if (window != null)
            DestroyImmediate(window);

        window = CreateInstance<SequenceEditor>();
        window.minSize = new Vector2(1000f, 250f);
        window.Show();

        window.RedrawImage();
    }

    public void OnFocus()
    {
        lastWindowSize = position;

        RedrawImage();
    }

    private void Update()
    {
        if (_isPlaying && !EditorApplication.isPlaying)
        {
            _progress = Time.realtimeSinceStartup - _startTime;
            Repaint();
        }
        else if(EditorApplication.isPlaying)
        {
            //Fetch the rhythmcontroller
            var rhythmController = FindObjectOfType<RhythmController>();
            //get the loaded sequence
            if (rhythmController.LoadedSequence != null && rhythmController.LoadedSequence != _active)
            {
                _active = rhythmController.LoadedSequence;
                _isPlaying = false;
                _progress = 0f;
                RedrawImage();
            }
            else if(rhythmController.MusicSource.isPlaying)
            {
                _progress = rhythmController.SongPosition;
                _isPlaying = true;
                Repaint();
            }

        }
    }

    private void OnGUI()
    {
        if (_active == null)
            return;

        #region Input

        var e = Event.current;

        switch(e.type)
        {
            case EventType.MouseUp:
                _movingEvent = -1;
                _active.ReorderEvents();
                break;
            case EventType.KeyUp:
                if (_recording && e.keyCode == KeyCode.Space)
                    _active.AddEvent(Mathf.Round(((_progress * _widthPerSec) / _widthPerBeat) * 2 / 1) * 1 / 2);
                if (_movingEvent >= 0 && e.keyCode == KeyCode.D)
                {
                    _active.EventPositions.RemoveAt(_movingEvent);
                    _movingEvent = -1;
                }
                break;
        }

        if (_movingEvent >= 0)
        {
            _active.EventPositions[_movingEvent] = Mathf.Round((e.mousePosition.x / _widthPerBeat) * 2 / 1) * 1 / 2;

            Repaint();
        }

        if (position != lastWindowSize)
        {
            lastWindowSize = position;
            RedrawImage();
        }

        #endregion

        #region Waveform tex

        GUI.DrawTexture(new Rect(0, position.size.y - _tex.height, position.size.x, position.size.y), _tex);

        #endregion

        #region Sequence events

        for (int i = 0; i < _active.EventPositions.Count; i++)
        {
            Rect buttonRect = new Rect(_widthPerBeat * _active.EventPositions[i] + _active.SongOffset, position.size.y - _tex.height, 1f, position.size.y);
            GUI.DrawTexture(buttonRect, _line);

            if(e.type == EventType.MouseDown)
            {
                Rect inputRect = new Rect(buttonRect.position - new Vector2(1f, 0f), buttonRect.size + new Vector2(2f, 0f));
                if (inputRect.Contains(e.mousePosition))
                {
                    _movingEvent = i;
                    Debug.Log("Moving event: " + i);
                }
            }
        }

        #endregion

        #region Visual progress while playing

        if (_isPlaying)
        {
            EditorGUI.DrawRect(new Rect(0f, position.size.y - _tex.height, _progress * _widthPerSec, _tex.height), new Color(0.7f, 0.7f, 0.7f, 0.5f));
            if (_progress >= _active.Song.length)
            {
                _isPlaying = false;
                _recording = false;
            }
        }

        #endregion

        #region Editor buttons

        EditorGUI.DrawRect(new Rect(0f, position.size.y - (_foldOutOpen ? (_sideOffset * 2f + _buttonSize) : 0f), _amtButtons * _buttonSize + _sideOffset * 2f, _sideOffset * 2f + _buttonSize), new Color(0.5f, 0.5f, 0.5f, 1f));

        if (GUI.Button(new Rect(_sideOffset * 2f + (_amtButtons - 1) * _buttonSize, position.size.y - (_foldOutOpen ? (_sideOffset * 2f + _buttonSize) : 0f) - _buttonSize / 2f, _buttonSize, _buttonSize / 2f), "^"))
        {
            _foldOutOpen = !_foldOutOpen;
            _movingEvent = -1;
            return;
        }

        if (_foldOutOpen && GUI.Button(new Rect(_sideOffset, position.size.y - _buttonSize - _sideOffset, _buttonSize, _buttonSize), new GUIContent(_addButtonTex)))
        {
            _active.EventPositions.Add(_active.EventPositions[_active.EventPositions.Count - 1] + 1);
            return;
        }

        if (_foldOutOpen && GUI.Button(new Rect(_sideOffset + _buttonSize, position.size.y - _buttonSize - _sideOffset, _buttonSize, _buttonSize), new GUIContent(_playButtonTex)))
        {
            StartClip();
            return;
        }

        if (_foldOutOpen && GUI.Button(new Rect(_sideOffset + _buttonSize * 2, position.size.y - _buttonSize - _sideOffset, _buttonSize, _buttonSize), new GUIContent(_pauseButtonTex)))
        {
            SequenceUtilities.StopClip(_active.Song);
            _isPlaying = false;
            _recording = false;
            return;
        }

        if(_foldOutOpen && GUI.Button(new Rect(_sideOffset + _buttonSize * 3, position.size.y - _buttonSize - _sideOffset, _buttonSize, _buttonSize), new GUIContent(_recordButtonTex)))
        {
            _recording = true;
            StartClip();
            return;
        }

        if (_foldOutOpen && GUI.Button(new Rect(_sideOffset + _buttonSize * 4, position.size.y - _buttonSize - _sideOffset, _buttonSize, _buttonSize), new GUIContent(_settingsButtonTex)))
        {
            return;
        }

        #endregion
    }

    private void OnSelectionChange()
    {
        RedrawImage();
    }

    public void ChangeSelection()
    {
        if (Selection.activeObject is Sequence)
        {
            _active = Selection.activeObject as Sequence;
        }
    }

    private void RedrawImage()
    {
        ChangeSelection();
        if (_active)
        {
            _tex = SequenceUtilities.GetWaveformTextureFromAudioClip(_active.Song, 1f, (int)position.size.x, (int)position.size.y, Color.white);
            _line = SequenceUtilities.GetTexture(1, (int)position.size.y, Color.cyan);


            //Define song length and width/beat
            _songLength = _active.Song.length;
            _widthPerSec = _tex.width / _songLength;
            float BeatPerSec = _active.Bpm / 60f;

            _widthPerBeat = _widthPerSec / BeatPerSec;

            _addButtonTex = Resources.Load("add") as Texture2D;
            _playButtonTex = Resources.Load("play") as Texture2D;
            _pauseButtonTex = Resources.Load("pause") as Texture2D;
            _recordButtonTex = Resources.Load("record") as Texture2D;
            _settingsButtonTex = Resources.Load("settings") as Texture2D;
        }

        Repaint();
    }

    private void StartClip()
    {
        SequenceUtilities.PlayClip(_active.Song);
        _startTime = Time.realtimeSinceStartup;
        _isPlaying = true;
    }
}

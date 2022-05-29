using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class RhythmBeatDragBox
{
    //Uses a call back whenever the beat on the drag box changed

    private int _beat = 0;
    private Rhythms_Editor.TrackTimeline _owningTimeline = null;

    private Vector2 _size;
    private Rect _view;
    private UnityAction<int, int> _onBeatChanged;
    private UnityAction _onBeatApplied;

    private bool _dragActive = false;

    public static RhythmBeatDragBox Create(int beat, Rhythms_Editor.TrackTimeline owningTimeline, Vector2 size, UnityAction<int, int> onBeatChanged, UnityAction onBeatApplied)
    {
        RhythmBeatDragBox newBox = new RhythmBeatDragBox();
        newBox._beat = beat;
        newBox._onBeatChanged = onBeatChanged;
        newBox._onBeatApplied = onBeatApplied;

        newBox._owningTimeline = owningTimeline;
        newBox._size = size;


        newBox.SetView();

        return newBox;
    }

    public void Draw()
    {
        SetView();

        EditorGUI.DrawRect(_view, Color.white);

        Input(Event.current);
    }

    private void SetView()
    {
        _view = new Rect(_owningTimeline.GetXForBeat(_beat) - _size.x * 0.5f, _owningTimeline.View.center.y - _size.y * 0.5f, _size.x, _size.y);
    }

    public void Input(Event e)
    {
        //Handle the mouse input for this hotcontrol
        //Check if we clicked our handle

        int controlID = GUIUtility.GetControlID("ResizeDrag".GetHashCode(), FocusType.Passive);
        int hotControl = GUIUtility.hotControl;

        switch(e.GetTypeForControl(controlID))
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (!_dragActive && _view.Contains(e.mousePosition))
                    {
                        _dragActive = true;
                        e.Use();
                    }
                }

                break;

            case EventType.MouseUp:
                if (e.button == 0)
                {
                    if (_dragActive)
                    {
                        _onBeatApplied?.Invoke();
                        _dragActive = false;
                    }
                }

                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    if (_dragActive)
                    {
                        //Check the beat for the current mouse position
                        //if the beat is 
                        int newBeat = _owningTimeline.GetBeatForPosition(e.mousePosition);
                        if (newBeat != _beat)
                        {
                            _onBeatChanged?.Invoke(_beat, newBeat);

                            _beat = newBeat;
                            SetView();
                        }
                    }
                }

                break;


        }
    }
}

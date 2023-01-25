using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UIElements;

public class StateView : VisualElement
{
    public Action<StateView> OnStateSelected;

    public BehaviourTree State;

    private TrackView _trackParent;

    #region Drag Var

    private int _startTrackId;
    private Vector2 _previousPosition;
    private enum EDragState
    {
        AtRest,
        Ready,
        Dragging
    }
    private EDragState _dragState;

    #endregion

    private bool _isSelected = false;

    public StateView(BehaviourTree state, TrackView track)
    {
        State = state;
        _trackParent = track;
        _dragState = EDragState.AtRest;

        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UIBuilderImplementations/Editor/StateView.uxml");
        visualTree.CloneTree(this);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/UIBuilderImplementations/Editor/StateView.uss");
        styleSheets.Add(styleSheet);

        style.left = State.Position.x;
        style.top = State.Position.y;
        style.height = TrackView.HEIGHT_PER_TRACK;

        RegisterCallback<MouseDownEvent>(OnMouseDown);
    }

    #region Mouse events

    private void OnMouseDown(MouseDownEvent downEvent)
    {
        _dragState = EDragState.Ready;
        Select();
    }

    private void OnMouseUp(MouseUpEvent upEvent)
    {
        if (_dragState != EDragState.AtRest && upEvent.button == 0)
        {
            StopDraggingStateView();
        }
    }

    private void OnMouseMove(MouseMoveEvent moveEvent)
    {
        if (_dragState == EDragState.Ready)
        {
            StartDraggingStateView(moveEvent.mousePosition);
        }
        else if (_dragState == EDragState.Dragging)
        {
            MoveStateView(moveEvent.mousePosition);
        }
    }

    private void OnMouseLeave(MouseLeaveEvent leaveEvent)
    {
        if (_dragState == EDragState.Dragging)
        {
            MoveStateView(leaveEvent.mousePosition);
        }
    }

    private void OnMouseLeaveParent(MouseLeaveEvent leaveEvent)
    {
        if (_dragState == EDragState.Dragging)
        {
            StopDraggingStateView();
        }
    }

    private void StartDraggingStateView(Vector2 mousePos)
    {
        AddToClassList("dragging");
        _dragState = EDragState.Dragging;

        Vector2 localPos = parent.WorldToLocal(mousePos);

        _previousPosition = _trackParent.WorldToLocal(mousePos);
        _startTrackId = _trackParent.ConvertPositionToTrackId(localPos);
    }

    private void MoveStateView(Vector2 newMousePos)
    {
        Vector2 localPos = parent.WorldToLocal(newMousePos);

        int beat = _trackParent.ConvertPositionToBeat(localPos);
        float xPos = _trackParent.ConvertBeatToPosition(beat);

        //Check if the mousePosition is within the last 20 pixels
        if (_trackParent.IsWithinExpandArea(localPos))
        {
            _trackParent.Expand();
        }
        else
        {
            _trackParent.TryShrink();
        }

        Vector2 mouseTrackLocalPos = _trackParent.WorldToLocal(newMousePos);
        _trackParent.TrackVerticalScroll(mouseTrackLocalPos, _previousPosition);
        _previousPosition = mouseTrackLocalPos;

        int trackId = _trackParent.ConvertPositionToTrackId(localPos);
        float yPos = _trackParent.ConvertTrackIdToPosition(trackId);

        style.left = xPos;
        style.top = yPos;

        State.Position.x = xPos;
        State.Position.y = yPos;

        State.Beat = beat;
        //A save might be necessary
    }

    private void StopDraggingStateView()
    {
        RemoveFromClassList("dragging");
        _dragState = EDragState.AtRest;
    }

    public void EnableMove()
    {
        _dragState = EDragState.AtRest;

        RegisterCallback<MouseUpEvent>(OnMouseUp);
        RegisterCallback<MouseMoveEvent>(OnMouseMove);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        parent.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveParent);

        if (_isSelected)
            _dragState = EDragState.Ready;
    }

    public void DisableMove()
    {
        UnregisterCallback<MouseUpEvent>(OnMouseUp);
        UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
        parent.UnregisterCallback<MouseLeaveEvent>(OnMouseLeaveParent);
    }

    #endregion

    public void Select()
    {
        _isSelected = true;
        OnStateSelected?.Invoke(this);
    }

}

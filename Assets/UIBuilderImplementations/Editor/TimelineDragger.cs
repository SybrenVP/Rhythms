using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TimelineDragger : VisualElement
{
    private enum EDragState
    {
        AtRest,
        Ready,
        Dragging
    }
    private EDragState _dragState;

    public new class UxmlFactory : UxmlFactory<TimelineDragger, TimelineDragger.UxmlTraits> { }

    public TimelineDragger() 
    {
        RegisterCallback<MouseDownEvent>(OnMouseDown);
        RegisterCallback<MouseUpEvent>(OnMouseUp);
        RegisterCallback<MouseMoveEvent>(OnMouseMove);
        RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
        if (parent != null)
            RegisterCallback<MouseLeaveEvent>(OnMouseLeaveParent);
    }

    private void OnMouseDown(MouseDownEvent mouseEvent)
    {
        _dragState = EDragState.Ready;
    }

    private void OnMouseUp(MouseUpEvent mouseEvent)
    {
        if (_dragState != EDragState.AtRest && mouseEvent.button == 0)
        {
            StopDragging();
        }
    }

    private void OnMouseMove(MouseMoveEvent mouseEvent)
    {
        if (_dragState == EDragState.Ready)
            StartDragging(mouseEvent.mousePosition);
        else if (_dragState == EDragState.Dragging)
            Move(mouseEvent.mousePosition);
    }

    private void OnMouseLeave(MouseLeaveEvent mouseEvent)
    {
        if (_dragState == EDragState.Dragging)
            Move(mouseEvent.mousePosition);
            
    }

    private void OnMouseLeaveParent(MouseLeaveEvent mouseEvent)
    {
        if (_dragState == EDragState.Dragging)
            StopDragging();
    }

    private void StartDragging(Vector2 mousePos)
    {
        _dragState = EDragState.Dragging;
    }

    private void Move(Vector2 mousePos)
    {
        style.left = parent.WorldToLocal(mousePos).x;
    }

    private void StopDragging()
    {
        _dragState = EDragState.AtRest;
    }
}

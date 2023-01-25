using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StateSelector
{
    private TrackView _trackView;

    private VisualElement _selectionRect;
    private Vector2 _start;
    private Vector2 _end;

    private VisualElement _scrollViewContent;

    private List<StateView> _selectedStates = new List<StateView>();

    //This class listens to the mouse events of the scrollViewContent

    public StateSelector(TrackView track, VisualElement scrollViewContent)
    {
        _trackView = track;

        _scrollViewContent = scrollViewContent;

        _selectionRect = _scrollViewContent.Q<VisualElement>("selectionrect");
        _selectionRect.visible = false;
        _selectionRect.BringToFront();
    }

    public void EnableSelection()
    {
        _scrollViewContent.RegisterCallback<MouseDownEvent>(OnMouseDown);
        _scrollViewContent.RegisterCallback<MouseUpEvent>(OnMouseUp);
        _scrollViewContent.RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
    }

    public void DisableSelection()
    {
        _scrollViewContent.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        _scrollViewContent.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        _scrollViewContent.UnregisterCallback<MouseLeaveEvent>(OnMouseLeave);
    }

    #region Mouse Events

    private void OnMouseDown(MouseDownEvent mouseDown)
    {
        if (mouseDown.button == 0)
        {
            Vector2 scrollViewContentPos = _scrollViewContent.WorldToLocal(mouseDown.mousePosition);
            _selectionRect.visible = true;

            _start = scrollViewContentPos;

            _selectionRect.style.left = scrollViewContentPos.x;
            _selectionRect.style.top = scrollViewContentPos.y;

            Vector2 bottomRight = _scrollViewContent.layout.size - scrollViewContentPos;
            _selectionRect.style.right = bottomRight.x;
            _selectionRect.style.bottom = bottomRight.y;

            _scrollViewContent.RegisterCallback<MouseMoveEvent>(OnMouseDrag);
        }
    }

    private void OnMouseDrag(MouseMoveEvent mouseMove)
    {
        //Update selection rect visual
        Vector2 scrollViewContentPos = _scrollViewContent.WorldToLocal(mouseMove.mousePosition);

        Vector2 positiveBottomRight = _scrollViewContent.layout.size - scrollViewContentPos;
        Vector2 negativeBottomRight = _scrollViewContent.layout.size - _start;

        Vector2 diff = scrollViewContentPos - _start;

        if (Mathf.Sign(diff.x) > 0)
        {
            _selectionRect.style.left = _start.x;
            _selectionRect.style.right = positiveBottomRight.x;
        }
        else
        {
            _selectionRect.style.left = scrollViewContentPos.x;
            _selectionRect.style.right = negativeBottomRight.x;
        }

        if (Mathf.Sign(diff.y) > 0)
        {
            _selectionRect.style.top = _start.y;
            _selectionRect.style.bottom = positiveBottomRight.y;
        }
        else
        {
            _selectionRect.style.top = scrollViewContentPos.y;
            _selectionRect.style.bottom = negativeBottomRight.y;
        }
    }

    private void OnMouseUp(MouseUpEvent mouseUp)
    {
        if (mouseUp.button == 0)
        {
            FinishSelectionRect(mouseUp.mousePosition);
        }
    }

    private void OnMouseLeave(MouseLeaveEvent mouseLeave)
    {
        FinishSelectionRect(mouseLeave.mousePosition);
    }

    private void FinishSelectionRect(Vector2 endPos)
    {
        _end = _scrollViewContent.WorldToLocal(endPos);
        RemoveSelectionRect();
    }

    private void RemoveSelectionRect()
    {
        _selectedStates = _trackView.GetRectBasedStateViews(_selectionRect.layout);

        _selectionRect.visible = false;
        _scrollViewContent.UnregisterCallback<MouseMoveEvent>(OnMouseDrag);
    }

    #endregion
}

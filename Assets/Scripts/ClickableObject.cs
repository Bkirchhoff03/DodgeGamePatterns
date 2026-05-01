using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    Action LeftClickAction;
    Action MiddleClickAction;
    Action RightClickAction;

    public void SetActions(Action leftClick, Action middleClick, Action rightClick)
    {
        LeftClickAction = leftClick;
        MiddleClickAction = middleClick;
        RightClickAction = rightClick;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) { LeftClickAction?.Invoke(); }
        else if (eventData.button == PointerEventData.InputButton.Middle) { MiddleClickAction?.Invoke(); }
        else if (eventData.button == PointerEventData.InputButton.Right) { RightClickAction?.Invoke(); }
    }
}

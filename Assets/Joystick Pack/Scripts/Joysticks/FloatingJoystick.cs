using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloatingJoystick : Joystick
{
    Vector2 originalPosition;

    protected override void Start()
    {
        base.Start();
        originalPosition = background.anchoredPosition;
        //background.gameObject.SetActive(false);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        Vector2 newPosition = ScreenPointToAnchoredPosition(eventData.position);
        background.anchoredPosition = newPosition;
        background.gameObject.SetActive(true);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        // background.gameObject.SetActive(false);
        background.anchoredPosition = originalPosition;
        base.OnPointerUp(eventData);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public Image JoystickBG;
    public Image JoystickImage;

    private Vector2 joystickInput = new Vector2();           

    public Vector2 GetJoystickInput()
    {
        return joystickInput;
    }

    public float GetHorizontalAxis()
    {
        return joystickInput.x;
    }

    public float GetVerticalAxis()
    {
        return joystickInput.y;
    }

    public float GetHorizontalAxisRaw()
    {
        if (joystickInput.x > 0.35f)
            return 1f;
        else if (joystickInput.x < -0.35f)
            return -1f;
        else
            return 0;
    }

    public float GetVerticalAxisRaw()
    {
        if (joystickInput.y > 0.35f)
            return 1f;
        else if (joystickInput.y < -0.35f)
            return -1f;
        else
            return 0;
    }


    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(JoystickBG.rectTransform
                                                                    , eventData.position
                                                                    , eventData.pressEventCamera
                                                                    , out pos))
        {
            pos.x = (pos.x / JoystickBG.rectTransform.sizeDelta.x) * 2f;
            pos.y = (pos.y / JoystickBG.rectTransform.sizeDelta.y) * 2f;
        }

        joystickInput = pos.magnitude > 1f ? pos.normalized : pos;

        JoystickImage.rectTransform.anchoredPosition = new Vector2(joystickInput.x * JoystickBG.rectTransform.sizeDelta.x / 3
                                                                 , joystickInput.y * JoystickBG.rectTransform.sizeDelta.y / 3);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        joystickInput = new Vector2();
        JoystickImage.rectTransform.anchoredPosition = new Vector2();
    }



}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonEvents : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    public bool IsButtonPressed { get; set; }   

    private void Awake()
    {
        IsButtonPressed = false;               
    }
     

    public void OnPointerDown(PointerEventData eventData)
    {
        IsButtonPressed = true;       
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsButtonPressed = false;       
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsButtonPressed = false;       
    }
}

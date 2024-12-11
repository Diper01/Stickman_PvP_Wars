using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotInputController : MonoBehaviour, IInputController
{
    public bool Crouch { get; set; }
    public bool Fire { get; set; }   
    public float HorizontalAxis { get; set; }    

    public event Action ChangeWeapon;
    public event Action Jump;

    public void OnChangeWeapon() {
        if (ChangeWeapon != null) {
            ChangeWeapon();
        }
    }

    public void OnJump() {
        if (Jump != null) {            
            Jump();
        }
    }

    public void ClearInput() {
        this.Crouch = false;
        this.Fire = false;
        this.HorizontalAxis = 0f;
    }

}

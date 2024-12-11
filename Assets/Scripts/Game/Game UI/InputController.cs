using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour, IInputController
{
    public event Action ChangeWeapon;
    public event Action Jump;
    public ButtonEvents ButtonFire;
    public ButtonEvents ButtonCrouchA;
    public ButtonEvents ButtonLeftA;
    public ButtonEvents ButtonRightA;
    public ButtonEvents ButtonCrouchB;
    public ButtonEvents ButtonLeftB;
    public ButtonEvents ButtonRightB;
    public VirtualJoystick joystick;
    public bool Fire { get; set; }   
    public bool Crouch { get; set; }
    public float HorizontalAxis { get; set; }

    private void Awake()
    {
        OnControllChanged(GameOptions.ControllType);
    }

    private void OnEnable()
    {
        GameOptions.ControllChanged += OnControllChanged;
    }

    private void OnDisable()
    {

        GameOptions.ControllChanged -= OnControllChanged;
    }

    private void Update()
    {       
        GetInput();                
    }

    private void GetInput() {
        if (Input.GetKeyDown(KeyCode.UpArrow) 
            || joystick.GetVerticalAxisRaw() == 1 
            || Input.GetButton("Jump") 
            || Input.GetAxisRaw("Vertical") > 0.5f)
        {
            OnJump();
        }

        if (ButtonRightA.IsButtonPressed || ButtonRightB.IsButtonPressed)
        {
            HorizontalAxis = 1f;
        }
        else if (ButtonLeftA.IsButtonPressed || ButtonLeftB.IsButtonPressed)
        {
            HorizontalAxis = -1f;
        }
        else if (joystick.GetHorizontalAxisRaw() != 0)
        {
            HorizontalAxis = joystick.GetHorizontalAxisRaw();
        }
        else
        {
            HorizontalAxis = Input.GetAxisRaw("Horizontal");
        }

        Fire = ButtonFire.IsButtonPressed || Input.GetKey(KeyCode.LeftControl) || Input.GetButton("Fire1");
        Crouch = ButtonCrouchA.IsButtonPressed || ButtonCrouchB.IsButtonPressed || joystick.GetVerticalAxisRaw() == -1f || Input.GetAxisRaw("Vertical") < -0.5f;

        if (Input.GetKeyDown(KeyCode.C) || Input.GetButtonDown("Fire3")) {
            OnChangeWeapon();
        }
    }

    public void OnChangeWeapon() {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        if (ChangeWeapon != null) {
            ChangeWeapon();
        }
    }

    public void OnJump() {
        if (Jump != null) {
            Jump();
        }
    } 

    private void OnControllChanged(ControllType type) {
        switch (type)
        {
            case ControllType.ButtonsA:
                ButtonCrouchA.gameObject.SetActive(true);
                ButtonLeftA.gameObject.SetActive(true);
                ButtonRightA.gameObject.SetActive(true);
                ButtonCrouchB.gameObject.SetActive(false);
                ButtonLeftB.gameObject.SetActive(false);
                ButtonRightB.gameObject.SetActive(false);
                joystick.gameObject.SetActive(false);
                break;
            case ControllType.ButtonsB:
                ButtonCrouchA.gameObject.SetActive(false);
                ButtonLeftA.gameObject.SetActive(false);
                ButtonRightA.gameObject.SetActive(false);
                ButtonCrouchB.gameObject.SetActive(true);
                ButtonLeftB.gameObject.SetActive(true);
                ButtonRightB.gameObject.SetActive(true);
                joystick.gameObject.SetActive(false);
                break;
            case ControllType.Joystick:
                ButtonCrouchA.gameObject.SetActive(false);
                ButtonLeftA.gameObject.SetActive(false);
                ButtonRightA.gameObject.SetActive(false);
                ButtonCrouchB.gameObject.SetActive(false);
                ButtonLeftB.gameObject.SetActive(false);
                ButtonRightB.gameObject.SetActive(false);
                joystick.gameObject.SetActive(true);
                break;           
        }
    }
}

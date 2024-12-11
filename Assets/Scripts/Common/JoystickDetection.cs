using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickDetection : MonoBehaviour {

    public static JoystickDetection Instace;

    private void Start()
    {
        if (Instace == null) {
            Instace = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else{
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetButtonDown("JoystickButton")) 
        {
            GameOptions.JoystickAttached = true;
        }
        else if (Input.touchCount >= 1)
        {
            GameOptions.JoystickAttached = false;
        }
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class NameInput : MonoBehaviour {

    private InputField inputField;

    private void Start()
    {
        inputField = GetComponent<InputField>();       

        if (DataManger.PlayerName != "") {
            inputField.text = DataManger.PlayerName;
        }
    }

    public void OnEndEditName() {
        DataManger.PlayerName = inputField.text;
        
    }
}

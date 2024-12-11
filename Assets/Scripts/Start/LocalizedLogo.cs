using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LocalizedLogo : MonoBehaviour {
    
    [SerializeField]Sprite logoENG;
    [SerializeField]Sprite logoRU;


    private void Start() {
        Image logoImage = GetComponent<Image>();
        if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian || Application.systemLanguage == SystemLanguage.Belarusian)
            logoImage.sprite = logoRU;        
        else 
            logoImage.sprite = logoENG;        
    }
}

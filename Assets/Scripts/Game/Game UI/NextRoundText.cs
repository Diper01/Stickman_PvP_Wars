using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class NextRoundText : MonoBehaviour {
    private Text text;

    private void Awake()
    {
        text = GetComponent<Text>();        
    }

    private void OnEnable()
    {       
        text.text = Localisation.GetString("NextRound").Replace("#", "20") + " " + Localisation.GetString("Seconds") + "!";        
    }
}

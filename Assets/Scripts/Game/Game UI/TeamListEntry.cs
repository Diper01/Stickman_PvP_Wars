using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamListEntry : MonoBehaviour {
    [SerializeField] Text playerNameText;

    public bool IsHighlighted {
        get { return isHighlighted; }
        set {
            isHighlighted = value;
            if (isHighlighted)
                playerNameText.color = Color.yellow;            
            else
                playerNameText.color = Color.white;
        }
    }

    private bool isHighlighted = false;

    public void SetPlayerName(string name) {
        playerNameText.text = name;
    }

}

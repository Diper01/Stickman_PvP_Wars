using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamPlayerListEntry : MonoBehaviour {
    [SerializeField] Text nicknameText;
    [SerializeField] Text fragsText;
    [SerializeField] Text deathsText;
  
    public void ShowEntry(PlayerData playerData)
    {
        this.gameObject.SetActive(true);
        nicknameText.text = playerData.PlayerName;
        fragsText.text = playerData.Kills.ToString();
        deathsText.text = playerData.Death.ToString();       
    }

    public void HideEntry()
    {
        this.gameObject.SetActive(false);
    }

}

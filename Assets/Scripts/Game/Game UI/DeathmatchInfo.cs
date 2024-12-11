using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathmatchInfo : Photon.PunBehaviour {
 
    [SerializeField] Text roomIdText;
    [SerializeField] List<PlayersListEntry> playersEntries;

    private GameManagerDeathmatch gameManager;

    private void OnEnable()
    {      
        SetupInfoPanel();    
    }


    private void SetupInfoPanel() {
        gameManager = GameManagersHolder.Instance.GameManagerDeathmatch;
        List<PlayerData> playersData = gameManager.GetPlayersDataList();       
        for (int i = 0; i < 6; i++) {
            if (i < playersData.Count)
            {
                playersEntries[i].ShowEntry(playersData[i]);
            }
            else {
                playersEntries[i].HideEntry();
            }
        }

        roomIdText.text = Localisation.GetString("GameInfo") + " " + PhotonNetwork.room.Name;
    }


    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        SetupInfoPanel();
    }
}

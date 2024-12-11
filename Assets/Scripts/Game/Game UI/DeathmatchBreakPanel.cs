using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathmatchBreakPanel : MonoBehaviour {

    [SerializeField] Text whoWinText;   
    [SerializeField] Text breakTimer;    
    [SerializeField] Text gameNumber;
    [SerializeField] List<PlayersListEntry> playersEntries;
     

    private void OnEnable()
    {           
        EventManager.BreakTimerUpdate += OnBreakTimerUpdate;
        SetupBreakePanel();
    }

    private void OnDisable()
    {
        EventManager.BreakTimerUpdate -= OnBreakTimerUpdate;
    }   

    public void SetupBreakePanel() {
        GameManagerDeathmatch gameManager = GameManagersHolder.Instance.GameManagerDeathmatch;
        List<PlayerData> playersData = gameManager.GetPlayersDataList();
        PlayerData winner = gameManager.FindWinner(playersData);
        this.gameObject.SetActive(true);           
        for (int i = 0; i < 6; i++) {
            if (i < playersData.Count) {
                playersEntries[i].ShowEntry(playersData[i]);
            }
            else {
                playersEntries[i].HideEntry();
            }
        }       

        if (winner != null)
        {          
            whoWinText.text = Localisation.GetString("PlayerWin").Replace("%", winner.PlayerName);
        }
        else {
            whoWinText.text = Localisation.GetString("NoWinner");
        }
        
        breakTimer.text = Localisation.GetString("NextRound").Replace("#", string.Format("{0}:{1:00}", 0, 0));               
        gameNumber.text = Localisation.GetString("GameNumber") + " " + PhotonNetwork.room.Name;
    }

    public void HideBreakPanel() {
        this.gameObject.SetActive(false);
    }

    private void OnBreakTimerUpdate(int timeSec) {
        if (PhotonNetwork.room == null) {
            return;
        }

        int minutes = timeSec / 60;
        int seconds = timeSec - minutes * 60;
        breakTimer.text = Localisation.GetString("NextRound").Replace("%", PhotonNetwork.room.Name).Replace("#", string.Format("{0}:{1:00}", minutes, seconds));        
    }  
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTFBreakPanel : Photon.PunBehaviour {

    [SerializeField] Text whoWinText;
    [SerializeField] Text breakTimer;
    [SerializeField] Text gameNumber;
    [SerializeField] Text redTeamWins;
    [SerializeField] Text blueTeamWins;
    [SerializeField] Text redScore;
    [SerializeField] Text blueScore;
    [SerializeField] List<TeamPlayerListEntry> redPlayerList;
    [SerializeField] List<TeamPlayerListEntry> bluePlayerList;

    private GameManagerTeamDeathmatch gameManager;

    private void OnEnable()
    {
        gameManager = GameManagersHolder.Instance.GameManagerTeamDeathmatch;
        EventManager.BreakTimerUpdate += OnBreakTimerUpdate;
        SetupBreakePanel();
    }

    private void OnDisable()
    {
        EventManager.BreakTimerUpdate -= OnBreakTimerUpdate;
    }

    public void SetupBreakePanel()
    {
        this.gameObject.SetActive(true);
        SetupWhoWinText();
        SetupTeamWinsText();
        SetupScore();
        SetupRedPlayerList();
        SetupBluePlayerList();
        SetupNextRoundText();
        gameNumber.text = Localisation.GetString("GameNumber") + " " + PhotonNetwork.room.Name;
    }

    private void SetupWhoWinText()
    {
        if (gameManager.RedTeamScore > gameManager.BlueTeamScore)
            whoWinText.text = Localisation.GetString("RedWin");
        else if (gameManager.RedTeamScore < gameManager.BlueTeamScore)
            whoWinText.text = Localisation.GetString("BlueWin");
        else
            whoWinText.text = Localisation.GetString("Draw");
    }

    private void SetupTeamWinsText()
    {
        redTeamWins.text = Localisation.GetString("Wins") + " " + gameManager.RedTeamWins;
        blueTeamWins.text = Localisation.GetString("Wins") + " " + gameManager.BlueTeamWins;
    }

    private void SetupScore()
    {
        redScore.text = gameManager.RedTeamScore.ToString();
        blueScore.text = gameManager.BlueTeamScore.ToString();
    }

    private void SetupRedPlayerList()
    {
        List<PlayerData> redPlayers = gameManager.GetRedPlayersDataList();
        for (int i = 0; i < 6; i++)
        {
            if (i < redPlayers.Count)
            {
                redPlayerList[i].ShowEntry(redPlayers[i]);
            }
            else
            {
                redPlayerList[i].HideEntry();
            }
        }

    }

    private void SetupBluePlayerList()
    {
        List<PlayerData> bluePlayers = gameManager.GetBluePlayersDataList();
        for (int i = 0; i < 6; i++)
        {
            if (i < bluePlayers.Count)
            {
                bluePlayerList[i].ShowEntry(bluePlayers[i]);
            }
            else
            {
                bluePlayerList[i].HideEntry();
            }
        }
    }

    private void SetupNextRoundText()
    {
        breakTimer.text = Localisation.GetString("NextRound").Replace("#", string.Format("{0}:{1:00}", 0, 0));
    }

    public void HideBreakPanel()
    {
        this.gameObject.SetActive(false);
    }

    private void OnBreakTimerUpdate(int timeSec)
    {
        if (PhotonNetwork.room == null)
        {
            return;
        }

        int minutes = timeSec / 60;
        int seconds = timeSec - minutes * 60;
        breakTimer.text = Localisation.GetString("NextRound").Replace("%", PhotonNetwork.room.Name).Replace("#", string.Format("{0}:{1:00}", minutes, seconds));
    }
}

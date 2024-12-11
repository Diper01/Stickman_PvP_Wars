using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTFMatchInfo : Photon.PunBehaviour
{
    [SerializeField] Text roomIdText;
    [SerializeField] Text redTeamWins;
    [SerializeField] Text blueTeamWins;
    [SerializeField] Text redTeamScore;
    [SerializeField] Text blueTeamScore;
    [SerializeField] List<TeamPlayerListEntry> redPlayersEntries;
    [SerializeField] List<TeamPlayerListEntry> bluePlayersEntries;

    private GameManagerTeamDeathmatch gameManager;

    private void OnEnable()
    {
        roomIdText.text = Localisation.GetString("GameInfo") + " " + PhotonNetwork.room.Name;
        SetupPlayersList();
        SetupTeamWins();
        SetupTeamScore();
    }

    private void SetupPlayersList()
    {
        gameManager = GameManagersHolder.Instance.GameManagerTeamDeathmatch;
        List<PlayerData> redPlayers = gameManager.GetRedPlayersDataList();
        List<PlayerData> bluePlayers = gameManager.GetBluePlayersDataList();

        for (int i = 0; i < 6; i++)
        {
            if (i < redPlayers.Count)
            {
                redPlayersEntries[i].ShowEntry(redPlayers[i]);
            }
            else
            {
                redPlayersEntries[i].HideEntry();
            }
        }

        for (int i = 0; i < 6; i++)
        {
            if (i < bluePlayers.Count)
            {
                bluePlayersEntries[i].ShowEntry(bluePlayers[i]);
            }
            else
            {
                bluePlayersEntries[i].HideEntry();
            }
        }

        roomIdText.text = Localisation.GetString("GameInfo") + " " + PhotonNetwork.room.Name;
    }

    private void SetupTeamWins()
    {
        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.RedTeamWins))
            redTeamWins.text = Localisation.GetString("Wins") + " " + (int)PhotonNetwork.room.CustomProperties[RoomProperty.RedTeamWins];
        else
            redTeamWins.text = Localisation.GetString("Wins") + " 0";

        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BlueTeamWins))
            blueTeamWins.text = Localisation.GetString("Wins") + " " + (int)PhotonNetwork.room.CustomProperties[RoomProperty.BlueTeamWins];
        else
            blueTeamWins.text = Localisation.GetString("Wins") + " 0";
    }

    private void SetupTeamScore()
    {
        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.RedTeamScore))
            redTeamScore.text = ((int)PhotonNetwork.room.CustomProperties[RoomProperty.RedTeamScore]).ToString();
        else
            redTeamScore.text = "0";

        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BlueTeamScore))
            blueTeamScore.text = ((int)PhotonNetwork.room.CustomProperties[RoomProperty.BlueTeamScore]).ToString();
        else
            blueTeamScore.text = "0";
    }


    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        SetupPlayersList();
    }

    public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        SetupTeamWins();
        SetupTeamScore();
    }
}

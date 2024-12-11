using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class SelectTeam : Photon.PunBehaviour {
    [SerializeField] Text header;
    [SerializeField] Text roundStartTimer;
    [SerializeField] Text redMapWins;
    [SerializeField] Text blueMapWins;
    [SerializeField] GameObject teamListEntryPref;
    [SerializeField] RectTransform redTeamListHolder;
    [SerializeField] RectTransform blueTeamListHolder;
    [SerializeField] GameObject selectRedButton;
    [SerializeField] GameObject selectBlueButton;

    private void Start()
    {
        SetupTeamWins();
        SeupPlayersLists();
        SetupSelectButtons();
    }

    private void OnEnable()
    {
        EventManager.TeamSelectionTimerUpdate += OnTimerUpdate;   
    }

    private void OnDisable()
    {
        EventManager.TeamSelectionTimerUpdate -= OnTimerUpdate;
    }

    private void SeupPlayersLists() {
        foreach (Transform child in redTeamListHolder) {
            Destroy(child.gameObject);
        }
        foreach (Transform child in blueTeamListHolder) {
            Destroy(child.gameObject);
        }

        foreach (var player in PhotonNetwork.playerList) {
            GameObject teamListEntryGO;
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team)
                && ((Team)player.CustomProperties[PlayerProperties.Team]) == Team.RED)
            {
                teamListEntryGO = GameObject.Instantiate(teamListEntryPref, redTeamListHolder);               
            }
            else {
                teamListEntryGO = GameObject.Instantiate(teamListEntryPref, blueTeamListHolder);
            }

            TeamListEntry teamListEntry = teamListEntryGO.GetComponent<TeamListEntry>();
            teamListEntry.SetPlayerName(player.NickName);
            if (player.ID == PhotonNetwork.player.ID) {
                teamListEntry.IsHighlighted = true;
            }
        }

        Dictionary<int, string> bots = BotsInformation.GetBotsDictionary();
        foreach (int botId in bots.Keys)
        {
            GameObject teamListEntryGO;

            if (BotsInformation.GetBotTeam(botId) == Team.RED)
            {
                teamListEntryGO = GameObject.Instantiate(teamListEntryPref, redTeamListHolder);
            }
            else {
                teamListEntryGO = GameObject.Instantiate(teamListEntryPref, blueTeamListHolder);
            }

            TeamListEntry teamListEntry = teamListEntryGO.GetComponent<TeamListEntry>();
            teamListEntry.SetPlayerName(bots[botId]);          
        }
    }

    private void SetupTeamWins()
    {
        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.RedTeamWins))
            redMapWins.text = Localisation.GetString("Wins") + " " + (int)PhotonNetwork.room.CustomProperties[RoomProperty.RedTeamWins];
        else
            redMapWins.text = Localisation.GetString("Wins") + " 0";

        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BlueTeamWins))
            blueMapWins.text = Localisation.GetString("Wins") + " " + (int)PhotonNetwork.room.CustomProperties[RoomProperty.BlueTeamWins];
        else
            blueMapWins.text = Localisation.GetString("Wins") + " 0";
    }

    private void SetupSelectButtons() {
        if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.Private)
            && (bool)PhotonNetwork.room.CustomProperties[RoomProperty.Private] == true)
        {
            header.gameObject.SetActive(true);
            selectRedButton.SetActive(true);
            selectBlueButton.SetActive(true);
        }
        else {
            header.gameObject.SetActive(false);
            selectRedButton.SetActive(false);
            selectBlueButton.SetActive(false);
        }
    }

    public override void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        SeupPlayersLists();
    }

    public override void OnPhotonCustomRoomPropertiesChanged(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        SeupPlayersLists();
    }

    public void OnSelectRedTeam() {
        int blueCount = 0;
        foreach (var player in PhotonNetwork.playerList) {
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team) && (Team)player.CustomProperties[PlayerProperties.Team] == Team.BLUE)
                blueCount++;
        }
        foreach (var botId in BotsInformation.GetBotsDictionary().Keys) {
            if (BotsInformation.GetBotTeam(botId) == Team.BLUE)
                blueCount++;
        }
        if (blueCount <= 1)
            return;

        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties.Add(PlayerProperties.Team, Team.RED);
        PhotonNetwork.player.SetCustomProperties(playerProperties);
    }

    public void OnSelectBlueTeam() {
        int redCount = 0;
        foreach (var player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team) && (Team)player.CustomProperties[PlayerProperties.Team] == Team.RED)
                redCount++;
        }
        foreach (var botId in BotsInformation.GetBotsDictionary().Keys)
        {
            if (BotsInformation.GetBotTeam(botId) == Team.RED)
                redCount++;
        }
        if (redCount <= 1)
            return;

        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties.Add(PlayerProperties.Team, Team.BLUE);
        PhotonNetwork.player.SetCustomProperties(playerProperties);
    }

    private void OnTimerUpdate(int timeSec)
    {
        roundStartTimer.text = Localisation.GetString("RoundStars").Replace("%", timeSec.ToString());
    }

}

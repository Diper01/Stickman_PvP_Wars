using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class RoomLobby : Photon.PunBehaviour {
    [SerializeField] GameObject loadingPanel;
    [SerializeField] Text descriptionText;
    [SerializeField] Text gameTypeText;
    [SerializeField] Text roomIdText;
    [SerializeField] Button startButton;
    [SerializeField] MatchmakingMenu menu;
    [SerializeField] GameObject mapsListEntryPref;
    [SerializeField] GameObject mapsQueueHolder;

    private string publicRoomDescription;
    private string privateRoomDescription;
    private GameType gameType;
    private GameMode gameMode;
    private List<MapQueueEntry> mapQueueu;
    private bool isPrivate = false;
    private float botTime = 1f;
    private float startSearching;
    private int playersToStartGame = 4;

    private void OnEnable()
    {
        SetupRoomLobby();       
    }   

    private void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (isPrivate)
            {
                startButton.interactable = PhotonNetwork.room.PlayerCount >= 2 ? true : false;
            }
            else
            {
                AddBotUpdate();
                if ((PhotonNetwork.room.PlayerCount + BotsInformation.GetBotsDictionary().Count) >= playersToStartGame)
                {
                    OnStartRoomButton();
                }
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (PhotonNetwork.room != null && PhotonNetwork.isMasterClient && PhotonNetwork.room.PlayerCount > 1)
            {
                PhotonNetwork.SetMasterClient(PhotonNetwork.masterClient.GetNext());
                PhotonNetwork.SendOutgoingCommands();
            }
        }
    }

    private void SetupRoomLobby()
    {
        BotsInformation.Reset();
        RefreshBotTimer();       
        mapQueueu = MapQueue.StringToList((string)PhotonNetwork.room.CustomProperties[RoomProperty.MapQueue]);
        gameType = (GameType)PhotonNetwork.room.CustomProperties[RoomProperty.GameType];
        gameMode = (GameMode)PhotonNetwork.room.CustomProperties[RoomProperty.GameMode];
        isPrivate = (bool)PhotonNetwork.room.CustomProperties[RoomProperty.Private];
        string roomName = PhotonNetwork.room.Name;

        if (gameType == GameType.FFA)
            gameTypeText.text = Localisation.GetString("FFA");
        else if(gameMode == GameMode.DEATHMATCH)
            gameTypeText.text = Localisation.GetString("Deathmatch");
        else if(gameMode == GameMode.CAPTURE_THE_FLAG)
            gameTypeText.text = Localisation.GetString("CTF");

        publicRoomDescription = Localisation.GetString("PublicGameCondition");
        privateRoomDescription = Localisation.GetString("PrivateGameCondition");

        descriptionText.text = isPrivate ? privateRoomDescription : publicRoomDescription;
        roomIdText.text = Localisation.GetString("GameNumber") + " " + roomName;

        if (isPrivate && PhotonNetwork.isMasterClient)
            startButton.gameObject.SetActive(true);
        else
            startButton.gameObject.SetActive(false);
        SetupMapsQueue();
        if (PhotonNetwork.isMasterClient)
        {
            SetNewPlayerTeam(PhotonNetwork.player);
        }
    }

    private void SetupMapsQueue()
    {
        foreach (Transform child in mapsQueueHolder.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var entry in mapQueueu)
        {
            GameObject mapListEntry = GameObject.Instantiate(mapsListEntryPref, mapsQueueHolder.transform);
            mapListEntry.GetComponentInChildren<Text>().text = GetLocalizedMapName(entry.Map);
        }
    }

    private string GetLocalizedMapName(Maps map)
    {
        switch (map)
        {
            case Maps.CITY_FFA:
                return Localisation.GetString("CityMap");
            case Maps.JUNGLE_FFA:
                return Localisation.GetString("JungleMap");
            case Maps.FACTORY_FFA:
                return Localisation.GetString("FactoryMap");
            case Maps.UNDERWATER_TEAM:
                return Localisation.GetString("UnderwaterBase");
            case Maps.ROCKET_TEAM:
                return Localisation.GetString("RocketBase");
            case Maps.FACTORY_TEAM:
                return Localisation.GetString("FactoryMap");
            case Maps.CITY_CTF:
                return Localisation.GetString("CityMap");
            case Maps.JUNGLE_CTF:
                return Localisation.GetString("JungleMap");
            case Maps.UNDERWATER_CTF:
                return Localisation.GetString("UnderwaterBase");
            default:
                return "";
        }
    }

    private void AddBotUpdate()
    {
        if (Time.time - startSearching > botTime)
        {
            int botId = BotsInformation.AddBot();
            SetNewBotTeam(botId);
            RefreshBotTimer();           
        }
    }

    private void RefreshBotTimer()
    {
        //botTime = UnityEngine.Random.Range(5f, 10f);
        startSearching = Time.time;
    }

    private void SetNewPlayerTeam(PhotonPlayer newPlayer) {
        if ((GameType)PhotonNetwork.room.CustomProperties[RoomProperty.GameType] == GameType.FFA)
            return;

        int redCount = 0;
        int blueCount = 0;
        foreach (var player in PhotonNetwork.playerList) {
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team))
            {
                if ((Team)player.CustomProperties[PlayerProperties.Team] == Team.RED)
                    redCount++;
                else
                    blueCount++;
            }
        }
        foreach (var botId in BotsInformation.GetBotsDictionary().Keys) {
            if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BotTeam + botId))
            {
                if((Team)PhotonNetwork.room.CustomProperties[RoomProperty.BotTeam + botId] == Team.RED)
                    redCount++;
                else
                    blueCount++;
            }
        }      
        if (redCount < blueCount)
        {
            ExitGames.Client.Photon.Hashtable playerProp = new ExitGames.Client.Photon.Hashtable();
            playerProp.Add(PlayerProperties.Team, Team.RED);
            newPlayer.SetCustomProperties(playerProp);          
        }
        else
        {
            ExitGames.Client.Photon.Hashtable playerProp = new ExitGames.Client.Photon.Hashtable();
            playerProp.Add(PlayerProperties.Team, Team.BLUE);
            newPlayer.SetCustomProperties(playerProp);           
        }     
    }

    private void SetNewBotTeam(int newBotIt) {
        if ((GameType)PhotonNetwork.room.CustomProperties[RoomProperty.GameType] == GameType.FFA)
            return;

        int redCount = 0;
        int blueCount = 0;
        foreach (var player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team))
            {
                if ((Team)player.CustomProperties[PlayerProperties.Team] == Team.RED)
                    redCount++;
                else
                    blueCount++;
            }
        }
        foreach (var botId in BotsInformation.GetBotsDictionary().Keys)
        {
            if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BotTeam + botId))
            {
                if ((Team)PhotonNetwork.room.CustomProperties[RoomProperty.BotTeam + botId] == Team.RED)
                    redCount++;
                else
                    blueCount++;
            }
        }

        if (redCount < blueCount)
        {
            ExitGames.Client.Photon.Hashtable roomProp = new ExitGames.Client.Photon.Hashtable();
            roomProp.Add(RoomProperty.BotTeam + newBotIt, Team.RED);
            PhotonNetwork.room.SetCustomProperties(roomProp);
        }
        else
        {
            ExitGames.Client.Photon.Hashtable roomProp = new ExitGames.Client.Photon.Hashtable();
            roomProp.Add(RoomProperty.BotTeam + newBotIt, Team.BLUE);
            PhotonNetwork.room.SetCustomProperties(roomProp);
        }
    }


    private void ClearPlayerData()
    {
        ExitGames.Client.Photon.Hashtable playerProp = new ExitGames.Client.Photon.Hashtable();
        foreach (var key in PhotonNetwork.player.CustomProperties.Keys)
        {
            playerProp.Add(key, null);
        }
        PhotonNetwork.player.SetCustomProperties(playerProp);
    }

    public void OnLeaveRoomButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        ClearPlayerData();
        PhotonNetwork.LeaveRoom();
        menu.ShowTabsPanle();
    }

    public void OnStartRoomButton()
    {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        menu.ShowLoading();
        PhotonNetwork.LoadLevel(ScenesIndexes.GetMapSceneIndex(mapQueueu[0].Map));       
    }

    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (PhotonNetwork.player.ID == newMasterClient.ID)
        {
            startButton.gameObject.SetActive(true);
        }
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (PhotonNetwork.isMasterClient)
        {
            SetNewPlayerTeam(newPlayer);
        }
    }
}

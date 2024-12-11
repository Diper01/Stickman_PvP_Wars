using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class FindRoomTab : Photon.PunBehaviour {
    [SerializeField] Dropdown gameTypeDropdown;
    [SerializeField] MatchmakingMenu menu;

    private int SelectedGameType {
        get {
            return PlayerPrefs.GetInt("FindRoomGameType", 0);
        }
        set {
            PlayerPrefs.SetInt("FindRoomGameType", value);
        }
    }
    private List<MapQueueEntry> defaultFFAMapQueue;
    private List<MapQueueEntry> defaultDeathmatchMapQueue;
    private List<MapQueueEntry> defaultCTFMapQueue;
    private List<string> gameTypes = new List<string>();

    private void Start()
    {
        defaultFFAMapQueue = new List<MapQueueEntry>() { new MapQueueEntry() { Map = Maps.CITY_FFA, Mode = GameMode.DEATHMATCH}
            ,new MapQueueEntry() { Map = Maps.JUNGLE_FFA, Mode = GameMode.DEATHMATCH}
            ,new MapQueueEntry() { Map = Maps.FACTORY_FFA, Mode = GameMode.DEATHMATCH}};
        defaultDeathmatchMapQueue = new List<MapQueueEntry>() { new MapQueueEntry() { Map = Maps.UNDERWATER_TEAM, Mode = GameMode.DEATHMATCH}
            ,new MapQueueEntry() { Map = Maps.ROCKET_TEAM, Mode = GameMode.DEATHMATCH}
            ,new MapQueueEntry() { Map = Maps.FACTORY_TEAM, Mode = GameMode.DEATHMATCH}};
        defaultCTFMapQueue = new List<MapQueueEntry>() { new MapQueueEntry() { Map = Maps.CITY_CTF, Mode = GameMode.CAPTURE_THE_FLAG }
            ,new MapQueueEntry() { Map = Maps.JUNGLE_CTF, Mode = GameMode.CAPTURE_THE_FLAG }
            ,new MapQueueEntry() { Map = Maps.UNDERWATER_CTF, Mode = GameMode.CAPTURE_THE_FLAG }};
        SetGameTypeDropDown();
    }

    private void SetGameTypeDropDown()
    {
        List<Dropdown.OptionData> gameTypeOptions = new List<Dropdown.OptionData>();
        gameTypes = new List<string>() { Localisation.GetString("FFA"), Localisation.GetString("Deathmatch"), Localisation.GetString("CTF") };
        foreach (string option in gameTypes)
        {
            gameTypeOptions.Add(new Dropdown.OptionData(option));
        }
        gameTypeDropdown.options = gameTypeOptions;
        gameTypeDropdown.value = SelectedGameType;
    }

    public void OnGameTypeDropdown()
    {
        SelectedGameType = gameTypeDropdown.value;
    }

    public void OnFindGameButton() {
        if (PhotonNetwork.connectedAndReady)
        {
            menu.ShowLoading();
            SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
            if (PhotonNetwork.offlineMode == false) {
                PhotonConnector.Instance.FindRoom(GetSelectedGameType(), GetSelectedGameMode());
            }
            else {
                StartDefaultRoom();
            }
        }
    }

    private GameType GetSelectedGameType() {   
        if (SelectedGameType == 0) 
            return GameType.FFA;                   
        else
            return GameType.TEAMS;
    }

    private GameMode GetSelectedGameMode() {
        if (SelectedGameType == 0 || SelectedGameType == 1)
            return GameMode.DEATHMATCH;
        else
            return GameMode.CAPTURE_THE_FLAG;
    }

    private string GetDefaultMapQueue() {
        if (SelectedGameType == 0)
            return MapQueue.ListToString(defaultFFAMapQueue);
        else if (SelectedGameType == 1)
            return MapQueue.ListToString(defaultDeathmatchMapQueue);
        else
            return MapQueue.ListToString(defaultCTFMapQueue);
    }

    private void StartDefaultRoom() {
        string roomId = PhotonConnector.Instance.CreateNewRoomId();
        GameType gameType = GetSelectedGameType();
        GameMode gameMode = GetSelectedGameMode();
        string mapQueue = GetDefaultMapQueue();
        PhotonConnector.Instance.CreateRooom(roomId, mapQueue, gameType, gameMode, false);
    }

    #region PHOTON CALLBACKS


    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        print("find random room failed");
        StartDefaultRoom();
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        print("create room failed");
        menu.ShowTabsPanle();
    }

    public override void OnJoinedRoom()
    {
        menu.ShowRoomLobby();
    }

    #endregion

}

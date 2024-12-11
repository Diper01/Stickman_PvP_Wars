using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CreateRoomTab : Photon.PunBehaviour {
    [SerializeField] MatchmakingMenu menu;
    [SerializeField] Dropdown gameTypeDropdown;
    [SerializeField] Dropdown mapDropdown;
    [SerializeField] GameObject mapListEntryPref;
    [SerializeField] Transform mapQueueHolder;
    [SerializeField] GameObject mapQueueDescritionText;
    [SerializeField] Button createButton;
    [SerializeField] Toggle privateToggle;
    [SerializeField] Toggle friendlyFireToggle;
    [SerializeField] Button AddMapButton;


    private int SelectedGameType
    {
        get
        {
            return PlayerPrefs.GetInt("CreateRoomGameType", 0);
        }
        set
        {
            PlayerPrefs.SetInt("CreateRoomGameType", value);
        }
    }
    private bool PrivateToggleValue {
        get {
            return Convert.ToBoolean(PlayerPrefs.GetInt("PrivateToggle", 0));
        }
        set {
            PlayerPrefs.SetInt("PrivateToggle", Convert.ToInt32(value));
        }
    }
    private bool FriendlyFireToggleValue {
        get {
            return Convert.ToBoolean(PlayerPrefs.GetInt("FriendlyFireToggle", 0));
        }
        set {
            PlayerPrefs.SetInt("FriendlyFireToggle", Convert.ToInt32(value));
        }
    }


    private List<MapQueueEntry> mapQueueList;
    private int selectedMap = 0;
    private System.Random rand;
    private int maxMapQueueCount = 7;
    private List<string> gameTypes = new List<string>();
    private List<Maps> mapsFFA = new List<Maps>() { Maps.CITY_FFA, Maps.JUNGLE_FFA, Maps.FACTORY_FFA };
    private List<Maps> mapsDeathmatch = new List<Maps>() { Maps.UNDERWATER_TEAM, Maps.ROCKET_TEAM, Maps.FACTORY_TEAM};
    private List<Maps> mapsCTF = new List<Maps>() { Maps.CITY_CTF, Maps.JUNGLE_CTF, Maps.UNDERWATER_CTF };

    private void Awake()
    {
        mapQueueList = new List<MapQueueEntry>();
        rand = new System.Random();
        privateToggle.isOn = PrivateToggleValue;
        friendlyFireToggle.isOn = FriendlyFireToggleValue;
        SetGameTypeDropDown();      
        SetupMapDropDown();
        ReDrawMapQueue();
        SetupFriendlyFireToggle();
    }

    private void SetGameTypeDropDown() {
        List<Dropdown.OptionData> gameTypeOptions = new List<Dropdown.OptionData>();
        gameTypes = new List<string>() { Localisation.GetString("FFA"), Localisation.GetString("Deathmatch"), Localisation.GetString("CTF") };
        foreach (string option in gameTypes)
        {
            gameTypeOptions.Add(new Dropdown.OptionData(option));
        }
        gameTypeDropdown.options = gameTypeOptions;
        gameTypeDropdown.value = SelectedGameType;               
    }
   
    private void SetupMapDropDown() {
        List<Dropdown.OptionData> mapOptions = new List<Dropdown.OptionData>();
        switch (SelectedGameType)
        {
            case 0:
                foreach (Maps map in mapsFFA) {
                    mapOptions.Add(new Dropdown.OptionData(GetLocalizedMapName(map)));
                }
                break;
            case 1:
                foreach (Maps map in mapsDeathmatch) {
                    mapOptions.Add(new Dropdown.OptionData(GetLocalizedMapName(map)));
                }
                break;
            case 2:
                foreach (Maps map in mapsCTF) {
                    mapOptions.Add(new Dropdown.OptionData(GetLocalizedMapName(map)));
                }
                break;
        }
        
        mapDropdown.options = mapOptions;
        mapDropdown.value = selectedMap;
    }

    private void SetupFriendlyFireToggle() {
        if (gameTypeDropdown.value == 0)
        {
            friendlyFireToggle.gameObject.SetActive(false);
        }
        else 
        {
            friendlyFireToggle.gameObject.SetActive(true);
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


    private MapListUIEntry CreateMapEntry(MapQueueEntry entry) {
        GameObject mapListEntryGO = Instantiate(mapListEntryPref, mapQueueHolder);
        MapListUIEntry mapListUIEntry = mapListEntryGO.GetComponent<MapListUIEntry>();
        mapListUIEntry.RemoveButtonClick += OnRemoveMap;
        mapListUIEntry.SetupEntry(entry);
        return mapListUIEntry;
    }

    private void AddMapQueueEntry(MapQueueEntry entry) {
        mapQueueList.Add(entry);
        ReDrawMapQueue();
    }

    private void RemoveMapQueueEntry(int index) {      
        mapQueueList.RemoveAt(index);
        ReDrawMapQueue();
    }

    private void ReDrawMapQueue() {
        foreach (Transform childrem in mapQueueHolder) {
            Destroy(childrem.gameObject);
        }

        if (mapQueueList.Count > 0)
        {
            mapQueueDescritionText.SetActive(false);           
            createButton.interactable = true;
            for (int i = 0; i < mapQueueList.Count; i++)
            {
                MapListUIEntry mapListUIEntry = CreateMapEntry(mapQueueList[i]);
                mapListUIEntry.EntryIndex = i;
            }
        }
        else {           
            createButton.interactable = false;
            mapQueueDescritionText.SetActive(true);
        }
    }

    private GameType GetSelectedGameType()
    {
        if (SelectedGameType == 0)
            return GameType.FFA;
        else
            return GameType.TEAMS;
    }

    private GameMode GetSelectedGameMode()
    {
        if (SelectedGameType == 0 || SelectedGameType == 1)
            return GameMode.DEATHMATCH;
        else
            return GameMode.CAPTURE_THE_FLAG;
    }

    private Maps GetSelectedMap() {      
        switch (SelectedGameType)
        {
            case 0:
                return GetSelectedMapFFA();               
            case 1:
                return GetSelectedMapDeathmatch();
            case 2:
                return GetSelectedMapCTF();
        }
        return Maps.CITY_FFA;
    }

    private Maps GetSelectedMapFFA() {
        switch (selectedMap)
        {
            case 0:
                return Maps.CITY_FFA;              
            case 1:
                return Maps.JUNGLE_FFA;
            case 2:
                return Maps.FACTORY_FFA;
        }
        return Maps.CITY_FFA;
    }

    private Maps GetSelectedMapDeathmatch()
    {
        switch (selectedMap)
        {
            case 0:
                return Maps.UNDERWATER_TEAM;
            case 1:
                return Maps.ROCKET_TEAM;
            case 2:
                return Maps.FACTORY_TEAM;    
        }
        return Maps.UNDERWATER_TEAM;
    }

    private Maps GetSelectedMapCTF()
    {
        switch (selectedMap)
        {
            case 0:
                return Maps.CITY_CTF;
            case 1:
                return Maps.JUNGLE_CTF;
            case 2:
                return Maps.UNDERWATER_CTF;
        }
        return Maps.CITY_FFA;
    }


    #region UI HANDLERS
    public void OnGameTypeDropdown() {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        if (SelectedGameType != gameTypeDropdown.value) {
            SelectedGameType = gameTypeDropdown.value;
            selectedMap = 0;
            mapQueueList.Clear();
            ReDrawMapQueue();
            SetupMapDropDown();
            SetupFriendlyFireToggle();
        }          
    }
   
    public void OnMapDropDown() {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        selectedMap = mapDropdown.value;
    }

    public void OnAddMap() {
        if (mapQueueList.Count >= maxMapQueueCount)
            return;

        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);

        GameMode gameMode = GetSelectedGameMode();
        Maps map = GetSelectedMap();       
        MapQueueEntry entry = new MapQueueEntry() { Map = map, Mode = gameMode };
        AddMapQueueEntry(entry);
    }

    public void OnRemoveMap(int index) {
        SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
        RemoveMapQueueEntry(index);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(AddMapButton.gameObject);
    }

    public void OnCreateButton() {
        if (PhotonNetwork.connectedAndReady)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
            menu.ShowLoading();
            GameType gameType = GetSelectedGameType();
            GameMode gameMode = GetSelectedGameMode();
            string roomId = PhotonConnector.Instance.CreateNewRoomId();
            string mapQueue = MapQueue.ListToString(mapQueueList);
            bool isPrivate = privateToggle.isOn;
            bool friendlyFire = friendlyFireToggle.isOn;
            PhotonConnector.Instance.CreateRooom(roomId, mapQueue, gameType, gameMode, isPrivate, friendlyFire);
        }
    }

    public void OnPrivateToggle() {
        PrivateToggleValue = privateToggle.isOn;
    }

    public void OnFriendlyFireToggle() {
        FriendlyFireToggleValue = friendlyFireToggle.isOn;
    }
       
    #endregion

    #region PHOTON CALLBACKS

    public override void OnCreatedRoom()
    {
        print("Room created");
        menu.ShowRoomLobby();
    }

    public override void OnJoinedRoom()
    {
        print("Room joined");
        menu.ShowRoomLobby();
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        print("Failed to create room in create room tab");
        menu.ShowTabsPanle();
    }


    #endregion
}

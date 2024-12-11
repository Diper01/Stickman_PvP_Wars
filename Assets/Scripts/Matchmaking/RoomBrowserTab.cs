using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomBrowserTab : Photon.PunBehaviour {
    [SerializeField] GameObject roomListEntryPref;
    [SerializeField] GameObject listHolder;
    [SerializeField] Button joinRoomButton;
    [SerializeField] Button findGameButton;
    [SerializeField] InputField roomIdInputField;
    [SerializeField] GameObject roomLobyPanel;
    [SerializeField] MatchmakingMenu menu;
    [SerializeField] Text gameNotFound;

    private List<RoomListEntry> listEntrys;
    private string selectedEntryId = "";
    private bool showSearchResult = false;

    #region UNITY CALLBACKS

    private void OnEnable()
    {
        UpdateRoomList();
    }

    private void OnDisable()
    {
        foreach (var entry in listEntrys)
        {
            entry.OnClick -= OnListEntryClick;
        }
    }

    private void Update()
    {
        SetJoinButtonInteractable();
    }

    private void SetJoinButtonInteractable()
    {
        if (selectedEntryId != "")
        {
            RoomInfo selectedRoom = null;
            foreach (RoomInfo room in PhotonNetwork.GetRoomList())
            {
                if (selectedEntryId == room.Name)
                {
                    selectedRoom = room;
                }
            }

            if (selectedRoom == null)
                return;

            if (selectedRoom.PlayerCount < PhotonConnector.Instance.MaxPlayersPerRoom)
                joinRoomButton.interactable = true;
            else
                joinRoomButton.interactable = false;
        }
        else
        {
            joinRoomButton.interactable = false;
        }
    }

    #endregion

    #region ROOMS LIST

    private void OnListEntryClick(string roomId)
    {
        foreach (RoomListEntry entry in listEntrys)
        {
            if (roomId == entry.RoomId)
            {
                entry.IsHighlighted = true;
                selectedEntryId = roomId;
            }
            else
            {
                entry.IsHighlighted = false;
            }
        }
    }

    private void UpdateRoomList()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(findGameButton.gameObject);
        gameNotFound.gameObject.SetActive(false);
        CleanRoomsList();

        if (showSearchResult)
            ShowSearchResult();
        else
            ShowAllPublickRooms();
    }

    private void ShowSearchResult()
    {
        if (roomIdInputField.text == "")
        {
            showSearchResult = false;
            ShowAllPublickRooms();
            return;
        }

        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            if (roomIdInputField.text == room.Name)
            {
                RoomListEntry entry = CreateRoomListEntry(room);
                listEntrys.Add(entry);
                OnListEntryClick(entry.RoomId);
                break;
            }
        }
        if (listEntrys.Count == 0)
        {
            gameNotFound.gameObject.SetActive(true);
            gameNotFound.text = Localisation.GetString("GameNotFound").Replace("#", roomIdInputField.text);
        }
    }

    private void ShowAllPublickRooms()
    {
        bool selectedRoomExist = false;
        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            if ((bool)room.CustomProperties[RoomProperty.Private])
            {
                continue;
            }

            RoomListEntry entry = CreateRoomListEntry(room);
            listEntrys.Add(entry);

            if (selectedEntryId == entry.RoomId)
            {
                entry.IsHighlighted = true;
                selectedRoomExist = true;
            }
        }

        if (!selectedRoomExist)
        {
            selectedEntryId = "";
        }

        if (listEntrys.Count == 0)
        {
            gameNotFound.gameObject.SetActive(true);
            gameNotFound.text = Localisation.GetString("NoGames");
        }
    }

    private RoomListEntry CreateRoomListEntry(RoomInfo room)
    {
        var roomListEntry = GameObject.Instantiate(roomListEntryPref, listHolder.transform);
        RoomListEntry entry = roomListEntry.GetComponent<RoomListEntry>();
        entry.OnClick += OnListEntryClick;

        string gameType = GetRoomGameType(room);
        string roomId = room.Name;
        int connectedPlayers = room.PlayerCount;
        entry.SetEntryData(gameType, roomId, connectedPlayers);
        return entry;
    }

    private string GetRoomGameType(RoomInfo room) {
        GameType gameType = (GameType)room.CustomProperties[RoomProperty.GameType];
        GameMode gameMode = (GameMode)room.CustomProperties[RoomProperty.GameMode];

        if (gameType == GameType.FFA)        
            return Localisation.GetString("FFA");        
        else if (gameType == GameType.TEAMS && gameMode == GameMode.DEATHMATCH)        
            return Localisation.GetString("Deathmatch");        
        else 
            return Localisation.GetString("CTF");
    }

    private void CleanRoomsList()
    {
        listEntrys = new List<RoomListEntry>();
        foreach (Transform entry in listHolder.transform)
        {
            Destroy(entry.gameObject);
        }
    }

    #endregion

    #region PHOTON CALLBACKS

    public override void OnReceivedRoomListUpdate()
    {
        UpdateRoomList();
    }

    public override void OnJoinedRoom()
    {
        menu.ShowRoomLobby();
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        menu.ShowTabsPanle();
        print("Join room failed");
    }

    public override void OnLeftRoom()
    {
        menu.ShowTabsPanle();
    }

    #endregion

    #region BUTTONS HANDLERS

    public void OnJoinRoomButton()
    {
        if (PhotonNetwork.connectedAndReady)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
            menu.ShowLoading();
            PhotonConnector.Instance.JoinRoom(selectedEntryId);
        }
    }

    public void OnFindButton()
    {
        if (PhotonNetwork.connectedAndReady)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.ClipButtonClick);
            showSearchResult = true;
            UpdateRoomList();
        }
    }

    #endregion

}

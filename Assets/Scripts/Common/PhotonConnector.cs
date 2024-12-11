using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonConnector : Photon.PunBehaviour {

    public static PhotonConnector Instance { get; private set; }      

    public bool IsConnected
    {
        get { return PhotonNetwork.connectionState == ConnectionState.Connected; }
    }
    public byte MaxPlayersPerRoom { get; set; }     
 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Init() {
        DontDestroyOnLoad(this.gameObject);
        MaxPlayersPerRoom = 6;
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.autoCleanUpPlayerObjects = true;
        PhotonNetwork.sendRate = 7;
        PhotonNetwork.sendRateOnSerialize = 6;       
        Screen.sleepTimeout = SleepTimeout.NeverSleep;        
    }

    public void Connect() {
        try
        {          
            PhotonNetwork.ConnectToRegion(GameOptions.SelectedServer, Application.version);        
        }
        catch
        {
            Debug.LogWarning("Couldn't connect to server");
        }
    }
    
    public void FindRoom(GameType gameType, GameMode gameMode)
    {
        ClearPlayerData();
        ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable();
        expectedProperties.Add(RoomProperty.Private, false);
        expectedProperties.Add(RoomProperty.GameType, gameType);
        expectedProperties.Add(RoomProperty.GameMode, gameMode);
        PhotonNetwork.JoinRandomRoom(expectedProperties, MaxPlayersPerRoom);
    }

    public void JoinRoom(string roomId) {
        ClearPlayerData();
        PhotonNetwork.JoinRoom(roomId);
    }
   
    public void CreateRooom(string roomId,string mapQueue,GameType gameType, GameMode gameMode, bool isPrivate, bool friendlyFire = false)
    {
        ClearPlayerData();
        if (PhotonNetwork.connected)
        {
            RoomOptions roomOption = new RoomOptions();
            roomOption.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
            roomOption.MaxPlayers = MaxPlayersPerRoom;
            roomOption.CustomRoomProperties.Add(RoomProperty.MapQueue, mapQueue);
            roomOption.CustomRoomProperties.Add(RoomProperty.MapQueueIndex, 0);
            roomOption.CustomRoomProperties.Add(RoomProperty.GameType, gameType);
            roomOption.CustomRoomProperties.Add(RoomProperty.GameMode, gameMode);
            roomOption.CustomRoomProperties.Add(RoomProperty.Private, isPrivate);
            roomOption.CustomRoomProperties.Add(RoomProperty.FriendlyFire, friendlyFire);
            roomOption.CustomRoomPropertiesForLobby = new string[] {
                RoomProperty.GameType,
                RoomProperty.GameMode,
                RoomProperty.Private
            };

            PhotonNetwork.CreateRoom(roomId, roomOption, null);
        }
    }

    public string CreateNewRoomId() {
        string roomId;
        bool isIdExist = false;
        do
        {
            roomId = ((int)Random.Range(100000f, 999999f)).ToString();    
            foreach (var room in PhotonNetwork.GetRoomList())
            {
                if (room.Name == roomId) {
                    isIdExist = true;                    
                }
            }
        }while (isIdExist);        

        return roomId;
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

}

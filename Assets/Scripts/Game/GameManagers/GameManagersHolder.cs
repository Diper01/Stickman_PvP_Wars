using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagersHolder : MonoBehaviour {
    public static GameManagersHolder Instance;

    public GameType GameType { get; private set; }
    public GameMode GameMode { get; private set; }   
    public GameManagerDeathmatch GameManagerDeathmatch {
        get { return gameManagerDeathmatch; }
    }
    public GameManagerTeamDeathmatch GameManagerTeamDeathmatch {
        get { return gameManagerTeamDeathmatch; }
    }
    public GameManagerCTF GameManagerCTF {
        get { return gameManagerCTF; }
    }

    [SerializeField] GameManagerDeathmatch gameManagerDeathmatch;
    [SerializeField] GameManagerTeamDeathmatch gameManagerTeamDeathmatch;
    [SerializeField] GameManagerCTF gameManagerCTF;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        Instance = this;
        InitializeGameManager();
    }

    private void InitializeGameManager() {
        List<MapQueueEntry> mapList = MapQueue.StringToList((string)PhotonNetwork.room.CustomProperties[RoomProperty.MapQueue]);
        int currentMapIndex = (int)PhotonNetwork.room.CustomProperties[RoomProperty.MapQueueIndex];
        GameMode = mapList[currentMapIndex].Mode;
        GameType = (GameType)PhotonNetwork.room.CustomProperties[RoomProperty.GameType];

        if (GameType == GameType.FFA) {
            gameManagerDeathmatch.Initialize();
            Destroy(gameManagerTeamDeathmatch);
            Destroy(gameManagerCTF);           
        }
        else if (GameMode == GameMode.DEATHMATCH) {
            Destroy(gameManagerDeathmatch);         
            gameManagerTeamDeathmatch.Initialize();        
            Destroy(gameManagerCTF);
        }
        else if (GameMode == GameMode.CAPTURE_THE_FLAG) {
            Destroy(gameManagerDeathmatch);
            Destroy(gameManagerTeamDeathmatch);           
            gameManagerCTF.Initialize();
        }
    }

}

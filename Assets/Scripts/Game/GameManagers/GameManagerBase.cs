using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class GameManagerBase : Photon.PunBehaviour, IPunObservable {

    public GameMode Mode { get; set; }
    public int RoundTimer {
        get { return roundTimer; }
        set {
            if (roundTimer != value)
            {
                if (value >= 0)
                    roundTimer = value;
                else
                    roundTimer = 0;
                EventManager.OnRoundTimerUpdate(roundTimer);
            }
        }
    }
    public int BreakTimer {
        get { return breakTimer; }
        set
        {
            if (breakTimer != value)
            {
                if (value >= 0)
                    breakTimer = value;
                else
                    breakTimer = 0;
                EventManager.OnBreakTimerUpdate(breakTimer);
            }
        }
    }

    public GameObject playerPref;
    public GameObject playerBotPref;

    protected Map map;
    protected Player player;
    protected double roundStartTime= 0;
    protected double roundTime = 3 * 60;
    protected double breakTime = 20;
    protected System.Random rand;

    protected int roundTimer;
    protected int breakTimer;
    protected bool isInitialized = false;
    protected bool roundFinished = false;
    protected bool breakFinished = false;

    protected virtual void Update()
    {
        if (!isInitialized)
            return;

        UpdateTimer();
    }

    public virtual void Initialize() {
        isInitialized = true;
        rand = new System.Random(PhotonNetwork.player.ID + (int)PhotonNetwork.time);
        map = FindObjectOfType<Map>();
        if (PhotonNetwork.isMasterClient && roundStartTime == 0)
        {
            roundStartTime = PhotonNetwork.time;
        }
    }
    
    protected virtual void UpdateTimer() {
        if (roundStartTime == 0)
            return;

        double timeSinceStart = PhotonNetwork.time - roundStartTime;
        RoundTimer = (int)(roundTime - timeSinceStart);
        BreakTimer = (int)(roundTime + breakTime - timeSinceStart);
        if (RoundTimer == 0 && !roundFinished) {
            roundFinished = true;
            OnRoundFinished();
        }
        if (BreakTimer == 0 && !breakFinished) {
            breakFinished = true;
            OnBreakeFinished();
        }
    }

    protected abstract void OnRoundFinished();

    protected abstract void OnBreakeFinished();

    protected Player CreatePlayer() {
        GameObject playerGO = PhotonNetwork.Instantiate(playerPref.name, new Vector3(), Quaternion.identity, 0);
        CameraFollow.Instance.SetTarget(playerGO.transform);
        Player newPlayer = playerGO.GetComponent<Player>();
        newPlayer.Mode = this.Mode;
        newPlayer.FriendlyFire = isFrinedlyFireEnable();
        newPlayer.IsInteractable = false;
        newPlayer.gameObject.AddComponent<AudioListener>();
        return newPlayer;
    }

    protected void CreateBot(int botId, string botName)
    {
        GameObject BotGO = PhotonNetwork.InstantiateSceneObject(playerBotPref.name, new Vector3(), Quaternion.identity, 0, null);
        PlayerBot playerBot = BotGO.GetComponent<PlayerBot>();
        playerBot.PlayerId = botId;
        playerBot.PlayerName = botName;
        playerBot.Mode = this.Mode;
        playerBot.FriendlyFire = isFrinedlyFireEnable();
        playerBot.IsInteractable = false;
        SpawnBot(playerBot);
    }

    protected bool isFrinedlyFireEnable() {
        if (PhotonNetwork.room != null && PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.FriendlyFire))        
            return (bool)PhotonNetwork.room.CustomProperties[RoomProperty.FriendlyFire];        
        else
            return false;
    }

    protected abstract void SpawnPlayer(Player player);

    protected abstract void SpawnBot(PlayerBot playerBot);

    protected void ActivateAllPLayers()
    {
        foreach (var player in FindObjectsOfType<Player>())
            player.IsInteractable = true;
        foreach (var bot in FindObjectsOfType<PlayerBot>())
            bot.IsInteractable = true;
    }

    protected void DeactivateAllPLayers()
    {
        foreach (var player in FindObjectsOfType<Player>())
            player.IsInteractable = false;
        foreach (var bot in FindObjectsOfType<PlayerBot>())
            bot.IsInteractable = false;
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!isInitialized)
            return;

        if (stream.isWriting)
        {
            stream.SendNext(roundStartTime);         
        }
        else
        {
            roundStartTime = (double)stream.ReceiveNext();           
        }
    }
   
}

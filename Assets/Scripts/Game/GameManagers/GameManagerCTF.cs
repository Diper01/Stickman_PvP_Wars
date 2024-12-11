using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManagerCTF : GameManagerBase {
    private Tile lastSpawnTileRed;
    private Tile lastSpawnTileBlue;

    public int RedTeamScore
    {
        get
        {
            if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.RedTeamScore))
            {
                return (int)PhotonNetwork.room.CustomProperties[RoomProperty.RedTeamScore];
            }
            else
            {
                return 0;
            }
        }
        set
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties.Add(RoomProperty.RedTeamScore, value);
            PhotonNetwork.room.SetCustomProperties(roomProperties);
        }
    }
    public int BlueTeamScore
    {
        get
        {
            if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BlueTeamScore))
            {
                return (int)PhotonNetwork.room.CustomProperties[RoomProperty.BlueTeamScore];
            }
            else
            {
                return 0;
            }
        }
        set
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties.Add(RoomProperty.BlueTeamScore, value);
            PhotonNetwork.room.SetCustomProperties(roomProperties);
        }
    }
    public int RedTeamWins
    {
        get
        {
            if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.RedTeamWins))
            {
                return (int)PhotonNetwork.room.CustomProperties[RoomProperty.RedTeamWins];
            }
            else
            {
                return 0;
            }
        }
        set
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties.Add(RoomProperty.RedTeamWins, value);
            PhotonNetwork.room.SetCustomProperties(roomProperties);
        }
    }
    public int BlueTeamWins
    {
        get
        {
            if (PhotonNetwork.room.CustomProperties.ContainsKey(RoomProperty.BlueTeamWins))
            {
                return (int)PhotonNetwork.room.CustomProperties[RoomProperty.BlueTeamWins];
            }
            else
            {
                return 0;
            }
        }
        set
        {
            ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
            roomProperties.Add(RoomProperty.BlueTeamWins, value);
            PhotonNetwork.room.SetCustomProperties(roomProperties);
        }
    }
    public int TeamSelectionTimer
    {
        get { return teamSelectionTimer; }
        set
        {
            if (teamSelectionTimer != value)
            {
                if (value >= 0)
                    teamSelectionTimer = value;
                else
                    teamSelectionTimer = 0;
                EventManager.OnTeamSelectionTimerUpdate(teamSelectionTimer);
            }
        }
    }

    public Flag RedFlag;
    public Flag BlueFlag;
    public FlagStand RedStand;
    public FlagStand BlueStand;

    protected int teamSelectionTimer;
    private double teamSelectionTime = 10f;
    protected bool teamSelectionFinished = false;

    public override void Initialize()
    {
        base.Initialize();
        Mode = GameMode.CAPTURE_THE_FLAG;
        StartCoroutine(StartGame());
    }

    private void OnEnable()
    {
        EventManager.PlayerDie += OnPlayerDie;
        EventManager.BotDie += OnBotDie;
        EventManager.FlagCaptured += OnFlagCaptured;
    }

    private void OnDisable()
    {
        EventManager.PlayerDie -= OnPlayerDie;
        EventManager.BotDie -= OnBotDie;
        EventManager.FlagCaptured -= OnFlagCaptured;
    }

    protected override void UpdateTimer()
    {
        if (roundStartTime == 0)
            return;

        double timeSinceStart = PhotonNetwork.time - roundStartTime;

        TeamSelectionTimer = (int)(teamSelectionTime - timeSinceStart);
        RoundTimer = (int)(roundTime + teamSelectionTime - timeSinceStart);
        BreakTimer = (int)(roundTime + teamSelectionTime + breakTime - timeSinceStart);

        if (TeamSelectionTimer == 0 && !teamSelectionFinished)
        {
            teamSelectionFinished = true;
            OnTeamSelectionFinished();
        }
        if (RoundTimer == 0 && !roundFinished)
        {
            roundFinished = true;
            OnRoundFinished();
        }
        if (BreakTimer == 0 && !breakFinished)
        {
            breakFinished = true;
            OnBreakeFinished();
        }
    }

    private void OnTeamSelectionFinished()
    {
        StartCoroutine(StartRound());
    }

    protected override void OnRoundFinished()
    {
        EventManager.OnRoundFinished();
        DeactivateAllPLayers();
        if (PhotonNetwork.isMasterClient)
        {
            if (RedTeamScore > BlueTeamScore)
                RedTeamWins++;
            else if (RedTeamScore < BlueTeamScore)
                BlueTeamWins++;
        }
    }

    protected override void OnBreakeFinished()
    {
        ResetTeamsScore();
        LoadNextMap();
    }

    private IEnumerator StartGame()
    {
        yield return null;
        yield return new WaitUntil(() => roundStartTime != 0);
        player = CreatePlayer();
        SetPlayerCustomProperties();

        if (PhotonNetwork.isMasterClient)
        {
            var bots = BotsInformation.GetBotsDictionary();
            foreach (var botId in bots.Keys)
            {
                CreateBot(botId, bots[botId]);
            }
        }

        double timeSinceStart = PhotonNetwork.time - roundStartTime;
        if (timeSinceStart < teamSelectionTime)
        {
            EventManager.OnTeamSelectionStart();
            TryReshufflePlayers();
        }
    }

    private void SetPlayerTeamAtStart(PhotonPlayer newPlayer)
    {
        if (newPlayer.CustomProperties.ContainsKey(PlayerProperties.Team))
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

    private IEnumerator StartRound()
    {
        yield return new WaitUntil(() => player != null);
        EventManager.OnRoundStart();
        SpawnPlayer(player);
        if (PhotonNetwork.isMasterClient)
        {
            ResetTeamsScore();
            foreach (PlayerBot bot in FindObjectsOfType<PlayerBot>())
            {
                SpawnBot(bot);
            }
        }
        yield return new WaitForSeconds(1f);
        ActivateAllPLayers();
    }

    private void SetPlayerCustomProperties()
    {
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties.Add(PlayerProperties.Kills, 0);
        playerProperties.Add(PlayerProperties.Death, 0);
        PhotonNetwork.player.SetCustomProperties(playerProperties);
    }

    private void ResetTeamsScore()
    {
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties.Add(RoomProperty.RedTeamScore, 0);
        roomProperties.Add(RoomProperty.BlueTeamScore, 0);
        PhotonNetwork.room.SetCustomProperties(roomProperties);
    }

    protected override void SpawnBot(PlayerBot playerBot)
    {
        Tile spawnTile = GetSpawnTile(playerBot.Team);      
        playerBot.SpawanPlayer(spawnTile.transform.position);
    }

    protected override void SpawnPlayer(Player player)
    {
        Tile spawnTile = GetSpawnTile(player.Team);        
        player.SpawanPlayer(spawnTile.transform.position);
    }

    private Tile GetSpawnTile(Team team) {
        Tile spawnTile;
        if (team == Team.RED)
        {
            do
            {
                spawnTile = map.RedSpawnPositions[rand.Next(0, map.RedSpawnPositions.Count)];
            } while (spawnTile == lastSpawnTileRed);
            lastSpawnTileRed = spawnTile;
        }
        else {
            do
            {
                spawnTile = map.BlueSpawnPosition[rand.Next(0, map.BlueSpawnPosition.Count)];
            } while (spawnTile == lastSpawnTileBlue);
            lastSpawnTileBlue = spawnTile;
        }
        return spawnTile;
    }

    private void OnPlayerDie(Player player)
    {              
        StartCoroutine(RespawnPlayer(player));       
    }

    private void OnBotDie(PlayerBot bot)
    {       
        StartCoroutine(RespawnBot(bot));
    }

    private void OnFlagCaptured(Team team) {
        if (!PhotonNetwork.isMasterClient)
            return;

        if (team == Team.RED)
        {
            RedTeamScore++;
        }
        else if (team == Team.BLUE)
        {
            BlueTeamScore++;
        }
        RedFlag.PutFlagOnStand();
        BlueFlag.PutFlagOnStand();
        photonView.RPC("FlagCaptured", PhotonTargets.All, team);
    }
    
    [PunRPC]
    private void FlagCaptured(Team team) {
        EventManager.OnShowFlagCapturedMessage(team);        
    }

    private IEnumerator DisableFlagsAfterCapture() {
        RedFlag.IsActive = false;
        BlueFlag.IsActive = false;        
        yield return new WaitForSeconds(2f);
        RedFlag.IsActive = true;
        BlueFlag.IsActive = true;
    }

    private IEnumerator RespawnPlayer(Player respPlayer)
    {
        yield return new WaitForSeconds(2f);
        SpawnPlayer(respPlayer);
    }

    private IEnumerator RespawnBot(PlayerBot respBot)
    {
        yield return new WaitForSeconds(2f);
        SpawnBot(respBot);
    }

    public List<PlayerData> GetPlayersDataList()
    {
        List<PlayerData> playersData = new List<PlayerData>();
        PlayerBot[] bots = FindObjectsOfType<PlayerBot>();

        foreach (var player in PhotonNetwork.playerList)
        {
            int kills = (int)player.CustomProperties[PlayerProperties.Kills];
            int death = (int)player.CustomProperties[PlayerProperties.Death];
            playersData.Add(new PlayerData() { PlayerName = player.NickName, PlayerId = player.ID, Kills = kills, Death = death });
        }
        foreach (var bot in bots)
        {
            if (playersData.Count > 6) break;
            playersData.Add(new PlayerData() { PlayerName = bot.PlayerName, PlayerId = bot.PlayerId, Kills = bot.Frags, Death = bot.Deaths });
        }

        playersData = playersData.OrderByDescending(p => p.Kills).ThenBy(p => p.Death).ToList();

        return playersData;
    }

    public List<PlayerData> GetRedPlayersDataList()
    {
        List<PlayerData> playersData = new List<PlayerData>();
        PlayerBot[] bots = FindObjectsOfType<PlayerBot>();

        foreach (var player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team)
                && (Team)player.CustomProperties[PlayerProperties.Team] == Team.RED)
            {
                int kills = player.CustomProperties.ContainsKey(PlayerProperties.Kills) ? (int)player.CustomProperties[PlayerProperties.Kills] : 0;
                int death = player.CustomProperties.ContainsKey(PlayerProperties.Death) ? (int)player.CustomProperties[PlayerProperties.Death] : 0;
                playersData.Add(new PlayerData() { PlayerName = player.NickName, PlayerId = player.ID, Kills = kills, Death = death });
            }
        }
        foreach (var bot in bots)
        {
            if (playersData.Count > 6)
                break;

            if (bot.Team == Team.RED)
                playersData.Add(new PlayerData() { PlayerName = bot.PlayerName, PlayerId = bot.PlayerId, Kills = bot.Frags, Death = bot.Deaths });
        }

        playersData = playersData.OrderByDescending(p => p.Kills).ThenBy(p => p.Death).ToList();

        return playersData;
    }

    public List<PlayerData> GetBluePlayersDataList()
    {
        List<PlayerData> playersData = new List<PlayerData>();
        PlayerBot[] bots = FindObjectsOfType<PlayerBot>();

        foreach (var player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team)
                && (Team)player.CustomProperties[PlayerProperties.Team] == Team.BLUE)
            {
                int kills = player.CustomProperties.ContainsKey(PlayerProperties.Kills) ? (int)player.CustomProperties[PlayerProperties.Kills] : 0;
                int death = player.CustomProperties.ContainsKey(PlayerProperties.Death) ? (int)player.CustomProperties[PlayerProperties.Death] : 0;
                playersData.Add(new PlayerData() { PlayerName = player.NickName, PlayerId = player.ID, Kills = kills, Death = death });
            }
        }
        foreach (var bot in bots)
        {
            if (playersData.Count > 6)
                break;

            if (bot.Team == Team.BLUE)
                playersData.Add(new PlayerData() { PlayerName = bot.PlayerName, PlayerId = bot.PlayerId, Kills = bot.Frags, Death = bot.Deaths });
        }

        playersData = playersData.OrderByDescending(p => p.Kills).ThenBy(p => p.Death).ToList();

        return playersData;
    }

    private void TryReshufflePlayers()
    {
        if (!PhotonNetwork.isMasterClient || (bool)PhotonNetwork.room.CustomProperties[RoomProperty.Private] == true)
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

        if (redCount - blueCount >= 2)
        {
            RemovePlayerFromRedToBlue();
        }
        else if (redCount - blueCount <= -2)
        {
            RemovePlayerFromBlueToRed();
        }
    }

    private void RemovePlayerFromRedToBlue()
    {
        foreach (var player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team) && (Team)player.CustomProperties[PlayerProperties.Team] == Team.RED)
            {
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
                playerProperties.Add(PlayerProperties.Team, Team.BLUE);
                player.SetCustomProperties(playerProperties);
                return;
            }
        }

        foreach (var botId in BotsInformation.GetBotsDictionary().Keys)
        {
            if (BotsInformation.GetBotTeam(botId) == Team.RED)
            {
                BotsInformation.SetBotTeam(botId, Team.BLUE);
                return;
            }
        }
    }

    private void RemovePlayerFromBlueToRed()
    {
        foreach (var player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey(PlayerProperties.Team) && (Team)player.CustomProperties[PlayerProperties.Team] == Team.BLUE)
            {
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
                playerProperties.Add(PlayerProperties.Team, Team.RED);
                player.SetCustomProperties(playerProperties);
                return;
            }
        }

        foreach (var botId in BotsInformation.GetBotsDictionary().Keys)
        {
            if (BotsInformation.GetBotTeam(botId) == Team.BLUE)
            {
                BotsInformation.SetBotTeam(botId, Team.RED);
                return;
            }
        }
    }
   
    private void LoadNextMap()
    {      
        if (PhotonNetwork.room == null || PhotonNetwork.room.CustomProperties == null)
            return;

        EventManager.OnLoadNextMap();
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.DestroyAll();

            if (MapQueue.MapQueueIndex >= MapQueue.MapQueueList.Count - 1)
                MapQueue.MapQueueIndex = 0;
            else
                MapQueue.MapQueueIndex++;

            PhotonNetwork.LoadLevel(ScenesIndexes.LoadNextMap);
        }      
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        EventManager.OnPlayerJoinGame(newPlayer.NickName);
        if (PhotonNetwork.isMasterClient)
        {
            PlayerBot[] bots = FindObjectsOfType<PlayerBot>();
            if (bots.Length > 0 && PhotonNetwork.room.PlayerCount + bots.Length > 6)
            {
                EventManager.OnPlayerLeaveGame(bots[0].PlayerName);
                BotsInformation.RemoveBot(bots[0].PlayerId);
                PhotonNetwork.Destroy(bots[0].GetComponent<PhotonView>());
            }
            SetPlayerTeamAtStart(newPlayer);
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        EventManager.OnPlayerLeaveGame(otherPlayer.NickName);
        PhotonNetwork.RemoveRPCs(otherPlayer);
        PhotonNetwork.DestroyPlayerObjects(otherPlayer);
    }
  
    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (PhotonNetwork.player.IsMasterClient)
        {
            if (PhotonNetwork.time - roundStartTime < roundTime + teamSelectionTime)
            {
                ActivateAllPLayers();
            }
            else
            {
                DeactivateAllPLayers();
            }
        }
    }

    public override void OnLeftRoom()
    {
        EventManager.OnRoomDisconnected();
    }
}

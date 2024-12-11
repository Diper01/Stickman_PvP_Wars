using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameManagerDeathmatch : GameManagerBase
{

    private Tile lastSpawnTile;

    public override void Initialize()
    {
        base.Initialize();
        Mode = GameMode.DEATHMATCH;
        StartCoroutine(StartGame());
    }

    private void OnEnable()
    {
        EventManager.PlayerDie += OnPlayerDie;
        EventManager.BotDie += OnBotDie;
    }

    private void OnDisable()
    {
        EventManager.PlayerDie -= OnPlayerDie;
        EventManager.BotDie -= OnBotDie;
    }

    protected override void OnRoundFinished()
    {
        EventManager.OnRoundFinished();
        DeactivateAllPLayers();
        if (PhotonNetwork.isMasterClient)
        {
            List<PlayerData> playersData = GetPlayersDataList();
            PlayerData winner = FindWinner(playersData);

            if (winner != null)
            {
                AddRoundScore(winner);
            }
        }       
    }

    protected override void OnBreakeFinished()
    {               
        LoadNextMap();              
    }
  
    private IEnumerator StartGame() {      
        EventManager.OnRoundStart();
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

        SpawnPlayer(player);
        if (PhotonNetwork.isMasterClient) {
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
        if (!PhotonNetwork.player.CustomProperties.ContainsKey(PlayerProperties.RoundWon))
        {
            playerProperties.Add(PlayerProperties.RoundWon, 0);
        }
        PhotonNetwork.player.SetCustomProperties(playerProperties);       
    }

    protected override void SpawnBot(PlayerBot playerBot)
    {
        Tile spawnTile;
        do
        {          
            spawnTile = map.SpawnPositions[rand.Next(0, map.SpawnPositions.Count)];
        } while (spawnTile == lastSpawnTile);
        lastSpawnTile = spawnTile;
        playerBot.SpawanPlayer(lastSpawnTile.transform.position);
    }

    protected override void SpawnPlayer(Player player)
    {
        Tile spawnTile;
        do
        {
            spawnTile = map.SpawnPositions[rand.Next(0, map.SpawnPositions.Count)];
        } while (spawnTile == lastSpawnTile);
        lastSpawnTile = spawnTile;
        player.SpawanPlayer(lastSpawnTile.transform.position);
    }

    private void OnPlayerDie(Player player)
    {       
        StartCoroutine(RespawnPlayer(player));
    }

    private void OnBotDie(PlayerBot bot)
    {
        StartCoroutine(RespawnBot(bot));
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
            int roundWon = (int)player.CustomProperties[PlayerProperties.RoundWon];
            playersData.Add(new PlayerData() { PlayerName = player.NickName, PlayerId = player.ID, Player = player, Kills = kills, Death = death, RoundWon = roundWon });
        }
        foreach (var bot in bots)
        {
            if (playersData.Count > 6) break;
            playersData.Add(new PlayerData() { PlayerName = bot.PlayerName, PlayerId = bot.PlayerId, PlayerBot = bot, Kills = bot.Frags, Death = bot.Deaths, RoundWon = bot.RoundsWon });
        }

        playersData = playersData.OrderByDescending(p => p.Kills).ThenBy(p => p.Death).ToList();

        return playersData;
    }

    public PlayerData FindWinner(List<PlayerData> playersData)
    {
        PlayerData winner = null;

        if (playersData.Count >= 2)
        {
            if ((playersData[0].Kills > playersData[1].Kills)
                || (playersData[0].Kills == playersData[1].Kills && playersData[0].Death < playersData[1].Death))
            {
                winner = playersData[0];
            }
        }

        return winner;
    }

    private void AddRoundScore(PlayerData winner)
    {
        winner.RoundWon++;
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable();
        playerProperties.Add(PlayerProperties.RoundWon, winner.RoundWon);

        foreach (var player in PhotonNetwork.playerList)
        {
            if (winner.PlayerId == player.ID)
            {
                player.SetCustomProperties(playerProperties);
                return;
            }
        }

        foreach (var bot in FindObjectsOfType<PlayerBot>())
        {
            if (winner.PlayerId == bot.PlayerId && bot.photonView.isMine)
            {
                bot.RoundsWon++;
                break;
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
                MapQueue.MapQueueIndex ++;
                      
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
        if (PhotonNetwork.player.IsMasterClient) {
            print(PhotonNetwork.time - roundStartTime);
            if (PhotonNetwork.time - roundStartTime < roundTime) {
                ActivateAllPLayers();
            }
            else {
                DeactivateAllPLayers();              
            }
        }
    }

    public override void OnLeftRoom()
    {
        EventManager.OnRoomDisconnected();
    }
}

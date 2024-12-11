using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BotTestGameManager : Photon.PunBehaviour {
    [SerializeField] GameObject playerBotPref;
    [SerializeField] Text bot1ScoreText;
    [SerializeField] Text bot2ScoreText;

    private System.Random rand;
    private Map map;
    private int bot1Score = 0;
    private int bot2Score = 0;


    private void Awake()
    {
        rand = new System.Random();
        map = FindObjectOfType<Map>();
        CreateBot();
        CreateBot();
    }

    private void OnEnable()
    {
        EventManager.PlayerDie += OnBotDie;
    }

    private void OnDisable()
    {
        EventManager.PlayerDie -= OnBotDie;
    }

    private void OnBotDie(Player player) {
        if (player.PlayerId == 101) {
            bot2Score++;
            bot2ScoreText.text = bot2Score.ToString();
            
        }
        else if (player.PlayerId == 102)
        {
            bot1Score++;
            bot1ScoreText.text = bot1Score.ToString();
        }
        StartCoroutine(RespawnBot(player));
    }

    private void CreateBot() {
        GameObject BotGO = PhotonNetwork.Instantiate(playerBotPref.name, new Vector3(14.5f, 6, 0), Quaternion.identity, 0);
        PlayerBot playerBot = BotGO.GetComponent<PlayerBot>();
        Vector2 spawnPos = map.SpawnPositions[rand.Next(0, map.SpawnPositions.Count)].transform.position;
        playerBot.SpawanPlayer(spawnPos);
    }

    private IEnumerator RespawnBot(Player playerBot) {
        yield return new WaitForSeconds(2f);
        Vector2 spawnPos = map.SpawnPositions[rand.Next(0, map.SpawnPositions.Count)].transform.position;
        playerBot.SpawanPlayer(spawnPos);
    }
}

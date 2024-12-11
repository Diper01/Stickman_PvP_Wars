using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour {  
    public bool EnemyForward { get; set; }
    public bool EnemyBack { get; set; }
    public bool EnemyRight { get; set; }
    public bool EnemyLeft { get; set; }

    [SerializeField] PlayerBot playerBot;
    private static System.Random rand = new System.Random();
    private GameType gameType;
    private bool isTriggerStay = false;

    private void Start()
    {
        gameType = GameManagersHolder.Instance.GameType;
    }

    private void Update()
    {
        if (!PhotonNetwork.isMasterClient || rand.Next(0, 3) != 0)
            return;

        string enemyLayer = FindEnemyPhysicsLayer();
        FindEnemyForward(enemyLayer);
        if (!EnemyForward)
        {
            FindEnemyBack(enemyLayer);
        }
        else
        {
            EnemyBack = false;
        }
    }

    public void Refresh()
    {
        EnemyForward = false;
        EnemyBack = false;
        EnemyRight = false;
        EnemyLeft = false;
    }

    private string FindEnemyPhysicsLayer() {
        if (PhotonNetwork.masterClient.CustomProperties.ContainsKey(PlayerProperties.Team))
        {
            Team masterClientTeam = (Team)PhotonNetwork.masterClient.CustomProperties[PlayerProperties.Team];
            if (masterClientTeam == BotsInformation.GetBotTeam(playerBot.PlayerId))
                return "EnemyPlayer";
            else
                return "Player";
        }
        else {
            return "EnemyPlayer";
        }
    }

    private void FindEnemyForward(string enemyLayer) {
        Vector2 direction = transform.parent.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        Vector2 startPosition = (Vector2)transform.position + direction + new Vector2(0, -0.2f);

        RaycastHit2D hit;
        if (GameManagersHolder.Instance.GameType == GameType.FFA)
            hit = Physics2D.Raycast(startPosition, direction, 10f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("EnemyPlayer"));
        else
            hit = Physics2D.Raycast(startPosition, direction, 10f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer(enemyLayer));

        if (hit.transform != null && hit.transform.tag == "Player") {
            
            EnemyForward = true;

            if(direction == Vector2.right) {
                EnemyRight = true;
                EnemyLeft = false;
            }
            else {
                EnemyRight = false;
                EnemyLeft = true;
            }
        }
        else
            EnemyForward = false;
    }

    private void FindEnemyBack(string enemyLayer) {
        Vector2 direction = transform.parent.transform.localScale.x > 0 ? Vector2.left : Vector2.right;
        Vector2 startPosition = (Vector2)transform.position + direction * 1.5f + new Vector2(0, -0.2f);

        RaycastHit2D hit;
        if (GameManagersHolder.Instance.GameType == GameType.FFA)
            hit = Physics2D.Raycast(startPosition, direction, 10f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("EnemyPlayer"));
        else
            hit = Physics2D.Raycast(startPosition, direction, 10f, 1 << LayerMask.NameToLayer("Ground") | 1 << LayerMask.NameToLayer(enemyLayer));

        if (hit.transform != null && hit.transform.tag == "Player") {            
            EnemyBack = true;

            if (direction == Vector2.right) {
                EnemyRight = true;
                EnemyLeft = false;
            }
            else {
                EnemyRight = false;
                EnemyLeft = true;
            }
        }
        else
            EnemyBack = false;
    }
}

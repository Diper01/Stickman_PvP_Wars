using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : Photon.PunBehaviour, IPunObservable {
    public Team FlagTeam;
    public FlagState State {
        get { return state; }
        set {           
            if (state == value)
                return;
            state = value;        
            switch (state)
            {
                case FlagState.ON_STAND:
                    carryingPlayer = null;
                    carryingPlayerBot = null;                  
                    SetupFlagOnStand();
                    break;
                case FlagState.ON_PLAYER:
                    carryingPlayer = null;
                    carryingPlayerBot = null;
                    SetupFlagOnPlayer();
                    break;
                case FlagState.ON_FIELD:
                    carryingPlayer = null;
                    carryingPlayerBot = null;
                    SetupFlagOnField();
                    break;                
            }
        }
    }
    public int CarryingPlayerId {
        get { return carryingPlayerId; }
        set {
            if (carryingPlayerId == value)
                return;

            carryingPlayerId = value;
           
            if (State == FlagState.ON_PLAYER) {
                SetupFlagOnPlayer();
            }
        }
    }
    public Vector2 DropPosition {
        get { return dropPosition; }
        set {        
            if (dropPosition == value)
                return;

            dropPosition = value;          
            if (State == FlagState.ON_FIELD) {
                SetupFlagOnField();
            }
        }
    }
    public bool IsActive {
        get { return isActive; }
        set {
            isActive = value;
            if (isActive) {
                spriteRenderer.enabled = true;              
            }
            else {
                spriteRenderer.enabled = false;              
            }
        }
    }
    public int X
    {
        get
        {
            float x = transform.localPosition.x;
            if (x > 0)
                return (int)Mathf.Round(Mathf.Floor(x));
            else
                return (int)Mathf.Round(Mathf.Ceil(x));
        }
    }
    public int Y
    {
        get
        {
            float y = transform.localPosition.y;
            if (y > 0)
                return (int)Mathf.Round(Mathf.Floor(y));
            else
                return (int)Mathf.Round(Mathf.Ceil(y));
        }
    }


    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] BoxCollider2D boxCollider2D;

    private FlagState state = FlagState.ON_STAND;
    private int carryingPlayerId = 0; 
    private Player carryingPlayer = null;
    private PlayerBot carryingPlayerBot = null;
    private Vector2 dropPosition = new Vector2();
    private bool isActive = true;
    private Vector2 positionOnPlayer = new Vector2(-0.13f, 0.1f);
    private Vector2 positionOnStand = new Vector2(0, 0.7f);
    private int dropPlayerId = 0;   

    public void PutFlagOnPlayer(int playerId) {      
        CarryingPlayerId = playerId;
        State = FlagState.ON_PLAYER;
    }

    public void PlayerDropFlag(Vector2 dropPos, int playerId) {
        DropPosition = dropPos;               
        State = FlagState.ON_FIELD;
        dropPlayerId = playerId;
        StartCoroutine(ResetDropPlayerId());
        if (!PhotonNetwork.isMasterClient) {
            photonView.RPC("DropFlag", PhotonTargets.Others, dropPos, playerId);
        }
    }

    [PunRPC]
    private void DropFlag(Vector2 dropPos, int playerId) {
        DropPosition = dropPos;       
        State = FlagState.ON_FIELD;
        dropPlayerId = playerId;
        StartCoroutine(ResetDropPlayerId());
    }

    public void PutFlagOnStand() {
        if (!PhotonNetwork.isMasterClient)
            return;
        State = FlagState.ON_STAND;
    }

    private void SetupFlagOnField() {      
        transform.parent = Map.Instance.transform;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        transform.localPosition = DropPosition;
        boxCollider2D.enabled = true;
    }
 
    private void SetupFlagOnPlayer() {                          
        foreach (var player in FindObjectsOfType<Player>()) {
            if (player.PlayerId == carryingPlayerId) {
                carryingPlayer = player;
                break;
            }
        }
        if (carryingPlayer == null) {
            foreach (var playerBot in FindObjectsOfType<PlayerBot>()) {
                if (playerBot.PlayerId == carryingPlayerId) {
                    carryingPlayerBot = playerBot;
                    break;
                }
            }
        }
        
        if (carryingPlayer != null || carryingPlayerBot != null) {
            boxCollider2D.enabled = false;
            this.transform.parent = null;            
        }
        else
        {
            StartCoroutine(SetupFlagOnPlayerOnStart());
        }
    }

    private IEnumerator SetupFlagOnPlayerOnStart() {
        while (true) {
            yield return null;

            if (State != FlagState.ON_PLAYER) {            
                break;
            }

            foreach (var player in FindObjectsOfType<Player>())
            {
                if (player.PlayerId == carryingPlayerId)
                {
                    carryingPlayer = player;
                    break;
                }
            }
            if (carryingPlayer == null)
            {
                foreach (var playerBot in FindObjectsOfType<PlayerBot>())
                {
                    if (playerBot.PlayerId == carryingPlayerId)
                    {
                        carryingPlayerBot = playerBot;
                        break;
                    }
                }
            }

            if (carryingPlayer != null || carryingPlayerBot != null)
            {
                boxCollider2D.enabled = false;
                this.transform.parent = null;
                break;
            }
        }
    }

    private void SetupFlagOnStand() {       
        FlagStand stand;
        boxCollider2D.enabled = false;
        if (FlagTeam == Team.RED)
            stand = GameManagersHolder.Instance.GameManagerCTF.RedStand;
        else 
            stand = GameManagersHolder.Instance.GameManagerCTF.BlueStand;

        transform.parent = stand.transform;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        transform.localPosition = positionOnStand;
        stand.ResetCollider();
    }

    private void Update()
    {
        if (State == FlagState.ON_PLAYER) {
            FollowWithPlayer();
        }
    }

    private void FollowWithPlayer() {
        if (carryingPlayer != null && carryingPlayer.Health > 0)
        {
            if (carryingPlayer.transform.localScale.x > 0)
            {
                transform.position = (Vector2)carryingPlayer.transform.position + positionOnPlayer;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.position = (Vector2)carryingPlayer.transform.position + new Vector2(-positionOnPlayer.x, positionOnPlayer.y);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else if (carryingPlayerBot != null && carryingPlayerBot.Health > 0) {
            if (carryingPlayerBot.transform.localScale.x > 0)
            {
                transform.position = (Vector2)carryingPlayerBot.transform.position + positionOnPlayer;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else
            {
                transform.position = (Vector2)carryingPlayerBot.transform.position + new Vector2(-positionOnPlayer.x, positionOnPlayer.y);
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else if (PhotonNetwork.isMasterClient)
        {
            Vector2 dropPosition = new Vector2(Mathf.Ceil(transform.position.x) - 0.5f, Mathf.Ceil(transform.position.y) - 0.5f);
            PlayerDropFlag(dropPosition, carryingPlayerId);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.isMasterClient || collision.tag != "Player")
            return;

        switch (State)
        {
            case FlagState.ON_STAND:            
                break;
            case FlagState.ON_PLAYER:
                break;
            case FlagState.ON_FIELD:
                OnFieldCollision(collision);
                break;           
        }
    } 
             

    private void OnFieldCollision(Collider2D collision) {       
        Player player = collision.GetComponent<Player>();
        PlayerBot playerBot = null;
        Team playerTeam;
        int playerId = 0;
        if (player != null)
        {
            playerTeam = player.Team;
            playerId = player.PlayerId;
        }
        else
        {           
            playerBot = collision.GetComponent<PlayerBot>();
            playerTeam = playerBot.Team;
            playerId = playerBot.PlayerId;
        }        

        if (player != null && (!player.CanPickupItem || player.PlayerId == dropPlayerId))
            return;

        if (playerBot != null && (!playerBot.CanPickupItem || playerBot.PlayerId == dropPlayerId))
            return;


        if (playerTeam != FlagTeam)
        {
            State = FlagState.ON_PLAYER;
            CarryingPlayerId = playerId;
        }
        else {
            State = FlagState.ON_STAND;
        }      
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {      
        if (stream.isWriting)
        {            
            stream.SendNext(carryingPlayerId);
            stream.SendNext(dropPosition);
            stream.SendNext(state);
        }
        else
        {
            CarryingPlayerId = (int)stream.ReceiveNext();          
            DropPosition = (Vector2)stream.ReceiveNext();           
            State = (FlagState)stream.ReceiveNext();          
        }
    }

    private IEnumerator ResetDropPlayerId() {
        yield return new WaitForSeconds(2f);
        dropPlayerId = 0;
    }
}

public enum FlagState {
    ON_STAND,
    ON_PLAYER,
    ON_FIELD
}


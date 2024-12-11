using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagStand : Photon.PunBehaviour {
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
    public bool HasFlag { get { return GetComponentInChildren<Flag>() != null; } }
    public Team StandTeam;

    [SerializeField] Flag flag;
    [SerializeField] BoxCollider2D boxCollider2D;

    public void ResetCollider() {
        StartCoroutine(ResetColliderCoroutine());
    }

    private IEnumerator ResetColliderCoroutine() {
        boxCollider2D.enabled = false;
        yield return null;
        boxCollider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!PhotonNetwork.isMasterClient || !HasFlag || !flag.IsActive || collision.tag != "Player")
            return;

        Player player = collision.GetComponent<Player>();
        PlayerBot playerBot = null;
        Team playerTeam;
        if (player != null)
        {          
            playerTeam = player.Team;
        }
        else
        {
            playerBot = collision.GetComponent<PlayerBot>();            
            playerTeam = playerBot.Team;
        }

        if ((player != null && !player.CanPickupItem) || (playerBot != null && !playerBot.CanPickupItem))
            return;

        if (playerTeam == StandTeam)
        {            
            if ((player != null && player.HasFlag) || (playerBot != null && playerBot.HasFlag))
            {              
                EventManager.OnFlagCaptured(StandTeam);
            }
        }
        else {           
            if (player != null)
                flag.PutFlagOnPlayer(player.PlayerId);
            else if (playerBot != null)
                flag.PutFlagOnPlayer(playerBot.PlayerId);
        }
    }

    private void OnDrawGizmos()
    {
        float x = Mathf.Ceil(transform.position.x) - 0.5f;
        float y = Mathf.Ceil(transform.position.y) - 0.5f;
        this.transform.position = new Vector2(x, y);      
    }
}

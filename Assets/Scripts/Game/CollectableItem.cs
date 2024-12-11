using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : Photon.PunBehaviour, IPunObservable {
    public CollectableItemType Type;
    public int Value;
    public bool IsActive
    {
        get
        {
            return isActive;
        }
        set
        {
            if (value == isActive)
                return;
            isActive = value;
            if (isActive)
            {
                sprRenderer.enabled = true;
                boxCollider2D.enabled = true;
                if(parentBoxCollider2D!= null) parentBoxCollider2D.enabled = true;
            }
            else {
                sprRenderer.enabled = false;
                if (GameOptions.Sound)
                    source.Play();

                boxCollider2D.enabled = false;
                if (parentBoxCollider2D != null) parentBoxCollider2D.enabled = false;
            }
        }
    }

    private AudioSource source;
    private bool isActive;
    private SpriteRenderer sprRenderer;
    private BoxCollider2D boxCollider2D;
    private BoxCollider2D parentBoxCollider2D;
    float respawnTime = 18f;

    private void Awake()
    {
        this.source = GetComponent<AudioSource>();
        this.sprRenderer = GetComponent<SpriteRenderer>();       
        this.boxCollider2D = GetComponent<BoxCollider2D>();
        parentBoxCollider2D = transform.parent.GetComponent<BoxCollider2D>();
        IsActive = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (photonView.isMine && collision.tag == "Player") {
            byte playerId = 0;
            Player player = collision.GetComponent<Player>();           

            if (player != null)
            {
                if (player.Health <= 0)
                    return;
                else
                    playerId = (byte)player.PlayerId;
            }
            else {
                var playerBot = collision.GetComponent<PlayerBot>();
                if (playerBot != null)
                {
                    if (playerBot.Health <= 0)
                        return;
                    else
                        playerId = (byte)playerBot.PlayerId;
                }
            }        

            byte value = (byte)this.Value;
            byte itemType = (byte)this.Type;
            byte[] eventContent = new byte[] { playerId, value, itemType };

            if (PhotonNetwork.offlineMode == false)
            {
                RaiseEventOptions options = new RaiseEventOptions();
                options.Receivers = ReceiverGroup.All;
                PhotonNetwork.RaiseEvent(PhotonEventCodes.ItemPickup, eventContent, true, options);
            }
            else {
                EventManager.OnPhotonOfflineEvent(PhotonEventCodes.ItemPickup, eventContent, PhotonNetwork.player.ID);
            }

            IsActive = false;           
            StartCoroutine(Respawn(respawnTime));
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(this.IsActive);
        }
        else {
            this.IsActive = (bool)stream.ReceiveNext();
        }
    }

    private IEnumerator Respawn(float timeDelay) {
        yield return new WaitForSeconds(timeDelay);
        IsActive = true;
    }
  
}

public enum CollectableItemType {
          AUTOMATON_BULLETS
        , FLAME_BULLETS
        , ROKET_BULLETS
        , LASER_BULLETS
        , MINES
        , HEALTH
        , ARMOR
}
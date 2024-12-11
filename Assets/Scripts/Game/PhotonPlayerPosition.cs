using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonPlayerPosition : Photon.MonoBehaviour, IPunObservable
{
    [SerializeField] float teleportDistance = 3;
    [SerializeField] float lerpSpeed = 10;

    private Rigidbody2D rigitBody2D;
    private Vector2 targetPosition;

    private void Awake()
    {
        rigitBody2D = GetComponent<Rigidbody2D>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext((Vector2)transform.localPosition);
            stream.SendNext(rigitBody2D.velocity);
        }
        else {
            transform.localPosition = (Vector2)stream.ReceiveNext();         
            rigitBody2D.velocity = (Vector2)stream.ReceiveNext();
        }
    }

    public Vector3 UpdatePosition(Vector3 currentPosition)
    {             
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * lerpSpeed);  
                        
        if (Vector3.Distance(currentPosition, targetPosition) > teleportDistance)
        {
            currentPosition = targetPosition;
        }        
        return currentPosition;
    }
}

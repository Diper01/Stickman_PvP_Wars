using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BotItemsCollider : MonoBehaviour {

    public event Action<Vector2i> OnTrigger;

    [SerializeField] PlayerBot playerBot;   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnTrigger == null)
            return;

        Tile tile = collision.GetComponent<Tile>();
        if (tile != null) {
            OnTrigger(new Vector2i(tile.X, tile.Y));
            return;
        }

        FlagStand stand = collision.GetComponent<FlagStand>();
        if (stand != null && stand.StandTeam != playerBot.Team && stand.HasFlag) {
            OnTrigger(new Vector2i(stand.X, stand.Y));
            return;
        }

        Flag flag = collision.GetComponent<Flag>();
        if (flag != null){
            OnTrigger(new Vector2i(flag.X, flag.Y));
            return;
        }
    }

 
}

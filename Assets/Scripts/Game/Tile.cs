using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int X
    {
        get
        {
            float x = transform.position.x;
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
            float y = transform.position.y;
            if (y > 0)
                return (int)Mathf.Round(Mathf.Floor(y));
            else
                return (int)Mathf.Round(Mathf.Ceil(y));
        }
    }
    public TileType Type;
    public CollectableItem CollectableItem;    

    private void OnDrawGizmos()
    {
        float x = Mathf.Ceil(transform.position.x) - 0.5f;
        float y = Mathf.Ceil(transform.position.y) - 0.5f;
        this.transform.position = new Vector2(x, y);
        if (Type == TileType.MoveNode)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 0.45f);
        }
        else if (Type == TileType.SpawnPosition) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.45f);
        }   
    }


}

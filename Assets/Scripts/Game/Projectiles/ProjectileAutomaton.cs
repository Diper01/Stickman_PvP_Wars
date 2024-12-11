using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAutomaton : ProjectileBase
{
    public override void ProjectileHitPlayer()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        Destroy(this.gameObject);
    }

    protected override void ObstacleCollision(Collider2D collision)
    {        
        Destroy(this.gameObject);
    }
}

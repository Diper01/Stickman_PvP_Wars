using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRocket : ProjectileBase {

    public GameObject RocketExplosion;

    private bool isExploded = false;

    public override void ProjectileHitPlayer()
    {
        if (!isExploded)
        {
            ExplodeRocket();
        }
    }

    protected override void ObstacleCollision(Collider2D collision)
    {
        ExplodeRocket();
    }

    private void ExplodeRocket() {
        var rocketExplosion = Instantiate(RocketExplosion);
        rocketExplosion.GetComponent<ProjectileExplosion>().LaunchProjectile(new Vector2(), this.transform.position, Time.time, this.PlayerId, this.PlayerTeam);
        isExploded = true;
        GetComponent<BoxCollider2D>().enabled = false;   
        Destroy(this.gameObject);
    }
}

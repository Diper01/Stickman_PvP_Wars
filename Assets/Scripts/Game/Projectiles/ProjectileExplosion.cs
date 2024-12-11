using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ProjectileExplosion : ProjectileBase {

    [SerializeField] AudioSource source;

    private float apearTime;

    public override void LaunchProjectile(Vector2 direction, Vector2 position, double shotTime, int playerId, Team playerTeam = Team.RED)
    {
        if (GameOptions.Sound)
            source.enabled = true;
        base.LaunchProjectile(direction, position, shotTime, playerId, playerTeam);
        apearTime = Time.time;
        StartCoroutine(DisableCollider());
    }

    public override void ProjectileHitPlayer(){
        GetComponent<CircleCollider2D>().enabled = false;
    }

    protected override void ObstacleCollision(Collider2D collision) { }

    protected override void MoveUpdate() {
        float timePassed = Time.time - apearTime;
       
        if (timePassed > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DisableCollider() {
        yield return new WaitForSeconds( 0.1f);
        GetComponent<CircleCollider2D>().enabled = false;      
    }
}

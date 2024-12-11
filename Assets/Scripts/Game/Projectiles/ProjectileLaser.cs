using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLaser : ProjectileBase
{
    private float apearTime;

    public override void LaunchProjectile(Vector2 direction, Vector2 position, double shotTime, int playerId, Team playerTeam = Team.RED)
    {       
        base.LaunchProjectile(direction, position, shotTime, playerId, playerTeam);
        apearTime = Time.time;

        var hit = Physics2D.Raycast(transform.position, flyDirection, 100f, 1 << LayerMask.NameToLayer("Ground"));
        if (hit.transform != null) {            
            Vector2 endPoint = hit.point;
            float length = Math.Abs(endPoint.x - transform.position.x);
            transform.localScale = new Vector3(length * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else {
            transform.localScale = new Vector3(100f * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        //StartCoroutine(DisableCollider());
        StartCoroutine(AnimateScale());
    }

    public override void ProjectileHitPlayer() {
        GetComponent<BoxCollider2D>().enabled = false;
    }

    protected override void ObstacleCollision(Collider2D collision) { }

    protected override void MoveUpdate()
    {
        float timePassed = Time.time - apearTime;

        if (timePassed > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DisableCollider() {
        yield return null;
        GetComponent<BoxCollider2D>().enabled = false;
    }

    private IEnumerator AnimateScale() {
        while (true) {
            yield return null;
            float y = transform.localScale.y - Time.deltaTime * 3f;
            if (y < 0) {
                transform.localScale = new Vector3(transform.localScale.x, 0);
                break;
            }
            else {
                transform.localScale = new Vector3(transform.localScale.x, y);
            }           
        }
    }
}

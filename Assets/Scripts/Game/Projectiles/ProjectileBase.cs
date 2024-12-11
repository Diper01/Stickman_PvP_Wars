using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour {
    public ProjectileType Type;
    public int PlayerId { get; set; }
    public Team PlayerTeam { get; set; }
    public int damage;
    public float lifeTime;
    public float speed;
    public float maxUpdateDistance;

    protected Vector2 flyDirection;
    protected Vector2 startPosition;
    protected double shotTime;

    public void SetDamage(int damage) {
        this.damage = damage;
    }
   
    public virtual void LaunchProjectile(Vector2 direction, Vector2 position, double shotTime, int playerId, Team playerTeam = Team.RED)
    {
        flyDirection = direction;
        startPosition = position;
        this.shotTime = shotTime;
        PlayerId = playerId;
        PlayerTeam = playerTeam;
        transform.position = startPosition;
       
        if (direction.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    private void FixedUpdate()
    {
        MoveUpdate();
    }

    protected virtual void MoveUpdate() {
        float timePassed = (float)(PhotonNetwork.time - shotTime);

        transform.position = Vector3.MoveTowards(transform.position, startPosition + flyDirection * speed * timePassed, maxUpdateDistance);

        if (timePassed > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player" && collision.tag != "PlayerDead" && speed != 0)
        {
            ObstacleCollision(collision);
        }
    }

    public abstract void ProjectileHitPlayer();

    protected abstract void ObstacleCollision(Collider2D collision);  
}


public enum ProjectileType { BULLET, FLAME, ROCKET, EXPLOSION, LASER, MINE }
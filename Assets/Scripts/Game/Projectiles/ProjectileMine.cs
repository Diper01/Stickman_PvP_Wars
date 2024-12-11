using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMine : ProjectileBase {
    public GameObject MineExplosion;
    public Rigidbody2D Rigidbody;
    public float explosionDelay = 0.3f;
    public CircleCollider2D NonTriggerCollider;   
    public bool IsLanded { get; private set; }

    private bool isTriggered;

    private void Awake()
    {
        IsLanded = false;
        isTriggered = false;
    }

    public override void ProjectileHitPlayer()
    {
        //if (!isLanded)
        //{
        //    ExplodeMine();
        //}
        //else {
        //    StartCoroutine(ExplodeMineWithDelay(explosionDelay));
        //}
        ExplodeMine();
    }

    protected override void MoveUpdate()
    {
        
    }

    public override void LaunchProjectile(Vector2 direction, Vector2 position, double shotTime, int playerId, Team playerTeam = Team.RED)
    {
        base.LaunchProjectile(direction, position, shotTime, playerId, playerTeam);
        Rigidbody.AddForce(direction * 500f);
    }

    protected override void ObstacleCollision(Collider2D collision)
    {
        if (IsLanded && collision.transform.tag == "Projectile")
        {
            var projectile = collision.transform.GetComponent<ProjectileBase>();
            if (projectile != null && projectile.Type != ProjectileType.MINE)
            {
                ExplodeMine();
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (!IsLanded)
        {
            PlaceMine(collision);
        }
        else if(collision.transform.tag == "Projectile") {            
            ExplodeMine();
        }
    }

    private void PlaceMine(Collision2D collision) {       
        IsLanded = true;
        NonTriggerCollider.radius = 0.8f;
        Rigidbody.bodyType = RigidbodyType2D.Kinematic;
        Rigidbody.velocity = new Vector2();
        Rigidbody.angularVelocity = 0f;
        transform.rotation = new Quaternion();
        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = false;
    
        float upperY = collision.collider.bounds.center.y + collision.collider.bounds.extents.y;
        float bottomY = collision.collider.bounds.center.y - collision.collider.bounds.extents.y;
        float rightX = collision.collider.bounds.center.x + collision.collider.bounds.extents.x;
        float leftX = collision.collider.bounds.center.x - collision.collider.bounds.extents.x;

        float upperDeltaY = Math.Abs(Math.Abs(upperY) - Math.Abs(transform.position.y));
        float bottomDeltaY = Math.Abs(Math.Abs(bottomY) - Math.Abs(transform.position.y));
        float rightDeltaX = Math.Abs(Math.Abs(rightX) - Math.Abs(transform.position.x));
        float leftDeltaX = Math.Abs(Math.Abs(leftX) - Math.Abs(transform.position.x));

        float[] sizeDelta = new float[] { upperDeltaY, bottomDeltaY, rightDeltaX, leftDeltaX };
        int minDeltaIdenx = 0;
        float minValue = 100f;
        for (int i = 0; i < sizeDelta.Length; i++) {
            if (sizeDelta[i] < minValue) {
                minValue = sizeDelta[i];
                minDeltaIdenx = i;
            }
        }
        
        switch (minDeltaIdenx)
        {
            case 0:               
                transform.position = new Vector2(collision.contacts[0].point.x, upperY);
                break;
            case 1:               
                transform.position = new Vector2(collision.contacts[0].point.x, bottomY);
                transform.Rotate(Vector3.forward * 180);
                break;
            case 2:              
                transform.position = new Vector2(rightX, collision.contacts[0].point.y); 
                transform.Rotate(Vector3.forward * -90);
                break;
            case 3:                
                transform.position = new Vector2(leftX, collision.contacts[0].point.y);
                transform.Rotate(Vector3.forward * 90);
                break;
        }

    }

    private IEnumerator ExplodeMineWithDelay(float delayTime) {
        yield return new WaitForSeconds(delayTime);
        ExplodeMine();
    }

    private void ExplodeMine() {
        if (!isTriggered)
        {
            var mineExplosion = Instantiate(MineExplosion);
            ProjectileExplosion explosion = mineExplosion.GetComponent<ProjectileExplosion>();
            explosion.SetDamage(this.damage);
            explosion.LaunchProjectile(new Vector2(), this.transform.position, Time.time, this.PlayerId, this.PlayerTeam);
            isTriggered = true;
            Destroy(this.gameObject);
        }
    }
}

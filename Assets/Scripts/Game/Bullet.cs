using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public WeaponType BulletType;
    public int PlayerId { get; set; }
    public int damage = 10;
    public float lifeTime = 5f;
    public float speed = 10f;
    public float maxUpdateDistance = 0.5f;

    private Vector2 flyDirection;
    private Vector2 startPosition;
    private double shotTime;
    private bool launched = false;


    public void LaunchBullet(Vector2 direction, Vector2 position, double shotTime, int playerId)
    {       
        flyDirection = direction;
        startPosition = position;
        this.shotTime = shotTime;
        PlayerId = playerId;
        transform.position = startPosition;
        launched = true;

        if (direction.x < 0) {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void FixedUpdate()
    {
        if (!launched) {
            return;
        }

        float timePassed = (float)(Time.time - shotTime);

        transform.position = Vector3.MoveTowards(transform.position, startPosition + flyDirection * speed * timePassed, maxUpdateDistance);

        if (timePassed > lifeTime)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Player" && speed != 0) {
            Destroy(this.gameObject);
        }
    }
  
}

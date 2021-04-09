using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingDemon : EnemyController
{
    // Unicque FlyingDemonController variables
    private const float movementDistance = 8f;
    private Vector2 waypointA;
    private Vector2 waypointB;

    private const float bulletSpeed = 5f;
    private const float shootCooldown = 2f;
    private float shootTimer;

    // Component variables
    private Rigidbody2D rb2;

    // Other classes variables
    public GameObject bulletPrefab;

    void Start()
    {
        damagePoints = 10;
        healthPoints = 1;
        moveSpeed = 5.0f;
        sightDistance = 12.0f;
        shootTimer = 0f;

        waypointA = new Vector2(spawnPosition.x - movementDistance, spawnPosition.y);
        waypointB = new Vector2(spawnPosition.x + movementDistance, spawnPosition.y);

        rb2 = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        move();
    }

    public void move()
    {
        // Act (move and shoot) if player is in sight distance
        Vector2 distanceEnemyPlayer = player.transform.position - transform.position;
        if (Mathf.Abs(distanceEnemyPlayer.x) < sightDistance)
        {
            // Store currentPosition 
            //Vector2 currentPosition = transform.position;
            // Calculate and Normalize distance from currentPosition to playerPosition
            //Vector2 distanceEnemyPlayer = new Vector2(player.transform.position.x - currentPosition.x, 0f);
            distanceEnemyPlayer.Normalize();
            // Calculate distance to move each frame
            Vector2 distanceToMove = distanceEnemyPlayer * Vector2.right * moveSpeed * Time.deltaTime;

            // Move self if in movement range 
            if ((transform.position.x + distanceToMove.x) > waypointA.x && (transform.position.x + distanceToMove.x) < waypointB.x)
            {
                rb2.MovePosition((Vector2)transform.position + distanceToMove);
            }

            // shoot if timer equals cooldown
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootCooldown)
            {
                shoot();
                shootTimer = 0f;
            }
        }
    }

    public void shoot()
    {
        // Instantiate Bullet projectile and set its position equal to Enemy
        GameObject b = Instantiate(bulletPrefab);
        b.transform.position = transform.position;

        // Set bullet's destiny direction and speed
        Vector2 distanceTarget = (player.transform.position - b.transform.position);
        distanceTarget.Normalize();
        b.GetComponent<Rigidbody2D>().velocity = distanceTarget * bulletSpeed;
    }


}
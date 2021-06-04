using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    // Movement
    private float movementSpeed;

    // Health
    private const int maxHealth = 100;
    private int health = maxHealth;

    // Melee Attack
    private const int meleeDamage = 20;
    private const float meleeAtkTime = 2f;
    private float meleeAtkTimer = 0f;

    // Slime Spit Attack
    private const float spitRange = 10f;
    private const float spitTime = 5f;
    private float spitTimer;// = 0f;


    // Other GameObjects
    public PlayerController player;
    private float distanceToPlayer;
    
    public Slime slimeCopy;
    private Vector2 mouthPosition;

    // Components
    private Rigidbody2D rb2;


    void Start()
    {
        spitTimer = spitTime;

        rb2 = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Update movement speed
        movementSpeed = player.moveSpeed;

        // Updated distance between Boss and Player
        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        // Update mouth position
        mouthPosition = new Vector2(transform.position.x + 2f, transform.position.y + 2f); // test coord

        // Spit enemy if player is in spitRange and cooldown passed
        if (distanceToPlayer >= spitRange)
        {
            Spit();
        }
    }

    private void FixedUpdate()
    {
        Move();
        if (player.offCamera)
            transform.position = new Vector2(player.transform.position.x - 10, transform.position.y);
    }


    // Move self towards player
    private void Move()
    {
        if (distanceToPlayer > 10)
        {
            Vector2 positionToMove = new Vector2(player.transform.position.x, transform.position.y);
            transform.position = Vector2.MoveTowards(transform.position, positionToMove, movementSpeed * Time.deltaTime);
        }
    }



    // Spit logic
    private void Spit()
    {
        if (spitTimer <= spitTime)
        {
            spitTimer += Time.deltaTime;
        }
        else
        {
            spitTimer = 0f;
            SpitSlime();
        }
        Debug.Log(spitTimer);
    }

    // Spit a Slime
    private void SpitSlime()
    {
        GameObject slimeClone = Instantiate(slimeCopy.gameObject, mouthPosition, Quaternion.identity);
        Rigidbody2D rbSlime = slimeClone.GetComponent<Rigidbody2D>();
        rbSlime.AddForce(new Vector2(12f, 9f), ForceMode2D.Impulse);
    }
}

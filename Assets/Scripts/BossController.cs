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
        spitTimer = 0f;

        rb2 = GetComponent<Rigidbody2D>();
        mouthPosition = new Vector2(transform.position.x + 2f, transform.position.y + 6); // test coord
    }

    void Update()
    {
        // Updated distance between Boss and Player
        distanceToPlayer = (player.transform.position - transform.position).magnitude;

        // Spit enemy if player is in spitRange and cooldown passed
        if (distanceToPlayer < spitRange)
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
        }
        Debug.Log(spitTimer);
    }



    // Spit a Slime
    private void SpitSlime()
    {
        Debug.Log("spitting slime");
        GameObject slimeClone = Instantiate(slimeCopy.gameObject, mouthPosition, Quaternion.identity);
        Rigidbody2D rbSlime = slimeClone.GetComponent<Rigidbody2D>();
        //rbSlime.AddForce(new Vector2(30f, 10f), ForceMode2D.Impulse);
    }
}

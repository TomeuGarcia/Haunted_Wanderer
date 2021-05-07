using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Object's variables
    public int damagePoints;
    protected private int healthPoints;

    protected private float moveSpeed;
    protected private float jumpSpeed;

    protected private float sightDistance;

    protected private Vector2 spawnPosition;

    // Enemy boolean flags (tags)
    public bool canSpawn = true;

    [Header("Enemy Spawns When")]
    [SerializeField] public bool spawnsHighSanity;
    [SerializeField] public bool spawnsMediumSanity;
    [SerializeField] public bool spawnsLowSanity;

    [Header("Enemy Exists When")]
    [SerializeField] public bool highSanity;
    [SerializeField] public bool mediumSanity;
    [SerializeField] public bool lowSanity;


    // Other classes variables
    public GameObject player;


    void Awake()
    {
        // Set spawnPosition
        spawnPosition = transform.position;
    }


    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if Collided with player
        if (collision.collider.CompareTag("Player"))
        {
            // if player jumped on top "kill" enemy
            if (collision.contacts[0].normal.y < -0.5)
            {
                setActiveState(false);
                // add +1 to sanityLossLimiter
                collision.collider.GetComponent<PlayerController>().addSanityLossLimiter();
            }
            // else damage player 
            else
            {
                collision.collider.GetComponent<PlayerController>().loseSanity(damagePoints);
                // reset player's sanityLossLimiter
                collision.collider.GetComponent<PlayerController>().resetSanityLossLimiter();
            }
        }
    }
    */

    // OTHER methods
    public void setActiveState(bool isActive)
    {
        if (isActive)
        {
            transform.position = spawnPosition;
            canSpawn = false;
            gameObject.SetActive(true);
        }
        else
        {
            // canSpawn should turn true after cooldown
            canSpawn = true;
            gameObject.SetActive(false);
        }
    }

    public void hurt()
    {
        healthPoints--;
        if (healthPoints < 1)
            setActiveState(false);
    }
}
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


    // ACTIVATION METHODS
    // Function that activates or deactivates the enemy based on isActive parameter
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


    // DAMAGE METHODS
    // Function that hurts the enemy, substracting 1 health point
    public void hurt()
    {
        healthPoints--;
        if (healthPoints < 1)
            setActiveState(false);
    }
}
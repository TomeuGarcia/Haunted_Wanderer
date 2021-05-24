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
    public bool isDead = false;

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
    public Animator animator;


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
            isDead = false;
            gameObject.SetActive(true);
        }
        else
        {
            // canSpawn should turn true after cooldown
            canSpawn = true;
            isDead = true;
            animator.SetBool("isDead", true);
            if (gameObject.active) // if not checking if active, coroutine can start when not active - resulting in error
                StartCoroutine(Die());
        }
    }

    //Flip enemies sprites
    public void Flip()
    {
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    // DAMAGE METHODS
    // Function that hurts the enemy, substracting 1 health point
    public void Hurt()
    {
        healthPoints--;
        if (healthPoints < 1)
            setActiveState(false);
    }

    //Coroutine
    IEnumerator Die()
    {
        yield return new WaitForSeconds(0.8f);
        gameObject.SetActive(false);
    }
}
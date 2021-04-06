using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Position variables
    private Vector2 spawnPosition;

    // Movement variables
    private const float moveSpeed = 6.0f;
    private const float moveSpeedHigh = moveSpeed;
    private const float moveSpeedMedium = moveSpeed * 1.2f;
    private const float moveSpeedLow = moveSpeed * 1.4f;
    private float currentMoveSpeed;
    // Movement physics variables
    private Vector2 direction;
    private float linearDrag = 4.0f;

    // Jump (movement) variables
    private const float jumpSpeed = 10.0f;
    private float currentJumpSpeed;
    // Jump (movement) physics variables
    private bool onGround = false;
    private const float groundLenght = 0.6f;
    private Vector3 colliderOffset = new Vector3(0.45f, 0.0f, 0.0f);
    private float gravity = 1.0f;
    private float fallMultiplier = 5.0f;
    private float jumpDelay = 0.25f;
    private float jumpTimer = 0.0f;

    // Sanity variables
    public enum SanityState { HIGH, MEDIUM, LOW };
    private SanityState currentSanityState;
    public const int maxSanity = 100;
    private int currentSanity;

    private const float sanityLossCooldown = 2.0f;
    private float sanityLossTimer;
    public int sanityLossLimiter; // Can only equal 1,2,3,4,5 --- 1 = 10% , 2 = 20% , ... , 5 = 50%


    // Component variables
    public Rigidbody2D rb2;
    public Collider2D c2;
    public LayerMask groundLayer;


    void Start()
    {
        // Position
        spawnPosition = transform.position;

        // Movement
        currentMoveSpeed = moveSpeedLow;
        currentJumpSpeed = jumpSpeed;

        // Sanity
        currentSanityState = SanityState.HIGH;
        currentSanity = maxSanity;
        sanityLossTimer = 0.0f;
        sanityLossLimiter = 1;

        // Components
        rb2 = GetComponent<Rigidbody2D>();
        c2 = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if player is touching groundLayer (mask)
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLenght, groundLayer) ||
                   Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLenght, groundLayer);
        // Start jump timer when Space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpTimer = Time.time + jumpDelay;
        }

        // Get input for direction to move the player (Left: A , LeftArrow   Right: D , RightArrow)
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));



        // Cheat button: I -> gain 5 sanity
        if (Input.GetKeyDown(KeyCode.Z))
        {
            gainSanity(5);
        }
        // Cheat button: O -> lose 5 sanity
        if (Input.GetKeyDown(KeyCode.X))
        {
            loseSanity(5);
        }

    }

    private void FixedUpdate()
    {
        move(direction.x);
        if (jumpTimer > Time.time && onGround)
        {
            jump();
        }
        modifyPhysics();
    }


    // GETTER methods
    // Function that returns player's current SanityState
    public SanityState getSanityState()
    {
        if (currentSanity < maxSanity * 0.3)
            return SanityState.LOW;
        else if (currentSanity < maxSanity * 0.6)
            return SanityState.MEDIUM;
        else
            return SanityState.HIGH;
    }

    // Function that returns player's current Sanity (int)
    public int getCurrentSanity() { return currentSanity; }

    // Function that returns player's maxSanity (constant) (int)
    public int getMaxSanity() { return maxSanity; }


    // MODIFY ATTRIBUTES methods
    // Function that updates player's SanityState
    public void updateSanityState() { currentSanityState = getSanityState(); }

    // Function that updates player's movement velocity based on its sanityLevel
    public void updateMovementSpeed()
    {
        switch (currentSanityState)
        {
            case SanityState.LOW:
                currentMoveSpeed = moveSpeedLow;
                // test color red
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
            case SanityState.MEDIUM:
                currentMoveSpeed = moveSpeedMedium;
                // test color yellow
                GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            case SanityState.HIGH:
                currentMoveSpeed = moveSpeedHigh;
                // test color white
                GetComponent<SpriteRenderer>().color = Color.white;
                break;
        }
    }

    // Function that adds gainAmount of Sanity to player, Sanity cannot be equal or greater than maxSanity
    public void gainSanity(int gainAmount)
    {
        currentSanity = (currentSanity + gainAmount < maxSanity) ? currentSanity + gainAmount : maxSanity;
    }

    // Function that substracts lossAmount of Sanity to player, Sanity cannot be equal or less than 0
    public void loseSanity(int lossAmount)
    {
        currentSanity = (currentSanity - lossAmount > 0) ? currentSanity - lossAmount : 0;
    }

    // Function that makes player lose 1 point of Sanity every sanityLossCooldown seconds
    public void loseSanityViaTime()
    {
        sanityLossTimer += Time.deltaTime;
        if (sanityLossTimer >= sanityLossCooldown)
        {
            loseSanity(1);
            sanityLossTimer = 0.0f;
        }
    }

    // Function that adds +1 to sanityLossLimiter if limit (5) wasn't reached
    public void addSanityLossLimiter() { sanityLossLimiter = sanityLossLimiter < 5 ? sanityLossLimiter++ : sanityLossLimiter; }

    // Function that resets sanityLossLimiter to 1
    public void resetSanityLossLimiter() { sanityLossLimiter = 1; }


    // OTHER methods (movement and physics)
    // Function that moves the player
    private void move(float horizontal)
    {
        rb2.AddForce(Vector2.right * horizontal * currentMoveSpeed);

        if (Mathf.Abs(rb2.velocity.x) > currentMoveSpeed)
        {
            rb2.velocity = new Vector2(Mathf.Sign(rb2.velocity.x) * currentMoveSpeed, rb2.velocity.y);
        }
    }

    // Functions that lets player jump
    private void jump()
    {
        rb2.velocity = new Vector2(rb2.velocity.x, 0);
        rb2.AddForce(Vector2.up * currentJumpSpeed, ForceMode2D.Impulse);
        jumpTimer = 0.0f;
    }

    // Function that modifies drag and gravity physics for movement
    private void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rb2.velocity.x < 0) || (direction.x < 0 && rb2.velocity.x > 0);
        if (onGround)
        {
            if (Mathf.Abs(direction.x) < linearDrag * 0.1f || changingDirections)
            {
                rb2.drag = linearDrag;
            }
            else
            {
                rb2.drag = 0.0f;
            }
            rb2.gravityScale = 0.0f;
        }
        else
        {
            rb2.gravityScale = gravity;
            rb2.drag = linearDrag * 0.15f;
            // if jump height reached, multiply gravity 
            if (rb2.velocity.y < 0)
            {
                rb2.gravityScale = gravity * fallMultiplier;
            }
            // if jumping but not pressing Space, limit the jump's height
            else if (rb2.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rb2.gravityScale = gravity * (fallMultiplier / 2);
            }
        }

    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("MediumSanityPlatforms") || collision.collider.CompareTag("LowSanityPlatforms"))
        {
            //collision.collider.bounds.Contains(transform.position)
            if (collision.collider.bounds.Intersects(c2.bounds))
            {
                Debug.Log("intersection collision");

                rb2.MovePosition((Vector2)transform.position + (collision.collider.bounds.size * Vector2.up));
            }
        }
    }
    */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if Collided with player
        if (collision.collider.CompareTag("MediumSanityEnemies") || collision.collider.CompareTag("LowSanityEnemies"))
        {
            EnemyController enemy = collision.collider.GetComponent<EnemyController>();
            // if player jumped on top "kill" enemy
            if (collision.contacts[0].normal.y > 0.5)
            {
                enemy.hurt();
                // add +1 to sanityLossLimiter
                addSanityLossLimiter();
            }
            else
            {
                // hurt player
                loseSanity(enemy.damagePoints);
                // reset player's sanityLossLimiter
                resetSanityLossLimiter();
            }
        }
    }

}

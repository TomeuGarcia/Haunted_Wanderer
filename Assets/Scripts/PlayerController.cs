using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Position variables
    private Vector2 respawnPosition;
    public bool offCamera;

    //healthBar
    public HealthBar healthBar;
    public HPGained hpGained;
    //can move
    public bool canMove;

    //public int currentSanity2;
    //public int maxSanity2 = 100;

    // Movement variables
    private const float moveSpeed = 6.0f;
    private const float moveSpeedHigh = moveSpeed;
    private const float moveSpeedMedium = moveSpeed * 1.35f;
    private const float moveSpeedLow = moveSpeed * 1.7f;
    private float currentMoveSpeed;
    private bool facingRight = true;
    // Movement physics variables
    private Vector2 direction;
    private float linearDrag = 4.0f;

    // Jump (movement) variables
    private bool onWall = false;
    private const float jumpSpeed = 10.0f;
    private float currentJumpSpeed;
    private const float wallLenght = 0.6f;
    private Vector3 wallColliderOffset = new Vector3(0.0f, 0.5f, 0.0f);

    // Jump (movement) physics variables
    private bool onGround = false;
    private const float groundLength = 0.6f;
    private Vector3 groundColliderOffset = new Vector3(0.45f, 0.0f, 0.0f);
    private float gravity = 1.0f;
    private float fallMultiplier = 5.0f;
    private float jumpDelay = 0.25f;
    private float jumpTimer = 0.0f;
    public int jumpDirection = 0; // jumped left direction = -1 ; jumped no direction = 0 ; jumped right direction = 1

    // Sanity variables
    public enum SanityState { HIGH, MEDIUM, LOW };
    private SanityState currentSanityState;
    public bool canUpdateSanity;
    public const int maxSanity = 100;
    private int currentSanity;
    private int limit;

    private const float sanityLossCooldown = 2.0f;
    private float sanityLossTimer;
    public int sanityLossLimiter; // Can only equal 1,2,3,4,5 -> 1 = 10% , 2 = 20% , ... , 5 = 50%

    // Heal sanity
    public bool hasGApple;
    private int healValue;
    private bool isImmune = false;

    // Component variables
    public Rigidbody2D rb2;
    public Collider2D c2;
    private Renderer r;
    private Color c;
    public LayerMask groundLayer;
    public GameObject goldenAppleSprite1;
    public GameObject goldenAppleSprite2;



    //Animations
    public Animator animator;


    void Start()
    {
        // Position
        respawnPosition = transform.position;
        offCamera = false;

        // Movement
        currentMoveSpeed = moveSpeedLow;
        currentJumpSpeed = jumpSpeed;

        // Sanity
        currentSanityState = SanityState.HIGH;
        canUpdateSanity = true;
        currentSanity = maxSanity/2;
        healthBar.SetMaxSanity(maxSanity);
        //hpGained.sanityLimit(1);
        limit = 10;

        sanityLossTimer = 0.0f;
        sanityLossLimiter = 1;

        hasGApple = false;
        healValue = 0;
        goldenAppleSprite1.SetActive(true);
        goldenAppleSprite2.SetActive(false);

        // Components
        rb2 = GetComponent<Rigidbody2D>();
        c2 = GetComponent<Collider2D>();
        r = GetComponent<Renderer>();
        c = r.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        if (!canMove)
        {
            return;
        }
        // Check if player is touching groundLayer (mask) - walls and floor
        //onWall = Physics2D.Raycast(transform.position + wallColliderOffset, Vector2.right, wallLenght, groundLayer) ||
        //         Physics2D.Raycast(transform.position - wallColliderOffset, Vector2.right, wallLenght, groundLayer) ||
        //         Physics2D.Raycast(transform.position + wallColliderOffset, Vector2.left, wallLenght, groundLayer) ||
        //         Physics2D.Raycast(transform.position - wallColliderOffset, Vector2.left, wallLenght, groundLayer);

        onGround = Physics2D.Raycast(transform.position + groundColliderOffset, Vector2.down, groundLength, groundLayer) ||
                   Physics2D.Raycast(transform.position - groundColliderOffset, Vector2.down, groundLength, groundLayer);

        // Start jump timer when Space key is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpTimer = Time.time + jumpDelay;
        }

        // Get input for direction to move the player (Left: A , LeftArrow   Right: D , RightArrow)
        direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // Use consumible and heal sanity
        if (Input.GetKeyDown(KeyCode.Q) && hasGApple)
        {
            gainSanity(healValue);
            hasGApple = false;
            healValue = 0;
            goldenAppleSprite1.SetActive(true);
            goldenAppleSprite2.SetActive(false);
        }


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
        if (Input.GetKeyDown(KeyCode.C))
        {
                limit += 5;
        }
        // Cheat button: O -> lose 5 sanity
        if (Input.GetKeyDown(KeyCode.V))
        {
                limit -=5;
        }
        healthBar.SetHealth(currentSanity);
        hpGained.sanityLimit(limit);

    }

    private void FixedUpdate()
    {
        if (onGround && direction.x > 0)
            jumpDirection = 1;
        else if (onGround && direction.x == 0)
            jumpDirection = 0;
        else if (onGround && direction.x < 0)
            jumpDirection = -1;

        move(direction.x);

        if (jumpTimer > Time.time && onGround)
        {
            jump();
        }
        modifyPhysics();

        //Function to change the direction the sprite is loocking
        /*
        if (!facingRight && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight && moveInput < 0)
        {
            Flip();
        }
        */
    }

    //Function to change the direction the sprite is loocking
    /*
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
    */

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

    //Function that returns the limit
    public int getLimit() { return limit; }

    // Function that returns player's maxSanity (constant) (int)
    public int getMaxSanity() { return maxSanity; }


    // MODIFY ATTRIBUTES methods
    // Function that updates player's SanityState
    public void updateSanityState() { 
        if (canUpdateSanity)
        {
            currentSanityState = getSanityState();
        }
    }

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
        // Player on top of ground and wants to move
        if (onGround && horizontal != 0)
        {
            rb2.velocity = new Vector2(currentMoveSpeed * horizontal, rb2.velocity.y);
        }

        // Player in the air
        if (!onGround)
        {
            // Player slides down if contact with wall
            //if (onWall)
            //{
            //    rb2.velocity = new Vector2(0.0f, rb2.velocity.y);
            //}
            // Player can correct jump if direction is inverted in contrast to jump direction
            if ((jumpDirection == 1 && horizontal < 0) || (jumpDirection == -1 && horizontal > 0) || (jumpDirection == 0 && horizontal != 0))
            {
                rb2.velocity = new Vector2(currentMoveSpeed * horizontal * 0.5f, rb2.velocity.y);
            }
        }

        /*
        rb2.AddForce(Vector2.right * horizontal * currentMoveSpeed);

        if (Mathf.Abs(rb2.velocity.x) > currentMoveSpeed)
        {
            rb2.velocity = new Vector2(Mathf.Sign(rb2.velocity.x) * currentMoveSpeed, rb2.velocity.y);
        }
        */
    }

    // Functions that lets player jump
    private void jump()
    {
        // Apply progressive jump force
        // makes velocity force on x axis weaker
        //rb2.velocity = new Vector2(rb2.velocity.x * 0.75f, 0);
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


    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if player collided with a Hazard
        if (collision.collider.CompareTag("Hazard"))
        {
            // Check if Hazard is Spikes
            HazardController hc = GetComponent<HazardController>();
            if (hc.isSpikes)
            {
                //Hurt player 
                loseSanity(10);

                //Teleport player to last checkpoint
                transform.position = new Vector2(respawnPosition.x, respawnPosition.y);
                offCamera = true;

                //Vector3 middlePosition = collision.collider.transform.position;
                //float spikesWidth = collision.collider.GetComponent<BoxCollider2D>().size.x;
                //gameObject.transform.position = new Vector3(middlePosition.x - spikesWidth - 1, middlePosition.y, transform.position.z);
            }
        }


    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Load new respawn position if players enters checkpoint
        if (collision.CompareTag("Checkpoint"))
        {
            Checkpoint cp = collision.GetComponent<Checkpoint>();
            respawnPosition = new Vector2(cp.X, cp.Y);
        }

        // Lock player's sanity state (can't be updated)
        else if (collision.CompareTag("SanityLocker"))
        {
            canUpdateSanity = false;
        }
        // Unlock player's sanity state (can be updated)
        else if (collision.CompareTag("SanityUnlocker"))
        {
            canUpdateSanity = true;
        }

        // Add Golden apple power up for player to use
        else if (collision.CompareTag("GoldenApple"))
        {
            GoldenApple ga = collision.GetComponent<GoldenApple>();
            hasGApple = true;
            healValue = ga.healingPoints;
            Destroy(collision.gameObject);
            goldenAppleSprite1.SetActive(false);
            goldenAppleSprite2.SetActive(true);
        }

        // Check if Collided with player
        if (collision.CompareTag("Enemy") && !isImmune)
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            // if player jumped on top "kill" enemy
            if (transform.position.y > collision.transform.position.y + groundLength)
            {
                enemy.hurt();
                // add +1 to sanityLossLimiter
                addSanityLossLimiter();
                limit += 5;
            }
            else
            {
                // hurt player
                loseSanity(enemy.damagePoints);
                // reset player's sanityLossLimiter
                resetSanityLossLimiter();

                // Make player immune to enemies for 2 seconds
                StartCoroutine("Invulnerable");
            }
        }
    }


    IEnumerator Invulnerable()
    {
        isImmune = true;
        c.a = 0.5f;
        r.material.color = c;
        yield return new WaitForSeconds(2.0f);
        isImmune = false;
        c.a = 1.0f;
        r.material.color = c;
    }




}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Position variables
    private Vector2 respawnPosition;
    public bool offCamera = false;

    //can move
    public bool canMove;
    //particle system
    public ParticleSystem dust;

    private bool facingRight = true;

    // Movement
    private const float highMoveSpeed = 5f;
    private const float mediumMoveSpeed = 6f;
    private const float lowMoveSpeed = 7f;
    private float maxMoveSpeed = highMoveSpeed;
    private float moveSpeed = highMoveSpeed;
    private Vector2 direction;
    private Vector2 onJumpDirection;
    private const float moveTime = 0.1f;
    private float moveTimer = 0f;

    // Jump
    private const float maxJumpForce = 9f;
    private float jumpForce = maxJumpForce;
    private const float jumpTime = 0.3f;
    private float jumpTimer = 0f;
    private bool onGround;
    private bool jumping = false;
    private Vector3 colliderOffset = new Vector3(0.45f, 0f, 0f);
    private float groundLength = 1.5f;
    private float ceilingLength = 1.05f;
    private bool hitCeiling;
    private float startMass;

    // Sanity variables
    public enum SanityState { HIGH, MEDIUM, LOW };
    private SanityState currentSanityState = SanityState.HIGH;
    public bool canUpdateSanity = true;
    public const int maxSanity = 100;
    private int currentSanity;
    public const int maxLimiter = maxSanity / 2;
    public const int startLimiter = maxLimiter / 5;
    public int currentLimiter = startLimiter;

    private const float sanityLossCooldown = 1f;
    private float sanityLossTimer = 0f;

    public HealthBar healthbar;

    // Heal sanity
    public bool hasGApple = false;
    private int healValue = 0;
    private bool isImmune = false;


    // Component variables
    public Rigidbody2D rb2;
    public Collider2D c2;
    private Renderer r;
    private Color c;
    public LayerMask groundLayer;
    public GameObject goldenAppleSprite1;
    public GameObject goldenAppleSprite2;

    //Dash
    //public float dashSpeed;
    //private float dashTime;
    //public float startDashTime;

    //Animations
    public Animator animator;


    //shroom variables
    private float shroomTimer = 0f;
    private float shroomCooldown = 1.0f;

    [Header("Audio Elements")]
    [SerializeField] public AudioSource audio;
    [SerializeField] public AudioClip hurtedSound01;

    void Start()
    {
        // Position
        respawnPosition = transform.position;

        // Sanity
        currentSanity = (int)(maxSanity*0.9);
        healthbar.bar = maxSanity;
        healthbar.SetMaxHealth();
        healthbar.limiter = currentLimiter;
        healthbar.SetStartLimiter();

        // UI sprites
        goldenAppleSprite1.SetActive(true);
        goldenAppleSprite2.SetActive(false);

        // Components
        rb2 = GetComponent<Rigidbody2D>();
        c2 = GetComponent<Collider2D>();
        r = GetComponent<Renderer>();
        c = r.material.color;
        startMass = rb2.mass;
        //Dash
        //dashTime = startDashTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove)
        {
            return;
        }


        if (!hitCeiling && jumping)
            hitCeiling = Physics2D.Raycast(transform.position + colliderOffset, Vector2.up, ceilingLength, groundLayer) ||
                         Physics2D.Raycast(transform.position - colliderOffset, Vector2.up, ceilingLength, groundLayer);

        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
                   Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);

        // JUMP
        if (Input.GetButtonDown("Jump") && onGround)
        {
            CreateDust();
            hitCeiling = false;
            jumping = true;
            jumpTimer = 0f;
            jumpForce = maxJumpForce;
            onJumpDirection = direction;
        }
        else if (Input.GetButtonUp("Jump"))
            jumping = false;

        if (jumping)
            jump();

        else if (!onGround && !jumping)
            CreateDust();

        // MOVE
        // Get direction from input
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        
        if (direction.x != 0)
            move();
        else
        {
            if (onGround)
                move();
            moveTimer = 0f;
        }

        if (onGround)
        {
            hitCeiling = false;
            rb2.mass = startMass;
        }
            
        if (hitCeiling)
        {
            hitCeiling = false;
            jumping = false;
            rb2.velocity = new Vector2(rb2.velocity.x, 0f);
        }

        


        // CONTROLS
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
            currentLimiter += startLimiter;
        }
        // Cheat button: O -> lose 5 sanity
        if (Input.GetKeyDown(KeyCode.V))
        {
            currentLimiter -= startLimiter;
        }
        healthbar.bar = currentSanity;
        healthbar.SetHealth();
        healthbar.limiter = currentLimiter;
        healthbar.SetLimiter();


        //Change animation between IDLE / WALK / RUN 
        animator.SetFloat("Speed", Mathf.Abs(direction.x));

        if (currentSanity == 0)
        {
            animator.SetInteger("Died", 3);
        }

    }

    private void FixedUpdate()
    {
        //Function to change the direction the sprite is loocking
        if ((!facingRight && direction.x > 0) || (facingRight && direction.x < 0))
        {
            Flip();
        }

        //Dash
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //{
        //    if (!facingRight)
        //    {
        //        rb2.velocity = new Vector2(0, rb2.velocity.y);
        //        //rb2.AddForce(Vector2.left * dashSpeed, ForceMode2D.Impulse);
        //        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 10, transform.position.y, transform.position.z), Time.deltaTime*100);
        //    }
        //    else
        //    {
        //        rb2.velocity = new Vector2(0, rb2.velocity.y);
        //        //rb2.AddForce(Vector2.right * dashSpeed, ForceMode2D.Impulse);
        //        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 10, transform.position.y, transform.position.z), Time.deltaTime*100);
        //    }
        //}

    }

    //Function to change the direction the sprite is loocking
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
        CreateDust();
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
                maxMoveSpeed = lowMoveSpeed;
                animator.SetInteger("Died", 2);
                break;
            case SanityState.MEDIUM:
                maxMoveSpeed = mediumMoveSpeed;
                animator.SetInteger("Died", 1);
                break;
            case SanityState.HIGH:
                maxMoveSpeed = highMoveSpeed;
                animator.SetInteger("Died", 0);
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
    public void addSanityLossLimiter() { currentLimiter += (currentLimiter < maxLimiter) ? (startLimiter) : 0; }

    // Function that resets sanityLossLimiter to 1
    public void resetSanityLossLimiter() { currentLimiter = startLimiter; }


    // OTHER methods (movement and physics)
    // Function that moves the player
    private void move()
    {
        // Moving while on the floor
        if (onGround)
        {
            if (moveTimer < moveTime)
            {
                moveTimer += Time.deltaTime;
                rb2.velocity = new Vector2(direction.x * moveSpeed / 3, rb2.velocity.y);
            }
            else
            {
                moveSpeed = maxMoveSpeed;
                rb2.velocity = new Vector2(direction.x * moveSpeed, rb2.velocity.y);
            }
        }

        // Moving while in the air
        else
        {
            if (onJumpDirection != direction)
            {
                onJumpDirection = direction;
                moveSpeed = maxMoveSpeed / 2f;
            }
            rb2.velocity = new Vector2(direction.x * moveSpeed, rb2.velocity.y);
        }
    }

    // Functions that lets player jump
    private void jump()
    {
        if (jumpTimer < jumpTime)
        {
            rb2.velocity = new Vector2(rb2.velocity.x, jumpForce);
            jumpTimer += Time.deltaTime;
            rb2.mass += 0.2f;
        }
        else
            jumping = false;
    }



    private void OnCollisionEnter2D(Collision2D collision)
    { 
        // Check if player collided with a Hazard
        if (collision.collider.CompareTag("Hazard"))
        {
            // Check if Hazard is Spikes
            HazardController hc = collision.collider.GetComponent<HazardController>();
            if (hc.isSpikes)
            {
                //Hurt player 
                loseSanity(10);

                //Teleport player to last checkpoint
                transform.position = new Vector2(respawnPosition.x, respawnPosition.y);
                offCamera = true;
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

        // Check if Collided with enemy
        else if (collision.CompareTag("Enemy") && !isImmune)
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            // if player jumped on top "kill" enemy
            if (transform.position.y > collision.transform.position.y && (transform.position.x < collision.transform.position.x + 0.4 && transform.position.x > collision.transform.position.x - 0.4))
            {
                enemy.hurt();
                addSanityLossLimiter();
            }
            else
            {
                audio.PlayOneShot(hurtedSound01);
                // hurt player
                loseSanity(enemy.damagePoints);
                // reset player's sanityLossLimiter
                resetSanityLossLimiter();

                // Make player immune to enemies for 2 seconds
                StartCoroutine("Invulnerable");
            }
        }

        else if (collision.CompareTag("MovingPlatform"))
        {
            //jumping = false;
            transform.parent = collision.gameObject.transform;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Mushrooms"))
        {
            if (shroomTimer < shroomCooldown)
            {
                shroomTimer += Time.deltaTime;
                if(shroomTimer >= shroomCooldown)
                {
                    loseSanity(2);
                    shroomTimer = 0.0f;
                }
            }
        }
        // Add Golden apple power up for player to use
        else if (collision.CompareTag("GoldenApple") && !hasGApple)
        {
            GoldenApple ga = collision.GetComponent<GoldenApple>();
            hasGApple = true;
            healValue = ga.healingPoints;
            Destroy(collision.gameObject);
            goldenAppleSprite1.SetActive(false);
            goldenAppleSprite2.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
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




    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.up * ceilingLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.up * ceilingLength);
    }

    void CreateDust()
    {
        dust.Play();
    }

}

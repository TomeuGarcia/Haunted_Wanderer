using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    // Position variables
    public Vector2 respawnPosition;
    public bool offCamera = false;

    // Flag that determines if player can Move
    public bool canMove;

    // Particles
    public ParticleSystem dust;

    // Movement
    private const float highMoveSpeed = 6f;// 7f;
    private const float mediumMoveSpeed = 7f;//8f;
    private const float lowMoveSpeed = 8f;//9f;
    private float maxMoveSpeed = highMoveSpeed;
    public float moveSpeed = highMoveSpeed;
    private Vector2 direction;
    private Vector2 onJumpDirection;
    private const float moveTime = 0.1f;
    private float moveTimer = 0f;

    // Jump
    public float maxJumpForce;
    public float jumpForce = 0.0f;
    public const float jumpTime = 0.2f;
    private float jumpTimer = 0f;
    private bool onGround;
    private bool jumping = false;
    private Vector3 colliderOffset = new Vector3(0.45f, 0f, 0f);
    private float groundLength = 1.5f;
    private float ceilingLength = 1.05f;
    private bool hitCeiling;
    private float startMass;
    public float massIncrement;

    private bool stompEnemy = false;

    // Sanity variables
    public enum SanityState { HIGH, MEDIUM, LOW };
    public SanityState currentSanityState = SanityState.HIGH;
    public bool canUpdateSanity = true;
    public const int maxSanity = 100;
    public int currentSanity = maxSanity;
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
    public LayerMask enemyLayer;
    public GameObject goldenAppleSprite1;
    public GameObject goldenAppleSprite2;

    //Dash
    //public float dashSpeed;
    //private float dashTime;
    //public float startDashTime;

    //Animations
    private bool facingRight = true;
    public Animator animator;


    //shroom variables
    private float shroomTimer = 0f;
    private float shroomCooldown = 1.0f;

    [Header("Audio Elements")]
    [SerializeField] public AudioSource audio;
    [SerializeField] public AudioClip hurtedSound01;
    [SerializeField] public AudioClip biteApple;
    [SerializeField] public AudioClip fallingOnSpikes;

    void Start()
    {
        // Set default respawn position
        respawnPosition = transform.position;

        // Set default sanity 
        canUpdateSanity = true;
        healthbar.bar = maxSanity;
        healthbar.SetMaxHealth();
        healthbar.limiter = currentLimiter;
        healthbar.SetStartLimiter();

        // UI sprites
        goldenAppleSprite1.SetActive(true);
        goldenAppleSprite2.SetActive(false);

        // Get game object's components
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
        // CONTROLS
        // Use consumible and heal sanity
        if (Input.GetKeyDown(KeyCode.Q) && hasGApple)
        {
            EatApple();
        }

        // Cheat button: I -> gain 5 sanity
        if (Input.GetKeyDown(KeyCode.Z))
            GainSanity(5);
        // Cheat button: O -> lose 5 sanity
        if (Input.GetKeyDown(KeyCode.X))
            LoseSanity(5);

        // Cheat button: C -> reset sanityLimiter
        if (Input.GetKeyDown(KeyCode.C))
            ResetSanityLimiter();
        // Cheat button: V -> increment sanityLimiter 
        if (Input.GetKeyDown(KeyCode.V))
            IncrementSanityLimiter();


        // check if player hit the ceiling
        if (!hitCeiling && jumping)
            hitCeiling = Physics2D.Raycast(transform.position + colliderOffset, Vector2.up, ceilingLength, groundLayer) ||
                         Physics2D.Raycast(transform.position - colliderOffset, Vector2.up, ceilingLength, groundLayer);

        // check if player hit the ground
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
                   Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        
        // If the player hit the ground, reset mass
        if (onGround)
            rb2.mass = startMass;

        // JUMP
        // if "Jump" button was pressed while player on ground, enable jumping
        if (Input.GetButtonDown("Jump") && onGround)
        {
            CreateDust();
            jumping = true;
            jumpTimer = 0f;
            jumpForce = maxJumpForce;
            onJumpDirection = direction;
        }
        // if "Jump" button was unpressed OR player hit the ceiling, disable jumping
        else if (Input.GetButtonUp("Jump") || hitCeiling)
        {
            hitCeiling = false;
            jumping = false;
        }

        // MOVE
        // Get movement direction from user's inputs
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));


        // ANIMATIONS
        // Change animation between IDLE / WALK / RUN 
        animator.SetFloat("Speed", Mathf.Abs(direction.x));

        // if player has no sanity left, play Death animation
        if (currentSanity == 0)
            animator.SetInteger("Died", 3);

        // Flip player's sprite
        if ((!facingRight && direction.x > 0) || (facingRight && direction.x < 0))
            Flip();


        // CAMERA
        if (offCamera && canMove)
            canMove = false;
        else if (!offCamera && !canMove)
            canMove = true;
    }

    private void FixedUpdate()
    {
        // if flag disabled, don't execute any movement 
        if (!canMove)
            return;

        // JUMP
        // if jumping enabled, execute Jump()
        if (jumping)
            Jump();

        // MOVE
        if (direction.x != 0)
            Move();
        else
        {
            if (onGround)
                Move();
            moveTimer = 0f;
        }

        //// DASH
        //if (Input.GetKeyDown(KeyCode.LeftShift))
        //    Dash();

    }



    // MOVEMENT / PHYSICS RELATED METHODS
    // Function that moves the player
    private void Move()
    {
        // Moving while on the ground
        if (onGround)// || (!onGround && direction.x != 0))
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
            //if (onJumpDirection.x != 0)
            //{
            //    onJumpDirection = direction;
            //    moveSpeed = maxMoveSpeed / 1.5f;
            //}
            if (onJumpDirection != direction && direction.x != 0)
            {
                //Debug.Log("move in air");
                onJumpDirection = direction;
                moveSpeed = maxMoveSpeed / 2f;
            }
            else
            {
                moveSpeed = maxMoveSpeed + 1.5f; //0.5f; // make movement on air slightly faster
            }
            rb2.velocity = new Vector2(direction.x * moveSpeed, rb2.velocity.y);
        }
    }

    // Functions that lets player Jump
    private void Jump()
    {
        if (jumpTimer < jumpTime)
        {
            rb2.velocity = new Vector2(rb2.velocity.x, jumpForce);
            jumpTimer += Time.deltaTime;
            //rb2.mass += massIncrement;
        }
        else
            jumping = false;
    }

    private void Dash()
    {
        if (!facingRight)
        {
            rb2.velocity = new Vector2(0, rb2.velocity.y);
            //rb2.AddForce(Vector2.left * dashSpeed, ForceMode2D.Impulse);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x - 10, transform.position.y, transform.position.z), Time.deltaTime * 100);
        }
        else
        {
            rb2.velocity = new Vector2(0, rb2.velocity.y);
            //rb2.AddForce(Vector2.right * dashSpeed, ForceMode2D.Impulse);
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x + 10, transform.position.y, transform.position.z), Time.deltaTime * 100);
        }
    }


    private void BounceOnEnemy()
    {
        rb2.mass = startMass;
        rb2.AddForce(transform.up * 130, ForceMode2D.Impulse);
    }


    public void KnockedForward(Vector2 knockForce)
    {
        rb2.AddForce(knockForce, ForceMode2D.Impulse);
    }


    // SANITY RELATED METHODS

    // Function that returns player's current SanityState
    public SanityState GetSanityState()
    {
        if (currentSanity < maxSanity * 0.33)
            return SanityState.LOW;
        else if (currentSanity < maxSanity * 0.66)
            return SanityState.MEDIUM;
        else
            return SanityState.HIGH;
    }


    // Function that returns player's current Sanity (int)
    public int GetCurrentSanity() { return currentSanity; }


    // Function that returns player's maxSanity (constant) (int)
    public int GetMaxSanity() { return maxSanity; }


    // Function that updates player's SanityState
    public void UpdateSanityState() { 
        if (canUpdateSanity)
            currentSanityState = GetSanityState();
    }


    // Function that updates player's movement velocity based on its sanityLevel
    public void UpdateMovementSpeed()
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
    public void GainSanity(int gainAmount)
    {
        currentSanity = (currentSanity + gainAmount < maxSanity) ? currentSanity + gainAmount : maxSanity;
        UpdateHealthbarBar();
    }

    // Function that substracts lossAmount of Sanity to player, Sanity cannot be equal or less than 0
    public void LoseSanity(int lossAmount)
    {
        currentSanity = (currentSanity - lossAmount > 0) ? currentSanity - lossAmount : 0;
        UpdateHealthbarBar();
    }


    // Function that makes player lose 1 point of Sanity every sanityLossCooldown seconds
    public void LoseSanityViaTime()
    {
        sanityLossTimer += Time.deltaTime;
        if (sanityLossTimer >= sanityLossCooldown)
        {
            LoseSanity(1);
            sanityLossTimer = 0.0f;
        }
    }


    // Function that adds increments currentLimiter if limit, can't exceed maxLimiter
    public void IncrementSanityLimiter()
    {
        currentLimiter = (currentLimiter < maxLimiter) ? currentLimiter + startLimiter : currentLimiter;
        UpdateHealthbarLimiter();
    }


    // Function that resets currentLimiter to startLimiter
    public void ResetSanityLimiter()
    {
        currentLimiter = startLimiter;
        UpdateHealthbarLimiter();
    }


    private void EatApple()
    {
        audio.PlayOneShot(biteApple, 0.5f);
        GainSanity(healValue);
        hasGApple = false;
        healValue = 0;
        goldenAppleSprite1.SetActive(true);
        goldenAppleSprite2.SetActive(false);
    }



    // UI RELATED METHODS
    // Function that updates the HealthBar 
    private void UpdateHealthbarBar()
    {
        healthbar.bar = currentSanity;
        healthbar.SetHealth();
    }


    // Function that updates the HealthBar's sanity Limiter
    public void UpdateHealthbarLimiter()
    {
        healthbar.limiter = currentLimiter;
        healthbar.SetLimiter();
    }


    
    // COLLISION METHODS
    private void OnCollisionEnter2D(Collision2D collision)
    { 
        // Check if player collided with a Hazard
        if (collision.collider.CompareTag("Hazard"))
        {
            // Check if Hazard is Spikes
            HazardController hc = collision.collider.GetComponent<HazardController>();
            if (hc.isSpikes)
            {
                // Play sound
                audio.PlayOneShot(fallingOnSpikes, 0.5f);

                //Hurt player 
                LoseSanity(10);

                //Teleport player to last checkpoint
                transform.position = respawnPosition;
                offCamera = true;
                canMove = false;
            }
        }
    }



    // TRIGGER METHODS
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Lock player's sanity state (can't be updated)
        if (other.CompareTag("SanityLocker"))
        {
            canUpdateSanity = false;
        }
        // Unlock player's sanity state (can be updated)
        else if (other.CompareTag("SanityUnlocker"))
        {
            canUpdateSanity = true;
        }

        // Check if Collided with enemy
        else if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            //skip collision if enemy is dying (playing death acyion)
            if (enemy.isDead) { return; }


            //stompEnemy = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, enemyLayer) ||
            //             Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, enemyLayer);

            // if player jumped on top "kill" enemy
            if (transform.position.y > enemy.transform.position.y && (transform.position.x < enemy.transform.position.x + 0.5 && transform.position.x > enemy.transform.position.x - 0.5))
            //if (stompEnemy)
            {
                enemy.Hurt();
                IncrementSanityLimiter();
                BounceOnEnemy();
            }
            else if (!isImmune)
            {
                audio.PlayOneShot(hurtedSound01);
                // Hurt player
                LoseSanity(enemy.damagePoints);
                // reset player's sanityLossLimiter
                ResetSanityLimiter();

                // Make player immune to enemies for 2 seconds
                StartCoroutine("Invulnerable");
            }
        }
        else if (other.CompareTag("Bullet") && !isImmune)
        {
            Bullet bullet = other.GetComponent<Bullet>();
            audio.PlayOneShot(hurtedSound01);
            // Hurt player
            LoseSanity(bullet.damagePoints);
            // reset player's sanityLossLimiter
            ResetSanityLimiter();

            // Make player immune to enemies for 2 seconds
            StartCoroutine("Invulnerable");
        }

        else if (other.CompareTag("MovingPlatform"))
        {
            //jumping = false;
            transform.parent = other.gameObject.transform;
        }
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Mushrooms"))
        {
            if (shroomTimer < shroomCooldown)
            {
                shroomTimer += Time.deltaTime;
                if(shroomTimer >= shroomCooldown)
                {
                    LoseSanity(2);
                    shroomTimer = 0.0f;
                }
            }
        }
        // Add Golden apple power up for player to use
        else if (other.CompareTag("GoldenApple") && !hasGApple)
        {
            GoldenApple ga = other.GetComponent<GoldenApple>();
            hasGApple = true;
            healValue = ga.healingPoints;
            Destroy(other.gameObject);
            goldenAppleSprite1.SetActive(false);
            goldenAppleSprite2.SetActive(true);
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MovingPlatform"))
        {
            transform.parent = null;
        }
    }

    //Function that changes the sprite's direction
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
        CreateDust();
    }


    // PARTICLE RELATED METHODS
    // Function that spawns dust particles
    void CreateDust()
    {
        dust.Play();
    }
    
    
    
    // COROUTINES
    // Coroutine that makes the player invulnerable for 2 seconds and makes player's sprite fade
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

    // GIZMOS METHODS
    // Function that draws lines repressenting onGround and onCeiling
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.up * ceilingLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.up * ceilingLength);
    }

}

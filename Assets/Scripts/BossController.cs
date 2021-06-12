using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private bool didReset = false;
    private bool reset = false;

    // Spawn
    private bool startSpawning = false;
    private bool hasSpawned = false;
    private const float spawnTime = 3f;
    private float spawnTimer = 0f;
    public BoxCollider2D spawnTrigger;

    public GameObject spawnCamWall;
    private Vector2 camWallEndPosition = new Vector2(200, 0);


    // Movement
    private float movementSpeed;
    private bool canMove;
    private const float waitTime = 2f;
    private float waitTimer = 0f;
    private bool waitStill = false;

    // Sanity
    //public enum SanityState { HIGH, MEDIUM, LOW };
    private PlayerController.SanityState currentSanityState = PlayerController.SanityState.HIGH;

    // Melee Attack
    private const int meleeDamage = 10;

    // Slime Spit Attack
    private Vector2 mouthPosition;

    // Charge Attack
    private bool charging = false;
    private const float chargeTime = 0.8f;
    private float chargeTimer = 0f;
    private bool resting = false;
    private const float restingTime = 0.5f;
    private float restingTimer = 0f;
    private float chargeSpeed = 17f;


    // Other GameObjects
    public PlayerController player;
    private float distanceToPlayer;
    
    public Slime slimeCopy;

    // Components
    public Collider2D bossCollider;
    private Rigidbody2D rb2;

    [Header("Audio Elements")]
    [SerializeField] public AudioSource audio;
    [SerializeField] public AudioClip spit;


    public Animator animator;

    void Start()
    {
        //spitTimer = spitTime;

        rb2 = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Firstly check if is spawning
        if (startSpawning)
        {
            Spawn();
        }
        if (!hasSpawned)
        {
            return;
        }


        // Reset if player was sent back to a checkpoint
        if (!didReset && player.offCamera) didReset = reset = true;
        else if (!player.offCamera) didReset = reset = false;


        // update Sanity State
        UpdateSanityState();

        // Update movement speed
        if (!resting && !charging)
            UpdateMoveSpeed(player.moveSpeed);
        else if (resting && !charging)
            UpdateMoveSpeed(0f);
        else if (!resting && charging)
            UpdateMoveSpeed(chargeSpeed);

        if (player.canMove && !waitStill) canMove = true;


        if (waitStill) WaitStill();


        // Updated distance between Boss and Player
        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        // Update mouth position
        mouthPosition = new Vector2(transform.position.x + 2f, transform.position.y + 2f); // test coord

    }

    private void FixedUpdate()
    {
        // Firstly check if is spawning
        if (!hasSpawned) return;

        if (charging || resting)
            Charge();

        if (canMove)
            Move();



        // Move boss with player if teleported to checkpoint 
        if (player.offCamera)
        {
            transform.position = new Vector2(player.transform.position.x - 12f, transform.position.y);
            canMove = false;
        }
    }



    private void UpdateMoveSpeed(float speed)
    {
        movementSpeed = speed;
    }


    // Move self towards player
    private void Move()
    {
        //if (distanceToPlayer > 10)
        //{
        //    Vector2 positionToMove = new Vector2(player.transform.position.x, transform.position.y);
        //    transform.position = Vector2.MoveTowards(transform.position, positionToMove, movementSpeed * Time.deltaTime);
        //}
        Vector2 positionToMove = new Vector2(player.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, positionToMove, movementSpeed * Time.deltaTime);
    }


    // Update Boss' sanity state
    private void UpdateSanityState()
    {
        if (currentSanityState != player.currentSanityState)
        {
            currentSanityState = player.currentSanityState;
            
            // Update sprite
            if (currentSanityState == PlayerController.SanityState.HIGH)
            {
                animator.SetInteger("Sanity", 1);
            }
            else if (currentSanityState == PlayerController.SanityState.MEDIUM)
            {
                animator.SetInteger("Sanity", 2);
                chargeTimer = 0f;
            }
            else    //else if (currentSanityState == PlayerController.SanityState.LOW)
            {
                animator.SetInteger("Sanity", 3);
            }
        }
            
    }


    // Spawn the boss
    private void Spawn()
    {
        // start spawn animation

        // count
        if (spawnTimer < spawnTime)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer > spawnTime - 1.9f)
            {
                // move spawnCamWall
                MoveCameraRightWall();
                player.canMove = false;
            }
        }
        else
        {
            spawnTimer = 0f; // not needed
            startSpawning = false;
            hasSpawned = true;

            player.canMove = true;

            //bossCollider.isTrigger = false;
            // deactivate spawnTrigger
        }

    }

    private void MoveCameraRightWall()
    {
        spawnCamWall.transform.position = camWallEndPosition;
    }



    // Spit a Slime
    private void SpitSlime()
    {
        audio.PlayOneShot(spit, 0.5f);
        GameObject slimeClone = Instantiate(slimeCopy.gameObject, mouthPosition, Quaternion.identity);
        Rigidbody2D rbSlime = slimeClone.GetComponent<Rigidbody2D>();
        rbSlime.AddForce(new Vector2(12f, 9f), ForceMode2D.Impulse);
    }


    // Charge 
    private void Charge()
    {
        if (charging)
            ChargeCounter();
        if (resting)
            RestingCounter();
    }

    private void ChargeCounter()
    {
        if (chargeTimer < chargeTime)
        {
            chargeTimer += Time.deltaTime;
        }
        else
        {
            chargeTimer = 0f;
            charging = false;
            resting = true;
        }
    }

    private void RestingCounter()
    {
        if (restingTimer < restingTime)
        {
            restingTimer += Time.deltaTime;
        }
        else
        {
            restingTimer = 0f;
            resting = false;
        }
    }


    private void WaitStill()
    {
        if (waitTimer < waitTime)
        {
            waitTimer += Time.deltaTime;
        }
        else
        {
            waitTimer = 0f;
            waitStill = false;
            canMove = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // When collides with player
        if (other.CompareTag("Player"))
        {
            // Spawn Boss
            if (other.IsTouching(spawnTrigger) && !hasSpawned)
            {
                startSpawning = true;
                Debug.Log("BOSS SPAWNS");
            }

            // Damage player
            if (other.IsTouching(bossCollider) && hasSpawned)
            {
                player.KnockedForward(new Vector2(500, 100));
                waitStill = true;
                canMove = false;
                player.LoseSanity(meleeDamage);
                Debug.Log("hit player");
            }
        }

        // Spit Slimes (only SanityState = HIGH)
        else if (other.CompareTag("SlimeSpitTrigger") && currentSanityState == PlayerController.SanityState.HIGH)
        {
            if (other.IsTouching(bossCollider))
            {
                SpitSlime();
                Debug.Log("spit");
            }
        }

        // Charge (only SanityState = MEDIUM)
        else if (other.CompareTag("ChargeAttackTrigger") && currentSanityState == PlayerController.SanityState.MEDIUM)
        {
            if (other.IsTouching(bossCollider))
            {
                charging = true;
                chargeTimer = restingTimer = 0f; //
                Debug.Log("charge");
            }
        }


    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private bool didReset = false;
    private bool reset = false;

    // Movement
    private float movementSpeed;

    // Sanity
    //public enum SanityState { HIGH, MEDIUM, LOW };
    private PlayerController.SanityState currentSanityState = PlayerController.SanityState.HIGH;

    // Melee Attack
    private const int meleeDamage = 20;
    private const float meleeAtkTime = 2f;
    private float meleeAtkTimer = 0f;

    // Slime Spit Attack
    private Vector2 mouthPosition;

    // Charge Attack
    private bool charging = false;
    private const float chargeTime = 5f;
    private float chargeTimer = 0f;
    private bool resting = false;
    private const float restingTime = 3f;
    private float restingTimer = 0f;
    private Vector2 chargeSpeed = new Vector2(30, 0);


    // Other GameObjects
    public PlayerController player;
    private float distanceToPlayer;
    
    public Slime slimeCopy;

    // Components
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
        // Reset if player was sent back to a checkpoint
        if (!didReset && player.offCamera) didReset = reset = true;
        else if (!player.offCamera) didReset = reset = false;


        // update Sanity State
        UpdateSanityState();

        // Update movement speed
        if (!resting && !charging)
            UpdateMoveSpeed();

        // Updated distance between Boss and Player
        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);

        // Update mouth position
        mouthPosition = new Vector2(transform.position.x + 2f, transform.position.y + 2f); // test coord
    }

    private void FixedUpdate()
    {
        if (!resting && !charging)
            Move();

        CheckCharge();        


        // Move boss with player if teleported to checkpoint 
        if (player.offCamera)
        {
            transform.position = new Vector2(player.transform.position.x - 12f, transform.position.y);
        }
    }



    private void UpdateMoveSpeed()
    {
        movementSpeed = player.moveSpeed;
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


    // Spit a Slime
    private void SpitSlime()
    {
        audio.PlayOneShot(spit, 0.5f);
        GameObject slimeClone = Instantiate(slimeCopy.gameObject, mouthPosition, Quaternion.identity);
        Rigidbody2D rbSlime = slimeClone.GetComponent<Rigidbody2D>();
        rbSlime.AddForce(new Vector2(12f, 9f), ForceMode2D.Impulse);
    }


    // Charge 
    private void CheckCharge()
    {
        if (currentSanityState == PlayerController.SanityState.MEDIUM)
        {
            if (!resting)
            {
                ChargeCounter();
                if (charging)
                {
                    rb2.AddForce(chargeSpeed, ForceMode2D.Impulse);
                    charging = false;
                    resting = true;
                }
            }
            else
            {
                RestingCounter();
            }
        }
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
            charging = true;
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



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("SlimeSpitTrigger") && currentSanityState == PlayerController.SanityState.HIGH)
        {
            SpitSlime();
        }
    }


}

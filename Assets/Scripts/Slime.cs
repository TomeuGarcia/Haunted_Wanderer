using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : EnemyController
{
    // Unicque SlimeController variables
    private const float moveCooldown = 1.5f;
    private float moveTimer;

    private bool onGround = false;
    private const float groundLength = 0.2f;

    private bool onRightWall = false;
    private bool onLeftWall = false;
    private bool onWall = false;
    private const float wallLength = 0.6f;
    private bool facingRight = true;
    private bool cantMove = false;
    private Vector2 vectorEnemyPlayer;

    // Component variables
    private Rigidbody2D rb2;
    public LayerMask groundLayer;

    private Collider2D collider;
    private Vector2 startCollSize;


    void Start()
    {
        damagePoints = 20;
        healthPoints = 1;
        moveSpeed = 3.0f;
        jumpSpeed = 5.0f;
        sightDistance = 12.0f;
        moveTimer = 0.0f;

        rb2 = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        startCollSize = collider.bounds.size;
    }

    private void FixedUpdate()
    {
        onRightWall = Physics2D.Raycast(transform.position, Vector2.right, wallLength, groundLayer);
        onLeftWall = Physics2D.Raycast(transform.position, Vector2.left, wallLength, groundLayer);
        onWall = onRightWall || onLeftWall;   
        facingRight = player.transform.position.x > transform.position.x;
        cantMove = (facingRight && onRightWall) || (!facingRight && onLeftWall);

        vectorEnemyPlayer = player.transform.position - transform.position;


        if (onWall && !onGround)
            rb2.velocity = new Vector2(0, -5);

        if (!onGround && Physics2D.Raycast(transform.position, Vector2.down, groundLength, groundLayer))
            rb2.velocity = new Vector2(0, 0);

        onGround = Physics2D.Raycast(transform.position, Vector2.down, groundLength, groundLayer);
        if (onGround)
        {
            rb2.gravityScale = 0;
            if (!cantMove)
                Move();

        }
        else
        {
            rb2.gravityScale = 1;
        }

        Attack();
    }


    private void Move()
    {
        // Move if player is within range of sightDistance
        if (Mathf.Abs(vectorEnemyPlayer.magnitude) < sightDistance)
        {
            // Slime moves (jumps) once every moveCooldown (1.5 seconds)
            moveTimer += Time.deltaTime;
            if (moveTimer >= moveCooldown && onGround)
            {
                // Move to the right if player is located to the right of the Slmie
                if (vectorEnemyPlayer.x > 0)
                {
                    rb2.velocity = new Vector2(moveSpeed, jumpSpeed);
                    facingRight = true;
                }
                // Move to the left if player is located to the left of the Slmie
                else
                {
                    rb2.velocity = new Vector2(-moveSpeed, jumpSpeed);
                    facingRight = false;
                }
                moveTimer = 0.0f;
                animator.SetBool("isJumping", false);
            }
            else if (moveTimer >= moveCooldown - Time.deltaTime*10 && onGround)
            {
                animator.SetBool("isJumping", true);
            }
        }
    }
    private void Attack()
    {
        if(vectorEnemyPlayer.magnitude < 1f)
        {
            GetComponent<BoxCollider2D>().size = new Vector2(2f, 1.8f);
            animator.SetBool("isAttacking", true);
        }

    }
}
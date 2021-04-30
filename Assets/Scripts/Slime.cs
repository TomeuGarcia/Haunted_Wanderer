﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : EnemyController
{
    // Unicque SlimeController variables
    private const float moveCooldown = 1.5f;
    private float moveTimer;

    private bool onGround = false;
    private const float groundLength = 0.6f;

    // Component variables
    private Rigidbody2D rb2;
    public LayerMask groundLayer;


    void Start()
    {
        damagePoints = 20;
        healthPoints = 1;
        moveSpeed = 3.0f;
        jumpSpeed = 5.0f;
        sightDistance = 12.0f;
        moveTimer = 0.0f;

        rb2 = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        onGround = Physics2D.Raycast(transform.position, Vector2.down, groundLength, groundLayer);
    }


    void FixedUpdate()
    {
        move();
    }


    private void move()
    {
        Vector2 vectorEnemyPlayer = player.transform.position - transform.position;
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
                }
                // Move to the left if player is located to the left of the Slmie
                else
                {
                    rb2.velocity = new Vector2(-moveSpeed, jumpSpeed);
                }
                moveTimer = 0.0f;
            }
        }
    }


}
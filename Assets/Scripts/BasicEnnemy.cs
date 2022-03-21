using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

enum BasicEnnemyEtats
{
    IDLE,
    CHASING,
    ATTACKING
}

public class BasicEnnemy : Ennemy
{

    private Rigidbody2D rigidbodyEnnemy;
    
    private BasicEnnemyEtats state;
    private Vector2 startPos;
    private int direction = 1;
    public float moveSpeed = 6.0f;
    public float moveRadius = 2.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        state = BasicEnnemyEtats.IDLE;
        rigidbodyEnnemy = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        switch (state)
        {
            case BasicEnnemyEtats.IDLE:
                Move();

                break;
            case BasicEnnemyEtats.CHASING:
                
                break;
            case BasicEnnemyEtats.ATTACKING:

                break;
        }
    }

    override public void Attack()
    {
        
    }

    override public void Damage()
    {
        
    }


    override public void Parried()
    {
        
    }

    override public void Die()
    {
              
    }

    private void Move()
    {
        if (direction == 1)
        {
            if (transform.position.x <= startPos.x + moveRadius)
            {
                rigidbodyEnnemy.velocity = new Vector2(moveSpeed, 0);
            }
            else
            {
                direction = -1;
            }
        }
        else if (direction == -1)
        {
            if (transform.position.x >= startPos.x - moveRadius)
            {
                rigidbodyEnnemy.velocity = new Vector2(-moveSpeed, 0);
            }
            else
            {
                direction = 1;
            }
        } 
    }
}

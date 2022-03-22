using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEditor;
using UnityEngine;

enum BasicEnnemyEtats
{
    IDLE,
    CHASING,
    ATTACKING,
    STUNNED
}

public class BasicEnnemy : Ennemy
{

    [SerializeField] private LayerMask platformLayerMask;
    
    public GameObject player;
    
    private Rigidbody2D rigidbodyEnemy;
    private CircleCollider2D colliderEnemy;
    
    private BasicEnnemyEtats state;
    private Vector2 startPos;
    private int direction = 1;
    public float moveSpeed = 2.0f;
    public float moveRadius = 2.0f;

    private bool vulnerable = true;
    private CircleCollider2D colliderAttack;
    private bool attack_started = false;
    
    public float detectionRadius = 6.0f;
    public float attackRadius = 3.0f;

    private bool coroutine_attack_running = false;
    private bool attack_cooldown = false;
    private bool is_stunned = false;
    
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        state = BasicEnnemyEtats.IDLE;
        rigidbodyEnemy = GetComponent<Rigidbody2D>();
        colliderEnemy = GetComponent<CircleCollider2D>();
        colliderAttack = gameObject.transform.GetChild(0).GetComponent<CircleCollider2D>();
        colliderAttack.enabled = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsGrounded()) rigidbodyEnemy.gravityScale = 4;
        else if (IsGrounded()) rigidbodyEnemy.gravityScale = 1;
        
        switch (state)
        {
            case BasicEnnemyEtats.IDLE:
                Debug.Log("IDLE");
                Move();
                if (Math.Abs(player.transform.position.x - transform.position.x) <= detectionRadius && Math.Abs(player.transform.position.y - transform.position.y) <= 2.0f)
                {
                    state = BasicEnnemyEtats.CHASING;
                }
                break;
            case BasicEnnemyEtats.CHASING:
                Debug.Log("CHASE");
                TrackPlayer();
                if (Math.Abs(player.transform.position.x - transform.position.x) >= 2 * detectionRadius)
                    state = BasicEnnemyEtats.IDLE;
                else if (Math.Abs(player.transform.position.x - transform.position.x) <= attackRadius && !attack_cooldown)
                    state = BasicEnnemyEtats.ATTACKING;
                break;
            case BasicEnnemyEtats.ATTACKING:
                Debug.Log("ATTACK");
                if (!attack_started && !coroutine_attack_running)
                {
                    Attack();
                }
                else if (coroutine_attack_running)
                {
                    Debug.Log("COROUTINE RUNNING");
                    rigidbodyEnemy.velocity = Vector2.zero;
                }
                else if (!coroutine_attack_running && attack_started)
                {
                    vulnerable = true;
                    colliderAttack.enabled = false;
                    attack_started = false;
                    StartCoroutine(cooldownAttack());
                    state = BasicEnnemyEtats.CHASING;
                } 
                break;
            case BasicEnnemyEtats.STUNNED:
                if (!is_stunned) StartCoroutine(stunTimer());
                else
                {
                    Debug.Log("STUNNED");
                    rigidbodyEnemy.velocity = Vector2.zero;
                }
                break;
        }
    }

    override public void Attack()
    {
        rigidbodyEnemy.velocity = Vector2.zero;
        attack_started = true;
        colliderAttack.enabled = true;
        var coroutine = windUpAttack();
        StartCoroutine(coroutine);

    }

    override public void Damage()
    {
        Debug.Log("DAMAGE");
        if (vulnerable)
        {
            if (--HP == 0) Die();
        }
    }
    
    override public void Parried()
    {
        state = BasicEnnemyEtats.STUNNED;
    }

    override public void Die()
    {
        HP = 2;
        Debug.Log("ENEMY DEAD");
        gameObject.SetActive(false);
    }

    private bool IsGrounded()
    {
        float extraHeightText = .1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(colliderEnemy.bounds.center, colliderEnemy.bounds.size, 0f,
            Vector2.down, extraHeightText, platformLayerMask);

        return raycastHit.collider != null;
    }
    
    private void Move()
    {
        if (direction == 1)
        {
            if (transform.position.x <= startPos.x + moveRadius)
            {
                rigidbodyEnemy.velocity = new Vector2(moveSpeed, 0);
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
                rigidbodyEnemy.velocity = new Vector2(-moveSpeed, 0);
            }
            else
            {
                direction = 1;
            }
        } 
    }

    private void TrackPlayer()
    {
        var moveDirection = (player.transform.position - transform.position).normalized * moveSpeed;
        if (player.transform.position.x - transform.position.x < 0) direction = -1;
        else if (player.transform.position.x - transform.position.x > 0) direction = 1;
        rigidbodyEnemy.velocity = new Vector2(moveDirection.x, 0);
    }

    IEnumerator windUpAttack()
    {
        coroutine_attack_running = true;
        yield return new WaitForSeconds(.5f);
        rigidbodyEnemy.velocity = new Vector2(75, 0) * direction;
        coroutine_attack_running = false;
    }

    IEnumerator cooldownAttack()
    {
        attack_cooldown = true;
        yield return new WaitForSeconds(.5f);
        attack_cooldown = false;
    }

    IEnumerator stunTimer()
    {
        is_stunned = true;
        vulnerable = true;
        yield return new WaitForSeconds(3.0f);
        state = BasicEnnemyEtats.CHASING;
        is_stunned = false;
    }
}

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
    STUNNED,
    HIT
}

public class BasicEnnemy : Ennemy
{

    [SerializeField] private LayerMask platformLayerMask;
    
    private GameObject player;

    public Animator animator;
     
    private Rigidbody2D rigidbodyEnemy;
    private CircleCollider2D colliderEnemy;
    
    private BasicEnnemyEtats state;
    private Vector2 startPos;
    private int direction;
    public float moveSpeed = 2.0f;
    public float moveRadius = 2.0f;
    public float chaseSpeed = 4.0f;

    private bool vulnerable = true;
    private CircleCollider2D colliderAttack;
    private bool attack_started = false;
    
    public float detectionRadius = 6.0f;
    public float attackRadius = 1.0f;

    private bool coroutine_attack_running = false;
    private bool attack_cooldown = false;
    private bool is_stunned = false;

    private float attackAnimationTime = 0.5f;
    private float hurtAnimationTime = 0.4f;
    private float deathAnimationTime = 0.833f / 0.5f;
    private float attackLength = 0.1f;

    private float scaleX;

    public int startingDirection;

    private AudioSource hurtSFX;
    private AudioSource dieSFX;
    private AudioSource parriedSFX;
    private AudioSource hitSFX;
    private AudioSource splatSFX;

    // Start is called before the first frame update
    void Start()
    {
        direction = startingDirection;
        
        scaleX = transform.localScale.x;
        
        player = GameObject.FindWithTag("Player");

        startPos = transform.position;
        state = BasicEnnemyEtats.IDLE;
        rigidbodyEnemy = GetComponent<Rigidbody2D>();
        colliderEnemy = GetComponent<CircleCollider2D>();
        colliderAttack = gameObject.transform.GetChild(0).GetComponent<CircleCollider2D>();
        colliderAttack.enabled = false;
    }
    
    void Awake()
    {
        hurtSFX = transform.Find("SFX/Hurt").GetComponent<AudioSource>();
        dieSFX = transform.Find("SFX/Die").GetComponent<AudioSource>();
        parriedSFX = transform.Find("SFX/Parried").GetComponent<AudioSource>();
        hitSFX = transform.Find("SFX/Hit").GetComponent<AudioSource>();
        splatSFX = transform.Find("SFX/Splat").GetComponent<AudioSource>();
    }

    private void Update()
    {
        transform.localScale = new Vector3(scaleX * direction, transform.localScale.y, transform.localScale.z);
        
        switch (state)
        {
            case BasicEnnemyEtats.IDLE:
                animator.SetBool("chasing",false);
                break;
            case BasicEnnemyEtats.CHASING:
                animator.SetBool("chasing", true);
                break;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!IsGrounded()) rigidbodyEnemy.gravityScale = 4;
        else if (IsGrounded()) rigidbodyEnemy.gravityScale = 1;
        
        switch (state)
        {
            case BasicEnnemyEtats.IDLE:
                Move();
                if (Mathf.Abs(player.transform.position.x - transform.position.x) <= detectionRadius && Mathf.Abs(player.transform.position.y - transform.position.y) <= 2.0f)
                {
                    state = BasicEnnemyEtats.CHASING;
                }
                break;
            case BasicEnnemyEtats.CHASING:
                TrackPlayer();
                if (Mathf.Abs(player.transform.position.x - transform.position.x) >= 2 * detectionRadius)
                    state = BasicEnnemyEtats.IDLE;
                else if (Mathf.Abs(player.transform.position.x - transform.position.x) <= attackRadius && !attack_cooldown)
                    state = BasicEnnemyEtats.ATTACKING;
                break;
            case BasicEnnemyEtats.ATTACKING:
                if (!attack_started) Attack();
                    break;
            case BasicEnnemyEtats.STUNNED:
                if (!is_stunned) StartCoroutine(stunTimer());
                else
                {
                    rigidbodyEnemy.velocity = Vector2.zero;
                    rigidbodyEnemy.angularVelocity = 0;
                }
                break;
            case BasicEnnemyEtats.HIT:
                rigidbodyEnemy.velocity = Vector2.zero;
                break;
        }
    }

    override public void Attack()
    {
        rigidbodyEnemy.velocity = Vector2.zero;
        StartCoroutine(startAttack());
    }

    IEnumerator startAttack()
    {
        attack_started = true;
        animator.Play("attack");
        yield return new WaitForSeconds(attackAnimationTime);
        attack_started = false;
        colliderAttack.enabled = true;
        yield return new WaitForSeconds(attackLength);
        colliderAttack.enabled = false;
        state = BasicEnnemyEtats.IDLE;
    }
    
    override public void Damage()
    {
        if (vulnerable)
        {
            hitSFX.Play();
            var directionAttack = player.transform.position - transform.position;
            directionAttack.y = 0;
            rigidbodyEnemy.AddForce(directionAttack * 100,ForceMode2D.Force);
            if (--HP <= 0) Die();
            else
            {
                
                hurtSFX.Play();
                StartCoroutine(IFrames());
            }
            
            
        }
    }
    
    override public void Parried()
    {
        StopAllCoroutines();
        parriedSFX.Play();
        animator.Play("hurt");
        animator.SetBool("stunned", true);
        state = BasicEnnemyEtats.STUNNED;
        colliderAttack.enabled = false;
    }

    override public void Die()
    {
        StopAllCoroutines();
        rigidbodyEnemy.velocity = Vector2.zero;
        state = BasicEnnemyEtats.HIT;
        dieSFX.Play();
        StartCoroutine(waitDeathAnimation());
    }

    IEnumerator waitDeathAnimation()
    {
        vulnerable = false;
        animator.Play("die");
        colliderAttack.gameObject.SetActive(false);
        state = BasicEnnemyEtats.HIT;
        yield return new WaitForSeconds(deathAnimationTime);
        StartCoroutine(waitSplatSound());
        HP = 2;
        
    }

    IEnumerator waitSplatSound()
    {
        splatSFX.Play();
        yield return new WaitWhile(() => splatSFX.isPlaying);
        colliderAttack.gameObject.SetActive(true);
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
                rigidbodyEnemy.velocity = new Vector2(moveSpeed, rigidbodyEnemy.velocity.y);
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
                rigidbodyEnemy.velocity = new Vector2(-moveSpeed, rigidbodyEnemy.velocity.y);
            }
            else
            {
                direction = 1;
            }
        } 
    }

    private void TrackPlayer()
    {
        var moveDirection = (player.transform.position - transform.position).normalized * chaseSpeed;
        if (player.transform.position.x - transform.position.x < 0) direction = -1;
        else if (player.transform.position.x - transform.position.x > 0) direction = 1;
        rigidbodyEnemy.velocity = new Vector2(moveDirection.x, rigidbodyEnemy.velocity.y);
    }

    IEnumerator windUpAttack()
    {
        coroutine_attack_running = true;
        yield return new WaitForSeconds(.5f);
        rigidbodyEnemy.velocity = new Vector2(15, 0) * direction;
        animator.SetBool("isAttacking", true);
        coroutine_attack_running = false;
    }

    IEnumerator cooldownAttack()
    {
        attack_cooldown = true;
        yield return new WaitForSeconds(.5f);
        attack_cooldown = false;
    }

    IEnumerator IFrames()
    {
        vulnerable = false;
        state = BasicEnnemyEtats.HIT;
        animator.Play("hurt");
        yield return new WaitForSeconds(hurtAnimationTime);
        state = BasicEnnemyEtats.IDLE;
        vulnerable = true;
    }
    IEnumerator stunTimer()
    {
        is_stunned = true;
        vulnerable = true;
        yield return new WaitForSeconds(3.0f);
        state = BasicEnnemyEtats.IDLE;
        animator.SetBool("stunned", false);
        is_stunned = false;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField] private LayerMask platformLayerMask;

    public InputActionAsset inputManager;
    private InputActionMap actionMap;
    
    public GameObject attackPoint;
    public float attackRange = 1.0f;
    public float distanceAttack = 0.75f;
    [SerializeField] public LayerMask enemiesLayerMask;
    [SerializeField] public LayerMask attacksLayerMask;
    public float parryRange = 1.0f;
    private bool parry = false;
    private bool hurt = false;

    [Header("SFX")] 
    private AudioSource footstepsSFX;
    private AudioSource jumpSFX;
    private AudioSource landingSFX;
    private AudioSource attackSFX;
    private AudioSource parrySFX;
    private AudioSource hitSFX;
    
    [Header("Actions")] 
    private InputAction saut;
    private InputAction mouvement;
    private InputAction attaque;
    private InputAction parrage;

    [Header("Stats")] 
    private int HP = 5;
    private bool invulnerable = false;
    private int nbShardLostTales = 0;

    private Rigidbody2D rigidbodyJoueur;
    private BoxCollider2D colliderJoueur;

    public float jumpSpeed = 12.5f;
    public float walkSpeed = 7f;
    private float glideSpeed;
    [HideInInspector] public int orientation = 1;

    private Vector2 currentMove;
    
    private float vitesseX;

    public Animator animator;
    public GameObject healthBar;

    private float scaleX;

    private bool iframes = false;

    public PauseControl pause;

    private bool isGrounded = true;
    private bool prevIsGrounded = true;

    private float temps_parry;
    private float startTime;
    
    private bool parry_started = false;

    private float deathAnimationTime = 3.125f;
    private float attackAnimationTime = 0.4f;
    private float airAttackAnimationTime = 0.5f;

    private bool attacking = false;

    private TextMeshProUGUI zoneShardCount;
    
    void Awake()
    {
        scaleX = transform.localScale.x;

        footstepsSFX = GameObject.Find("Joueur/SFX/Footsteps").GetComponent<AudioSource>();
        jumpSFX = GameObject.Find("Joueur/SFX/Jump").GetComponent<AudioSource>();
        landingSFX = GameObject.Find("Joueur/SFX/Landing").GetComponent<AudioSource>();
        attackSFX = GameObject.Find("Joueur/SFX/Attack").GetComponent<AudioSource>();
        parrySFX = GameObject.Find("Joueur/SFX/Parry").GetComponent<AudioSource>();
        hitSFX = GameObject.Find("Joueur/SFX/Hit").GetComponent<AudioSource>();
        
        rigidbodyJoueur = GetComponent<Rigidbody2D>();
        colliderJoueur = GetComponent<BoxCollider2D>();
        actionMap = inputManager.FindActionMap("Player");

        saut = actionMap.FindAction("Jump");
        mouvement = actionMap.FindAction("Move");
        attaque = actionMap.FindAction("Attack");
        parrage = actionMap.FindAction("Parry");
        
        glideSpeed = walkSpeed / 2;

        zoneShardCount = GameObject.Find("CanvasText/Content/NbShards").GetComponent<TextMeshProUGUI>();
        zoneShardCount.text = nbShardLostTales.ToString();

    }

    void Update()
    {
        IsGrounded();
        
        if (orientation == 1)
            gameObject.transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        else if (orientation == -1)
            gameObject.transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
        
        
        
        if (isGrounded) animator.SetBool("isGrounded", true);
        else if (!isGrounded) animator.SetBool("isGrounded", false);
        

        vitesseX = rigidbodyJoueur.velocity.x;
        
        if (Math.Abs(vitesseX) >= 0.0001f && !footstepsSFX.isPlaying && isGrounded && !landingSFX.isPlaying) footstepsSFX.Play();
        if (Math.Abs(vitesseX) < 0.0001f && footstepsSFX.isPlaying || !isGrounded) footstepsSFX.Stop();

        if (isGrounded && !prevIsGrounded)
        {
            landingSFX.Play();
            prevIsGrounded = true;
        }

        if (!isGrounded && prevIsGrounded) prevIsGrounded = false;
        
        
        animator.SetFloat("vitesseX",Mathf.Abs(vitesseX));

        animator.SetFloat("vitesseY", rigidbodyJoueur.velocity.y);
        
        if (hurt) animator.SetBool("hurt", true);
        else if (!hurt) animator.SetBool("hurt", false);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt")) hurt = false;

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Parry"))
        {
            parry_started = true;
        }
        
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Parry") && parry_started)
        {
            parry = false;
            parry_started = false;
        }
        
        if (!parry && !iframes) invulnerable = false;

        // uncomment if inversion scale removed
        //if (distanceAttack * orientation != attackPoint.transform.localPosition.x)
        //   attackPoint.transform.localPosition = new Vector3(distanceAttack * orientation, 0, 0);

    }
    
    void FixedUpdate()
    {

        vitesseX = rigidbodyJoueur.velocity.x;

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Parry") &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !animator.GetCurrentAnimatorStateInfo(0).IsName("AirAttack"))
        {
            if (isGrounded)
            {
                rigidbodyJoueur.velocity = new Vector2(currentMove.x * walkSpeed, rigidbodyJoueur.velocity.y);
            }
            else if (!isGrounded)
            {
                rigidbodyJoueur.velocity = new Vector2(currentMove.x * glideSpeed, rigidbodyJoueur.velocity.y);
            }
        }

        if (!attacking)
        {
            if (rigidbodyJoueur.velocity.y < -0.01f && !isGrounded) rigidbodyJoueur.gravityScale = 3;
            else if (rigidbodyJoueur.velocity.y >= 0 || isGrounded) rigidbodyJoueur.gravityScale = 2;
        }


        if (colliderJoueur.IsTouchingLayers(attacksLayerMask) && !parry && !invulnerable)
        {
            Vector3 directionAttaque = Vector2.zero;
            Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), attacksLayerMask);
            foreach (var attaque in hits)
            {
                directionAttaque += attaque.transform.position;
            }
            Damage(directionAttaque);
            hitSFX.Play();
        }
        else if (parry)
        {
            Parry();
        }

        if (attacking)
        {
            Attack();
        }

    }

    private void IsGrounded()
    {
        float extraHeightText = .1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(colliderJoueur.bounds.center, colliderJoueur.bounds.size, 0f,
            Vector2.down, extraHeightText, platformLayerMask);
        
        isGrounded = raycastHit.collider != null;
    }

    void Attack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.transform.position, attackRange, enemiesLayerMask);
        foreach (var enemy in hitEnemies)
        {
            enemy.gameObject.GetComponent<Ennemy>().Damage();
        }
    }

    void Parry()
    {
        Collider2D[] parriedAttacks =
            Physics2D.OverlapCircleAll(transform.position, parryRange, attacksLayerMask);
        foreach (var att in parriedAttacks)
        {
            att.gameObject.GetComponentInParent<Ennemy>().Parried();
        }
    }

    void Damage(Vector3 direction)
    {
        hurt = true;
        invulnerable = true;
        rigidbodyJoueur.AddForce( transform.position - direction, ForceMode2D.Force);
        if (--HP <= 0) StartCoroutine(Die());
        healthBar.GetComponent<RengeGames.HealthBars.UltimateCircularHealthBar>().AddRemoveSegments(1);
        StartCoroutine(iFrames(3.0f));
    }

    public int getHP()
    {
        return HP;
    }
    
    public void Heal(int h)
    {
        HP += h;
        healthBar.GetComponent<RengeGames.HealthBars.UltimateCircularHealthBar>().AddRemoveSegments(-h);
    }
    
    IEnumerator Die()
    {
        animator.Play("Death");
        yield return new WaitForSeconds(deathAnimationTime);
        HP = 5;
        pause.PauseGame();
        pause.text.SetText(ProgressionControllerLvl1.textes.mortJoueur);
        Time.timeScale = 0f;
        Debug.Log("le joueur est mort");
    }

    public void addShardsOfLostTales(int n)
    {
        nbShardLostTales += n;
        zoneShardCount.text = nbShardLostTales.ToString();
    }

    public void spendShardsOfLostTales(int n)
    {
        nbShardLostTales -= n;
        zoneShardCount.text = nbShardLostTales.ToString();
    }
    
    
    public void doJump(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded)
        {
            rigidbodyJoueur.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            jumpSFX.Play();
        }
    }

    public void doMove(InputAction.CallbackContext context)
    {
        currentMove = new Vector2(context.ReadValue<float>(), 0);
        if (context.ReadValue<float>() != 0)
        {
            
            if (context.ReadValue<float>() < 0) orientation = -1;
            else if (context.ReadValue<float>() > 0) orientation = 1;
        }
        
    }

    public void doAttack(InputAction.CallbackContext context)
    {
        
        if (context.started && !attacking)
        {
            if (isGrounded)
            {
                
                StartCoroutine(waitForAttack());
            }
            else
            {
                StartCoroutine(waitForAirAttack());
            }
            attackSFX.Play();
        }
    }

    public void doParry(InputAction.CallbackContext context)
    {
        if (context.started && isGrounded && !parry)
        {
            parrySFX.Play();
            parry = true;
            animator.Play("Parry");
            //animator.SetTrigger("parry");
            invulnerable = true;
        }
    }

    IEnumerator waitForAttack()
    {
        attacking = true;
        animator.Play("Attack");
        yield return new WaitForSeconds(attackAnimationTime);
        attacking = false;

    }

    IEnumerator waitForAirAttack()
    {
        animator.Play("AirAttack");
        rigidbodyJoueur.gravityScale = 6;
        attacking = true;
        yield return new WaitForSeconds(airAttackAnimationTime);
        attacking = false;
        rigidbodyJoueur.gravityScale = 2;
    }
    
    IEnumerator parryTiming(float s)
    {
        yield return new WaitForSeconds(s);
        parry = false;
        invulnerable = false;
    }

    IEnumerator iFrames(float s)
    {
        // ghost layer
        iframes = true;
        gameObject.layer = LayerMask.NameToLayer("Ghost");
        yield return new WaitForSeconds(s);
        invulnerable = false;
        iframes = false;
        // player layer
        gameObject.layer = LayerMask.NameToLayer("Player");
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.transform.position, attackRange);
    }
}
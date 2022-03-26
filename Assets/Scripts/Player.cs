using System;
using System.Collections;
using System.Collections.Generic;
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
    
    [Header("Actions")] 
    private InputAction saut;
    private InputAction mouvement;
    private InputAction attaque;
    private InputAction parrage;

    [Header("Stats")] 
    private int HP = 5;
    private bool invulnerable = false;

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
    
    void Awake()
    {
        scaleX = transform.localScale.x;
        
        rigidbodyJoueur = GetComponent<Rigidbody2D>();
        colliderJoueur = GetComponent<BoxCollider2D>();
        actionMap = inputManager.FindActionMap("Player");

        saut = actionMap.FindAction("Jump");
        mouvement = actionMap.FindAction("Move");
        attaque = actionMap.FindAction("Attack");
        parrage = actionMap.FindAction("Parry");
        
        glideSpeed = walkSpeed / 2;
    }

    void Update()
    {

        
        if (orientation == 1)
            gameObject.transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        else if (orientation == -1)
            gameObject.transform.localScale = new Vector3(-scaleX, transform.localScale.y, transform.localScale.z);
        
        
        
        if (IsGrounded()) animator.SetBool("isGrounded", true);
        else if (!IsGrounded()) animator.SetBool("isGrounded", false);
        

        vitesseX = rigidbodyJoueur.velocity.x;

        
        animator.SetFloat("vitesseX",Mathf.Abs(vitesseX));

        animator.SetFloat("vitesseY", rigidbodyJoueur.velocity.y);
        
        if (hurt) animator.SetBool("hurt", true);
        else if (!hurt) animator.SetBool("hurt", false);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Hurt")) hurt = false;

        if (parry) animator.SetBool("isParrying", true);
        else if (!parry) animator.SetBool("isParrying", false);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Parry"))
        {
            parry = false;
        }

        if (!parry && !iframes) invulnerable = false;

        // uncomment if inversion scale removed
        //if (distanceAttack * orientation != attackPoint.transform.localPosition.x)
        //   attackPoint.transform.localPosition = new Vector3(distanceAttack * orientation, 0, 0);

    }
    
    void FixedUpdate()
    {

        vitesseX = rigidbodyJoueur.velocity.x;
        
        
        if (IsGrounded())
        {
            rigidbodyJoueur.velocity = new Vector2(currentMove.x * walkSpeed, rigidbodyJoueur.velocity.y) ;
        }
        else if (!IsGrounded())
        {
            rigidbodyJoueur.velocity = new Vector2(currentMove.x * glideSpeed, rigidbodyJoueur.velocity.y) ;
        }
    
        
        
        
        if (rigidbodyJoueur.velocity.y < -0.01f && !IsGrounded()) rigidbodyJoueur.gravityScale = 3;
        else if (rigidbodyJoueur.velocity.y >= 0 || IsGrounded())
        {
            rigidbodyJoueur.gravityScale = 2;
            

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
        }
        else if (parry)
        {
            Parry();
            
        }

    }

    private bool IsGrounded()
    {
        float extraHeightText = .1f;
        RaycastHit2D raycastHit = Physics2D.BoxCast(colliderJoueur.bounds.center, colliderJoueur.bounds.size, 0f,
            Vector2.down, extraHeightText, platformLayerMask);
        
        return raycastHit.collider != null;
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
        rigidbodyJoueur.AddForce(50 * (transform.position - direction), ForceMode2D.Force);
        if (--HP <= 0) Die();
        healthBar.GetComponent<RengeGames.HealthBars.UltimateCircularHealthBar>().AddRemoveSegments(1);
        StartCoroutine(iFrames(3.0f));
    }

    void Die()
    {
        HP = 5;
        Debug.Log("le joueur est mort");
    }
    
    public void doJump(InputAction.CallbackContext context)
    {
        if (context.started && (IsGrounded()))
        {
            rigidbodyJoueur.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
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
        
        if (context.started)
        {
            Debug.Log("attaque");
            Attack();
        }
    }

    public void doParry(InputAction.CallbackContext context)
    {
        if (context.started && IsGrounded())
        {
            parry = true;
            invulnerable = true;
        }
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


}
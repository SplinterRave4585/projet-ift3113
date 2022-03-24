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
    
    
    [Header("Actions")] 
    private InputAction saut;
    private InputAction mouvement;
    private InputAction attaque;
    private InputAction parrage;

    [Header("Stats")] 
    private int HP = 3;
    private bool invulnerable = false;

    private Rigidbody2D rigidbodyJoueur;
    private BoxCollider2D colliderJoueur;

    public float maxSpeedX = 10f;

    public float jumpSpeed = 20f;
    public float walkSpeed = 10f;
    private float glideSpeed;
    [HideInInspector] public float orientation = 1;

    private Vector2 currentMove;
    
    private float vitesseX;
    private bool landed = false;

    public Animator animator;
    

    void Awake()
    {
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

        if (IsGrounded()) animator.SetBool("isGrounded", true);
        else if (!IsGrounded()) animator.SetBool("isGrounded", false);

        vitesseX = rigidbodyJoueur.velocity.x;
        
        animator.SetFloat("vitesseX",vitesseX);
        
        animator.SetFloat("vitesseY", rigidbodyJoueur.velocity.y);

        if (distanceAttack * orientation != attackPoint.transform.localPosition.x)
            attackPoint.transform.localPosition = new Vector3(distanceAttack * orientation, 0, 0);

        
        
    }

    void FixedUpdate()
    {
        
        if (IsGrounded())
        {
            if (!landed)
            {
                rigidbodyJoueur.velocity = new Vector2(vitesseX, 0);
                landed = true;
            }
            rigidbodyJoueur.AddForce(currentMove * walkSpeed, ForceMode2D.Force);
        }
        else if (!IsGrounded())
        {
            rigidbodyJoueur.AddForce(currentMove * glideSpeed, ForceMode2D.Force);
            landed = false;
            vitesseX = rigidbodyJoueur.velocity.x;

        }

        if (rigidbodyJoueur.velocity.y < -0.01f && !IsGrounded()) rigidbodyJoueur.gravityScale = 4;
        else if (rigidbodyJoueur.velocity.y >= 0 || IsGrounded())
        {
            rigidbodyJoueur.gravityScale = 2;
            rigidbodyJoueur.velocity = new Vector2(vitesseX, rigidbodyJoueur.velocity.y);

        }
        if (Math.Abs(rigidbodyJoueur.velocity.x) > maxSpeedX)
        {
            rigidbodyJoueur.velocity =  new Vector2(maxSpeedX * orientation, rigidbodyJoueur.velocity.y);
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
        Debug.Log("player damaged");
        invulnerable = true;
        rigidbodyJoueur.AddForce(50 * (transform.position - direction), ForceMode2D.Force);
        if (--HP <= 0) Die();
        StartCoroutine(iFrames(3.0f));
    }

    void Die()
    {
        HP = 3;
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
            if (context.ReadValue<float>() < 0) orientation = -1.0f;
            else if (context.ReadValue<float>() > 0) orientation = 1.0f;
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
        if (context.started)
        {
            Debug.Log("parry");
            parry = true;
            invulnerable = true;
            StartCoroutine(parryTiming(0.5f));
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
        yield return new WaitForSeconds(s);
        invulnerable = false;
    }
    


}

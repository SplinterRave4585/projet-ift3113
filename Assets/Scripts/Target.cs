using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Etats
{
    STUNNED,
    ATTACKING,
    IDLE
}
public class Target : Ennemy
{
    private Etats state = Etats.IDLE;

    private BoxCollider2D collider_attack;

    private float attack_time = 0.5f;
    private float idle_time = 5.0f;
    private float stun_time = 3.0f;
    
    
    void Awake()
    {
        collider_attack = gameObject.GetComponentInChildren<BoxCollider2D>();
        collider_attack.enabled = false;
    }

    void Update()
    {
        switch (state)
        {
            case (Etats.IDLE) : 
                StartCoroutine(waitIdle());
                break;
            case (Etats.STUNNED) :
                StartCoroutine(waitStun());
                break;
            case (Etats.ATTACKING) :
                Attack();
                break;
        }
    }

    public override void Attack()
    {
        collider_attack.enabled = true;
        StartCoroutine(waitAttack());
    }
    public override void Damage()
    {
        Debug.Log("HP ennemi : " + (HP - 1));
        if (--HP == 0) Die();

    }

    public override void Parried()
    {
        state = Etats.STUNNED;
        StopCoroutine(waitAttack());
        collider_attack.enabled = false;
        Debug.Log("Ennemy stunned");

    }

    public override void Die()
    {
        Debug.Log("Ennemi tué");
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(1,1,0));
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(2,2,0));
    }

    IEnumerator waitIdle()
    {
        yield return new WaitForSeconds(idle_time);
        state = Etats.ATTACKING;
    }

    IEnumerator waitStun()
    {
        yield return new WaitForSeconds(stun_time);
        state = Etats.IDLE;
        Debug.Log("Stun over");
    }
    
    
    IEnumerator waitAttack()
    {
        yield return new WaitForSeconds(attack_time);
        state = Etats.IDLE;
        collider_attack.enabled = false;
    }
}

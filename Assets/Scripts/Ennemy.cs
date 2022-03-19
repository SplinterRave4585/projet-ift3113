using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ennemy : MonoBehaviour
{
    public int HP;
    abstract public void Attack();

    abstract public void Damage();


    abstract public void Parried();

    virtual public void Die()
    {
        Debug.Log("Ennemy dead");
        Destroy(gameObject);
    }
}

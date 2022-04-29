using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class HealingItem : MonoBehaviour
{
    private GameObject player;
    public GameObject sprite;
    public ParticleSystem particles;
    public GameObject light;
    public AudioSource pickupSFX;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Joueur");
        
        var emission = particles.emission;
        emission.enabled = false;
    }

    private void Update()
    {
        if (GetComponent<CapsuleCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()))
        {
            Heal();
            GetComponent<CapsuleCollider2D>().enabled = false;
            light.SetActive(false);
            sprite.SetActive(false);
            pickupSFX.Play();
            StartCoroutine(waitParticles());

        }
    }
    
    void Heal()
    {
        player.GetComponent<Player>().Heal(1);
    }

    IEnumerator waitParticles()
    {
        var emission = particles.emission;
        emission.enabled = true;
        yield return new WaitForSeconds(1.0f);
        emission.enabled = false;
        gameObject.SetActive(false);
    }
    
}

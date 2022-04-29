using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleItem : MonoBehaviour
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
            addShardLostTale();
            GetComponent<CapsuleCollider2D>().enabled = false;
            light.SetActive(false);
            sprite.SetActive(false);
            pickupSFX.Play();
            StartCoroutine(waitParticles());

        }
    }

    private void addShardLostTale()
    {
        player.GetComponent<Player>().addShardsOfLostTales(1);
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

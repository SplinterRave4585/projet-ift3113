using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxX : MonoBehaviour
{

    private float lengthX, startPosX;
    public GameObject cam;
    public float parallaxEffect;
    
    // Start is called before the first frame update
    void Start()
    {
        startPosX = transform.position.x;
        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float tempX = (cam.transform.position.x * (1 - parallaxEffect));
        float distX = (cam.transform.position.x * parallaxEffect);
        
        transform.position = new Vector3(startPosX + distX, transform.position.y, transform.position.z);

        if (tempX > startPosX + lengthX) startPosX += lengthX;
        else if (tempX < startPosX - lengthX) startPosX -= lengthX;
    }
}

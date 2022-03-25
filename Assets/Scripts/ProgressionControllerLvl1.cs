using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionControllerLvl1 : MonoBehaviour
{
    public Collider2D triggerFinLvl;
    public Collider2D triggerTutoJump;
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == triggerFinLvl)
        {
            Application.Quit();
            Debug.Log("quitte");
        }
        else if (other == triggerTutoJump)
        {
            
        }
    }
    
}

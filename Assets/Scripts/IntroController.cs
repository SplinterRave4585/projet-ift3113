using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

public class IntroController : MonoBehaviour
{

    public PlayableDirector director;
    public bool cutsceneFinished = false;
    private bool waiting = false;
    
    void Update()
    {
        if (!waiting)
        {
            if (Input.anyKey && cutsceneFinished)
            {
                SceneManager.LoadScene("Lvl1");
            }
            else if (Input.anyKey && !cutsceneFinished)
            {
                skipCutscene();
            }
        }
    }

    void skipCutscene()
    {
        director.time = 1890;
        cutsceneFinished = true;
        StartCoroutine(waitForNextClick());
    }

    
    public void cutsceneIsFinished()
    {
        cutsceneFinished = true;
    }

    IEnumerator waitForNextClick()
    {
        waiting = true;
        yield return new WaitForSeconds(0.5f);
        waiting = false;
    }
    
}

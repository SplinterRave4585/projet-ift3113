using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class EndingController : MonoBehaviour
{
    private bool videoFinished = false;
    public PlayableDirector director;
    private bool waiting = false;
    

    // Update is called once per frame
    void Update()
    {
        if (!waiting)
        {
            if (!videoFinished && Input.anyKey) skipCutscene();
            else if (videoFinished)
            {
                if (Input.GetKeyUp(KeyCode.Escape)) SceneManager.LoadScene("Lvl1");
                else if (Input.GetKeyUp(KeyCode.Q))
                {
                    Debug.Log("Quit");
                    Application.Quit();
                }
            }
        }
    }

    public void videoIsFinished()
    {
        videoFinished = true;
    }

    void skipCutscene()
    {
        director.time = 2700;
        videoFinished = true;
        
    }

    IEnumerator waitForNextClick()
    {
        waiting = true;
        yield return new WaitForSeconds(0.3f);
        waiting = false;
    }
    
}

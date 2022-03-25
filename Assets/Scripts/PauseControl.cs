using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseControl : MonoBehaviour
{
    public static bool gameIsPaused;

    public PlayerInput joueurInput;
    private PlayerInput pausedInput;

    // Start is called before the first frame update
    void Start()
    {
        gameIsPaused = false;

        pausedInput = GetComponent<PlayerInput>();
        pausedInput.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Pauses the game
    public void PauseGame()
    {
        gameIsPaused = true;
        joueurInput.enabled = false;
        pausedInput.enabled = true;

        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        gameIsPaused = false;
        pausedInput.enabled = false;
        joueurInput.enabled = true;

        Time.timeScale = 1;
    }
}

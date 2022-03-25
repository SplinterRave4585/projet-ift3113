using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseControl : MonoBehaviour
{
    public static bool gameIsPaused;

    public PlayerInput joueurInput;
    private PlayerInput pausedInput;
    private Canvas menuScreen;

    // Start is called before the first frame update
    void Start()
    {   
        pausedInput = GetComponent<PlayerInput>();

        menuScreen = GetComponent<Canvas>();
    }

    // Pauses the game
    public void PauseGame()
    {
        menuScreen.enabled = true;
        joueurInput.enabled = false;
        pausedInput.enabled = true;

        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        menuScreen.enabled = false;
        pausedInput.enabled = false;
        joueurInput.enabled = true;

        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("called: Application.Quit()");
    }
}

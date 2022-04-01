using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Scene = UnityEditor.SearchService.Scene;

public class PauseControl : MonoBehaviour
{
    public static bool gameIsPaused;

    public PlayerInput joueurInput;
    private PlayerInput pausedInput;
    private Canvas menuScreen;

    public TextMeshProUGUI text;

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
        text.SetText("PAUSED");

        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        if (text.text != "PAUSED")
        {
            var scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
        else
        {
            menuScreen.enabled = false;
            pausedInput.enabled = false;
            joueurInput.enabled = true;
        }

        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("called: Application.Quit()");
    }
}

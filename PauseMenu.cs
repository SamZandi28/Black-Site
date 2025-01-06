using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    //initiating stuff
    public GameObject PauseMenuUI;
    public bool PauseMenuActive = true;    

    void Start()
    {
        DisplayPauseMenu();
    }
    //callback 
    public void PauseButtonPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DisplayPauseMenu();
        }
    }

    public void DisplayPauseMenu()
    {
        if (PauseMenuActive)
        {
            PauseMenuUI.SetActive(false);
            PauseMenuActive = false;
            Time.timeScale = 1;
        }
        else if (!PauseMenuActive)
        {
            PauseMenuUI.SetActive(true);
            PauseMenuActive = true;
            Time.timeScale = 0;
        }
    }
    public void ResumeGame()
    {
        DisplayPauseMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}


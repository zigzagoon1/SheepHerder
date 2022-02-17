using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//main menu and pause menu functionality
public class Menu : MonoBehaviour
{
    Controls playerInput;
    public GameObject mainMenu, levelSelect, settings, pauseMenu;
    //[SerializeField] Button playButton, levelSelectButton, settingsButton, quitButton;




    public void PlayButtonPressed()
    {
        Debug.Log("Loading Level 1...");
        GameManager.instance.LoadSceneByName("Level 1");
    }

    public void LevelSelectButtonPressed() 
    {
        GameManager.instance.LoadSceneByName("Settings");
    }

    public void SettingsButtonsPressed()
    {
        Debug.Log("Loading settings...");
        GameManager.instance.LoadSettingsMenu();
    }

    public void QuitButtonPressed()
    {
        Debug.Log("quitting game...");
        GameManager.instance.QuitGame();
    }
}

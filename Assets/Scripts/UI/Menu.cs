using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//main menu and pause menu functionality
public class Menu : MonoBehaviour
{
    public GameObject mainMenu, levelSelect, settings, pauseMenu;

    public void PlayButtonPressed()
    {
        Debug.Log("Loading Level 1...");
        GameManager.instance.LoadSceneByName("Level 1");
    }

    public void LevelSelectButtonPressed() 
    {
        //GameManager.instance.LoadSceneByName("LevelSelect");
        Debug.Log("Loading level select...");
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

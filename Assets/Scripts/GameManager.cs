using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
        playerInput = new Controls();
    }
    #endregion

    Controls playerInput;
    public static bool GamePaused = false;

    public List<Level> levels;
    public Level currentLevel;

    public GameObject pauseMenu;

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    private void Start()
    {
    }
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        if (sceneName.Contains("Level"))
        {
            currentLevel = levels.Find(x => x.name == sceneName);
        }
    }

    public void OnPauseGame()
    {
        Debug.Log("Game paused");
        pauseMenu.SetActive(true);
        GamePaused = true;
        Time.timeScale = 0f;

    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        GamePaused = false;
        Time.timeScale = 1f;
    }

    public void LoadSettingsMenu()
    {
        //open settings menu
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}

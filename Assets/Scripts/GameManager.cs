using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //
    #region Singleton
    public static GameManager instance;
    public List<Sheep> activeSheep;
    public List<Wolf> activeWolves;
    public delegate void OnUpdateSheep();
    public delegate void OnUpdateWolves();
    public OnUpdateSheep onUpdateSheepCallback;
    public OnUpdateWolves onUpdateWolvesCallback;

    private void OnEnable()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }
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
        activeSheep = FindObjectsOfType<Sheep>().ToList<Sheep>();
        activeWolves = FindObjectsOfType<Wolf>().ToList<Wolf>();
    }
    #endregion

    Controls playerInput;
    public static bool GamePaused = false;

    public static List<Level> levels;
    public Level currentLevel;

    public GameObject pauseMenu;

/*    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
*/
    private void Start()
    {
        pauseMenu.SetActive(false);
        
    }
    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        if (sceneName.Contains("Level"))
        {
            currentLevel = levels.Find(x => x.name == sceneName);
            //instantiate level start enemy/ies here after getting random spawn location/s
        }
    }

    public void OnPauseGame()
    {
        Debug.Log("Game paused");
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        GamePaused = !GamePaused;
        if (GamePaused == true)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void UpdateActiveSheep()
    {
        activeSheep = FindObjectsOfType<Sheep>().ToList<Sheep>();
        onUpdateSheepCallback.Invoke();
    }

    public void UpdateActiveWolves()
    {
        activeWolves = FindObjectsOfType<Wolf>().ToList<Wolf>();
        onUpdateWolvesCallback.Invoke();
    }
    public void LevelComplete()
    {
        Time.timeScale = 0;
        Debug.Log("Level Complete!");
        //show level complete text
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

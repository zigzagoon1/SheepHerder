using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    public static GameManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public Level[] levels;
    [SerializeField] Level currentLevel;
    public Enemy[] currentEnemies;

    SceneManager sceneManager;

/*    private void Start()
    {
        
        //currentEnemies = currentLevel.enemies;
    }*/
    
}

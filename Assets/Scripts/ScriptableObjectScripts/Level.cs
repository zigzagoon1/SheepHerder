using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu()]
public class Level : ScriptableObject
{
    public string sceneName;
    public int levelNumber;
    public EnemyType[] enemies;
    public Transform[] enemyStartPositions;
    //map/terrain/layout
    //UI Display Info variable (to display timer or no timer or any level-specific info)

    private void Awake()
    {
        

    }
}

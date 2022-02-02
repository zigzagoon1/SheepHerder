using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Level : ScriptableObject
{
    public int levelNumber;
    public int enemyCount;
    public EnemyType[] enemies;
    public Transform[] enemyStartPositions;
    //map/terrain/layout
    //UI Display Info variable (to display timer or no timer or any level-specific info)

    private void Awake()
    {
        enemies = new EnemyType[enemyCount];
        enemyStartPositions = new Transform[enemyCount];

    }
}

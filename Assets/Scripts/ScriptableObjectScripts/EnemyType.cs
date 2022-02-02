using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu()]
public class EnemyType : ScriptableObject
{
    public HitsToDefeat hitsToDefeat;
    public SpeedSO baseSpeed;

    public GameObject prefab;
    public Animator animator;
    public NavMeshAgent agent;
}

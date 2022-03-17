using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyType type;

    public HitCount hitCount;

    //increasing or decreasing this variable affects the total cooldown time before enemy can reuse attack
    public float attackCooldown;
    //this timer is incremented until it reaches attackCooldown

    private bool isFacingTarget = false;

    [HideInInspector] public GameObject player;


    //serialize just to make sure it's working, remove when done
    public GameObject target = null;
    
    public int _hitsToDefeat;
    public float _speed;

    public NavMeshAgent _agent;

    [HideInInspector] public Animator _animator;

   

    [HideInInspector] public Vector3 chaseStartLocation;
   
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
    //if player or sheep is targeted, face it-- needs fixing
    public IEnumerator FaceTarget()
    {
        if (target == null)
        {
            target = player;
        }
        while (isFacingTarget == false)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            if (direction == Vector3.zero)
            {
                direction = new Vector3(0.01f, 0.01f, 0.01f);
            }
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            float angle = Vector3.Angle(transform.position, target.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, (angle / 360) * 2f);
            yield return null;
            
        }
    }
}

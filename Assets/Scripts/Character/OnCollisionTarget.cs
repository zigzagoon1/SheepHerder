using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//on rock collision with object, set players target to add a hit point if necessary
public class OnCollisionTarget : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    GameObject hitCharacter;
    PlayerController player;
    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        //play particle effect when rock hits an object
        particles.Play();

        //if rock collides with enemy, set player target to enemy and player script adds hitpoint- same for sheep below
        if (collision.gameObject.layer == 8) //enemy collision
        {
            hitCharacter = collision.gameObject;
            player.AttackTarget = hitCharacter;
            player.AttackTargetType = "Enemy";
            player.onAttackTargetSetCallback.Invoke();
            
        }
        else if (collision.gameObject.layer == 9) //sheep collision
        {
            hitCharacter = collision.gameObject;
            player.AttackTarget = hitCharacter;
            player.AttackTargetType = "Sheep";
            player.onAttackTargetSetCallback.Invoke();
        }

        //if rock hits environment, player script ignores the set target
        else if (collision.gameObject.layer == 10) //environment/obstacle collision
        {
            player.AttackTargetType = "Environment";
        }
    }
}

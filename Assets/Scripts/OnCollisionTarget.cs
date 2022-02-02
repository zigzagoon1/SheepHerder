using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollisionTarget : MonoBehaviour
{
    GameObject hitCharacter;
    PlayerController player;
    Rigidbody rb;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
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
        else if (collision.gameObject.layer == 10) //environment/obstacle collision
        {
            player.AttackTargetType = "Environment";
        }
    }
}

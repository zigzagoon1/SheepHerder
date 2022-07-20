using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//on rock collision with object, add to objects hit count or play particle effect
public class OnCollisionTarget : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    float timeStamp;
    [SerializeField] float coolDownSeconds;
    public bool hitEnemy = false;
    private Rigidbody rB;

    private void Awake()
    {
        rB = GetComponent<Rigidbody>();
    }
    public void OnCollisionEnter(Collision collision)
    {
        GameObject hitCharacter = collision.gameObject;
        
        if (hitCharacter.CompareTag("Sheep") || hitCharacter.CompareTag("Enemy"))
        {
            //cooldown to ensure hit character does not get more than one point added, maybe redundant
            if (timeStamp <= Time.time)
            {
                hitCharacter.GetComponentInParent<HitCount>().Value += 1;
            }
            timeStamp = Time.time + coolDownSeconds;
            //probably removing hitEnemy
            if (hitCharacter.CompareTag("Enemy"))
            {
                hitEnemy = true;
            }
            else
            {
                hitEnemy = false;
            }
        }
        else
        {
            
            particles.Play();
            
            //disable collider so that animation does not play when rock is touched by a moving character or if it collides with ground again;
            //keep disabled until rock is launched again
            //GetComponent<Collider>().enabled = false;
            hitEnemy = false;
        }
    }
}

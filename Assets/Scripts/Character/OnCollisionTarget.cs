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
    public void OnCollisionEnter(Collision collision)
    {
        GameObject hitCharacter = collision.gameObject;
        Debug.Log(hitCharacter.name);
        if (hitCharacter.CompareTag("Sheep") || hitCharacter.CompareTag("Enemy"))
        {
            if (timeStamp <= Time.time)
            {
                hitCharacter.GetComponentInParent<HitCount>().Value += 1;
            }
            timeStamp = Time.time + coolDownSeconds;

        }
        else
        {
            particles.Play();
            
        }
    }
}

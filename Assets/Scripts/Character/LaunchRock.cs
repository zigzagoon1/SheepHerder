using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LaunchRock : MonoBehaviour
{
    public GameObject rockPrefab;
    public float launchVelocity;

    
    [SerializeField] private int rockCount = 0;
    [SerializeField] private int maxRockCount = 5;
    private GameObject[] rocks;

    PlayerController player;
    
    int counter = 0;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        //add this scripts OnPlayerAttack method to callback for when player attacks, so that rock is launched when player attacks
        player.onAttackCallback += OnPlayerAttack;
        rocks = new GameObject[5];
        for (int i = 0; i < rocks.Length; i++)
        {
            rocks[i] = null;
        }
    }
    public void OnPlayerAttack()
    {
        // to begin level, instantiate new rock objects until reaching maxRockCount
        if (rockCount < maxRockCount)
        {
            GameObject rock = Instantiate(rockPrefab, transform.position, transform.rotation);
            Rigidbody rockRB = rock.GetComponent<Rigidbody>();
            rockRB.AddRelativeForce(Vector3.forward * launchVelocity, ForceMode.VelocityChange);
            rocks[System.Array.FindIndex(rocks, x => x == null)] = rock;
        }

        // once maxRockCount has been reached, instead of instantiating new game objects, move existing rock back to start position and fire that one again
        //this cycles through the rocks starting with the one that was launched least recently, so they disappear from where they landed in the order they were fired
        else
        {
            rockCount = rocks.Count<GameObject>();

            //re-enable the collider which is set to inactive in OnCollisionTarget, script attached to each rock game object 
            //rocks[counter].GetComponent<Collider>().enabled = true;

            //return an already launched rock to original launch position to be launched again
            rocks[counter].transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            rocks[counter].GetComponent<Rigidbody>().AddForce(transform.forward * launchVelocity, ForceMode.VelocityChange);
            counter++;
        }
        if (counter >= maxRockCount)
        {
            counter = 0;
        }
        rockCount++;
    }
}

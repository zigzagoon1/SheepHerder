using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LaunchRock : MonoBehaviour
{
    public GameObject rockPrefab;
    public float launchVelocity;
    public float drag;

    [SerializeField] private int rockCount = 0;
    private GameObject[] rocks;

    PlayerController player;
    //
    int counter = 0;
    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        player.onAttackCallback += OnPlayerAttack;
        rocks = new GameObject[5];
        for (int i = 0; i < rocks.Length; i++)
        {
            rocks[i] = null;
        }
    }
    public void OnPlayerAttack()
    {
        if (rockCount < 5)
        {
            GameObject rock = Instantiate(rockPrefab, transform.position, transform.rotation);
            Rigidbody rockRB = rock.GetComponent<Rigidbody>();
            rockRB.AddRelativeForce(Vector3.forward * launchVelocity, ForceMode.Impulse);
            rockRB.drag = drag;
            if (rockCount == 0)
            {
                rocks[0] = rock;
            }
            else
            {
                rocks[System.Array.FindIndex(rocks, x => x == null)] = rock;
            }
        }
        else
        {
            rockCount = rocks.Count<GameObject>();
            rocks[counter].transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            rocks[counter].GetComponent<Rigidbody>().AddForce(transform.forward * launchVelocity, ForceMode.Impulse);
            rocks[counter].GetComponent<Rigidbody>().drag = drag;
            counter++;
        }
        if (counter >= 5)
        {
            counter = 0;
        }
        rockCount++;
    }
}

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
    int counter = 0;
    private void Start()
    {

    }

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
            rockRB.AddRelativeForce(new Vector3(0, 0, launchVelocity), ForceMode.Impulse);
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
        if (rockCount >= 5)
        {
            rockCount = rocks.Count<GameObject>();
            //rocks[counter].gameObject.transform.position = startPosition;
            //rocks[counter].gameObject.transform.rotation = startRotation;
            rocks[counter].gameObject.transform.position = transform.position;
            rocks[counter].gameObject.transform.rotation = transform.rotation;
            rocks[counter].GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, launchVelocity), ForceMode.Impulse);
            rocks[counter].GetComponent<Rigidbody>().drag = drag;
/*            Destroy(rocks[counter].gameObject);
            rocks[counter] = null;
            rocks[counter] = Instantiate(rockPrefab, transform.position, transform.rotation);*/
            counter++;
        }
        if (counter >= 5)
        {
            counter = 0;
        }




        rockCount++;


    }
}

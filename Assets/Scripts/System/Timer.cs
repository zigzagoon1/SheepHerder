using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public bool wolfCooldownTimer;
    public bool playerCooldownTimer;
    public bool isFleeing;

    public IEnumerator CooldownTimer(float cooldownTime, string caller)
    {
        float timer = 0;
        if (caller == "Wolf")
        {
            wolfCooldownTimer = true;
        }
        else if (caller == "Player")
        {
            playerCooldownTimer = true;
        }
        while (timer <= cooldownTime)
        {
            timer += Time.deltaTime;
            yield return null;//new WaitForSeconds(0.25f);
        }
        wolfCooldownTimer = false;
        playerCooldownTimer = false;
    }
    public IEnumerator FleeTimer(float fleeTimerEnd)
    {
        float fleeTimer = 0;
        isFleeing = true;

        while (fleeTimer < fleeTimerEnd)
        {
            fleeTimer += Time.deltaTime;
            yield return null;
        }
        isFleeing = false;
    }
}

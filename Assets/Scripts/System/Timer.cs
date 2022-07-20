using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;


public class Timer : MonoBehaviour
{
    public bool wolfCooldownTimerActive;
    public bool playerCooldownTimerActive;
    public bool followBellTimerActive;
    public bool isFleeing;

    //cooldown timer for player and wolf attack, and player ringing bell
    public IEnumerator CooldownTimer(float cooldownTime, string caller, [CallerMemberName] string callingMethod = "")
    {
        float timer = 0;
        if (caller == "Wolf")
        {
            wolfCooldownTimerActive = true;
        }
        else if (caller == "Player")
        {
            if (callingMethod == "OnRingBell")
            {
                followBellTimerActive = true;
            }
            else
            {
                playerCooldownTimerActive = true;
            }
        }
        while (timer <= cooldownTime)
        {
            timer += Time.deltaTime;
            yield return null;//new WaitForSeconds(0.25f);
        }
        wolfCooldownTimerActive = false;
        playerCooldownTimerActive = false;
    }

    //timer for how long sheep flees for after being attacked
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

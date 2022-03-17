using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    public float radius;
    public float attackCooldown;
    public float bellTimer;
    public bool followBell = false;

    public Camera mainCamera;

    public delegate void Attack();
    public Attack onAttackCallback;

    [SerializeField] HitsToDefeat hitsToDefeat;
    [SerializeField] SpeedSO speed;
    [SerializeField] float rotationSpeed;
    [SerializeField] Transform playerInputSpace = default;

    private CharacterController playerController;
    //private NavMeshAgent agent;
    private Vector3 move;
    private readonly float gravity = -3f;
    private bool attackCooldownActive;
    private Vector3 desiredVelocity;
    private Timer timer;
    private void Start()
    {
        playerController = GetComponent<CharacterController>();
        timer = FindObjectOfType<Timer>();
        //agent = GetComponent<NavMeshAgent>();
        //agent.speed = speed.Value;
        attackCooldownActive = false;
    }
    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        if (playerInputSpace)
        {
            if (playerInputSpace)
            {
                Vector3 forward = playerInputSpace.forward;
                forward.y = 0f;
                forward.Normalize();
                Vector3 right = playerInputSpace.right;
                right.y = 0f;
                right.Normalize();
                move = (forward * inputVec.y + right * inputVec.x) * speed.Value;
                move.y = gravity * speed.Value;
            }
        }
        else
        {
            move = new Vector3(inputVec.x, gravity, inputVec.y);
        }
    }

    //play animation, add cooldown for attack
    public void OnAttack()
    {
        if (timer.playerCooldownTimer == false)
        {
            timer.StartCoroutine(timer.CooldownTimer(attackCooldown, "Player"));

            if (onAttackCallback != null)
            {
                onAttackCallback.Invoke();
            }
        }
        else
        {
            return;
        }
    }
    public void OnRingBell()
    {
        if (timer.playerCooldownTimer == false)
        {
            timer.StartCoroutine(timer.CooldownTimer(bellTimer, "Player"));
        }
    }


    //timer for various cooldowns
/*    public IEnumerator Timer([CallerMemberName] string callingMethod = "")
    {
        if (callingMethod == "OnAttack")
        {
            Debug.Log("on attack called, timer started");
            attackCooldownActive = true;
            timer = 0;
            while (timer <= attackCooldown)
            {
                timer += Time.deltaTime;
                yield return new WaitForSeconds(0.25f);
            }
            attackCooldownActive = false;
        }
        else if (callingMethod == "OnRingBell")
        {
            Debug.Log("bell rung, timer started");
            followBell = true;
            timer = 0;
            while (timer <= bellTimer)
            {
                timer += Time.deltaTime;
                yield return new WaitForSeconds(0.5f);
            }
            followBell = false;
        }
        else if (callingMethod == "")
        {
            Debug.Log("no calling method name");
        }
    }*/

    private void FixedUpdate()
    {
        if (move != Vector3.zero && move != null)
        {
            if (move.x != 0 || move.z != 0)
            {

                transform.Rotate(new Vector3(move.x, 0, move.y) * Time.deltaTime, Space.Self);
                transform.forward = new Vector3(move.x, 0, move.z);
            }
            playerController.Move(Time.deltaTime * move);
        }
    }
}

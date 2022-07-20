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
    [SerializeField, Range(0, 100)] float maxAcceleration = 10f;
    [SerializeField] Rect allowedArea;
    [SerializeField, Min(0f)] float probeDistance = 1f;

    private CharacterController playerController;
    //private NavMeshAgent agent;
    private Vector3 move;
    public readonly Vector3 gravity = new Vector3(0, -3f, 0);
    private Timer timer;
    private int stepsSinceLastGrounded;
    bool onGround;
    InputActionAsset playerInput;
    RockTrajectory trajectoryRenderer;
    private void Start()
    {
        playerController = GetComponent<CharacterController>();
        timer = FindObjectOfType<Timer>();
        trajectoryRenderer = GetComponentInChildren<RockTrajectory>();
        playerInput = GetComponent<PlayerInput>().actions;
        playerInput.Enable();
        playerInput.FindAction("LaunchAttack", true).performed += _ => OnAimRock();
        playerInput.FindAction("LaunchAttack", true).canceled += _ => PlayerAttack();
    }
    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        if (playerInputSpace)
        {

            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();
            Vector3 right = playerInputSpace.right;
            right.y = 0f;
            right.Normalize();
            move = ((forward * inputVec.y + right * inputVec.x));
            move.y = gravity.y * Time.deltaTime * speed.Value;
            move.Normalize();
        }
/*        else
        {
            move = new Vector3(inputVec.x, gravity, inputVec.y);
        }*/
    }


    //called when attack button is held down
    public void OnAimRock()
    {

        Debug.Log("aim rock");
        //trajectoryRenderer.DrawTrajectory();
        //if arrow keys pressed, move rock trajectory, otherwise launch rock in forward direction
        Vector2 input = playerInput.FindAction("AimAttack", true).ReadValue<Vector2>();
        //Vector2 input = playerInput.Player.AimAttack.ReadValue<Vector2>();
        trajectoryRenderer.transform.Rotate(new Vector2(input.y, input.x));
        trajectoryRenderer.DrawTrajectory();
    }

    //called when attack button is released
    public void PlayerAttack()
    {
        Debug.Log("attack");
        trajectoryRenderer.ClearTrajectory();

        if (timer.playerCooldownTimerActive == false)
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
        
        trajectoryRenderer.transform.localEulerAngles = Vector3.zero;
    }
    public void OnRingBell()
    {
        if (timer.followBellTimerActive == false)
        {
            timer.StartCoroutine(timer.CooldownTimer(bellTimer, "Player"));
            followBell = true;
        }
        else
        {
            followBell = false;
        }
    }
    private void FixedUpdate()
    {
        //Debug.Log(playerController.velocity);

        CheckOnGround();
        if (move.x != 0 || move.z != 0)
        {

            transform.Rotate(new Vector3(move.x, 0, move.y) * Time.deltaTime, Space.Self);
            transform.forward = new Vector3(move.x, 0, move.z);

            Vector3 newPosition = transform.localPosition + move;
            if (newPosition.x < allowedArea.xMin)
            {
                newPosition.x = allowedArea.xMin;
                move.x = 0f;
            }
            else if (newPosition.x > allowedArea.xMax)
            {
                newPosition.x = allowedArea.xMax;
                move.x = 0f;
            }
            if (newPosition.z < allowedArea.yMin)
            {
                newPosition.z = allowedArea.yMin;
                move.z = 0f;
            }
            else if (newPosition.z > allowedArea.yMax)
            {
                newPosition.z = allowedArea.yMax;
                move.z = 0f;
                
            }
        }
        playerController.Move(speed.Value * Time.deltaTime * move);
        
    }
    void CheckOnGround()
    {
        stepsSinceLastGrounded += 1;
        if (onGround || SnapToGround())
        {
            stepsSinceLastGrounded = 0;
        }
    }
    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1)
        {
            Debug.Log("steps since last grounded not greater than 1");
            return false;
        }
        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, probeDistance))
        {
            Debug.Log("no hit collider within probe distance");
            return false;
        }
        if (!hit.collider.gameObject.CompareTag("Ground"))
        {
            Debug.Log("hit object not ground");
            return false;
        }
        float dot = Vector3.Dot(move, hit.normal);
        if (dot > 0f)
        {
            move = (move - hit.normal * dot).normalized * speed.Value;
        }
        return true;
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = false;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;

public class PlayerController : MonoBehaviour
{
    public float radius;
    public float hitCount;
    public float attackCooldown;
    public float timer;
    public float bellTimer;
    public bool followBell = false;

    public Camera mainCamera;

    private GameObject attackTarget;
    public GameObject AttackTarget { get { return attackTarget; } set { attackTarget = value; } }
    private string attackTargetType;
    public string AttackTargetType { get { return attackTargetType; } set { attackTargetType = value; } }

    public delegate void Attack();
    public Attack onAttackCallback;

    public delegate void AttackTargetSet();
    public AttackTargetSet onAttackTargetSetCallback;

    [SerializeField] HitsToDefeat hitsToDefeat;
    [SerializeField] SpeedSO speed;
    [SerializeField] float rotationSpeed;
    private Controls controls;

    private CharacterController playerController;
    private Vector3 move;
    private float gravity = -3f;
    private bool attackCooldownActive;
    private CameraFollow cam;
    private void Awake()
    {
        controls = new Controls();
    }
    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
    private void Start()
    {
        playerController = GetComponent<CharacterController>();
        attackCooldownActive = false;
        onAttackTargetSetCallback += AddtoHitCount;
        cam = mainCamera.GetComponent<CameraFollow>();
        
    }
    

    public void OnMove(InputValue input)
    {
        Vector2 inputVec = input.Get<Vector2>();
        move = new Vector3(inputVec.x, gravity, inputVec.y);
    }
    public void OnAttack()
    {
        //play animation, add cooldown for attack

        if (attackCooldownActive == false)
        {
            StartCoroutine(Timer());

            if (onAttackCallback != null)
            {
                onAttackCallback.Invoke();
            }
        }
    }
    public void OnRingBell()
    {
        StartCoroutine(Timer());
    }
    public IEnumerator Timer([CallerMemberName] string callingMethod = "")
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

    }
    public void AddtoHitCount()
    {
        Debug.Log("add to hit count");
        if (attackCooldownActive == false)
        {
            if (attackTarget != null)
            {
                if (attackTargetType == "Enemy")
                {
                    Enemy enemy = attackTarget.GetComponent<Enemy>();
                    enemy._hitCount++;
                }
                else if (attackTargetType == "Sheep")
                {
                    Sheep sheep = attackTarget.GetComponent<Sheep>();
                    //sheep._hitCount++;
                }
                else if (attackTargetType == "Environment")
                {
                    //
                }
                else
                {
                    Debug.Log("attack target type = null");
                }
            }
        }


    }
    private void FixedUpdate()
    {
        if (move != Vector3.zero)
        {
            if (move.x != 0 || move.z != 0)
            {
/*                Quaternion toRotation = Quaternion.LookRotation(new Vector3(move.x, 0, move.z), Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);*/
                transform.Rotate(new Vector3(move.x, 0, move.y) * Time.deltaTime, Space.Self);
                transform.forward = new Vector3(move.x, 0, move.z);
            }
            playerController.Move(move * speed.Value * Time.deltaTime);
        }
    }



}

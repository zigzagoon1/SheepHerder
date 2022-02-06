using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour, ICharacter
{
    public GameObject player;
    public Vector3 attackerDirection;


    public HitsToDefeat hitsToDefeat;
    public SpeedSO speed;
    public SpeedSO fleeSpeed;

    public FSM fsm;
    public FSM.State _follow;
    public FSM.State _flee;
    FSM.State _wander;
    FSM.State _die;
    FSM.State _currentState;

    [SerializeField] int _hitCount;
    public int HitCount { get { return _hitCount; } set { _hitCount = value; } }
    [SerializeField] float wanderTime;
    [SerializeField] float wanderRadius;
    [SerializeField] float followTime;
    [SerializeField] float fleeTimer;
    [SerializeField] float fleeTimerEnd;
    public GameObject attacker;
    float timer;
    [SerializeField] bool isFleeing = false;

    Animator animator;
    NavMeshAgent agent;
    
    int _hitsToDefeat;
    int previousHitCount;
    float _speed;
    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        _flee = FSM_Flee;
        _follow = FSM_Follow;
        _wander = FSM_Wander;
        _die = FSM_Die;
        fsm = new FSM();
        fsm.OnSpawn(_wander);
        timer = wanderTime;
        _speed = speed.Value;
        _hitsToDefeat = hitsToDefeat.Value;
        
    }
    private void Update()
    {
        fsm.OnUpdate();
    }

    void FSM_Flee(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            //establish point in opposite direction of attack direction
            previousHitCount = HitCount;
            _currentState = _flee;
            animator.SetBool("isRunning", true);
            fleeTimerEnd = Random.Range(3.5f, 6);
            agent.speed = fleeSpeed.Value;
            attackerDirection = transform.position - attacker.transform.position;
            StartCoroutine(Timer());

            Debug.Log("sheep entered flee state");
            
        }
        if (step == FSM.Step.Update)
        {
            //begin movement towards established flee point
            

            agent.SetDestination(attacker.transform.position + (attackerDirection * 10));
            if (agent.isStopped)
            {
                agent.Move(Vector3.forward);
            }
            if (HitCount > previousHitCount)
            {
                StopCoroutine(Timer());
                StartCoroutine(Timer());
                agent.SetDestination(attacker.transform.position + (attackerDirection * 10));
            }
            if (!isFleeing)
            {
                fleeTimer = 0;
                fsm.TransitionTo(_wander);
            }
            if (player.GetComponent<PlayerController>().followBell)
            {
                fsm.TransitionTo(_follow);
            }
            if (_hitCount >= _hitsToDefeat)
            {
                fsm.TransitionTo(_die);
            }
        }
        if (step == FSM.Step.Exit)
        {
            animator.SetBool("isRunning", false);
            attacker = null;
        }
    }
    void FSM_Follow(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _follow;
            Debug.Log("sheep is following player");
            timer = 0;
            animator.SetBool("isWalking", true);
        }
        if (step == FSM.Step.Update)
        {
            //begin movement to player position
            timer += Time.deltaTime;
            agent.speed = _speed;
            agent.SetDestination(player.transform.position);
            if (timer >= followTime)
            {
                fsm.TransitionTo(_wander);
            }
            if (attacker != null)
            {
                fsm.TransitionTo(_flee);
            }
            if (_hitCount >= _hitsToDefeat)
            {
                fsm.TransitionTo(_die);
            }
            
        }
        if (step == FSM.Step.Exit)
        {
            animator.SetBool("isWalking", false);
            //return to wander after 3-4 seconds, or transition to flee if attacked
        }

    }
    void FSM_Wander(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _wander;
            Debug.Log("sheep is wandering");
            timer = wanderTime;
            animator.SetBool("isWalking", true);
        }
        if (step == FSM.Step.Update)
        {
            if (agent != null)
            {
                timer += Time.deltaTime;
                if (timer >= wanderTime)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    agent.speed = _speed;
                    agent.SetDestination(newPos);
                    timer = 0;
                }
            }
            if (player.GetComponent<PlayerController>().followBell)
            {
                fsm.TransitionTo(_follow);
            }
            if (attacker != null)
            {
                fsm.TransitionTo(_flee);
            }
            //if sheep is hit by enemy or player, transition to flee
            if (_hitCount >= _hitsToDefeat)
            {
                fsm.TransitionTo(_die);
            }
        }
        if (step == FSM.Step.Exit)
        {
            animator.SetBool("isWalking", false);
        }
    }
    public void FSM_Die(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _die;
            Debug.Log("sheep died");
        }
        if (step == FSM.Step.Update)
        {
            //
        }
        if (step == FSM.Step.Exit)
        {
            //
        }
    }
    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
    private IEnumerator Timer()
    {
        fleeTimer = 0;
        isFleeing = true;

        while (fleeTimer < fleeTimerEnd)
        {
            fleeTimer += Time.deltaTime;
            yield return null;
        }
        isFleeing = false;
    }
    public IEnumerator FaceTarget()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        yield return null;
    }
}

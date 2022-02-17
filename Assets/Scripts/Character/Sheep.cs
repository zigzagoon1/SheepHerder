using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Sheep : MonoBehaviour, ICharacter
{
    public GameObject player;
    public GameObject attacker = null;

    public Vector3 attackerDirection;

    public HitsToDefeat hitsToDefeat;
    public SpeedSO speed;
    public SpeedSO fleeSpeed;
    public int HitCount; //{ get { return _hitCount; } set { _hitCount = value; } }

    private float _speed;

    int _hitCount;
    FSM fsm;
    FSM.State _follow;
    FSM.State _flee;
    FSM.State _wander;
    FSM.State _die;
    FSM.State _currentState;

    [SerializeField] float wanderTime;
    [SerializeField] float wanderRadius;
    float timer;
    [SerializeField] float followTime;

    [SerializeField] float fleeTimer;
    [SerializeField] float fleeTimerEnd;
    [SerializeField] bool isFleeing = false;

    Animator animator;
    NavMeshAgent agent;
    
    int previousHitCount;
    private void Start()
    {
        _hitCount = GetComponent<HitCount>().Value;
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
        
    }
    private void LateUpdate()
    {
        fsm.OnUpdate();
    }

    void FSM_Flee(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _flee;
            //keep track of previous hit count in order to restart flee timer if attacked again
            previousHitCount = _hitCount;
            fleeTimerEnd = Random.Range(3.5f, 6);

            //set speed to flee speed
            animator.SetBool("isRunning", true);
            _speed = fleeSpeed.Value;
            agent.speed = _speed;

            //establish point in opposite direction of attack direction
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
            if (_hitCount > previousHitCount)
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
            if (_hitCount >= hitsToDefeat.Value)
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
            if (_hitCount >= hitsToDefeat.Value)
            {
                fsm.TransitionTo(_die);
            }
            
        }
        if (step == FSM.Step.Exit)
        {
            //animator.SetBool("isWalking", false);
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
            //if sheep is hit by enemy or player, transition to flee
            if (attacker != null)
            {
                fsm.TransitionTo(_flee);
            }
            
            if (_hitCount >= hitsToDefeat.Value)
            {
                fsm.TransitionTo(_die);
            }
        }
        if (step == FSM.Step.Exit)
        {
            //animator.SetBool("isWalking", false);
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

    public void AddToHitCount()
    {
        HitCount++;
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

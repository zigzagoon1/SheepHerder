using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, ICharacter
{

    public int _hitCount;
    [SerializeField] EnemyType type;

    [SerializeField] float wanderTimer, wanderRadius, distanceMultiplier, _speed;

    [SerializeField] int _hitsToDefeat;
    //destination points for paths for the enemy to walk along
    [SerializeField] Transform[] waypoints;

    [SerializeField] GameObject target = null, player;

    //increasing or decreasing this variable affects the total cooldown time before player can reuse attack
    [SerializeField] float attackCooldown;
    //this timer is incremented until it reaches attackCooldown
    private float attackTimer;
    private bool attackCooldownInactive = true, isWandering, isFacingTarget = false;

    NavMeshPath patrolPathA, patrolPathB;
    NavMeshAgent _agent;

    Animator _animator;

    FSM fsm;
    FSM.State _patrol, _chase, _attack, _die, _currentState;    

    float newPathTimer = 0;
    float radius = 15f;
    Sheep[] activeSheep;
    Vector3 chaseStartLocation;

    private void Start()
    {
        _hitCount = 0;
        _speed = type.baseSpeed.Value;
        _hitsToDefeat = type.hitsToDefeat.Value;
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        patrolPathA = new NavMeshPath();
        patrolPathB = new NavMeshPath();
        _patrol = FSM_Patrol;
        _chase = FSM_Chase;
        _attack = FSM_Attack;
        _die = FSM_Die;
        fsm = new FSM();
        fsm.OnSpawn(_patrol);
    }
    private void Awake()
    {
        activeSheep = FindObjectsOfType<Sheep>();
    }
    private void FixedUpdate()
    {
        fsm.OnUpdate();
    }
    void FSM_Patrol(FSM fsm, FSM.Step step, FSM.State state)
    {
        
        if (step == FSM.Step.Enter)
        {
            _agent.ResetPath();
            
            if (target != null)
            {
                target = null;
            }
            if (_agent.transform.position == waypoints[0].position)
            {
                _agent.CalculatePath(waypoints[1].position, patrolPathB);
                _agent.SetPath(patrolPathB);
            }
            else if (_agent.transform.position == waypoints[1].position)
            {
                _agent.CalculatePath(waypoints[0].position, patrolPathA);
                _agent.SetPath(patrolPathA);
            }
            else
            {
                float distance = Vector3.Distance(transform.position, waypoints[0].transform.position);
                float distance2 = Vector3.Distance(transform.position, waypoints[1].transform.position);
                if (distance < distance2)
                {
                    _agent.CalculatePath(waypoints[0].position, patrolPathA);
                    _agent.SetPath(patrolPathA);
                }
                else
                {
                    _agent.CalculatePath(waypoints[1].transform.position, patrolPathB);
                    _agent.SetPath(patrolPathB);
                }
            }
            _agent.speed = _speed;
            _currentState = _patrol;
            Debug.Log("enter patrol state");
        }
        if (step == FSM.Step.Update)
        {
            

            if (_agent.transform.position == _agent.pathEndPosition)
            {
                newPathTimer += Time.deltaTime;
                if (newPathTimer > 1)
                {
                    fsm.TransitionTo(_patrol);
                    newPathTimer = 0;
                }
            }
            
            if (target == null)
            {
                if (Vector3.Distance(player.transform.position, transform.position) < radius)
                {
                    target = player;
                    fsm.TransitionTo(_chase);
                }
            }

            for (int i = 0; i < activeSheep.Length; i++)
            {
                if (Vector3.Distance(activeSheep[i].transform.position, transform.position) < radius)
                {
                    if (_hitCount > 0)
                    {
                        break;
                    }
                    target = activeSheep[i].gameObject;
                    fsm.TransitionTo(_chase);
                }
            }
/*            if (isWandering && _agent != null)
            {
                timer += Time.deltaTime;
                if (timer >= wanderTimer)
                {
                    Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
                    _agent.SetDestination(newPos);
                    timer = 0;
                }
            }*/
            
            if (_hitCount >= _hitsToDefeat)
            {
                fsm.TransitionTo(_die);
            }
        }
        if (step == FSM.Step.Exit)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
    }
    void FSM_Chase(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _agent.speed = 2.5f;
            _currentState = _chase;
            Debug.Log("enter chase state");
            chaseStartLocation = transform.position;
        }
        if (step == FSM.Step.Update)
        {
            if (target != null)
            {
                //_agent.CalculatePath(target.transform.position, targetPath);
                _agent.SetDestination(target.transform.position);
                _agent.isStopped = false;
                if (Vector3.Distance(transform.position, target.transform.position) < 3f)
                {
                    fsm.TransitionTo(_attack);
                }
                if (Vector3.Distance(transform.position, target.transform.position) > radius)
                {
                    fsm.TransitionTo(_patrol);
                }
                if (_hitCount >= _hitsToDefeat)
                {
                    fsm.TransitionTo(_die);
                }
            }

            
        }
        if (step == FSM.Step.Exit)
        {
            
            _agent.isStopped = true;
            _agent.ResetPath();
        }

    }
    void FSM_Attack(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _attack;
            Debug.Log("enter attack state");
            if (target == player)
            {
                
            }
            else
            {
                target.GetComponent<Sheep>().attacker = this.gameObject;
                target.GetComponent<Sheep>().attackerDirection = transform.position - chaseStartLocation;
            }

        }
        if (step == FSM.Step.Update)
        {
            if (Vector3.Distance(transform.position, target.transform.position) >= 5f && attackCooldownInactive)
            {
                fsm.TransitionTo(_chase);
            }

            if (Vector3.Distance(transform.position, target.transform.position) < 3f)
            {
                if (attackCooldownInactive)
                {
                    if (target == player)
                    {
                        target.GetComponent<PlayerController>().hitCount++;
                        StartCoroutine(Timer());
                    }
                    else
                    {
                        target.GetComponent<Sheep>().HitCount++;
                        Debug.Log(target.GetComponent<Sheep>().HitCount);
                        StartCoroutine(Timer());
                    }
                }
            }


            //code to play attack animation, add to player/sheep hitCount for each attack

            if (_hitCount >= _hitsToDefeat)
            {
                fsm.TransitionTo(_die);
            }
        }
        if (step == FSM.Step.Exit)
        {
           
        }
    }
    public void FSM_Die(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _currentState = _die;
            Debug.Log("enter die state");
            //playAnimation
            //playSound
        }
        if (step == FSM.Step.Update)
        {
            //if animation is done playing, transition to exit 
        }
        if (step == FSM.Step.Exit)
        {
            Debug.Log($"{this.name} died!");
            Destroy(gameObject);
        }
    }
    public IEnumerator Timer()
    {
        attackTimer = 0;
        attackCooldownInactive = false;

        while (attackTimer <= attackCooldown)
        {
            attackTimer += Time.deltaTime;
            yield return null;//new WaitForSeconds(0.25f);
        }
        attackCooldownInactive = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 11)
        {
            _agent.isStopped = true;
            _hitCount++;
            target = player;
            StartCoroutine(FaceTarget());
            if (_currentState != _chase)
            {
                _agent.ResetPath();
                fsm.TransitionTo(_chase);

            }
        }
    }

    public Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;
        NavMesh.SamplePosition(randDirection, out NavMeshHit navHit, dist, layermask);
        return navHit.position;
    }
    //if player or sheep is targeted, face it-- needs fixing
    public IEnumerator FaceTarget()
    {
        while (isFacingTarget == false)
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            if (direction == Vector3.zero)
            {
                direction = new Vector3(0.01f, 0.01f, 0.01f);
            }
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
            yield return null;
            
        }
    }
}

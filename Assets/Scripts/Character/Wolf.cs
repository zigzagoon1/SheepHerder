using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Wolf : Enemy
{
    public string tactic;
    [SerializeField] Transform ground;
    //if distance to target is less than chase radius, transition to chase even if wolves aren't gathered/aren't hungry
    [SerializeField] float chaseRadius;
    //once hunger reaches hunger threshold, wolf will chase & attempt to attack player despite tactic
    [SerializeField] float hungerThreshold;

    FSM fsm;
    FSM.State _tactics, _chase, _attack, _die, _currentState;

    Timer timer;
    Vector3 previousTargetPosition;
    //path for the hiding wolf to use
    NavMeshPath hidePath;
    //path for the wolves trying to find the hidden wolf to use
    NavMeshPath findPath;
    bool isHidden;
    //radius for closest tree to use for hiding
    float treeRadius;
    //if distance is less than attackRadius, wolf will transition to attack
    float attackRadius;
    //the more wolves are gathered in one spot, the greater the probability to transition to chase or attack
    float probability;
    float hunger;

    List<Wolf> activeWolves;
    List<Sheep> activeSheep;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>().gameObject;
        timer = FindObjectOfType<Timer>();
        _speed = type.baseSpeed.Value;
        _hitsToDefeat = type.hitsToDefeat.Value;
        hidePath = new NavMeshPath();
        findPath = new NavMeshPath();
        hunger = 0f;

        //_animator = GetComponent<Animator>();
        fsm = new FSM();
        _tactics = FSM_Tactics;
        _chase = FSM_Chase;
        _attack = FSM_Attack;
        _die = FSM_Die;
    }

    private void OnEnable()
    {
        if (GameManager.instance != null && !GameManager.instance.activeWolves.Contains(this))
        {
            GameManager.instance.UpdateActiveWolves();
        }
    }
    private void Start()
    {
        GameManager.instance.onUpdateWolvesCallback += UpdateWolves;
        GameManager.instance.onUpdateSheepCallback += UpdateSheep;
        activeWolves = GameManager.instance.activeWolves;
        fsm.OnSpawn(_tactics);

    }
    private void FixedUpdate()
    {
        fsm.OnUpdate();

    }

    //wolves start level using tactics to team up and attack player & sheep
    void FSM_Tactics(FSM fsm, FSM.Step step, FSM.State state)
    {

        if (step == FSM.Step.Enter)
        {
            if (_agent.hasPath)
            {
                _agent.ResetPath();
            }
            if (target == null)
            {
                target = player;
            }
            _agent.speed = _speed;
            _currentState = _tactics;
            previousTargetPosition = target.transform.position;
            probability = 0.001f;
            hunger = 0;
            GetTactic();
            Debug.Log("enter tactics state");
        }
        if (step == FSM.Step.Update)
        {
            Vector3 targetPos = target.transform.position;

            if (tactic == "StayHidden")
            {
                Vector3 destination = StayHidden();
                _agent.CalculatePath(destination, hidePath);
                _agent.SetPath(hidePath);
                hunger += Time.deltaTime;
                //Debug.Log(hunger);
                if (targetPos != previousTargetPosition)
                {
                    destination = StayHidden();
                    _agent.CalculatePath(destination, hidePath);
                    _agent.SetPath(hidePath);
                    previousTargetPosition = targetPos;
                    if (!_agent.pathPending && !_agent.hasPath)
                    {
                        if (_agent.velocity.sqrMagnitude == 0f)
                        {
                            StartCoroutine(FaceTarget());
                        }
                    }
                }
                if (Vector3.Distance(transform.position, targetPos) < chaseRadius)
                {
                    fsm.TransitionTo(_chase);
                }
            }
            if (tactic == "FindOtherWolves")
            {
                _agent.stoppingDistance = 5f;
                Wolf hiddenWolf = GameManager.instance.activeWolves.Find(x => x.tactic == "StayHidden");
                _agent.CalculatePath(hiddenWolf.transform.position, findPath);
                _agent.SetPath(findPath);
                //if destination reached, StayHidden()
                if (Vector3.Distance(transform.position, targetPos) < chaseRadius)
                {
                    fsm.TransitionTo(_chase);
                }

                
                //if distance between 2 wolves is less than radius, probability to transition to chase goes up, wolves target different objects to chase & attack
                //have wolves keep to shadows and avoid players view...?
            }
            //if wolves take too long to find each other, they can still transition to chase if their hunger level reaches the threshold
            if (hunger >= hungerThreshold)
            {
                float distToPlayer = Vector3.Distance(transform.position, targetPos);
                float closestTarget = distToPlayer;
                GameObject closestSheep = GameManager.instance.activeSheep[0].gameObject;
                for (int i = 0; i < GameManager.instance.activeSheep.Count; i++)
                {
                    closestTarget = Mathf.Min(closestTarget, Vector3.Distance(transform.position, GameManager.instance.activeSheep[i].transform.position));
                    if (closestTarget == Vector3.Distance(transform.position, GameManager.instance.activeSheep[i].transform.position))
                    {
                        closestSheep = GameManager.instance.activeSheep[i].gameObject;
                    }
                }
                target = closestTarget == distToPlayer ? player : closestSheep;
                if (Vector3.Distance(transform.position, targetPos) <= attackRadius)
                {
                    fsm.TransitionTo(_attack);
                }
                else
                {
                    fsm.TransitionTo(_chase);
                }
            }
/*
            for (int w = 0; w < GameManager.instance.activeWolves.Count; w++)
            {
                if (Vector3.Distance(GameManager.instance.activeWolves[w].transform.position, playerPos) <= chaseRadius)
                {
                    probability += w / GameManager.instance.activeWolves.Count; 
                }
            }*/



           /* for (int s = 0; s < GameManager.instance.activeSheep.Count; s++)
            {
                if (Vector3.Distance(GameManager.instance.activeSheep[s].transform.position, transform.position) < radius)
                {
                    if (hitCount.Value > 0)
                    {
                        attackTarget = player;
                        fsm.TransitionTo(_chase);
                        break;
                    }
                    attackTarget = GameManager.instance.activeSheep[s].gameObject;
                    fsm.TransitionTo(_chase);
                }
                if (Vector3.Distance(player.transform.position, transform.position) < radius)
                {
                    attackTarget = player;
                    fsm.TransitionTo(_chase);
                }
            }*/
            
            if (hitCount.Value >= _hitsToDefeat)
            {
                fsm.TransitionTo(_die);
            }
        }
        if (step == FSM.Step.Exit)
        {
            
        }
    }
    //chase player or sheep to attack
    void FSM_Chase(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            _agent.speed += 1;
            _agent.stoppingDistance = 3f;
            _currentState = _chase;
            Debug.Log("enter chase state");
            chaseStartLocation = transform.position;
            if (target == null)
            {
                target = player;
            }
            previousTargetPosition = target.transform.position;

        }
        if (step == FSM.Step.Update)
        {
            Vector3 playerPos = player.transform.position;
            if (!_agent.hasPath || playerPos != previousTargetPosition)
            {
                _agent.SetDestination(playerPos);
            }

/*            if (Vector3.Distance(transform.position, playerPos) > 3 && Vector3.Distance(transform.position, playerPos) < radius)
            {
                //animation/s for wolf approaching?? 
            }*/
            if (target == null)
            {
                for (int i = 0; i < GameManager.instance.activeSheep.Count; i++)
                {
                    if (Vector3.Distance(transform.position, GameManager.instance.activeSheep[i].transform.position) <= _agent.stoppingDistance)
                    {
                        target = GameManager.instance.activeSheep[i].gameObject;
                        fsm.TransitionTo(_attack);
                    }
                    else
                    {
                        target = null;
                    }
                }
                if (Vector3.Distance(transform.position, playerPos) <= treeRadius)
                {
                    target = player;
                }
                else
                {
                    target = null;
                }
            }
            else
            {
                fsm.TransitionTo(_attack);
            }
        }
        if (step == FSM.Step.Exit)
        {

            _agent.isStopped = true;
            _agent.ResetPath();
        }

    }
    //state wolf must be in to cause damage (add to hit count) to player or sheep
    void FSM_Attack(FSM fsm, FSM.Step step, FSM.State state)
    {
        if (step == FSM.Step.Enter)
        {
            
            _currentState = _attack;
            Debug.Log("enter attack state");
            target.GetComponent<HitCount>().Value += 1;
            timer.StartCoroutine(timer.CooldownTimer(attackCooldown, "Wolf"));
            //will be switching to event 
            if (target != player)
            {
                target.GetComponent<Sheep>().attacker = this.gameObject;
                target.GetComponent<Sheep>().attackerDirection = transform.position - chaseStartLocation;
            }

        }
        if (step == FSM.Step.Update)
        {
            if (Vector3.Distance(transform.position, target.transform.position) >= 5f)
            {
                fsm.TransitionTo(_chase);
            }
            //add code to play attack animation

            if (Vector3.Distance(transform.position, target.transform.position) < 3f)
            {
                if (!timer.wolfCooldownTimer)
                {
                    if (target != null)
                    {
                        target.GetComponent<HitCount>().Value++;
                        //trigger event to give sheep direction for flee
                    }
                }
            }
            if (hitCount.Value >= _hitsToDefeat)
            {
                fsm.TransitionTo(_die);
            }
        }
        if (step == FSM.Step.Exit)
        {

        }
    }
    //final code to be executed upon wolf being killed by player
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
    //make wolf hide behind nearest tree out of view of player
    //needs improvement, add code to attempt to avoid view of camera as well
    Vector3 StayHidden()
    {
        Vector3 direction = player.transform.position - transform.position;
        int layermask = 1<<11;
        treeRadius = 10f;
        Vector3 goal = transform.position;
        Physics.Raycast(transform.position, direction, out RaycastHit hit);

        Debug.DrawRay(transform.position, direction);
        
        if (hit.collider.gameObject.name == "PlayerParent")
        {
            isHidden = false;
            Collider[] closestTrees = Physics.OverlapSphere(transform.position, treeRadius, layermask);
            if (closestTrees[0] != null)
            {
                Vector3 behindTreeDirection = player.transform.position - closestTrees[0].transform.position;
                //behindTreeDirection /= behindTreeDirection.magnitude;
                behindTreeDirection.y = 0;
                Debug.DrawLine(closestTrees[0].transform.position, behindTreeDirection);
                if (isHidden == false)
                {
                    Vector3 otherSide = closestTrees[0].ClosestPointOnBounds(closestTrees[0].transform.position - behindTreeDirection);
                    otherSide.y = 0;
                    NavMesh.SamplePosition(otherSide, out NavMeshHit navHit, 2f, 1);
                    goal = navHit.position;
                }
            }
        }
        else if (hit.collider.gameObject.layer != player.layer)
        {
            isHidden = true;
        }
        return goal;
    }
    //assign a tactic for the wolf to use when in tactic state
    //wolf with index 0 will stay hidden and all other active wolves will attempt to form a pack to attack player/sheep together
    void GetTactic()
    {
        int iD = GetInstanceID();
        int index = 0;
        for (int i = 0; i < activeWolves.Count; i++)
        {
            if (iD == activeWolves[i].GetInstanceID())
            {
                index = i;
            }
        }
        if (index == 0)
        {
            tactic = "StayHidden";
        }
        else
        {
            tactic = "FindOtherWolves";
        }
        Debug.Log(tactic);
    }
    //called when GameManager.instance.activeWolves changes
    void UpdateWolves()
    {
        activeWolves = GameManager.instance.activeWolves;
    }
    //called when GameManager.instance.activeSheep changes
    void UpdateSheep()
    {
        activeSheep = GameManager.instance.activeSheep;
    }
    //visualize tree radius for finding closest tree to hide behind
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, treeRadius);
    }
    //if a rock (layer 10) hits the wolf, add a point to hit count
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 10)
        {
            _agent.isStopped = true;
            hitCount.Value++;
            target = player;
            StartCoroutine(FaceTarget());
            if (_currentState != _chase)
            {
                _agent.ResetPath();
                fsm.TransitionTo(_chase);

            }
        }
    }
    private void OnDestroy()
    {
        GameManager.instance.UpdateActiveWolves();
    }
}

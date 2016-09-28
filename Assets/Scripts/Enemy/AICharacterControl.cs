using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class AICharacterControl : LivingEntity
{
    public enum AIStates
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    public float walkSpeed = 1.5f;
    public float runSpeed = 3.0f;
    public float rotateSpeed = 360f;
    public float stoppingDistance = 2f;
    public float distanceToAttack = 3f;

    [SerializeField]
    private AreaTrigger searchAreaTrigger;
    
    private Transform _transform;
    private NavMeshAgent agent;
    private Animator anim;

    private Transform target = null;
    private AIStates currentState;

    void Awake()
    {
        _transform = GetComponent<Transform>();
        agent = GetComponentInChildren<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("No NavMeshAgent assigned. Character will now be disabled.");
            this.enabled = false;
            return;
        }
        agent.speed = runSpeed;

        anim = GetComponentInChildren<Animator>();
        anim.applyRootMotion = false;
    }

    protected override void Start()
    {
        base.Start();

        if (searchAreaTrigger != null)
            searchAreaTrigger.OnAreaEnter += OnSearchAreaEnter;
    }

    private void Update()
    {
        // 목적지 설정
        if (target != null)
        {
            float distance = Vector3.Distance(target.position, _transform.position);

            if (distance > stoppingDistance)
            {

                agent.SetDestination(target.position);
            }
           

            if (agent.remainingDistance > agent.stoppingDistance)
            {
            }
            else
            {
            }

            //if ((_target.position - _transform.position).magnitude > 2f)
            //{
            //    _movement.Move((_target.position - _transform.position).normalized);
            //}
        }

        UpdateAnimator();
    }

    

    protected virtual void Die()
    {
        Debug.Log(name + " Die");

        Destroy(this.gameObject);
        Debug.Log(name + " Destroy");
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);

        Debug.Log(name + " TakeDamage " + damage);
        anim.SetTrigger("Hit");
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    // Search Player Callback
    private void OnSearchAreaEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(name + " OnSearchAreaEnter other Player");
            this.SetTarget(other.gameObject.transform);
        }
    }

    // Animator
    private void UpdateAnimator()
    {
        if (anim == null)
            return;

        //anim.SetFloat("Speed", new Vector3(_characterControl.Velocity.x, 0f, _characterControl.Velocity.z).magnitude);
        //anim.SetBool("IsGrounded", _characterControl.IsGrounded);
        //anim.SetFloat("VerticalVelocity", _characterControl.Velocity.y);
    }

    private void UpdatePath(Transform target)
    {
        if (target != null)
        {
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            Vector3 targetPosition = target.position - dirToTarget * stoppingDistance;
            if (!IsDead)
            {
                agent.SetDestination(targetPosition);
            }
        }
    }

    // AI

    protected IEnumerator StateRoutine()
    {
        while (this.enabled)
        {
            yield return new WaitForEndOfFrame();
            switch (currentState)
            {
                case AIStates.Idle:
                    yield return StartCoroutine(Idle());
                    break;
                case AIStates.Chase:
                    yield return StartCoroutine(Chase());
                    break;
                case AIStates.Patrol:
                    yield return StartCoroutine(Patrol());
                    break;
                case AIStates.Attack:
                    yield return StartCoroutine(Attack());
                    break;
            }
        }
    }

    protected IEnumerator Idle()
    {
        float timer = 0f;
        float idleTime = Random.Range(2, 5);
        while (this.enabled && currentState == AIStates.Idle)
        {
            timer += Time.deltaTime;
            agent.speed = Mathf.Lerp(agent.speed, 0f, 2f * Time.deltaTime);

            // check transitions
            if (HasTarget())
            {
                currentState = AIStates.Chase;
                break;
            }
            else if (timer > idleTime)
            {
                currentState = AIStates.Patrol;
                break;
            }

            yield return null;
        }
    }

    private bool HasTarget()
    {
        return this.target != null;
    }

    private bool CanAttackDistance()
    {
        if (!HasTarget())
            return false;

        float distance = Vector3.Distance(this.target.position, _transform.position);
        return distance <= distanceToAttack; 
    }

    protected IEnumerator Chase()
    {
        while (this.enabled && currentState == AIStates.Chase)
        {
            agent.speed = Mathf.Lerp(agent.speed, runSpeed, 2f * Time.deltaTime);
            this.UpdatePath(this.target);

            // check transitions
            if (!HasTarget())
            {
                currentState = AIStates.Patrol;
                break;
            }
            else if(CanAttackDistance())
            {
                currentState = AIStates.Attack;
            }

            yield return null;
        }
    }

    protected IEnumerator Patrol()
    {
        while (this.enabled && currentState == AIStates.Patrol)
        {
            agent.speed = Mathf.Lerp(agent.speed, walkSpeed, 2f * Time.deltaTime);
            Transform waypoint = null;

            this.UpdatePath(waypoint);

            // check transitions
            if (!HasTarget())
            {
                currentState = AIStates.Patrol;
                break;
            }

            yield return null;
        }
    }

    protected IEnumerator Attack()
    {
        while (this.enabled && currentState == AIStates.Chase)
        {
            // check transition
            if (!HasTarget())
            {
                currentState = AIStates.Patrol;
                break;
            }
            else if (!CanAttackDistance())
            {
                currentState = AIStates.Chase;
                break;
            }

            // attack

            yield return null;
        }
    }
}

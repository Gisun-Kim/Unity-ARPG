using UnityEngine;
using System.Collections;

/*
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterMovement))]
public class EnemyControl : CharacterEntity
{
    public string name = "enemy";

    [SerializeField]
    private AreaTrigger _searchAreaTrigger;

    private Transform _transform;
    private CharacterMovement _movement;
    private NavMeshAgent _agent;

    private Transform _target;

    // Animation control
    private Animator _animator;

    void Awake()
    {
        _transform = GetComponent<Transform>();

        _agent = GetComponentInChildren<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updatePosition = true;

        _movement = GetComponent<CharacterMovement>();

        _animator = GetComponentInChildren<Animator>();
        _animator.applyRootMotion = false;
    }

    protected override void Start()
    {
        base.Start();
        Debug.Log(name + " Start");

        if (_searchAreaTrigger != null)
            _searchAreaTrigger.OnAreaEnter += OnSearchAreaEnter;
    }

    private void FixedUpdate()
    {
        // 목적지 설정
        if (_target != null)
        {
            _agent.SetDestination(_target.position);

            if (_agent.remainingDistance > _agent.stoppingDistance)
            {
                _movement.Move(_agent.desiredVelocity);
            }
            else
            {
                _movement.Move(Vector3.zero);
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

        Destroy(this.gameObject);
        Debug.Log(name + " TakeDamage " + damage);
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    // Search Player Callback
    private void OnSearchAreaEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.SetTarget(other.gameObject.transform);
        }
    }

    // Animator
    private void UpdateAnimator()
    {
        if (_animator == null)
            return;

        if (_movement.ControlAllowed)
        {
            _animator.SetFloat("Speed", new Vector3(_movement.Velocity.x, 0f, _movement.Velocity.z).magnitude);
        }
        _animator.SetBool("Grounded", _movement.GroundStatus);
        if (!_movement.GroundStatus)
        {
            _animator.SetFloat("Jump", _movement.Velocity.y);
        }
    }
}
*/

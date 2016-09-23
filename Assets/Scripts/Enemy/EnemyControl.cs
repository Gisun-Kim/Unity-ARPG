using UnityEngine;
using System.Collections;

namespace Gisun
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(CharacterMovement))]
    public class EnemyControl : CharacterEntity
    {
        public string name = "enemy";

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
            if (_agent == null)
            {
                Debug.LogError("No NavMeshAgent assigned. Character will now be disabled.");
                this.enabled = false;
                return;
            }
            _agent.updateRotation = false;
            _agent.updatePosition = true;

            _movement = GetComponent<CharacterMovement>();
            if (_movement == null)
            {
                Debug.LogError("No CharacterMovement assigned. Character will now be disabled.");
                this.enabled = false;
                return;
            }

            _animator = GetComponentInChildren<Animator>();
            _animator.applyRootMotion = false;
        }

        protected override void Start()
        {
            base.Start();
            Debug.Log(name + " Start");
        }

        private void Update()
        {
            if (_target != null)
                _agent.SetDestination(_target.position);

            if (_agent.remainingDistance > _agent.stoppingDistance)
                _movement.Move(_agent.desiredVelocity);
            else
                _movement.Move(Vector3.zero);
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

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                this.SetTarget(other.gameObject.transform);
            }
        }
    }
}

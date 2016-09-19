using UnityEngine;
using System.Collections;

namespace Gisun
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed = 3.0f;
        [SerializeField]
        private float _turnSpeed = 360f;
        [SerializeField]
        private float _jumpSpeed = 5.0f;
        [SerializeField]
        private float _snapGround = 1f;
        [SerializeField]
        private float _gravityMultiplier = 1f;
        [SerializeField]
        private float _groundCheckDistance = 0.1f;
        [SerializeField]
        private float _groundCheckRadius = 0.1f;
        [SerializeField]
        private float _moveThreshold = 0.1f;

        private Transform _transform;
        private CharacterController _characterController;
        private Collider _collider;
        private Transform _camera;
        private bool _jump = false; // 점프 입력
        private Vector3 _movement = Vector3.zero; // 이동 입력
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _gravity = Vector3.zero;
        private Vector3 _groundNormal = Vector3.up;

        // Animation control
        private Animator _animator;

        public Vector3 velocity
        {
            get { return _characterController.velocity; }
        }

        private bool _groundStatus;
        public bool groundStatus
        {
            get { return _groundStatus; }
        }

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _characterController = GetComponent<CharacterController>();
            if (_characterController == null)
            {
                Debug.LogError("No CharacterController assigned. Character will now be disabled.");
                this.enabled = false;
                return;
            }

            _animator = GetComponentInChildren<Animator>();
        }

        private void Start()
        {
            if (_camera == null)
            {
                _camera = Camera.main.transform;
            }
        }

        private void Update()
        {
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            UpdateMovement();
        }

        public void Jump()
        {
            if (!_jump)
            {
                _jump = true;
            }
        }

        public void Move(Vector3 movement)
        {
            _movement = movement;
        }

        void UpdateMovement()
        {
            // Input
            // TO DO : 나중에 입력은 따로 빼자
            float h = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis("Horizontal");
            float v = UnityStandardAssets.CrossPlatformInput.CrossPlatformInputManager.GetAxis("Vertical");

            Vector3 movement = Vector3.zero;
            // calculate move direction to pass to character
            if (_camera != null)
            {
                // calculate camera relative direction to move:
                var camForward = Vector3.Scale(_camera.forward, new Vector3(1, 0, 1)).normalized;
                movement = v * camForward + h * _camera.right;
            }
            else
            {
                // we use world-relative directions in the case of no main camera
                movement = v * Vector3.forward + h * Vector3.right;
            }


            if (_movement.magnitude < _moveThreshold)
                _movement = Vector3.zero;

            if (_movement.magnitude > 1f)
                _movement.Normalize();

            // 지면을 체크
            CheckGroundStatus();
            // 표면 경사를 이동에 적용
            _movement = Vector3.ProjectOnPlane(_movement, _groundNormal);
            Debug.Log(_movement);

            // 이동 속도
            _moveDirection.x = _movement.x * _moveSpeed;
            _moveDirection.z = _movement.z * _moveSpeed;

            // trun
            if (!Mathf.Approximately(_moveDirection.magnitude, 0f))
            {
                Quaternion targetRotation = Quaternion.LookRotation(_moveDirection);
                _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, _turnSpeed * Time.fixedDeltaTime);
                //_transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.fixedDeltaTime * _turnSpeed);
            }

            // 지면을 꽉 눌러주기 위한 변수(안그러면 경사 면에서 통통 튐)
            float snapGround = 0f;

            // 땅에 닿아 있다면
            if (_characterController.isGrounded)
            {
                // 중력 속도 초기화
                _gravity = Vector3.zero;

                // jump
                if (_jump)
                {
                    _gravity = Vector3.up * _jumpSpeed;
                    _jump = false;
                }
                else
                {
                    snapGround = _snapGround;
                }
            }
            else
            {
                // 중력 가속도를 더해준다
                _gravity += Physics.gravity * _gravityMultiplier * Time.fixedDeltaTime;
            }

            // 이동
            _characterController.Move((_moveDirection + _gravity) * Time.fixedDeltaTime + (Vector3.down * snapGround));

        }

        // 지면 체크
        private void CheckGroundStatus()
        {
            // 상승중(점프중) 이면 체크 안함
            if (!_characterController.isGrounded && this.velocity.y > 0.0f)
            {
                _groundStatus = false;
                return;
            }

            _groundStatus = (CheckGroundRaycast(out _groundNormal) || _characterController.isGrounded);
        }

        // Raycast 로 접지 판정
        private bool CheckGroundRaycast(out Vector3 normal)
        {
            RaycastHit hitInfo;
            Vector3 origin = _transform.position + _characterController.center + (Vector3.down * _characterController.height / 2);
#if UNITY_EDITOR
            Debug.DrawLine(origin, origin + (Vector3.down * _groundCheckDistance));
#endif
            if (Physics.Raycast(origin, Vector3.down, out hitInfo, _groundCheckDistance))
            {
                normal = hitInfo.normal;
                return true;
            }
            else
            {
                normal = Vector3.up;
                return false;
            }
        }

        // SphereCast 로 접지 판정
        private bool CheckGroundSphereCast(out Vector3 normal)
        {
            RaycastHit hitInfo;
            Vector3 origin = _transform.position + _characterController.center + (Vector3.down * (_characterController.height / 2 - _characterController.radius));
            if (Physics.SphereCast(origin, _characterController.radius, Vector3.down, out hitInfo,
                                    _groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                normal = hitInfo.normal;
                return true;
            }
            else
            {
                normal = Vector3.up;
                return false;
            }
        }

        // Animator
        private void UpdateAnimator()
        {
            _animator.SetFloat("Speed", new Vector3(this.velocity.x, 0f, this.velocity.z).magnitude);
            _animator.SetBool("Grounded", this.groundStatus);
            if (!this.groundStatus)
            {
                _animator.SetFloat("Jump", this.velocity.y);
            }
        }

        private void OnDrawGizmos()
        {
            /*
            RaycastHit hitInfo;
            Vector3 origin = _transform.position + _characterController.center + (Vector3.down * (_characterController.height / 2 - _characterController.radius));

            if (Physics.SphereCast(origin, _characterController.radius, Vector3.down, out hitInfo,
                                _groundCheckDistance, Physics.AllLayers, QueryTriggerInteraction.Ignore))
            {
                Gizmos.DrawLine(origin, hitInfo.point);
                Gizmos.DrawWireSphere(origin + Vector3.down * hitInfo.distance, _characterController.radius);
            }
            else
            {
                Gizmos.DrawWireSphere(origin + Vector3.down * _groundCheckDistance, _characterController.radius);
            }
            */
        }
    }
}
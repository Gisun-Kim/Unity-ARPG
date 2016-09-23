using UnityEngine;
using System.Collections;

namespace Gisun
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterMovement : MonoBehaviour
    {
        public float _moveSpeed = 3.0f;
        public float _moveMultiplier = 1f;
        public float _turnSpeed = 360f;
        public float _jumpSpeed = 5.0f;
        public float _snapGround = 1f;
        public float _gravityMultiplier = 1f;
        public float _groundCheckDistance = 0.1f;
        public float _groundCheckRadius = 0.1f;
        public float _moveThreshold = 0.1f;

        public Vector3 Velocity
        {
            get { return _controller.velocity; }
        }

        private bool _groundStatus;
        public bool GroundStatus
        {
            get { return _groundStatus; }
        }

        // 입력에 의한 컨트롤 허용
        public bool ControlAllowed { get; set; }
        // 이동시 회전을 하지 않게 잠근다
        public bool LockRotate { get; set; }
        private Transform _transform;
        private CharacterController _controller;
        private Collider _collider;
        private Transform _camera;
        private bool _jump = false;
        private Vector3 _targetMovement = Vector3.zero;
        private Quaternion _targetRotation = new Quaternion(float.MaxValue, 0, 0, 0);
        private bool _forceTurn = false;
        private Vector3 _moveDirection = Vector3.zero;
        private Vector3 _gravity = Vector3.zero;
        private Vector3 _groundNormal = Vector3.up;

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _controller = GetComponent<CharacterController>();
            if (_controller == null)
            {
                Debug.LogError("No CharacterController assigned. Character will now be disabled.");
                this.enabled = false;
                return;
            }
        }

        private void Start()
        {
            if (_camera == null)
            {
                _camera = Camera.main.transform;
            }

            this.ControlAllowed = true;
            this.LockRotate = false;
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

        void UpdateMovement()
        {
            if (!this.ControlAllowed)
            {
                this.StopTurn();
                this.StopMove();
            }

            // 정규화
            if (_targetMovement.magnitude > 1f)
            {
                _targetMovement.Normalize();
            }
            // 임계값 이하면 이동 없음
            else if (_targetMovement.magnitude < _moveThreshold)
            {
                _targetMovement = Vector3.zero;
            }

            // 회전
            if (!this.LockRotate)
            {
                // 강제 회전이 아니면 이동 방향으로 targetRotation 설정(강제 회전을 우선시 하기 위해)
                if (!_forceTurn)
                {
                    if (_targetMovement.sqrMagnitude > 0f)
                    {
                        _targetRotation = Quaternion.LookRotation(_targetMovement);
                    }
                }

                // targetRotation 이 있으면 회전 시킨다
                if (_targetRotation.x != float.MaxValue)
                {
                    _transform.rotation = Quaternion.RotateTowards(_transform.rotation, _targetRotation, _turnSpeed * Time.fixedDeltaTime);
                    // 보간
                    //_transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.fixedDeltaTime * _turnSpeed);

                    // 목표까지 회전을 다 했으면 초기화
                    if (Quaternion.Dot(_transform.rotation, _targetRotation) >= 1f)
                    {
                        _targetRotation = new Quaternion(float.MaxValue, 0f, 0f, 0f);
                        if(_forceTurn)
                            _forceTurn = false ;
                    }
                }
            }

            // 지면을 체크
            CheckGroundStatus();
            // 표면 경사를 이동에 적용
            _moveDirection = Vector3.ProjectOnPlane(_targetMovement, _groundNormal) * _moveSpeed * _moveMultiplier;
            _moveDirection.y = 0f;

            // 지면을 꽉 눌러 주기위한 값
            Vector3 snapGround = Vector3.zero;
            // 땅에 닿아 있다면
            if (_controller.isGrounded)
            {
                // 중력 초기화
                _gravity = Vector3.zero;

                // 점프
                if (this.ControlAllowed && _jump)
                {
                    _gravity = Vector3.up * _jumpSpeed;
                    _jump = false;
                }
                else
                {
                    // 지면을 꽉 눌러줌(안그러면 경사 면에서 통통 튐)
                    snapGround = (Vector3.down * _snapGround);
                }
            }
            else
            {
                // 중력 가속도를 더해준다
                _gravity += Physics.gravity * _gravityMultiplier * Time.fixedDeltaTime;
            }

            // 이동
            _controller.Move((_moveDirection + _gravity) * Time.fixedDeltaTime + snapGround);

        }

        // 지면 체크
        private void CheckGroundStatus()
        {
            // 상승중(점프중) 이면 체크 안함
            if (!_controller.isGrounded && this.Velocity.y > 0.0f)
            {
                _groundStatus = false;
                return;
            }

            _groundStatus = (CheckGroundRaycast(out _groundNormal) || _controller.isGrounded);
        }

        // Raycast 로 접지 판정
        private bool CheckGroundRaycast(out Vector3 normal)
        {
            RaycastHit hitInfo;
            Vector3 origin = _transform.position + _controller.center + (Vector3.down * _controller.height / 2);
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
            Vector3 origin = _transform.position + _controller.center + (Vector3.down * (_controller.height / 2 - _controller.radius));
            if (Physics.SphereCast(origin, _controller.radius, Vector3.down, out hitInfo,
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

        // 즉시 회전
        public void SetRotate(Vector3 direction)
        {
            this.SetRotate(Quaternion.LookRotation(direction));
        }
        // 즉시 회전
        public void SetRotate(Quaternion rotation)
        {
            StopTurn();
            _transform.rotation = rotation;
        }

        // 목표 까지 강제로 프레임마다 회전
        public void ForceTurn(Vector3 direction)
        {
            this.ForceTurn(Quaternion.LookRotation(direction));
        }
        // 목표 까지 강제로 프레임마다 회전
        public void ForceTurn(Quaternion targetRotation)
        {
            _forceTurn = true;
            _targetRotation = targetRotation;
        }

        // 다음 프레임 회전을 멈춤
        public void StopTurn()
        {
            _forceTurn = false;
            _targetRotation = new Quaternion(float.MaxValue, 0f, 0f, 0f);
        }

        // 다음 프레임에 방향으로 이동
        public void Move(Vector3 movement)
        {
            _targetMovement = movement;
        }

        // 다음 프레임 이동을 멈춤
        public void StopMove()
        {
            Move(Vector3.zero);
        }
    }
}
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    public float runSpeed = 3.0f;
    public float sprintSpeed = 5.0f;
    public float moveMultiplier = 1f;
    public float rotateSpeed = 360f;
    public float jumpPower = 5.0f;
    public float snapGround = 0.1f;
    public float gravityMultiplier = 1f;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundCheckLayer = 1 << 0;

    // 이동 속도
    public Vector3 Velocity
    {
        get { return _controller.velocity; }
    }
    // 지면 접지 상태
    public bool IsGrounded { private set; get; }
    // 입력에 의한 컨트롤 허용
    public bool ControlAllowed { get; set; }
    // 이동시 회전을 하지 않게 잠근다
    public bool LockRotate { get; set; }

    private Transform _transform;
    private CharacterController _controller;
    private Collider _collider;
    private Transform _camera;
    private Animator _anim;

    private bool jump;
    private bool sprint;
    private Vector3 targetMovement;
    private Quaternion targetRotation;
    private Vector3 moveDirection = Vector3.zero;
    private Vector3 gravity = Vector3.zero;
    private RaycastHit groundHitInfo;
    private float groundDistance;

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
        CheckGround();
        UpdateAir();
        UpdateRotate();
        UpdateMovement();
    }

    void Init()
    {
        jump = false;
        sprint = false;
        targetMovement = Vector3.zero;
        targetRotation = _transform.rotation;
        moveDirection = Vector3.zero;
        gravity = Vector3.zero;
        groundDistance = float.MaxValue;
    }

    public void Jump()
    {
        if (!jump)
            jump = true;
    }

    void UpdateMovement()
    {
        // 정규화
        if (targetMovement.magnitude > 1f)
        {
            targetMovement.Normalize();
        }

        float speed = runSpeed + (sprint ? sprintSpeed : 0f);

        // 표면 경사를 이동에 적용
        moveDirection = Vector3.ProjectOnPlane(targetMovement, groundHitInfo.normal) * speed * moveMultiplier;
        moveDirection.y = 0f;

        // 이동
        _controller.Move(moveDirection * Time.fixedDeltaTime);
    }

    void UpdateRotate()
    {
        // 회전
        if (!this.LockRotate)
        {
            if (targetMovement.magnitude > 0f)
            {
                targetRotation = Quaternion.LookRotation(targetMovement, Vector3.up);
            }
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
        }
    }

    void UpdateAir()
    {
        // 지면을 꽉 눌러 주기위한 값
        Vector3 snapGround = Vector3.zero;
        // 땅에 닿아 있다면
        if (this.IsGrounded)
        {
            // 중력 초기화
            gravity = Vector3.zero;

            // 점프
            if (jump)
            {
                gravity = Vector3.up * jumpPower;
                jump = false;
            }
            else
            {
                // 지면을 꽉 눌러줌(안그러면 경사 면에서 통통 튐)
                snapGround = (Vector3.down * this.snapGround);
            }
        }
        else
        {
            // 중력 가속도를 더해준다
            gravity += Physics.gravity * gravityMultiplier * Time.fixedDeltaTime;
        }

        _controller.Move(gravity * Time.fixedDeltaTime + snapGround);
    }

    // 지면 체크
    private void CheckGround()
    {
        CheckGroundDistance();

        if (groundDistance <= 0.05f)
        {
            IsGrounded = true;
        }
        else if (groundDistance >= groundCheckDistance)
        {
            IsGrounded = false;
        }

        // 상승중(점프중) 이면 체크 안함
        //if (!_controller.isGrounded && this.Velocity.y > 0.0f)
        //{
        //    _groundStatus = false;
        //    return;
        //}

        //_groundStatus = (CheckGroundRaycast(out groundNormal) || _controller.isGrounded);
    }

    // 지면 거리 체크
    private void CheckGroundDistance()
    {
        float height = _controller.height;
        float radius = _controller.radius;
        float distance = float.MaxValue;
        // Raycast 로 거리 체크
        Ray ray = new Ray(_transform.position + Vector3.up * (height / 2f), Vector3.down);
        if (Physics.Raycast(ray, out groundHitInfo, 1f, groundCheckLayer))
        {
            distance = _transform.position.y - groundHitInfo.point.y;
        }
        // SphereCast 로 거리 체크
        ray = new Ray(_transform.position + Vector3.up * radius, Vector3.down);
        if (Physics.SphereCast(ray, radius, out groundHitInfo, 1f, groundCheckLayer))
        {
            // SphereCast 거리가 더 짧은지 체크
            if (distance > groundHitInfo.distance - radius)
            {
                distance = groundHitInfo.distance - radius;
            }
        }

        this.groundDistance = distance;
    }

    // 즉시 회전
    public void SetRotate(Vector3 direction)
    {
        this.SetRotate(Quaternion.LookRotation(direction));
    }

    // 즉시 회전
    public void SetRotate(Quaternion rotation)
    {
        _transform.rotation = rotation;
    }

    // 다음 프레임에 방향으로 이동
    public void Move(Vector3 movement, bool sprint = false)
    {
        this.sprint = sprint;
        this.targetMovement = movement;
    }

    // 다음 프레임 이동을 멈춤
    public void StopMove()
    {
        Move(Vector3.zero);
    }
}

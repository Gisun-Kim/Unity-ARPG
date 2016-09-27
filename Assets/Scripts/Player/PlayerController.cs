using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
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
    // 이동을 잠근다
    public bool lockMovement;
    // 회전을 하지 않게 잠근다
    public bool lockRotate;
    // 이동 방향으로 끝까지 회전시킨다
    public bool spotRotate;

    // 이동 속도
    public Vector3 Velocity
    {
        get { return _controller.velocity; }
    }
    // 지면 접지 상태
    public bool IsGrounded { private set; get; }
    // 이동 입력
    private bool inputJump;
    private bool inputSprint;
    private Vector3 inputMovement;
    private Vector3 targetDirection;
    private Vector3 moveDirection = Vector3.zero;
    public bool Jumping { private set; get; }

    private RaycastHit groundHitInfo;
    private float groundDistance;

    private Transform _transform;
    private CharacterController _controller;
    private Collider _collider;
    private Transform _camera;
    private Animator _anim;

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
        _anim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main.transform;
        }

        Init();
    }

    private void FixedUpdate()
    {
        CheckGround();
        UpdateAir();
        UpdateRotate();
        UpdateMovement();
        //ResetInput();
    }

    void Init()
    {
        IsGrounded = false;
        lockMovement = false;
        lockRotate = false;
        spotRotate = true;

        inputJump = false;
        inputSprint = false;
        inputMovement = Vector3.zero;
        targetDirection = Vector3.zero;
        moveDirection = Vector3.zero;
        groundDistance = float.MaxValue;
    }

    public void Jump()
    {
        if (!inputJump)
            inputJump = true;
    }

    void UpdateMovement()
    {
        if (lockMovement)
            return;

        // 정규화. 
        if (inputMovement.magnitude > 1f)
        {
            inputMovement.Normalize();
        }
        // 질주 일때는 전방으로 직진
        else if (inputSprint && inputMovement.magnitude > 0.01)
        {
            inputMovement = _transform.forward;
        }

        float speed = inputSprint ? sprintSpeed : runSpeed;

        Debug.Log("Ground Angle : " + GroundAngle());
        Vector3 movement = Vector3.ProjectOnPlane(inputMovement, groundHitInfo.normal);
        Debug.Log("ProjectOnPlane : " + movement);
        // 표면 경사를 이동에 적용 (오르막 일때만 80도 이하로 계단에서 걸림)
        if (IsGrounded && movement.y > 0 && GroundAngle() < 80f)
        {
            movement = movement * speed * moveMultiplier;
            moveDirection = new Vector3(movement.x, moveDirection.y, movement.z);
        }
        else
        {
            movement = inputMovement * speed * moveMultiplier;
            moveDirection = new Vector3(movement.x, moveDirection.y, movement.z);
        }

        // 지면을 꽉 눌러줌
        float snap = 0f;
        if (IsGrounded && !Jumping)
            snap = snapGround;

        // 이동
        _controller.Move(moveDirection * Time.fixedDeltaTime + new Vector3(0f, -snap, 0f));
    }

    void UpdateRotate()
    {
        if (lockRotate)
            return;

        if (spotRotate)
        {
            // 이동이 있어야 이동 방향으로 회전 방향 설정
            if (inputMovement.magnitude > 0f)
            {
                targetDirection = inputMovement;
            }
        }
        else
        {
            targetDirection = inputMovement;
        }

        if (targetDirection.magnitude > 0f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            // 회전 속도만큼 회전
            _transform.rotation = Quaternion.RotateTowards(_transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
                
            // 목표까지 회전을 다 했으면 초기화
            if (Quaternion.Dot(_transform.rotation, targetRotation) >= 1f)
            {
                targetDirection = Vector3.zero;
            }
        }
    }

    void UpdateAir()
    {
        // 땅에 닿아 있다면
        if (this.IsGrounded)
        {
            // 중력 초기화
            if(!Jumping)
                moveDirection.y = 0;

            if (Jumping && Velocity.y <= 0f)
            {
                Jumping = false;
            }
            
            // 점프
            if (inputJump)
            {
                moveDirection.y = jumpPower;
                inputJump = false;
                Jumping = true;
            }
        }
        else
        {
            // 중력 가속도를 더해준다
            moveDirection.y += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
        }
    }

    // 지면 체크
    private void CheckGround()
    {
        CheckGroundDistance();

        if (groundDistance >= groundCheckDistance)
        {
            IsGrounded = false;
        }
        else
        {
            IsGrounded = true;
        }
    }

    // 지면 거리 체크
    private void CheckGroundDistance()
    {
        float height = _controller.height;
        float radius = _controller.radius;
        float distance = float.MaxValue;
        Vector3 origin = (_transform.position + _controller.center) + Vector3.down * (height / 2 - radius); 
        // Raycast 로 거리 체크
        Ray ray = new Ray(origin, Vector3.down);
        if (Physics.Raycast(ray, out groundHitInfo, 10f, groundCheckLayer))
        {
            distance = (origin.y - radius) - groundHitInfo.point.y;
            Debug.DrawLine(origin, groundHitInfo.point);
        }
        // SphereCast 로 거리 체크
        ray = new Ray(origin, Vector3.down);
        if (Physics.SphereCast(ray, radius, out groundHitInfo, 10f, groundCheckLayer))
        {
            float d = (groundHitInfo.point - origin).magnitude - _controller.radius;
            Debug.DrawLine(origin, groundHitInfo.point);
            // SphereCast 거리가 더 짧은지 체크
            if (distance > d)
            {
                distance = d;
            }
        }

        this.groundDistance = distance;
    }

    public void OnDrawGizmos()
    {
        if (IsGrounded)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;

        Gizmos.DrawLine(groundHitInfo.point, groundHitInfo.point + groundHitInfo.normal);
    }

    float GroundAngle()
    {
        var groundAngle = Vector3.Angle(groundHitInfo.normal, Vector3.up);
        return groundAngle;
    }

    void ResetInput()
    {
        inputMovement = Vector3.zero;
        inputJump = false;
        inputSprint = false;
    }

    // 다음 프레임에 방향으로 회전
    public void RotateTo(Vector3 direction)
    {
        this.targetDirection = direction;
    }

    // 다음 프레임에 방향으로 이동
    public void Move(Vector3 movement, bool sprint = false)
    {
        this.inputSprint = sprint;
        this.inputMovement = movement;
    }

    // 다음 프레임 이동을 멈춤
    public void Stop()
    {
        inputMovement = Vector3.zero;
        targetDirection = Vector3.zero;
    }
}

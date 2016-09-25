using UnityEngine;
using System.Collections;

public class CharacterMotor : MonoBehaviour
{
    [Header("Layers")]
    [SerializeField]
    protected LayerMask groundCheckLayer = 1 << 0;
    [SerializeField]
    protected float groundCheckMinDistance = 0.1f;
    [SerializeField]
    protected float groundCheckMaxDistance = 0.5f;

    [SerializeField]
    protected bool lockMovement;
    [SerializeField]
    protected float walkSpeed = 1f;
    [SerializeField]
    protected float runSpeed = 1f;
    [SerializeField]
    protected float sprintSpeed = 1f;
    [SerializeField]
    protected float strafeSpeed = 1f;
    [SerializeField]
    protected float rotationSpeed = 10f;
    [SerializeField]
    protected float strafeRotationSpeed = 10f;

    [SerializeField]
    protected float stepOffsetEnd = 0.45f;
    [SerializeField]
    protected float stepOffsetStart = 0.05f;
    [SerializeField]
    protected float slopeLimit = 45f;
    [SerializeField]
    protected float gravityMultiplier = 1f;

    [SerializeField]
    protected bool jumpAirControl;
    [SerializeField]
    protected float jumpForce;

    protected bool isGround;
    protected float groundDistance;
    protected RaycastHit groundHitInfo;

    private Transform _transform;
    private Rigidbody _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private Transform _camera;

    [HideInInspector]
    public Vector2 inputMovement;

    private bool _jump = false;
    private Vector3 _targetMovement = Vector3.zero;
    private Quaternion _targetRotation = new Quaternion(float.MaxValue, 0, 0, 0);
    private bool _forceTurn = false;
    private Vector3 _moveDirection = Vector3.zero;
    private Vector3 _gravity = Vector3.zero;
    private Vector3 _groundNormal = Vector3.up;

    protected void UpdateMotor()
    {
        CheckGround();
    }

    void FreeMovement()
    {
        if (this.inputMovement != Vector2.zero)
        {

        }
    }

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {
        if (_camera == null)
        {
            _camera = Camera.main.transform;
        }

    }

    private void FixedUpdate()
    {
    }

    public void Jump()
    {
        if (!_jump)
        {
            _jump = true;
        }
    }


    // 지면 체크
    private float CheckGroundDistance()
    {
        float distance = float.MaxValue;
        // Raycast 로 거리 체크
        Ray ray = new Ray(_transform.position + Vector3.up * (_capsuleCollider.height / 2f), Vector3.down);
        if (Physics.Raycast(ray, out groundHitInfo, 1f, groundCheckLayer))
        {
            distance = _transform.position.y - groundHitInfo.point.y;
        }
        // SphereCast 로 거리 체크
        ray = new Ray(_transform.position + Vector3.up * (_capsuleCollider.radius), Vector3.down);
        if (Physics.SphereCast(ray, _capsuleCollider.radius, out groundHitInfo, 1f, groundCheckLayer))
        {
            // SphereCast 거리가 더 짧은지 체크
            if (distance > groundHitInfo.distance - _capsuleCollider.radius)
            {
                distance = groundHitInfo.distance - _capsuleCollider.radius;
            }
        }

        return distance;
    }

    private void CheckGround()
    {
        this.groundDistance = CheckGroundDistance();

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

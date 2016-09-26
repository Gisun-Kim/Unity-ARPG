using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
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

    protected bool       isGround;
    protected float      groundDistance;
    protected RaycastHit groundHitInfo;

    private Transform       _transform;
    private Rigidbody       _rigidbody;
    private CapsuleCollider _capsuleCollider;
    private Transform       _camera;

    [HideInInspector]
    public Vector2 input;

    private void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void Start()
    {


    }

    private void FixedUpdate()
    {
        UpdateMotor();
    }

    protected void UpdateMotor()
    {
        CheckGround();
    }

    // Movement

    public bool LockMovement = false;

    void FreeMovement()
    {
        Vector3 targetDirection = Vector3.zero;
        if (this.input != Vector2.zero)
        {
            
        }
    }

    private void CheckGround()
    {
        this.groundDistance = CheckGroundDistance();

    }

    // 지면 거리 체크
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
    
}

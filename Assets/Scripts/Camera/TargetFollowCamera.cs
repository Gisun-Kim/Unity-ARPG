using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;


public class TargetFollowCamera : TargetFollower
{
    public enum Mode
    {
        Free = 0,
        Lock = 1
    }

    // rotate
    public float initRotation = 0f;
    public float rotationSpeed = 1f;
    // tilt
    public float initTilt = 0f;
    public float minTilt = 90f;
    public float maxTilt = 90f;
    public float tiltSpeed = 1f;
    // zoom
    public float initDistance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 1f;
    // offset
    public Vector3 targetOffset = new Vector3(0f, 1f, 0f);
    // Mode
    public Mode mode = Mode.Free;

    private float rotation;
    private float tilt;
    private float distance;

    // cache this Transform 
    private Transform _transform;

    void Awake()
    {
        _transform = this.transform;
    }

    protected override void Start()
    {
        rotation = initRotation;
        tilt = Mathf.Clamp(initTilt, -minTilt, maxTilt);
        distance = Mathf.Clamp(initDistance, minDistance, maxDistance);
    }

    void Update()
    {
        HandleRotationMovement();
    }

    protected override void FollowTarget(float deltaTime)
    {
        if (this.Target == null)
            return;

        // 주시점
        Vector3 lookPosition = this.Target.position + this.targetOffset;

        Quaternion r = Quaternion.Euler(tilt, rotation, 0f);
        //Quaternion rotation = Quaternion.Slerp(_transform.rotation, Quaternion.Euler(_tilt, _rotation, 0f), deltaTime * turnSmoothing);

        // 위치
        Vector3 relativePos = r * Vector3.back * distance;

        // 이동 시킨다
        _transform.position = lookPosition + relativePos;

        // 타겟을 바라보게 한다
        this.transform.LookAt(lookPosition);
    }

    private void HandleRotationMovement()
    {
        if (Time.timeScale < float.Epsilon)
            return;

        float x = 0f;
        float y = 0f;
        float z = 0f;

        if (mode == Mode.Free)
        {
            // Read the user input
            x = CrossPlatformInputManager.GetAxis("Mouse X");
            y = CrossPlatformInputManager.GetAxis("Mouse Y");
            z = CrossPlatformInputManager.GetAxis("Mouse ScrollWheel");
        }

        rotation += x * rotationSpeed;
        rotation = Mathf.Repeat(rotation, 360f);

        tilt -= y * tiltSpeed;
        tilt = Mathf.Clamp(tilt, -minTilt, maxTilt);

        distance += z * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    public Vector3 Forward
    {
        get { return _transform.forward; }
    }
}


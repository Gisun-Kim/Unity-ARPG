using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace Gisun
{
    public class ThirdPersonCamera : TargetFollower
    {
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

        public bool rotateAllowed = true;

        private float _rotation;
        private float _tilt;
        private float _distance;

        // cache this Transform 
        private Transform _transform;

        void Awake()
        {
            _transform = this.transform;
        }

        protected override void Start()
        {
            _rotation = initRotation;
            _tilt = Mathf.Clamp(initTilt, -minTilt, maxTilt);
            _distance = Mathf.Clamp(initDistance, minDistance, maxDistance);
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

            Quaternion rotation = Quaternion.Euler(_tilt, _rotation, 0f);
            //Quaternion rotation = Quaternion.Slerp(_transform.rotation, Quaternion.Euler(_tilt, _rotation, 0f), deltaTime * turnSmoothing);

            // 위치
            Vector3 relativePos = rotation * Vector3.back * _distance;

            // 이동 시킨다
            _transform.position = lookPosition + relativePos;

            // 타겟을 바라보게 한다
            this.transform.LookAt(lookPosition);
        }

        private void HandleRotationMovement()
        {
            if (Time.timeScale < float.Epsilon)
                return;

            if (!this.rotateAllowed)
                return;

            // Read the user input
            var x = CrossPlatformInputManager.GetAxis("Mouse X");
            var y = CrossPlatformInputManager.GetAxis("Mouse Y");
            var z = CrossPlatformInputManager.GetAxis("Mouse ScrollWheel");

            _rotation += x * rotationSpeed;
            _rotation = Mathf.Repeat(_rotation, 360f);

            _tilt -= y * tiltSpeed;
            _tilt = Mathf.Clamp(_tilt, -minTilt, maxTilt);

            _distance += z * zoomSpeed;
            _distance = Mathf.Clamp(_distance, minDistance, maxDistance);
        }

        public Vector3 Forward
        {
            get { return _transform.forward; }
        }
    }
}

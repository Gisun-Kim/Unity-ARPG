using UnityEngine;
using System.Collections;
using System;
using UnityStandardAssets.Cameras;

namespace Gisun
{

    public class TopDownCamera : TargetFollower
    {
        public float startRotation = 45f;
        public float minRotation = 0f;
        public float maxRotation = 360f;
        public float rotationSpeed = 0.3f;

        public float startTilt = 45f;
        public float minTilt = 0f;
        public float maxTilt = 90f;
        public float tiltSpeed = 0.3f;

        public float startDistance = 5f;
        public float minDistance = 2f;
        public float maxDistance = 10f;
        public float zoomSpeed = 1f;

        public Vector3 targetOffset = new Vector3(0f, 1f, 0f);

        void Awake()
        {

        }

        protected override void Start()
        {

        }

        void Update()
        {

        }

        protected override void FollowTarget(float deltaTime)
        {
            // 주시점
            Vector3 lookPosition = this.Target.position + this.targetOffset;

            // 위치
            Vector3 relativePos = Quaternion.Euler(startTilt, startRotation, 0f) * Vector3.back * startDistance;

            // 이동 시킨다
            this.transform.position = lookPosition + relativePos;

            // 타겟을 바라보게 한다
            this.transform.LookAt(lookPosition);
        }
    }
}

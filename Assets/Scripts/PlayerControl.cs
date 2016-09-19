using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

namespace Gisun
{
    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerControl : MonoBehaviour
    {
        private Transform _transform;
        private CharacterMovement _characterMovement;
        private Transform _camera;

        // Animation control
        private Animator _animator;

        void Awake()
        {
            _transform = GetComponent<Transform>();
            _characterMovement = GetComponent<CharacterMovement>();
            if (_characterMovement == null)
            {
                Debug.LogError("No CharacterMovement assigned. Character will now be disabled.");
                this.enabled = false;
                return;
            }

            _animator = GetComponentInChildren<Animator>();
        }

        void Start()
        {
            if (_camera == null)
            {
                _camera = Camera.main.transform;
            }
        }

        void Update()
        {
            // input jump
            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                _characterMovement.Jump();
            }

            // Input move
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

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

            _characterMovement.Move(movement);

            // UpdateAnimator();
        }

        void FixedUpdate()
        {
        }
    }
}
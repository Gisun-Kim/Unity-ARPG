using UnityEngine;
using System;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

namespace Gisun
{
    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerControl : MonoBehaviour
    {
        private Transform _transform;
        private CharacterMovement _characterMovement;
        private Transform _camera;

        private bool _attacking;
        private bool _diving;

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
            Vector3 movement = Vector3.zero;

            // input jump
            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                _characterMovement.Jump();
            }

            // Input move
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

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

            // Input Attack
            if (CrossPlatformInputManager.GetButtonDown("Fire1"))
            {
                if (!_attacking)
                {
                    if (_characterMovement.GroundStatus)
                    {
                        _attacking = true;
                        _characterMovement.ControlAllowed = false;
                        _animator.applyRootMotion = true;
                        _animator.SetTrigger("Attack");
                        Invoke("OnEndAttack", 3f);
                    }
                    else
                    {
                        // 점프 공격
                        _attacking = true;
                        _characterMovement.ControlAllowed = false;
                        _animator.applyRootMotion = true;
                        _animator.SetTrigger("Attack");
                        Invoke("OnEndAttack", 1.2f);
                    }
                }
            }

            // Input Diving
            if (CrossPlatformInputManager.GetButtonDown("Fire3"))
            {
                if (_diving || !_characterMovement.GroundStatus || !_characterMovement.ControlAllowed)
                    return;

                _diving = true;
                _characterMovement.ControlAllowed = false;
                _animator.applyRootMotion = true;
                _animator.SetTrigger("Dive");
                Invoke("OnEndDiving", 1.2f);
            }

            UpdateAnimator();
        }

        void FixedUpdate()
        {
        }

        private void OnEndAttack()
        {
            _attacking = false;
            _characterMovement.ControlAllowed = true;
            _animator.applyRootMotion = false;
        }

        private void OnEndDiving()
        {
            _diving = false;
            _characterMovement.ControlAllowed = true;
            _animator.applyRootMotion = false;
        }

        // Animator
        private void UpdateAnimator()
        {
            if (_animator == null)
                return;

            if (_characterMovement.ControlAllowed)
            {
                _animator.SetFloat("Speed", new Vector3(_characterMovement.velocity.x, 0f, _characterMovement.velocity.z).magnitude);
            }
            _animator.SetBool("Grounded", _characterMovement.GroundStatus);
            if (!_characterMovement.GroundStatus)
            {
                _animator.SetFloat("Jump", _characterMovement.velocity.y);
            }
        }
    }
}
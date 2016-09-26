using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

namespace Gisun
{
    class Skill
    {
        public int skillID;
        public string name;
        public bool combo;
    }

    [RequireComponent(typeof(CharacterMovement))]
    public class PlayerControl : CharacterEntity
    {
        private Transform _transform;
        private CharacterMovement _movement;
        private Transform _camera;

        private bool _skillActivating;  // 스킬 시전중
        private bool _rolling;          // 구르는중

        private Dictionary<int, Skill> _skills = new Dictionary<int, Skill>();
        private Skill _prevActivatingSkill;
        private Skill _currentActivatingSkill;

        // Animation control
        private Animator _animator;

        void Awake()
        {
            _transform = GetComponent<Transform>();
            _movement = GetComponent<CharacterMovement>();
            _animator = GetComponentInChildren<Animator>();
            _animator.applyRootMotion = false;
        }

        protected override void Start()
        {
            if (_camera == null)
            {
                _camera = Camera.main.transform;
            }

            SetTestSkillData();
            Init();
        }

        private void Init()
        {
            _skillActivating = false;
            _rolling = false;
            _currentActivatingSkill = null;
        }

        // test code
        void SetTestSkillData()
        {
            _skills.Add(1, new Skill { skillID = 1, name = "Basic Melee Attack", combo = true });
            _skills.Add(2, new Skill { skillID = 2, name = "Heavy Melee Attack", combo = true });
        }

        void Update()
        {
            HandleMovementInput();
            HandleSkillInput();
            UpdateAnimator();
        }

        private void HandleMovementInput()
        {
            if (PossibleAction())
            {
                Vector3 movement = Vector3.zero;
                // input jump
                if (CrossPlatformInputManager.GetButtonDown("Jump"))
                {
                    _movement.Jump();
                }

                // Input movement
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
                _movement.Move(movement);

                // Input Roll
                if (CrossPlatformInputManager.GetButtonDown("Fire3"))
                {
                    Vector3 direction = (movement.sqrMagnitude > 0.01f) ? movement : _transform.forward;
                    StartCoroutine(ProcessRoll(1.2f, direction));
                }
            }
        }

        void HandleSkillInput()
        {
            // Input Skill
            if (PossibleSkillAction())
            {
                // skill 1
                if (CrossPlatformInputManager.GetButtonDown("Fire1"))
                {
                    Skill skill;
                    if (!_skills.TryGetValue(1, out skill))
                        return;
                    if(!_skillActivating || (_skillActivating && _currentActivatingSkill == skill && skill.combo))
                    {
                        _animator.SetTrigger("SkillActivate");
                        _animator.SetInteger("Skill_ID", skill.skillID);
                        _currentActivatingSkill = skill;
                    }
                }
                // skill 2
                if (CrossPlatformInputManager.GetButtonDown("Fire2"))
                {
                    Skill skill;
                    if (!_skills.TryGetValue(2, out skill))
                        return;
                    if (!_skillActivating || (_skillActivating && _currentActivatingSkill == skill && skill.combo))
                    {
                        _animator.SetTrigger("SkillActivate");
                        _animator.SetInteger("Skill_ID", skill.skillID);
                        _currentActivatingSkill = skill;
                    }
                }
                //// skill 3
                //if (CrossPlatformInputManager.GetButtonDown("Alpha1"))
                //{

                //}
                //// skill 4
                //if (CrossPlatformInputManager.GetButtonDown("Alpha2"))
                //{

                //}
                //// skill 5
                //if (CrossPlatformInputManager.GetButtonDown("Alpha3"))
                //{

                //}
                //// skill 6
                //if (CrossPlatformInputManager.GetButtonDown("Alpha4"))
                //{

                //}
            }
        }

        public void OnSkillEnter()
        {

        }

        public void InSkillActivating()
        {
            _skillActivating = true;
            _animator.applyRootMotion = true;
        }

        public void OnSkillExit()
        {
            _skillActivating = false;
            _animator.applyRootMotion = false;
        }

        // 구르기 처리
        IEnumerator ProcessRoll(float duration, Vector3 direction)
        {
            _animator.SetTrigger("Roll");

            _movement.StopTurn();
            _movement.StopMove();
            _movement.SetRotate(direction);

            _rolling = true;
            yield return new WaitForFixedUpdate();
            
            float timer = 0f;
            while(timer < duration)
            {
                timer += Time.fixedDeltaTime;
                _movement._moveMultiplier = Mathf.Sin(Mathf.Lerp(90f * Mathf.Deg2Rad, 0f, timer / duration)) * 1.3f;
                _movement.Move(_transform.forward);
                yield return new WaitForFixedUpdate();
            }

            _movement._moveMultiplier = 1f;
            _rolling = false;
        }

        // Animator
        private void UpdateAnimator()
        {
            if (_animator == null)
                return;

            if (_movement.ControlAllowed)
            {
                _animator.SetFloat("Speed", new Vector3(_movement.Velocity.x, 0f, _movement.Velocity.z).magnitude);
            }
            _animator.SetBool("Grounded", _movement.GroundStatus);
            if (!_movement.GroundStatus)
            {
                _animator.SetFloat("Jump", _movement.Velocity.y);
            }
        }

        private bool PossibleAction()
        {
            return _movement.GroundStatus && !_rolling && !_skillActivating;
        }

        private bool PossibleSkillAction()
        {
            return _movement.GroundStatus && !_rolling;
        }
    }
}
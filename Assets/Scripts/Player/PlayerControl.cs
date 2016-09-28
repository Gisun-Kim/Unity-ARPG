using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;


//class SkillInfo
//{
//    public enum CastType
//    {
//        Instant = 0,    // 즉시 시전
//        Time = 1     // 시전 시간
//    }
//    public enum SkillType
//    {
//        Self,
//        MeleeArea,
//        Projectile,
//        targetPosition
//    }

//    public int ID;
//    public string name;
//    public bool combo;
//    public SkillInfo comboNext;
//    public CastType castType = CastType.Instant;
//    public float castTime = 0f;
//    public SkillType skillType = SkillType.MeleeArea;
//    public float AreaRadius = 1f;
//    public float AreaAngle = 360f;
//    public Vector3 direction = Vector3.forward;
//}

[RequireComponent(typeof(UserCharacterControl))]
public class PlayerControl : PlayerCharacter
{
    private Transform _transform;
    private UserCharacterControl _characterControl;
    private Transform _camera;

    private bool _attacking;  // 공격중
    //private bool _rolling;          // 구르는중

    // Animation control
    private Animator _animator;

    void Awake()
    {
        _transform = GetComponent<Transform>();
        _characterControl = GetComponent<UserCharacterControl>();
        _animator = GetComponentInChildren<Animator>();
        _animator.applyRootMotion = false;
    }

    protected override void Start()
    {
        base.Start();

        if (_camera == null)
        {
            _camera = Camera.main.transform;
        }

        Init();
    }

    private void Init()
    {
        _attacking = false;
        //_rolling = false;
    }

    void Update()
    {
        HandleMovementInput();
        HandleSkillInput();
        UpdateAnimator();
    }

    private void HandleMovementInput()
    {
        if (CanAction())
        {
            Vector3 movement = Vector3.zero;
            // input jump
            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                _characterControl.Jump();
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

            bool sprint = false;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                sprint = true;
            }

            _characterControl.Move(movement, sprint);


            // Input Roll
            //if (CrossPlatformInputManager.GetButtonDown("Fire3"))
            //{
            //    Vector3 direction = (movement.sqrMagnitude > 0.01f) ? movement : _transform.forward;
            //    StartCoroutine(ProcessRoll(1.2f, direction));
            //}
        }
    }

    void HandleSkillInput()
    {
        //// Input Skill
        //if (PossibleSkillAction())
        //{
        //    // skill 1
        //    if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        //    {
        //        Skill skill;
        //        if (!_skills.TryGetValue(1, out skill))
        //            return;
        //        if(!_skillActivating || (_skillActivating && _currentActivatingSkill == skill && skill.combo))
        //        {
        //            _animator.SetTrigger("SkillActivate");
        //            _animator.SetInteger("Skill_ID", skill.skillID);
        //            _currentActivatingSkill = skill;
        //        }
        //    }
        //    // skill 2
        //    if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        //    {
        //        Skill skill;
        //        if (!_skills.TryGetValue(2, out skill))
        //            return;
        //        if (!_skillActivating || (_skillActivating && _currentActivatingSkill == skill && skill.combo))
        //        {
        //            _animator.SetTrigger("SkillActivate");
        //            _animator.SetInteger("Skill_ID", skill.skillID);
        //            _currentActivatingSkill = skill;
        //        }
        //    }
        //    //// skill 3
        //    //if (CrossPlatformInputManager.GetButtonDown("Alpha1"))
        //    //{

        //    //}
        //    //// skill 4
        //    //if (CrossPlatformInputManager.GetButtonDown("Alpha2"))
        //    //{

        //    //}
        //    //// skill 5
        //    //if (CrossPlatformInputManager.GetButtonDown("Alpha3"))
        //    //{

        //    //}
        //    //// skill 6
        //    //if (CrossPlatformInputManager.GetButtonDown("Alpha4"))
        //    //{

        //    //}
        //}
    }

    public void OnSkillEnter()
    {

    }

    public void InSkillActivating()
    {
        _attacking = true;
        _animator.applyRootMotion = true;
    }

    public void OnSkillExit()
    {
        _attacking = false;
        _animator.applyRootMotion = false;
    }

    // 구르기 처리
    //IEnumerator ProcessRoll(float duration, Vector3 direction)
    //{
    //    _animator.SetTrigger("Roll");

    //    _controller.StopTurn();
    //    _controller.StopMove();
    //    _controller.SetRotate(direction);

    //    _rolling = true;
    //    yield return new WaitForFixedUpdate();

    //    float timer = 0f;
    //    while(timer < duration)
    //    {
    //        timer += Time.fixedDeltaTime;
    //        _controller._moveMultiplier = Mathf.Sin(Mathf.Lerp(90f * Mathf.Deg2Rad, 0f, timer / duration)) * 1.3f;
    //        _controller.Move(_transform.forward);
    //        yield return new WaitForFixedUpdate();
    //    }

    //    _controller._moveMultiplier = 1f;
    //    _rolling = false;
    //}

    // Animator
    private void UpdateAnimator()
    {
        if (_animator == null)
            return;

        _animator.SetFloat("Speed", new Vector3(_characterControl.Velocity.x, 0f, _characterControl.Velocity.z).magnitude);
        _animator.SetBool("IsGrounded", _characterControl.IsGrounded);
        _animator.SetFloat("VerticalVelocity", _characterControl.Velocity.y);
        _animator.SetBool("Jumping", _characterControl.Jumping);
    }

    private bool CanAction()
    {
        return !_attacking;
    }
}

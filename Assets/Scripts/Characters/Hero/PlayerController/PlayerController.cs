using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;

public partial class PlayerController : MonoBehaviour
{
    // 애니 트랜지션 파라미터
    private const string IsWalk = "IsWalk";
    private const string IsSprint = "IsSprint";
    private const string IsJumnp = "IsJump";
    private const string Attack = "Attack";
    private const string EquipChange = "EquipChange";
    private const string ComboCount = "ComboCount";

    [Title("[Components]")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Transform _camera;
    [SerializeField] private Animator _animator;

    // 이동 방향 변수
    private Vector3 _direction;
    // 중력 적용
    private Vector3 _velocity;

    // 이동 속도 변수
    private float _applySpeed;
    private float _walkSpeed = 4f;
    private float _sprintSpeed = 6f;
    private float _jumpForce = 1f;

    // 캐릭터 회전 관련
    private float _turnSmoothTime = .1f;
    private float _turnSmoothVelocity;

    // 상태 변수
    private bool _isWalk = false;
    private bool _isSprint = false;
    private bool _isJump = false;

    // 플레이어 스탯 데이터
    // 스테미나는 정확하게 값을 매기도록 float 처리 (나중에 수정필요하면 수정하자)
    private int _playerHP;
    private float _playerStamina;
    private int _playerLevel;

    // 스태미나 충전에 걸리는 시간
    private float _currentStaminaChargeTime = 0f;

    private void Start()
    {
        _applySpeed = _walkSpeed;

        // 현재 데이터 처리는 간이로
        _playerHP = GameValue.INIT_HP;
        _playerStamina = GameValue.INIT_STAMINA;
        _playerLevel = GameValue.INIT_LEVEL;
    }

    private void Update()
    {
        ApplyGravity();
        MovePlayer();
    }

    // Actual move
    private void MovePlayer()
    {
        _isWalk = _direction.magnitude >= .1f;

        if (_isWalk)
        {
            float targetAngle = Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
            this.transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            _controller.Move(moveDirection.normalized * _applySpeed * Time.deltaTime);
        }

        _applySpeed = _isSprint ? _sprintSpeed : _walkSpeed;

        _animator.SetBool(IsWalk, _isWalk);
        _animator.SetBool(IsSprint, _isSprint && _isWalk);

        // 스태미나 세팅
        SetPlayerStamina(_isWalk, ref _isSprint);
    }

    // Walk
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            var inputValue = context.ReadValue<Vector3>();
            _direction = new Vector3(inputValue.x, 0f, inputValue.z);
        }
    }

    // Sprint
    public void OnSprintInput(InputAction.CallbackContext context)
    {
        _isSprint = false;

        if (context.performed && context.interaction is HoldInteraction)
            _isSprint = true;

        if (context.canceled)
            _isSprint = false;
    }

    // Jump
    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded)
        {
            _isJump = true;
            _velocity.y = Mathf.Sqrt(_jumpForce * -2f * GameValue.GRAVITY);
        }
    }

    // Stamina setting
    private void SetPlayerStamina(bool isWalk, ref bool isSprint)
    {
        // 달리는 중이면 스태미나 감소
        if (isWalk && isSprint)
        {
            if (_playerStamina > 0)
                _playerStamina -= GameValue.DECREASE_STAMINA_VALUE;
            else
            {
                _playerStamina = 0;
                isSprint = false;
            }
        }
        // 아니면 스태미나 충전
        else
        {
            if (!_isSprint)
            {
                // 스태미나 충전이 시간이 안됐으면
                if (_currentStaminaChargeTime < GameValue.CHARGE_STAMINA_TIME)
                {
                    _currentStaminaChargeTime += Time.deltaTime;

                    if (_currentStaminaChargeTime >= GameValue.CHARGE_STAMINA_TIME)
                        _currentStaminaChargeTime = GameValue.CHARGE_STAMINA_TIME;
                }
                // 스태미나 충전이 가능하면
                else
                {
                    if (_playerStamina < GameValue.INIT_STAMINA)
                        _playerStamina += GameValue.INCREASE_STAMINA_VALUE;
                    else
                        _playerStamina = GameValue.INIT_STAMINA;
                }
            }
        }
    }
}

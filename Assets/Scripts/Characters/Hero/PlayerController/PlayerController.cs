using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;

public partial class PlayerController : CharacterBase
{
    // 애니 트랜지션 파라미터
    // private const string ANIM_ISWALK = "IsWalk";
    // private const string ANIM_ISSPRINT = "IsSprint";
    // private const string ANIM_ATTACK = "Attack";
    private const string ANIM_COMBOCOUNT = "ComboCount";
    // private const string ANIM_GET_HIT = "GetHit";

    [Title("[Components]")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Transform _camera;
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimEventChecker _checker;

    [Title("[PlayerMeshData]")]
    [SerializeField] private PlayerSkinnedMesh _skinnedMeshData;

    [Title("[PlayerTransform]")]
    [SerializeField] private Transform _leftHandTransform;
    [SerializeField] private Transform _rightHandTransform;

    // 플레이어 액션
    private PlayerAction _playerAction;

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
    private bool _isGetHit => _checker.ProcessingGetHit;

    // 플레이어 스탯 데이터
    // 스테미나는 정확하게 값을 매기도록 float 처리 (나중에 수정필요하면 수정하자)
    private int _playerHP;
    private float _playerStamina;
    private int _playerLevel;

    // 스태미나 충전에 걸리는 시간
    private float _currentStaminaChargeTime = 0f;

    // Properties
    public Animator PlayerAnimator => _animator;
    public Transform LeftHand => _leftHandTransform;
    public Transform RightHand => _rightHandTransform;
    public PlayerSkinnedMesh SkinnedMeshData => _skinnedMeshData;

    #region 상호 작용
    private InputAction _InteractionAction;
    public UnityAction<ItemBase> _itemEnterAction;
    public UnityAction<ItemBase> _itemExitAction;
    private List<ItemBase> _interactItemList = new();
    #endregion

    private void Awake()
    {
        _applySpeed = _walkSpeed;
        _camera = Camera.main.transform;
        _input.camera = Camera.main;

        _playerHP = GameValue.INIT_HP;
        _playerStamina = GameValue.INIT_STAMINA;

        SetPlayerInputActions();        // 입력 감지
        SetPlayerActions();             // 콜백 세팅
    }

    private void OnEnable()
    {
        RegistActions(_inventoryAction, InventoryActionStarted);
        RegistActions(_mainMenuAction, MainMenuActionStarted);

        RegistActions(_escapeAction, EscapeActionStarted);

        RegistActions(_InteractionAction, InteractionActionStarted);

        RegistActions(_attackAction, null, AttackActionPerformed, null);
        RegistActions(_focusAction, FocusActionStarted, FocusActionPerformed, FocusActionCanceled);
    }

    private void OnDisable()
    {
        UnRegistActions(_inventoryAction, InventoryActionStarted);
        UnRegistActions(_mainMenuAction, MainMenuActionStarted);

        UnRegistActions(_escapeAction, EscapeActionStarted);

        UnRegistActions(_InteractionAction, InteractionActionStarted);

        UnRegistActions(_attackAction, null, AttackActionPerformed, null);
        UnRegistActions(_focusAction, FocusActionStarted, FocusActionPerformed, FocusActionCanceled);
    }

    // // 중력 적용
    // private void FixedUpdate()
    // {
    //     ApplyGravity();
    // }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        _controller.Move(_gravityVelocity * Time.deltaTime);
    }

    private void Update()
    {
        // 공격 중이거나 피격 중일 땐 이동할 수 없음
        if (!_checker.ProcessingAttack && !_isGetHit)
            MovePlayer();
        else
        {
            if (_isWalk)
            {
                _isWalk = false;
                _animator.SetBool(ANIM_ISWALK, _isWalk);
            }
        }

    }

    private void SetPlayerInputActions()
    {
        // PlayerActions
        _playerAction = new();

        _inventoryAction = _playerAction.Player.Shortcut_Inventory;
        _mainMenuAction = _playerAction.Player.Shortcut_MainMenu;

        _escapeAction = _playerAction.UI.Escape;

        _InteractionAction = _playerAction.Player.Interaction;

        _attackAction = _playerAction.Player.Attack;
        _focusAction = _playerAction.Player.Focus;
    }

    private void RegistActions(InputAction action,
                               Action<InputAction.CallbackContext> startCallback = null,
                               Action<InputAction.CallbackContext> performedCallback = null,
                               Action<InputAction.CallbackContext> cancelCallback = null)
    {
        action.Enable();

        if (startCallback != null)
            action.started += startCallback;

        if (performedCallback != null)
            action.performed += performedCallback;

        if (cancelCallback != null)
            action.canceled += cancelCallback;
    }

    private void UnRegistActions(InputAction action,
                               Action<InputAction.CallbackContext> startCallback = null,
                               Action<InputAction.CallbackContext> performedCallback = null,
                               Action<InputAction.CallbackContext> cancelCallback = null)
    {
        action.Disable();

        if (startCallback != null)
            action.started -= startCallback;

        if (performedCallback != null)
            action.performed -= performedCallback;

        if (cancelCallback != null)
            action.canceled -= cancelCallback;
    }

    private void SetPlayerActions()
    {
        _itemEnterAction = null;
        _itemExitAction = null;
        _interactItemList.Clear();
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

        _animator.SetBool(ANIM_ISWALK, _isWalk);
        _animator.SetBool(ANIM_ISSPRINT, _isSprint && _isWalk);

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

    // 줍기, 대화, ...
    private void InteractionActionStarted(InputAction.CallbackContext context)
    {

    }

    public void GetHit()
    {
        // 피격 애니가 실행 중이 아닐 때 실행
        // 중첩되어 실행되지 않도록 막기
        if (!_isGetHit)
            _animator.SetTrigger(ANIM_GET_HIT);
    }
}

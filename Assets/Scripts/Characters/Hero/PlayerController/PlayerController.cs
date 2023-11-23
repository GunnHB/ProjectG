using System;
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
    private const string ANIM_ISWALK = "IsWalk";
    private const string ANIM_ISSPRINT = "IsSprint";
    private const string ANIM_ATTACK = "Attack";
    private const string ANIM_COMBOCOUNT = "ComboCount";

    [Title("[Components]")]
    [SerializeField] private CharacterController _controller;
    [SerializeField] private PlayerInput _input;
    [SerializeField] private Transform _camera;
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimEventChecker _checker;

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

    // 플레이어 스탯 데이터
    // 스테미나는 정확하게 값을 매기도록 float 처리 (나중에 수정필요하면 수정하자)
    private int _playerHP;
    private float _playerStamina;
    private int _playerLevel;

    // 스태미나 충전에 걸리는 시간
    private float _currentStaminaChargeTime = 0f;

    // Properties
    public Animator PlayerAnimator => _animator;

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

        // 현재 데이터 처리는 간이로
        _playerHP = GameValue.INIT_HP;
        _playerStamina = GameValue.INIT_STAMINA;
        _playerLevel = GameValue.INIT_LEVEL;

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

    private void Update()
    {
        ApplyGravity();

        if (!_checker.ProcessingAttack)
            MovePlayer();
        else
        {
            // fsm으로 수정해야될지도...
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

    private void EnterItemCallback(ItemBase item)
    {
        _interactItemList.Add(item);

        // 이미 먼저 감지된 아이템이 있으면 걍 추가만
        if (_interactItemList.IndexOf(item) != 0)
            return;

        UIManager.Instance.ShowItemInteractUI(item);
    }

    private void ExitItemCallback(ItemBase item)
    {
        _interactItemList.Remove(item);

        if (_interactItemList.Count > 0)
            UIManager.Instance.ShowItemInteractUI(item);
        else
            UIManager.Instance.CloseItemInteractUI();
    }

    // 줍기, 대화, ...
    private void InteractionActionStarted(InputAction.CallbackContext context)
    {
        // if (GameManager.Instance.CurrentMode != GameManager.GameMode.InGame)
        //     return;

        // // 인벤토리 추가 테스트
        // AddToinventory();
    }

    private void AddToinventory()
    {
        Item.Data newItem = new Item.Data
        {
            id = 100010001,
            type = ItemType.Armor,
            name = "Item_Helmet_0001",
            desc = "Item_Helmet_Desc_0001",
            image = "Icon_Helmet_0001",
            ref_id = 200010001,
        };

        ItemManager.Instance.AddItemToInventory(newItem);
    }
}

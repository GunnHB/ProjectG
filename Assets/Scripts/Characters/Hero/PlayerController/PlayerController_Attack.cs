using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;
using System.Collections;

public partial class PlayerController : MonoBehaviour
{
    [Title("[AnimCtrlScriptableObject]")]
    [SerializeField] private PlayerAnimCtrlScriptableObject _animCtrlScriptableObject;

    private bool _isAttackStart = false;            // 공격 시작

    private Coroutine _chargeCoroutine = null;

    public PlayerAnimCtrlScriptableObject AnimCtrlScriptableObj => _animCtrlScriptableObject;

    public void OnClickMouseLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // 공격 상태로 변경
            _playerState = PlayerState.Attack;

            // 차징 공격
            if (context.interaction is HoldInteraction)
            {
                Debug.Log("여긴 차징");

                _isAttackStart = true;
            }
            // 일반 공격
            else if (context.interaction is PressInteraction)
            {
                Debug.Log("여긴 일반");

                _isAttackStart = true;
            }
        }
        else
        {
            _isAttackStart = false;
            _playerState = PlayerState.Idle;    // 일단 대기 상태로 돌림
        }
    }
}

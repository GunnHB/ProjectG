using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;
using System.Collections;

public partial class PlayerController : MonoBehaviour
{
    private InputAction _attackAction;
    private InputAction _focusAction;

    private bool _isAttack = false;
    private bool _isFocus = false;

    // Attack
    private void AttackActionStarted(InputAction.CallbackContext context)
    {
        _isAttack = true;
    }

    private void AttackActionPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
            Debug.Log("차징");
        else if (context.interaction is PressInteraction)
            Debug.Log("일반");
    }

    private void AttackActionCanceled(InputAction.CallbackContext context)
    {
        _isAttack = false;
    }

    // Focus
    private void FocusActionStarted(InputAction.CallbackContext context)
    {
        _isFocus = true;
    }

    private void FocusActionPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
            Debug.Log("포커싱");
    }

    private void FocusActionCanceled(InputAction.CallbackContext context)
    {
        _isFocus = false;
    }

    // [Title("[AnimCtrlScriptableObject]")]
    // [SerializeField] private PlayerAnimCtrlScriptableObject _animCtrlScriptableObject;

    // public PlayerAnimCtrlScriptableObject AnimCtrlScriptableObj => _animCtrlScriptableObject;

    // private Coroutine _chargeCoroutine = null;

    // public void OnClickMouseLeft(InputAction.CallbackContext context)
    // {
    //     if (context.performed)
    //     {
    //         // 차징 공격
    //         if (context.interaction is HoldInteraction)
    //         {
    //             Debug.Log("여긴 차징");
    //         }
    //         // 일반 공격
    //         else if (context.interaction is PressInteraction)
    //         {
    //             Debug.Log("여긴 일반");
    //             _animator.SetTrigger(ANIM_ATTACK);
    //         }
    //     }
    // }
}

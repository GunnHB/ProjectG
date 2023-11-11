using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;
using System.Collections;

public partial class PlayerController : MonoBehaviour
{
    [Title("[AnimCtrlScriptableObject]")]
    [SerializeField] private PlayerAnimCtrlScriptableObject _animCtrlScriptableObject;

    public PlayerAnimCtrlScriptableObject AnimCtrlScriptableObj => _animCtrlScriptableObject;

    private Coroutine _chargeCoroutine = null;

    public void OnClickMouseLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // 차징 공격
            if (context.interaction is HoldInteraction)
            {
                Debug.Log("여긴 차징");
            }
            // 일반 공격
            else if (context.interaction is PressInteraction)
            {
                Debug.Log("여긴 일반");
                _animator.SetTrigger(ANIM_ATTACK);
            }
        }
    }
}

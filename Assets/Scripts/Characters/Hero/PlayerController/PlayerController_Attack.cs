using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;

public partial class PlayerController : MonoBehaviour
{
    private int _maxComboCount = 5;
    private int _currentComboCount = 0;

    private float _validComboTime = .5f;
    private float _currentComboTimer = 0f;

    private float _maxChargeCount = 3f;
    private float _currentChargeCount = 0f;

    private bool _isAttack = false;
    private int _comboCount = 0;

    public void OnClickMouseLeft(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // 차징 공격
            if (context.interaction is HoldInteraction)
            {
                ;
            }
            // 일반 공격
            else if (context.interaction is PressInteraction)
            {
                if (_comboCount > 3)
                    _comboCount = 0;
            }
        }
    }
}

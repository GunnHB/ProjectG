using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;
using System.Collections;

public partial class PlayerController : MonoBehaviour
{
    private InputAction _attackAction;
    private InputAction _focusAction;

    // [공격 중]은 애니메이션 이벤트에서 확인
    private bool _isAttack = false;             // 공격 시작
    private bool _isFocus = false;              // 포커스 시작

    private bool _doCombo = false;              // 후속 공격 여부
    private int _comboCount = 0;                // 후속 공격 순번

    private Coroutine _continueAttackCoroutine;

    private void AttackActionPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
        {
            Debug.Log("charge");

            // 차징 애니 실행
        }
        else if (context.interaction is PressInteraction)
        {
            if (_continueAttackCoroutine != null)
                StopCoroutine(_continueAttackCoroutine);

            _continueAttackCoroutine = StartCoroutine(nameof(Cor_ContinueAttack));

            // 후속 공격하기로 했으면 리턴
            if (_doCombo)
                return;

            // 공격 중이면 후속 공격에 대한 입력을 확인함
            if (_checker.ProcessingAttack)
            {
                if (!_doCombo)
                {
                    _doCombo = true;
                    _comboCount++;

                    _animator.SetInteger(ANIM_COMBOCOUNT, _comboCount);
                }
            }
            else
                _animator.SetTrigger(ANIM_ATTACK);
        }
    }

    private IEnumerator Cor_ContinueAttack()
    {
        while (true)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                _comboCount = 0;
                _animator.SetInteger(ANIM_COMBOCOUNT, 0);
                _doCombo = false;

                yield break;
            }

            yield return null;
        }
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
}

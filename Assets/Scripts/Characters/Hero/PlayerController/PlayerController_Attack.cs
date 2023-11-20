using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;
using System.Collections;

public partial class PlayerController : MonoBehaviour
{
    private InputAction _attackAction;
    private InputAction _focusAction;

    private bool _isAttack = false;             // 공격 시작
    private bool _isFocus = false;              // 포커스 시작

    private bool _isAttacking = false;          // 공격 중

    private bool _doCombo = false;

    private int _comboCount = 0;

    private Coroutine _attackAnimEndCoroutine;
    private Coroutine _checkComboAttackCoroutine;

    private void AttackActionPerformed(InputAction.CallbackContext context)
    {
        if (!_isAttacking)
        {
            if (context.interaction is HoldInteraction)
            {
                Debug.Log("charge");

                // 차징 애니 실행
                _isAttacking = true;
            }
            else if (context.interaction is PressInteraction)
            {
                Debug.Log("normal");

                // 일반 공격 애니 실행
                _animator.SetTrigger(ANIM_ATTACK);
                _isAttacking = true;

                CheckComboAttack();
            }
        }
    }

    private void CheckComboAttack()
    {
        if (_checkComboAttackCoroutine != null)
        {
            StopCoroutine(_checkComboAttackCoroutine);
            _checkComboAttackCoroutine = null;
        }

        _checkComboAttackCoroutine = StartCoroutine(nameof(Cor_CheckComboAttack));
    }

    private IEnumerator Cor_CheckComboAttack()
    {
        while (true)
        {
            // 애니 시작할 때까지 대기
            if (!_checker.ProcessingAttack)
                yield return null;
            else
                break;
        }

        while (true)
        {
            // 애니메이션 이벤트로 공격이 끝났는지 확인
            if (!_checker.ProcessingAttack)
            {
                _isAttack = false;
                _isAttacking = false;

                yield break;
            }

            // 공격 애니가 진행 중인지 확인
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                Debug.Log(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                yield return null;
            }
            else
            {
                Debug.Log(_animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
                _isAttack = false;
                _isAttacking = false;

                yield break;
            }

            yield break;
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

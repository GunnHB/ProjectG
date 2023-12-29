using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

using Sirenix.OdinInspector;
using System.Collections;

public partial class PlayerController : CharacterBase
{
    private InputAction _attackAction;
    private InputAction _focusAction;

    // [공격 중]은 AnimEventChecker 에서 확인
    private bool _doCombo = false;              // 후속 공격 여부
    private int _comboCount = 0;                // 후속 공격 순번

    private Coroutine _continueAttackCoroutine;

    private AnimationClip _startedClip = null;

    public bool DoCombo => _doCombo;

    private void AttackActionPerformed(InputAction.CallbackContext context)
    {
        // 인게임 모드에서만 공격이 이루어짐
        if (GameManager.Instance.CurrentMode != GameManager.GameMode.InGame)
            return;

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

            if (_doCombo)
            {
                // 현재 재생되는 클립과 공격을 시작한 클립이 다르면 
                var currClip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;

                if (_startedClip == currClip)
                    return;
                else
                    _doCombo = false;
            }

            // 공격 중이면 후속 공격에 대한 입력을 확인함
            if (_checker.ProcessingAttack)
            {
                if (!_doCombo)
                {
                    _doCombo = true;
                    _comboCount++;

                    _animator.SetInteger(ANIM_COMBOCOUNT, _comboCount);

                    _startedClip = _animator.GetCurrentAnimatorClipInfo(0)[0].clip;
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
                _checker.ChangeProcessingAttack(false);     // 후속 공격 없이 공격이 끝났으면 수동으로 값을 변경

                yield break;
            }

            yield return null;
        }
    }

    // Focus
    private void FocusActionStarted(InputAction.CallbackContext context)
    {
        // _isFocus = true;
    }

    private void FocusActionPerformed(InputAction.CallbackContext context)
    {
        if (context.interaction is HoldInteraction)
            Debug.Log("포커싱");
    }

    private void FocusActionCanceled(InputAction.CallbackContext context)
    {
        // _isFocus = false;
    }
}

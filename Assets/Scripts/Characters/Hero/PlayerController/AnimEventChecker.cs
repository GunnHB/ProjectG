using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class AnimEventChecker : MonoBehaviour
{
    //  공격 진행 중인지 확인 (콤보 공격 확인하기 위함)
    private bool _processingAttack = false;
    public bool ProcessingAttack => _processingAttack;

    private bool _doCombo => GameManager.Instance.PController.DoCombo;

    private void StartAttack()
    {
        _processingAttack = true;
    }

    private void EndAttack()
    {
        if (_doCombo)
            return;

        _processingAttack = false;
    }

    private void StartCheckCollders()
    {

    }

    private void EndCheckCollders()
    {

    }

    public void ChangeProcessingAttack(bool active)
    {
        _processingAttack = active;
    }
}

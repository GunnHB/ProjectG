using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEventChecker : MonoBehaviour
{
    //  공격 진행 중인지 확인 (콤보 공격 확인하기 위함)
    private bool _processingAttack = false;
    public bool ProcessingAttack => _processingAttack;

    private void StartAttack()
    {
        _processingAttack = true;
    }

    private void EndAttack()
    {
        _processingAttack = false;
    }
}

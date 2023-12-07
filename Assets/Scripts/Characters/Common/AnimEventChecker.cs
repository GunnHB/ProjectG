using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class AnimEventChecker : MonoBehaviour
{
    //  공격 진행 중인지 확인
    protected bool _processingAttack = false;
    public bool ProcessingAttack => _processingAttack;

    protected Collider _collider;

    protected virtual void StartAttack()
    {
        _processingAttack = true;
    }

    protected virtual void EndAttack()
    {
        _processingAttack = false;
    }

    protected virtual void StartCheckCollders()
    {
        if (_collider == null)
            return;

        // 공격 시 트리거 감지를 위함
        _collider.isTrigger = true;
    }

    protected virtual void EndCheckColliders()
    {
        if (_collider == null)
            return;

        // 공격이 끝난 후 트리거를 끔
        _collider.isTrigger = false;
    }

    public void ChangeProcessingAttack(bool active)
    {
        _processingAttack = active;
    }
}

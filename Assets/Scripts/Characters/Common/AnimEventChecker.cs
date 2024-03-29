using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class AnimEventChecker : MonoBehaviour
{
    //  공격 진행 중인지 확인
    protected bool _processingAttack = false;
    public bool ProcessingAttack => _processingAttack;

    protected bool _processingGetHit = false;
    public bool ProcessingGetHit => _processingGetHit;

    // 해당 콜라이더는 무기입니다.
    // 무기와 상대방이 닿는 것을 확인하기 위함입니다.
    protected Collider _collider;

    protected virtual void Awake()
    {
        _processingAttack = false;
    }

    protected virtual void StartAttack()
    {
        _processingAttack = true;
    }

    protected virtual void EndAttack()
    {
        _processingAttack = false;
    }

    protected virtual void StartCheckColliders()
    {
        if (_collider == null)
            return;
    }

    protected virtual void EndCheckColliders()
    {
        if (_collider == null)
            return;
    }

    protected virtual void ActiveGetHit()
    {
        _processingGetHit = true;
    }

    protected virtual void DeactiveGetHit()
    {
        _processingGetHit = false;
    }

    public void SetOppnentCollider(Collider collider)
    {
        this._collider = collider;
    }

    public void ChangeProcessingAttack(bool active)
    {
        _processingAttack = active;
    }
}

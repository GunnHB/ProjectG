using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class CharacterBase : MonoBehaviour
{
    // 캐릭터가 기본적으로 사용하는 애니 파라미터
    // 공용으로 사용되는 것만 추가합시다.
    protected const string ANIM_ISWALK = "IsWalk";
    protected const string ANIM_ISSPRINT = "IsSprint";
    protected const string ANIM_ATTACK = "Attack";
    protected const string ANIM_GET_DAMAGED = "GetHit";

    [SerializeField] protected CharacterDataBase _dataBase;
    [SerializeField] protected AnimEventChecker _checker;
    [SerializeField] protected Collider _controller;

    // 캐릭터의 기본 상태 변수
    protected bool _isWalk;
    protected bool _isSprint;
    protected bool _isAttack => _checker.ProcessingAttack;
    protected bool _isGetDamaged => _checker.ProcessingGetHit;

    // 중력
    protected Vector3 _gravityVelocity;

    // 캐릭터 이동 속도
    protected float _applySpeed;

    protected virtual void FixedUpdate()
    {
        ApplyGravity();
    }

    /// <summary>
    /// 캐릭터에 중력을 적용하려면 각자마다 중력값을 추가해줍시다.
    /// </summary>
    private void ApplyGravity()
    {
        _gravityVelocity.y += GameValue.GRAVITY * Time.deltaTime;
    }
}

interface IDamageable
{
    void GetDamaged();
}

interface IAttackable
{
    void DoAttack();
}
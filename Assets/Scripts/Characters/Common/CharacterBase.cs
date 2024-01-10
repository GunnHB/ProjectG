using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sirenix.OdinInspector;

public class CharacterBase : MonoBehaviour
{
    // 캐릭터가 기본적으로 사용하는 애니 파라미터
    // 공용으로 사용되는 것만 추가합시다.
    public const string ANIM_IS_WALK = "IsWalk";
    public const string ANIM_IS_SPRINT = "IsSprint";
    public const string ANIM_ATTACK = "Attack";
    public const string ANIM_GET_DAMAGED = "GetHit";

    private const string GROUP_BASE_COMPONENTS = "[ Base Components ]";

    [BoxGroup(GROUP_BASE_COMPONENTS), SerializeField]
    protected AnimEventChecker _checker;
    [BoxGroup(GROUP_BASE_COMPONENTS), SerializeField]
    protected CharacterController _controller;
    [BoxGroup(GROUP_BASE_COMPONENTS), SerializeField]
    protected Animator _animator;

    // 캐릭터의 기본 상태 변수
    protected bool _isWalk;
    protected bool _isSprint;
    protected bool _isAttack => _checker.ProcessingAttack;
    protected bool _isGetDamaged => _checker.ProcessingGetHit;

    // 중력
    protected Vector3 _gravityVelocity;

    #region 프로퍼티
    // 애니 체커
    public AnimEventChecker AnimChecker => _checker;
    // 캐릭터 컨트롤러
    public CharacterController Controller => _controller;
    // 애니메이터
    public Animator ThisAnimator => _animator;
    // 캐릭터 이동 속도
    public float ApplySpeed { get; set; }
    #endregion

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
        _controller.Move(_gravityVelocity * Time.deltaTime);
    }
}

interface IDamageable
{
    void GetDamaged(int damage);
    void Dead();
}

interface IAttackable
{
    void DoAttack();
}
using System;
using System.Collections.Generic;

using UnityEngine;

using BehaviorTree;

using Sirenix.OdinInspector;

public class EnemyAIV2 : MonoBehaviour
{
    private const string ANIM_PARAM_WALK = "IsWalk";
    private const string ANIM_PARAM_CHASE = "IsChase";
    private const string ANIM_PARAM_ATTACK = "Attack";
    private const string ANIM_PARAM_ALERT = "IsAlert";

    public enum EnemyState
    {
        None = -1,
        Idle,
        Patrol,
        Chase,
        Attack,
        Alert,
    }

    [Title("[WayPoints]")]
    [SerializeField] private bool _setRandom = true;
    [SerializeField, HideIf(nameof(_setRandom))]
    private List<GameObject> _wayPointList = new();
    [SerializeField, ShowIf(nameof(_setRandom))]
    private float _radius = 1f;

    [Title("[Field of view]")]
    [SerializeField] private FieldOfView _fieldOfView;

    [Title("[Animation]")]
    [SerializeField] private Animator _animator;

    [Title("[Notice player field]")]
    [SerializeField] private bool _noticePlayer = false;
    [SerializeField] private float _noticePlayerGage = 0f;

    [Title("[State]")]
    [SerializeField] private EnemyState _state;

    [Title("[Event checker]")]
    [SerializeField] private EnemyAnimEventChecker _checker;

    [Title("[Draw gizmos]")]
    [SerializeField] private bool _drawGizmos;

    private BahaviorTreeRunner _btRunner;

    // 일반 대기 시간
    private float _idleWaitTime = 0f;
    private float _currIdleWaitTime = 0f;

    // 공격 대기 시간
    private float _attackWaitTime = 0f;
    private float _currAttackWaitTime = 0f;

    // 경계 대기 시간
    private float _alertWaitTime = 0f;
    private float _currAlertWaitTime = 0f;

    private float _patrolMovementSpeed = 1f;
    private float _chaseMovementSpeed = 2f;
    private float _applyMovementSpeed;

    // 현재 이동 중인 지점의 Vector
    private Vector3 _currWayPoint = Vector3.zero;
    private int _currListIndex;     // waypoint 리스트의 인덱스 정보

    // 캐릭터 회전 관련
    private float _turnSmoothTime = .2f;
    private float _turnSmoothVelocity;

    // 프리팹 최초 생성 시의 위치값
    private Vector3 _originPosition = Vector3.zero;
    // 추격 후 목표뮬이 사라지면 처음 생성됐던 위치로 돌아가기
    private bool _backToOriginPosition = false;

    private Transform _targetPlayer;
    private bool _findTarget;
    private bool _missTarget;

    private void Awake()
    {
        _btRunner = new BahaviorTreeRunner(SettingBT());

        // 순찰 경로가 세팅된 경우가 아니라면 내부에서 세팅
        // 랜덤 순찰의 경우 지점에 도착하면 잠시 대기 후 다시 랜덤한 지점을 지정한다.
        if (_setRandom)
            SetWayPointByRandom();

        _currListIndex = -1;
        _applyMovementSpeed = _patrolMovementSpeed;

        _originPosition = transform.position;
    }

    private void Update()
    {
        _btRunner.Operate();
    }

    // 시퀀스 : 노드의 평가를 진행하면서 하나라도 [실패]라면 [실패]를 반환
    //          모든 노드의 평가가 [성공]이어야 [성공] 반환
    // 셀렉트 : 노드의 평가를 진행하면서 [성공]을 반환받으면 [성공]을 반환
    //          모든 노드의 평가가 [실패]였을 때 [실패]를 반환
    private INode SettingBT()
    {
        return new SelectorNode(
            new List<INode>()
            {
                // 공격 시작
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(CheckAttacking),
                        new ActionNode(CheckAttackRange),
                        new ActionNode(WaitAfterAttack),
                        new ActionNode(DoAttack),
                    }
                ),
                // 적을 발견
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(CheckDetectTarget),
                        new ActionNode(MoveToTarget),
                    }
                ),
                // 적이 시야에서 사라짐
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(DoAlert),
                    }
                ),
                // 적 미발견 시 순찰
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(DoPatrol),
                        new ActionNode(DoIdle),
                    }
                )
            });
    }

    private INode.ENodeState CheckAttacking()
    {
        if (_state == EnemyState.Attack)
        {
            // 공격 중이면 running 반환
            if (_checker != null && _checker.ProcessingAttack)
                return INode.ENodeState.RunningState;
        }
        else
        {
            // 공격 이외의 상태에서는 대기 시간을 초기화
            _currAttackWaitTime = 0f;
            _attackWaitTime = 0f;
        }

        return INode.ENodeState.SuccessState;
    }

    private INode.ENodeState CheckAttackRange()
    {
        if (_targetPlayer == null || _fieldOfView == null)
            return INode.ENodeState.FailureState;

        // 원거리 공격이 가능한지 확인
        if (_fieldOfView.CanRangeAttack)
            return INode.ENodeState.FailureState;
        else
        {
            var distance = Vector3.SqrMagnitude(_targetPlayer.position - transform.position);

            if (distance < Mathf.Pow(_fieldOfView.MeleeAttackRange, 2))
                return INode.ENodeState.SuccessState;
        }

        return INode.ENodeState.FailureState;
    }

    private INode.ENodeState DoAttack()
    {
        if (_targetPlayer == null)
            return INode.ENodeState.FailureState;

        if (_targetPlayer.TryGetComponent(out Collider targetCollider))
            _checker.SetOppnentCollider(targetCollider);

        // 공격 상태로 변경
        SetEnemyState(EnemyState.Attack);

        _attackWaitTime = UnityEngine.Random.Range(1f, 3f);
        _currAttackWaitTime = 0f;

        return INode.ENodeState.SuccessState;
    }

    private INode.ENodeState WaitAfterAttack()
    {
        if (_currAttackWaitTime < _attackWaitTime)
        {
            _currAttackWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }

        SetEnemyState(EnemyState.Idle);
        return INode.ENodeState.SuccessState;
    }

    private INode.ENodeState CheckDetectTarget()
    {
        _fieldOfView.FindVisibleTargets();
        _targetPlayer = _fieldOfView.NearestTarget;

        // return _targetPlayer != null ? INode.ENodeState.SuccessState : INode.ENodeState.FailureState;

        if (_targetPlayer != null)
        {
            _findTarget = true;
            return INode.ENodeState.SuccessState;
        }
        else
        {
            if (_findTarget)
            {
                _findTarget = false;
                _missTarget = true;
            }

            return INode.ENodeState.FailureState;
        }
    }

    private INode.ENodeState MoveToTarget()
    {
        if (_targetPlayer == null)
            return INode.ENodeState.FailureState;

        var distance = Vector3.SqrMagnitude(_targetPlayer.position - transform.position);

        if (_fieldOfView.MeleeAttackRange > 0)
        {
            if (distance <= Mathf.Pow(_fieldOfView.MeleeAttackRange, 2))
                return INode.ENodeState.SuccessState;
        }

        // 추격 상태로 변경
        SetEnemyState(EnemyState.Chase);

        // 추격 속도로 세팅
        _applyMovementSpeed = _chaseMovementSpeed;
        transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.position, _applyMovementSpeed * Time.deltaTime);

        RotateToTarget(_targetPlayer.position);

        // 타겟까지 도착해야하므로 running 반환
        return INode.ENodeState.RunningState;
    }

    // 적이 시야에서 사라지면 그 자리에서 잠시 동안 경계
    private INode.ENodeState DoAlert()
    {
        if (!_missTarget)
            return INode.ENodeState.FailureState;

        if (_state != EnemyState.Alert)
        {
            // 경계 상태로 전환
            SetEnemyState(EnemyState.Alert);

            if (_alertWaitTime == 0f)
                _alertWaitTime = UnityEngine.Random.Range(10f, 25f);
        }

        if (_currAlertWaitTime < _alertWaitTime)
        {
            _currAlertWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }
        else
        {
            SetEnemyState(EnemyState.None);

            _currAlertWaitTime = 0f;
            _alertWaitTime = 0f;

            return INode.ENodeState.FailureState;
        }
    }

    private INode.ENodeState DoPatrol()
    {
        if (_missTarget)
            _missTarget = false;

        if (_setRandom)
            SetWayPointByRandom();
        else
            SetWayPointByList();

        // 현재 목표 지점이 없거나 0 이면 실패 반환
        if (_currWayPoint == null || _currWayPoint == Vector3.zero)
            return INode.ENodeState.FailureState;

        // 대기 상태일 때는 성공 반환
        if (_state == EnemyState.Idle)
            return INode.ENodeState.SuccessState;

        // 두 점간의 거리의 제곱에 루트를 한 값
        var distance = Vector3.SqrMagnitude(_currWayPoint - transform.position);

        // 어느 정도 목표 지점에 가까워지면
        if (distance <= float.Epsilon)
        {
            // 목표 지점을 0으로 초기화
            _currWayPoint = Vector3.zero;

            // 성공을 반환
            return INode.ENodeState.SuccessState;
        }

        // 순찰 속도로 지정
        _applyMovementSpeed = _patrolMovementSpeed;

        // 이동 및 회전
        transform.position = Vector3.MoveTowards(transform.position, _currWayPoint, _applyMovementSpeed * Time.deltaTime);
        RotateToTarget(_currWayPoint);

        if (_state != EnemyState.Patrol)
            SetEnemyState(EnemyState.Patrol);

        // 도착 전까지 running 반환
        return _currWayPoint != Vector3.zero ? INode.ENodeState.RunningState : INode.ENodeState.FailureState;
    }

    private Vector3 SetWayPointByRandom()
    {
        if (_currWayPoint == Vector3.zero)
        {
            // 현재 위치에서 랜덤한 지점을 반환
            var randomSite = (UnityEngine.Random.insideUnitSphere * _radius) + transform.position;

            _currWayPoint = new Vector3(randomSite.x, 0f, randomSite.z);
        }

        return _currWayPoint;
    }

    private Vector3 SetWayPointByList()
    {
        _currListIndex++;

        if (_wayPointList.Count >= _currListIndex)
            _currListIndex = 0;

        _currWayPoint = _wayPointList[_currListIndex].transform.localPosition;

        return _currWayPoint;
    }

    private void RotateToTarget(Vector3 target)
    {
        var directionToTarget = (target - transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

        transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
    }

    private INode.ENodeState DoIdle()
    {
        if (_state != EnemyState.Idle)
        {
            SetEnemyState(EnemyState.Idle);

            if (_idleWaitTime == 0f)
                _idleWaitTime = UnityEngine.Random.Range(5f, 10f);
        }

        if (_currIdleWaitTime <= _idleWaitTime)
        {
            _currIdleWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }
        else
        {
            SetEnemyState(EnemyState.None);

            _currIdleWaitTime = 0f;
            _idleWaitTime = 0f;

            return INode.ENodeState.SuccessState;
        }
    }

    // 상태가 바뀌면 애니가 바뀜요
    private void SetEnemyState(EnemyState state)
    {
        _state = state;

        switch (_state)
        {
            case EnemyState.Idle:
                SetAnimBoolParam(ANIM_PARAM_WALK, false);
                SetAnimBoolParam(ANIM_PARAM_CHASE, false);
                SetAnimBoolParam(ANIM_PARAM_ALERT, false);
                break;
            case EnemyState.Patrol:
                SetAnimBoolParam(ANIM_PARAM_WALK, true);
                SetAnimBoolParam(ANIM_PARAM_CHASE, false);
                SetAnimBoolParam(ANIM_PARAM_ALERT, false);
                break;
            case EnemyState.Chase:
                SetAnimBoolParam(ANIM_PARAM_CHASE, true);
                SetAnimBoolParam(ANIM_PARAM_ALERT, false);
                break;
            case EnemyState.Attack:
                SetAnimBoolParam(ANIM_PARAM_WALK, false);
                SetAnimBoolParam(ANIM_PARAM_CHASE, false);
                SetAnimBoolParam(ANIM_PARAM_ALERT, false);

                SetAnimTrigger(ANIM_PARAM_ATTACK);
                break;
            case EnemyState.Alert:
                SetAnimBoolParam(ANIM_PARAM_WALK, false);
                SetAnimBoolParam(ANIM_PARAM_CHASE, false);
                SetAnimBoolParam(ANIM_PARAM_ALERT, true);
                break;
            default:
                break;
        }
    }

    private void SetAnimBoolParam(string anim, bool state)
    {
        if (_animator == null)
            return;

        _animator.SetBool(anim, state);
    }

    private void SetAnimTrigger(string anim)
    {
        if (_animator == null)
            return;

        _animator.SetTrigger(anim);
    }

    // 확인용 기즈모
    private void OnDrawGizmos()
    {
        if (!_drawGizmos)
            return;

        if (_currWayPoint != Vector3.zero)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, _radius);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_currWayPoint, .3f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, _currWayPoint);
        }
    }
}

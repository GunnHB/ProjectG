using System;
using System.Collections.Generic;

using UnityEngine;

using BehaviorTree;

using Sirenix.OdinInspector;

public class EnemyBehaviorBase : MonoBehaviour
{
    public enum EnemyState
    {
        None = -1,
        Idle,               // 대기
        Patrol,             // 순찰
        Chase,              // 추격
        Attack,             // 공격
        Alert,              // 경계
        RunAway,            // 도망
    }

    [Title("[WayPoints]")]
    [SerializeField] private bool _setRandom = true;
    [SerializeField, HideIf(nameof(_setRandom))]
    protected List<GameObject> _wayPointList = new();
    [SerializeField, ShowIf(nameof(_setRandom))]
    protected float _radius = 1f;

    [Title("[Field of view]"), SerializeField]
    protected FieldOfView _fieldOfView;

    [Title("[State]"), SerializeField]
    protected EnemyState _state;

    [Title("[Draw gizmos]"), SerializeField]
    protected bool _drawGizmos;

    protected EnemyBase _enemyBase;

    private BahaviorTreeRunner _btRunner;

    // 현재 이동 중인 지점의 Vector
    protected Vector3 _currWayPoint = Vector3.zero;
    protected int _currListIndex;     // waypoint 리스트의 인덱스 정보

    // 캐릭터 회전 관련
    protected float _turnSmoothTime = .2f;
    protected float _turnSmoothVelocity;

    // 프리팹 최초 생성 시의 위치값
    protected Vector3 _originPosition = Vector3.zero;
    // 추격 후 목표뮬이 사라지면 처음 생성됐던 위치로 돌아가기
    protected bool _backToOriginPosition = false;

    protected Transform _targetPlayer;
    protected bool _findTarget;
    protected bool _missTarget;

    // ** 공통 행동트리 **
    // protected SequenceNode _attackNode;
    // protected SequenceNode _chaseNode;
    protected List<INode> _entireNodeList = new List<INode>();

    protected List<INode> _alertNodeList = new List<INode>();
    protected List<INode> _patrolNodeList = new List<INode>();

    private void Awake()
    {
        _enemyBase = this.GetComponent<EnemyBase>();

        if (_enemyBase == null)
            return;

        _btRunner = new BahaviorTreeRunner(SettingBT());

        // 순찰 경로가 세팅된 경우가 아니라면 내부에서 세팅
        // 랜덤 순찰의 경우 지점에 도착하면 잠시 대기 후 다시 랜덤한 지점을 지정한다.
        if (_setRandom)
            SetWayPointByRandom();

        _currListIndex = -1;
        _enemyBase.ApplySpeed = _enemyBase.Database.ThisWalkSpeed;

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
    protected INode SettingBT()
    {
        return EntireBehaviorNode();
        // return new SelectorNode(
        //     new List<INode>()
        //     {
        //         // // 공격 시작
        //         // new SequenceNode(
        //         //     new List<INode>()
        //         //     {
        //         //         new ActionNode(CheckAttacking),
        //         //         new ActionNode(CheckAttackRange),
        //         //         new ActionNode(WaitAfterAttack),
        //         //         new ActionNode(DoAttack),
        //         //     }
        //         // ),
        //         // // 적을 발견
        //         // new SequenceNode(
        //         //     new List<INode>()
        //         //     {
        //         //         new ActionNode(CheckDetectTarget),
        //         //         new ActionNode(MoveToTarget),
        //         //     }
        //         // ),
        //         // // 적이 시야에서 사라짐
        //         // new SequenceNode(
        //         //     new List<INode>()
        //         //     {
        //         //         new ActionNode(DoAlert),
        //         //     }
        //         // ),
        //         // // 적 미발견 시 순찰
        //         // new SequenceNode(
        //         //     new List<INode>()
        //         //     {
        //         //         new ActionNode(DoPatrol),
        //         //         new ActionNode(DoIdle),
        //         //     }
        //         // )

        //         AlertNode(),
        //         PatrolNode(),
        //     });
    }

    protected virtual SelectorNode EntireBehaviorNode()
    {
        _entireNodeList.Add(AlertNode());
        _entireNodeList.Add(PatrolNode());

        return new SelectorNode(_entireNodeList);
    }

    protected virtual SequenceNode AlertNode()
    {
        _alertNodeList.Add(new ActionNode(DoAlert));

        return new SequenceNode(_alertNodeList);
    }

    protected virtual SequenceNode PatrolNode()
    {
        _patrolNodeList.Add(new ActionNode(DoPatrol));
        _patrolNodeList.Add(new ActionNode(DoIdle));

        return new SequenceNode(_patrolNodeList);
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

            if (_enemyBase.Database.AlertWaitTime == 0f)
                _enemyBase.Database.AlertWaitTime = UnityEngine.Random.Range(10f, 20f);
        }

        if (_enemyBase.Database.CurrAlertWaitTime < _enemyBase.Database.AlertWaitTime)
        {
            _enemyBase.Database.CurrAlertWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }
        else
        {
            SetEnemyState(EnemyState.None);

            _enemyBase.Database.CurrAlertWaitTime = 0f;
            _enemyBase.Database.AlertWaitTime = 0f;

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
        // if (distance <= float.Epsilon)
        if (distance <= .1f)
        {
            // 목표 지점을 0으로 초기화
            _currWayPoint = Vector3.zero;

            // 성공을 반환
            return INode.ENodeState.SuccessState;
        }

        // 순찰 속도로 지정
        _enemyBase.ApplySpeed = _enemyBase.Database.ThisWalkSpeed;

        // 이동 및 회전
        MoveToTarget(_currWayPoint);

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

    /// <summary>
    /// 방향으로의 이동 및 회전 메소드
    /// </summary>
    /// <param name="target"></param>
    protected void MoveToTarget(Vector3 target)
    {
        var directionToTarget = (target - transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

        // 회전
        transform.rotation = Quaternion.Euler(new Vector3(0f, angle, 0f));
        // 이동
        _enemyBase.Controller.Move(directionToTarget * _enemyBase.ApplySpeed * Time.deltaTime);
    }

    private INode.ENodeState DoIdle()
    {
        if (_state != EnemyState.Idle)
        {
            SetEnemyState(EnemyState.Idle);

            if (_enemyBase.Database.IdleWaitTime == 0f)
                _enemyBase.Database.IdleWaitTime = UnityEngine.Random.Range(5f, 10f);
        }

        if (_enemyBase.Database.CurrIdelWaitTime <= _enemyBase.Database.IdleWaitTime)
        {
            _enemyBase.Database.CurrIdelWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }
        else
        {
            SetEnemyState(EnemyState.None);

            _enemyBase.Database.CurrIdelWaitTime = 0f;
            _enemyBase.Database.IdleWaitTime = 0f;

            return INode.ENodeState.SuccessState;
        }
    }

    // 상태가 바뀌면 애니가 바뀜요
    protected void SetEnemyState(EnemyState state)
    {
        _state = state;

        switch (_state)
        {
            case EnemyState.Idle:
                SetAnimBoolParam(EnemyBase.ANIM_IS_WALK, false);
                SetAnimBoolParam(EnemyBase.ANIM_IS_SPRINT, false);
                SetAnimBoolParam(EnemyBase.ANIM_PARAM_ALERT, false);
                break;
            case EnemyState.Patrol:
                SetAnimBoolParam(EnemyBase.ANIM_IS_WALK, true);
                SetAnimBoolParam(EnemyBase.ANIM_IS_SPRINT, false);
                SetAnimBoolParam(EnemyBase.ANIM_PARAM_ALERT, false);
                break;
            case EnemyState.Chase:
                SetAnimBoolParam(EnemyBase.ANIM_IS_SPRINT, true);
                SetAnimBoolParam(EnemyBase.ANIM_PARAM_ALERT, false);
                break;
            case EnemyState.Attack:
                SetAnimBoolParam(EnemyBase.ANIM_IS_WALK, false);
                SetAnimBoolParam(EnemyBase.ANIM_IS_SPRINT, false);
                SetAnimBoolParam(EnemyBase.ANIM_PARAM_ALERT, false);

                SetAnimTrigger(EnemyBase.ANIM_ATTACK);
                break;
            case EnemyState.Alert:
                SetAnimBoolParam(EnemyBase.ANIM_IS_WALK, false);
                SetAnimBoolParam(EnemyBase.ANIM_IS_SPRINT, false);
                SetAnimBoolParam(EnemyBase.ANIM_PARAM_ALERT, true);
                break;
            default:
                break;
        }
    }

    private void SetAnimBoolParam(string anim, bool state)
    {
        if (_enemyBase.ThisAnimator == null)
            return;

        _enemyBase.ThisAnimator.SetBool(anim, state);
    }

    private void SetAnimTrigger(string anim)
    {
        if (_enemyBase.ThisAnimator == null)
            return;

        _enemyBase.ThisAnimator.SetTrigger(anim);
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

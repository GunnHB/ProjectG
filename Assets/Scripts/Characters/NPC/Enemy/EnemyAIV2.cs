using System;
using System.Collections.Generic;

using UnityEngine;

using BehaviorTree;

using Sirenix.OdinInspector;
using Unity.Mathematics;

public class EnemyAIV2 : MonoBehaviour
{
    private const string IS_WALK = "IsWalk";
    private const string IS_CHASE = "IsChase";

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

    [Title("[Draw gizmos]")]
    [SerializeField] private bool _drawGizmos;

    private BahaviorTreeRunner _btRunner;

    private float _waitTime = 0f;
    private float _currWaitTime = 0f;

    private bool _idleState = false;
    private bool _patrolState = false;
    private bool _chaseState = false;

    private float _patrolMovementSpeed = 1f;
    private float _chaseMovementSpeed = 3f;
    private float _applyMovementSpeed;

    // 현재 이동 중인 지점의 Vector
    private Vector3 _currWayPoint = Vector3.zero;
    private int _currListIndex;     // waypoint 리스트의 인덱스 정보

    // 캐릭터 회전 관련
    private float _turnSmoothTime = .2f;
    private float _turnSmoothVelocity;

    // 프리팹 최초 생성 시의 위치값
    private Vector3 _originPosition = Vector3.zero;

    private Transform _targetPlayer;

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

    private INode.ENodeState CheckDetectTarget()
    {
        if (_fieldOfView == null)
            return INode.ENodeState.FailureState;

        _fieldOfView.FindVisibleTargets();
        _targetPlayer = _fieldOfView.NearestTarget;

        return _targetPlayer != null ? INode.ENodeState.SuccessState : INode.ENodeState.FailureState;
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

        // 추격 속도로 세팅
        _applyMovementSpeed = _chaseMovementSpeed;
        transform.position = Vector3.MoveTowards(transform.position, _targetPlayer.position, _applyMovementSpeed * Time.deltaTime);

        RotateToTarget(_targetPlayer.position);

        return INode.ENodeState.FailureState;
    }

    private INode.ENodeState DoAlert()
    {
        // if (_targetPlayer != null)
        return INode.ENodeState.FailureState;
    }

    private INode.ENodeState DoPatrol()
    {
        if (_setRandom)
            SetWayPointByRandom();
        else
            SetWayPointByList();

        // 현재 목표 지점이 없거나 0 이면 실패 반환
        if (_currWayPoint == null || _currWayPoint == Vector3.zero)
            return INode.ENodeState.FailureState;

        // 대기 상태일 때는 성공 반환
        if (_idleState)
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

        if (!_patrolState)
        {
            _patrolState = true;

            _idleState = false;
            _chaseState = false;

            SetAnimBoolParam(IS_WALK, true);
        }

        return INode.ENodeState.FailureState;
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
        if (!_idleState)
        {
            _idleState = true;

            _patrolState = false;
            _chaseState = false;

            SetAnimBoolParam(IS_WALK, false);

            if (_waitTime == 0f)
                _waitTime = UnityEngine.Random.Range(5f, 10f);
        }

        if (_currWaitTime <= _waitTime)
        {
            _currWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }
        else
        {
            _idleState = false;

            _currWaitTime = 0f;
            _waitTime = 0f;

            return INode.ENodeState.SuccessState;
        }
    }

    private void SetAnimBoolParam(string anim, bool state)
    {
        if (_animator == null)
            return;

        _animator.SetBool(anim, state);
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

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _currWayPoint);
        }
    }
}

using System;
using System.Collections.Generic;

using UnityEngine;

using BehaviorTree;

using Sirenix.OdinInspector;

public class EnemyAIV2 : MonoBehaviour
{
    [Title("[WayPoints]")]
    [SerializeField] private bool _setRandom = true;
    [SerializeField, HideIf(nameof(_setRandom))]
    private List<GameObject> _wayPointList = new();
    [SerializeField, ShowIf(nameof(_setRandom))]
    private float _circleRadius = 1f;

    [Title("[Field of view]")]
    [SerializeField] private FieldOfView _fieldOfView;

    [Title("[Notice player field]")]
    [SerializeField] private bool _noticePlayer = false;
    [SerializeField] private float _noticePlayerGage = 0f;

    [Title("[Draw gizmos]")]
    [SerializeField] private bool _drawGizmos;

    private BahaviorTreeRunner _btRunner;

    private float _waitTime = 0f;
    private float _currWaitTime = 0f;

    private bool _idleState = false;
    private bool _moveState = false;
    private bool _attackState = false;

    private float _patrolMovementSpeed = 2f;
    private float _chaseMovementSpeed = 4f;
    private float _applyMovementSpeed;

    // 현재 이동 중인 지점의 Vector
    private Vector3 _currWayPoint = Vector3.zero;
    private int _currListIndex;     // waypoint 리스트의 인덱스 정보

    private void Awake()
    {
        _btRunner = new BahaviorTreeRunner(SettingBT());

        // 순찰 경로가 세팅된 경우가 아니라면 내부에서 세팅
        // 핸덤 순찰의 경우 지점에 도착하면 잠시 대기 후 다시 랜덤한 지점을 지정한다.
        if (_setRandom)
            SetWayPointByRandom();

        _currListIndex = -1;
        _applyMovementSpeed = _patrolMovementSpeed;
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

        // 순찰 속도로 지정
        _applyMovementSpeed = _patrolMovementSpeed;

        Debug.Log(_currWayPoint);

        // 두 점간의 거리의 제곱에 루트를 한 값
        // 두 점간의 거리의 차이를 2차원 함수값으로 계산
        // Vector3.Distance 보다 연산이 빠름
        var distance = Vector3.SqrMagnitude(_currWayPoint - transform.position);

        // 어느 정도 목표 지점에 가까워지면
        if (distance <= float.Epsilon)
        {
            // 목표 지점을 0으로 초기화
            _currWayPoint = Vector3.zero;

            // 성공을 반환
            return INode.ENodeState.SuccessState;
        }

        transform.position = Vector3.MoveTowards(transform.position, _currWayPoint, _applyMovementSpeed * Time.deltaTime);

        return INode.ENodeState.FailureState;
    }

    private Vector3 SetWayPointByRandom()
    {
        if (_currWayPoint == Vector3.zero)
        {
            // 현재 위치에서 랜덤한 지점을 반환
            var randomSite = (UnityEngine.Random.insideUnitSphere * _circleRadius) + transform.position;

            // 둘째자리까지 반올림
            // _currWayPoint = new Vector3((float)Math.Round(randomSite.x, 2), 0f, (float)Math.Round(randomSite.z, 2));
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

    private INode.ENodeState DoIdle()
    {
        if (!_idleState)
        {
            _idleState = true;

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

    // 확인용 기즈모
    private void OnDrawGizmos()
    {
        if (!_drawGizmos)
            return;

        if (_currWayPoint != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_currWayPoint, .3f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _currWayPoint);
        }
    }
}

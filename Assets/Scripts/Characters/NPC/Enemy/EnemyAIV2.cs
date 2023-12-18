using UnityEngine;
using System.Collections.Generic;

using BehaviorTree;

using Sirenix.OdinInspector;

public class EnemyAIV2 : MonoBehaviour
{
    [Title("[Test State]")]
    [SerializeField] private INode.ENodeState _patrolNodeState;
    [SerializeField] private INode.ENodeState _idleNodeState;

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

    private BahaviorTreeRunner _btRunner;

    private bool _idleState = false;
    private bool _moveState = false;
    private bool _attackState = false;

    private float _patrolMovementSpeed = 3f;
    private float _chaseMovementSpeed = 5f;
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
                        // new ActionNode(DoIdle),
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

        Debug.Log(distance);

        // 근사치를 비교함
        if (Mathf.Approximately(distance, 0.0f))
            Debug.Log("Yahoo!");
        else
            Debug.Log("Na...");

        return INode.ENodeState.FailureState;
    }

    private Vector3 SetWayPointByRandom()
    {
        if (_currWayPoint == Vector3.zero)
        {
            // 현재 위치에서 랜덤한 지점을 반환
            var randomSite = Random.insideUnitSphere * _circleRadius;

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
        return INode.ENodeState.FailureState;
    }
}

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BehaviorTree;

using Sirenix.OdinInspector;

public class EnemyAI : MonoBehaviour
{
    [Title("[Components]")]
    [SerializeField] private FieldOfView _fieldOfView;
    [SerializeField] private Animator _animator;

    [Title("[Common]")]
    [PropertyRange(0, "MaxViewRadius")]
    [SerializeField] private float _meleeAttackRange;
    [SerializeField] private float _patrolMovementSpeed = 1f;
    [SerializeField] private float _chaseMovementSpeed = 3f;
    [SerializeField] private Transform[] _wayPoints;

    private Vector3 _originPosition = default;
    private BahaviorTreeRunner _btRunner = null;

    private Transform _enemyTransform = null;
    private Transform _detectedPlayer = null;
    private Transform _currentWayPoint = null;

    private const string ATTACK_ANIM_STATE_NAME = "Attack";
    private const string ATTACK_ANIM_TRIGGER_NAME = "attack";

    private float _turnSmoothTime = .2f;
    private float _turnSmoothVelocity;

    private int _currentWayPointIndex = 0;

    private bool _idleState = false;
    private bool _walkState = false;
    private bool _chaseState = false;

    private float _currentWaitTime = 0f;
    private float _waitTime = 0f;

    private float _applyMovementSpeed = 0f;

    public float MaxViewRadius => _fieldOfView.ViewRadius;

    public Animator ThisAnimator => _animator;
    public Transform ThisTransform => this.transform;

    private void Awake()
    {
        _originPosition = transform.position;
        _applyMovementSpeed = _patrolMovementSpeed;

        _btRunner = new BahaviorTreeRunner(SettingBT());
    }

    private void Update()
    {
        _btRunner.Operate();

        SetAnimator();
    }

    private INode SettingBT()
    {
        return new SelectorNode(
            new List<INode>()
            {
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(CheckMeleeAttacking),
                        new ActionNode(CheckPlayerWithinMeleeAttackRange),
                        new ActionNode(DoMeleeAttack),
                    }
                ),
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(CheckDetectPlayer),
                        new ActionNode(MoveToPlayer),
                    }
                ),
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(DoPatrol),
                        new ActionNode(DoIdle),
                    }
                )
            }
        );
    }

    private bool IsAnimationRunning(string stateName)
    {
        if (_animator != null)
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            {
                var normalizedTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                return normalizedTime != 0f && normalizedTime < 1f;
            }
        }

        return false;
    }

    // Attack node
    private INode.ENodeState CheckMeleeAttacking()
    {
        // 공격 중인지 확인
        return IsAnimationRunning(ATTACK_ANIM_STATE_NAME) ? INode.ENodeState.RunningState : INode.ENodeState.SuccessState;
    }

    private INode.ENodeState CheckPlayerWithinMeleeAttackRange()
    {
        // 공격 범위에 플레이어가 들어 왔는지 확인
        if (_detectedPlayer != null)
        {
            // SqrMagnitude -> 벡터의 크기의 제곱
            if (Vector3.SqrMagnitude(_detectedPlayer.position - transform.position) < (_meleeAttackRange * _meleeAttackRange))
                return INode.ENodeState.SuccessState;
        }

        return INode.ENodeState.FailureState;
    }

    private INode.ENodeState DoMeleeAttack()
    {
        // 공격 상태로 전환
        if (_detectedPlayer != null)
        {
            if (_animator != null)
                _animator.SetTrigger(ATTACK_ANIM_TRIGGER_NAME);

            return INode.ENodeState.SuccessState;
        }

        return INode.ENodeState.FailureState;
    }

    // Move or detect node
    private INode.ENodeState CheckDetectPlayer()
    {
        if (_fieldOfView == null)
            return INode.ENodeState.FailureState;

        _fieldOfView.FindVisibleTargets();
        _detectedPlayer = _fieldOfView.NearestTarget;

        return _detectedPlayer != null ? INode.ENodeState.SuccessState : INode.ENodeState.FailureState;
    }

    private INode.ENodeState DetectPlayerAndIdle()
    {
        // 플레이어를 발견하고 잠시 대기
        RotateToTarget(_detectedPlayer.position);

        if (_currentWaitTime < .5f)
        {
            _currentWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }
        else
        {
            _currentWaitTime = 0f;
            return INode.ENodeState.SuccessState;
        }
    }

    private INode.ENodeState MoveToPlayer()
    {
        if (_detectedPlayer != null)
        {
            _applyMovementSpeed = _chaseMovementSpeed;

            if (Vector3.SqrMagnitude(_detectedPlayer.position - transform.position) < (_meleeAttackRange * _meleeAttackRange))
                return INode.ENodeState.SuccessState;

            transform.position = Vector3.MoveTowards(transform.position, _detectedPlayer.position, Time.deltaTime * _applyMovementSpeed);

            if (_fieldOfView != null && _detectedPlayer != null)
                RotateToTarget(_detectedPlayer.position);

            _chaseState = true;
            _walkState = false;

            return INode.ENodeState.RunningState;
        }

        _chaseState = false;
        _walkState = false;

        return INode.ENodeState.FailureState;
    }

    private INode.ENodeState MissingPlayer()
    {
        // 플레이어를 놓치고 잠시 대기
        return DoIdle();
    }

    // // Move to origin position node
    // private INode.ENodeState MoveToOriginPosition()
    // {
    //     // float epsilon -> 부동 소수점 비교를 통해 오차를 정밀하게 계산
    //     if (Vector3.SqrMagnitude(_originPosition - transform.position) <= (float.Epsilon * float.Epsilon))
    //         return INode.ENodeState.SuccessState;
    //     else
    //     {
    //         transform.position = Vector3.MoveTowards(transform.position, _originPosition, _applyMovementSpeed * Time.deltaTime);

    //         if (_fieldOfView != null && _detectedPlayer != null)
    //             RotateToTarget(_detectedPlayer.position);

    //         return INode.ENodeState.RunningState;
    //     }
    // }

    // Patrol node
    private INode.ENodeState DoPatrol()
    {
        if (_wayPoints.Length == 0)
            return INode.ENodeState.FailureState;

        if (_idleState)
            return INode.ENodeState.SuccessState;

        _currentWayPoint = _wayPoints[_currentWayPointIndex];
        _applyMovementSpeed = _patrolMovementSpeed;

        if (Vector3.SqrMagnitude(_currentWayPoint.position - transform.position) <= (float.Epsilon * float.Epsilon))
        {
            // 도착하면 제거
            _currentWayPoint = null;
            _currentWayPointIndex = (_currentWayPointIndex + 1) % _wayPoints.Length;

            return INode.ENodeState.SuccessState;
        }

        transform.position = Vector3.MoveTowards(transform.position, _currentWayPoint.position, _applyMovementSpeed * Time.deltaTime);
        RotateToTarget(_currentWayPoint.position);

        _walkState = true;
        _chaseState = false;

        // 순찰 지점이 있는지 확인
        return _currentWayPoint != null ? INode.ENodeState.RunningState : INode.ENodeState.FailureState;
    }

    private INode.ENodeState DoIdle()
    {
        if (!_idleState)
        {
            _idleState = true;
            _walkState = false;
            _chaseState = false;

            // 대기 시간은 랜덤하게
            if (_waitTime == 0f)
                _waitTime = Random.Range(3f, 7f);
        }

        // 대기하는 동안은 running
        if (_currentWaitTime < _waitTime)
        {
            _currentWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }
        // 대기 끝나면 성공
        else
        {
            _idleState = false;
            _currentWaitTime = 0f;
            _waitTime = 0f;

            return INode.ENodeState.FailureState;
        }
    }

    private void RotateToTarget(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        float targetAngle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);

        this.transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void SetAnimator()
    {
        _animator.SetBool("IsWalk", _walkState);
        _animator.SetBool("IsChase", _chaseState);
    }
}

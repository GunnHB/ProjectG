using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class EnemyOffensiveBehavior : EnemyBehaviorBase
{
    protected List<INode> _attackNodeList = new();
    protected List<INode> _chaseNodeList = new();

    protected override SelectorNode EntireBehaviorNode()
    {
        _entireNodeList.Add(AttackNode());
        _entireNodeList.Add(ChaseNode());

        return base.EntireBehaviorNode();
    }

    protected virtual SequenceNode AttackNode()
    {
        _attackNodeList.Add(new ActionNode(CheckAttacking));
        _attackNodeList.Add(new ActionNode(CheckAttackRange));
        _attackNodeList.Add(new ActionNode(WaitAfterAttack));
        _attackNodeList.Add(new ActionNode(DoAttack));

        return new SequenceNode(_attackNodeList);
    }

    protected virtual SequenceNode ChaseNode()
    {
        _chaseNodeList.Add(new ActionNode(CheckDetectTarget));
        _chaseNodeList.Add(new ActionNode(ChaseToTarget));

        return new SequenceNode(_chaseNodeList);
    }

    private INode.ENodeState CheckAttacking()
    {
        if (_state == EnemyState.Attack)
        {
            // 공격 중이면 running 반환
            if (_enemyBase.AnimChecker != null && _enemyBase.AnimChecker.ProcessingAttack)
                return INode.ENodeState.RunningState;
        }
        else
        {
            // 공격 이외의 상태에서는 대기 시간을 초기화
            _enemyBase.Database.CurrAttackWaitTime = 0f;
            _enemyBase.Database.AttackWaitTime = 0f;
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

    private INode.ENodeState WaitAfterAttack()
    {
        if (_enemyBase.Database.CurrAttackWaitTime < _enemyBase.Database.AttackWaitTime)
        {
            _enemyBase.Database.CurrAttackWaitTime += Time.deltaTime;
            return INode.ENodeState.RunningState;
        }

        SetEnemyState(EnemyState.Idle);
        return INode.ENodeState.SuccessState;
    }

    private INode.ENodeState DoAttack()
    {
        if (_targetPlayer == null)
            return INode.ENodeState.FailureState;

        if (_targetPlayer.TryGetComponent(out Collider targetCollider))
            _enemyBase.AnimChecker.SetOppnentCollider(targetCollider);

        // 공격 상태로 변경
        SetEnemyState(EnemyState.Attack);

        _enemyBase.Database.AttackWaitTime = UnityEngine.Random.Range(1f, 3f);
        _enemyBase.Database.CurrAttackWaitTime = 0f;

        return INode.ENodeState.SuccessState;
    }

    private INode.ENodeState CheckDetectTarget()
    {
        _fieldOfView.FindVisibleTargets();
        _targetPlayer = _fieldOfView.NearestTarget;

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

    private INode.ENodeState ChaseToTarget()
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
        _enemyBase.ApplySpeed = _enemyBase.Database.ThisSprintSpeed;
        MoveToTarget(_targetPlayer.position);

        // 타겟까지 도착해야하므로 running 반환
        return INode.ENodeState.RunningState;
    }
}

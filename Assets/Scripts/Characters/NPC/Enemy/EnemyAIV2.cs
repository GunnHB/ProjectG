using UnityEngine;
using System.Collections.Generic;

using BehaviorTree;

using Sirenix.OdinInspector;

public class EnemyAIV2 : MonoBehaviour
{
    [Title("[Test State]")]
    [SerializeField] private INode.ENodeState _patrolNodeState;
    [SerializeField] private INode.ENodeState _idleNodeState;

    [SerializeField] private FieldOfView _fieldOfView;

    private BahaviorTreeRunner _btRunner;

    private bool _idleState = false;
    private bool _moveState = false;
    private bool _attackState = false;

    private float _patrolMovementSpeed = 3f;
    private float _chaseMovementSpeed = 5f;
    private float _applyMovementSpeed;

    private void Awake()
    {
        _btRunner = new BahaviorTreeRunner(SettingBT());
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
        Debug.Log("DoPatrol");
        return _patrolNodeState;
    }

    private INode.ENodeState DoIdle()
    {
        Debug.Log("DoIdle");
        return _idleNodeState;
    }
}

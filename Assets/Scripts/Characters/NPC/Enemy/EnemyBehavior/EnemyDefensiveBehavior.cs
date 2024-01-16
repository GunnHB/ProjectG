using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class EnemyDefensiveBehavior : EnemyBehaviorBase
{
    protected List<INode> _runAwayNodeList = new();

    protected virtual SequenceNode RunAwayNode()
    {
        _runAwayNodeList.Add(new ActionNode(RunToOtherWay));

        return new SequenceNode(_runAwayNodeList);
    }

    private INode.ENodeState RunToOtherWay()
    {
        return INode.ENodeState.FailureState;
    }
}

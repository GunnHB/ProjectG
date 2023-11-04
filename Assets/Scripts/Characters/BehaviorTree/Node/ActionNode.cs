using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 실제로 어떤 행동을 하는 노드
    /// 
    /// 델리게이트를 통해 행위를 전달받아 실행
    /// </summary>
    public class ActionNode : INode
    {
        Func<INode.ENodeState> _onUpdate = null;

        public ActionNode(Func<INode.ENodeState> onUpdate)
        {
            _onUpdate = onUpdate;
        }

        public INode.ENodeState Evaluate() => _onUpdate?.Invoke() ?? INode.ENodeState.FailureState;
    }
}

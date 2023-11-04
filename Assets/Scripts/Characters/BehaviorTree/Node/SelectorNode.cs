using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 자식 노드 중에서 처음으로 success 나 running 상태를 가진 노드가 발생하면
    /// 그 노드까지 진행하고 멈춘다.
    /// </summary>
    public class SelectorNode : INode
    {
        List<INode> _childs;

        public SelectorNode(List<INode> childs)
        {
            _childs = childs;
        }

        public INode.ENodeState Evaluate()
        {
            if (_childs == null)
                return INode.ENodeState.FailureState;

            foreach (var child in _childs)
            {
                switch (child.Evaluate())
                {
                    case INode.ENodeState.RunningState:
                        return INode.ENodeState.RunningState;
                    case INode.ENodeState.SuccessState:
                        return INode.ENodeState.SuccessState;
                }
            }

            return INode.ENodeState.FailureState;
        }
    }
}

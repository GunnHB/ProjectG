using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 노드의 결과를 반전시킴
    /// </summary>
    public class InvertorNode : INode
    {
        private INode _node;

        public InvertorNode(INode node)
        {
            this._node = node;
        }

        public INode.ENodeState Evaluate()
        {
            // running 상태는 그대로 반환
            switch (this._node.Evaluate())
            {
                case INode.ENodeState.SuccessState:
                    return INode.ENodeState.FailureState;
                case INode.ENodeState.FailureState:
                    return INode.ENodeState.SuccessState;
                default:
                    return INode.ENodeState.RunningState;
            }
        }
    }
}

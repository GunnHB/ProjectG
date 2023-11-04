using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    /// <summary>
    /// 자식 노드를 왼쪽에서 오른쪽으로 진행하면서 Failure 상태가 나올 때까지 진행
    /// 
    /// (*주의*)
    /// running 상태일 때는 그 상태를 유지해야 하기 때문에 다음 노드로 이동하면 안되고
    /// 다음 프레임 때도 그 자식에 대한 평가를 진행해야 한다.
    /// 
    /// 적을 발견하고 이동 시 도착하지 않았는데도 공격 상태로 변경되면 안됨
    /// </summary>
    public class SequenceNode : INode
    {
        List<INode> _childs;

        public SequenceNode(List<INode> childs)
        {
            _childs = childs;
        }

        public INode.ENodeState Evaluate()
        {
            if (_childs == null || _childs.Count == 0)
                return INode.ENodeState.FailureState;

            foreach (var child in _childs)
            {
                switch (child.Evaluate())
                {
                    case INode.ENodeState.RunningState:
                        return INode.ENodeState.RunningState;
                    case INode.ENodeState.SuccessState:
                        continue;
                    case INode.ENodeState.FailureState:
                        return INode.ENodeState.FailureState;
                }
            }

            return INode.ENodeState.SuccessState;
        }
    }
}

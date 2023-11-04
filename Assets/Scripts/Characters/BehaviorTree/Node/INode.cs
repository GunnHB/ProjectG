using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public interface INode
    {
        public enum ENodeState
        {
            RunningState,       // 동작 중
            SuccessState,       // 성공
            FailureState,       // 실패
        }

        public ENodeState Evaluate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorTree
{
    public class BahaviorTreeRunner
    {
        INode _rootNode;

        public BahaviorTreeRunner(INode rootNode)
        {
            _rootNode = rootNode;
        }

        public void Operate()
        {
            _rootNode.Evaluate();
        }
    }
}

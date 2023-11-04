using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class NPCAI : MonoBehaviour
{
    private BahaviorTreeRunner _btRunner = null;

    private void Awake()
    {
        _btRunner = new BahaviorTreeRunner(SettingBT());
    }

    private INode SettingBT()
    {
        return new SelectorNode(
            new List<INode>()
            {
                new SequenceNode(
                    new List<INode>()
                    {
                        new ActionNode(TestPlayer),
                    }
                ),
            }
        );
    }

    private INode.ENodeState TestPlayer()
    {
        return INode.ENodeState.SuccessState;
    }
}

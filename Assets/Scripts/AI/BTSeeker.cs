using AgentControllers;
using BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTSeeker : BaseBT
{
    protected override BaseNode InitializeBehaviourTree()
    {
        AgentController _actor = GetComponent<NPCController>();

        BaseNode root = new SelectorNode(new List<BaseNode>
        {
            new NodeEscapeFromBludger(_actor),
             new SequenceNode(new List<BaseNode>
            {
                new NodeCheckGoldenSnitchState(_actor),
                new NodeSeekGoldenSnitch(_actor),
                new NodeCheckCanTakeGoldenSnitch(_actor),
                new NodeTakeGoldenSnitch(_actor),
            }),
        });
        return root;
    }
}

using AgentControllers;
using BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTBeater : BaseBT
{
    protected override BaseNode InitializeBehaviourTree()
    {
        AgentController _actor = GetComponent<NPCController>();

        BaseNode root = new SelectorNode(new List<BaseNode>
        {
            new SequenceNode(new List<BaseNode>
            {
                new NodeCheckAvailableBeatBludger(_actor),
                new NodeBeateBludger(_actor),
            }),

            new SequenceNode(new List<BaseNode>
            {
                new NodeCheckPerceptBludger(_actor),
                new NodeChaseAvailableBludger(_actor),
            }),

            new NodeFlyInCircleBeater(_actor),
        });

        return root;
    }
}

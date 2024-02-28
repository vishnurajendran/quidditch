using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgentControllers;
using BT;

public class BTChaser : BaseBT
{
    protected override BaseNode InitializeBehaviourTree()
    {
        AgentController _actor = GetComponent<AgentController>();

        BaseNode root = new SelectorNode(new List<BaseNode>
            {
              new NodeFlyInCircle(_actor, new Vector3(10.0f, 8.0f, 10.0f), 30.0f),
            });

        return root;
    }
}

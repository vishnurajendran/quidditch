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
             
        });

        return root;
    }
}

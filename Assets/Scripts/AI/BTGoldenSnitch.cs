using AgentControllers;
using BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTGoldenSnitch : BaseBT
{
    protected override BaseNode InitializeBehaviourTree()
    {
        AgentController _actor = GetComponent<AgentController>();

        BaseNode root = new SequenceNode(new List<BaseNode> {
            new SelectorNode(new List<BaseNode>
            {
              new NodeSnitchWander(_actor),
            }),
            new NodeSnitchFloat(_actor, 2.0f, 2.0f),
    });
            
            
            

        return root;
    }
}
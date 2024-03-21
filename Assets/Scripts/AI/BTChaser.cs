using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AgentControllers;
using BT;

public class BTChaser : BaseBT
{
    protected override BaseNode InitializeBehaviourTree()
    {
        AgentController _actor = GetComponent<NPCController>();

        BaseNode root = new SelectorNode(new List<BaseNode>
        {
            new NodeEscapeFromBludger(_actor),

            //if the quaffle is in space, try to catch it 
            new SequenceNode(new List<BaseNode> 
            {
                new NodeCheckQuaffleStateSpace(_actor),
                new NodeChaseQuaffle(_actor),
                new NodeCheckCanTakeQuaffle(_actor),
                new NodeTakeQuaffle(_actor),
            }),
            
            //if the quaffle is cached by other, fly circle around it and find the opportunities to get the ball
            new SequenceNode(new List<BaseNode> 
            {
                new NodeCheckQuaffleCachedByOthers(_actor),
                new NodeFlyInCircle(_actor),
            }),

            //if the taken the quaffle, the NPC try to goto the throw Area and throw the quaffle to the target
            //todo: swap character?
            new SequenceNode(new List<BaseNode>
            {
                new NodeCheckChaserGotQuaffle(_actor),
                new NodeGoToThrowArea(_actor),
                new NodeCheckInThrowArea(_actor),
                new NodeThrowQuaffleToTarget(_actor),
            })
        });

        return root;
    }
}

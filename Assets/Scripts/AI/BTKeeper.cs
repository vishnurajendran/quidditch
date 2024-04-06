using AgentControllers;
using BT;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTKeeper : BaseBT
{
    protected override BaseNode InitializeBehaviourTree()
    {
        AgentController _actor = GetComponent<NPCController>();

        BaseNode root = new SelectorNode(new List<BaseNode>
        {
            //first priority: escape from the bludger
             new NodeEscapeFromBludger(_actor),

             //second priority: if can take quaffle, take it
             new SequenceNode(new List<BaseNode>
             {
                 new NodeCheckCanTakeQuaffle(_actor),
                 new NodeTakeQuaffle(_actor),
             }),

              new SequenceNode(new List<BaseNode>
              {
                  new NodeCheckLootQuaffle(_actor),
                  new NodeChaseQuaffle(_actor),
              }),

             //third priority: has taken the quaffle, pass it to the nearest chaser
            new SequenceNode(new List<BaseNode>
            {
                new NodeCheckChaserGotQuaffle(_actor),
                new NodeSeekNearestChaser(_actor),
                new NodeCheckPassDistance(_actor),
                new NodeThrowBallToFriend(_actor),
            }),

            //fourth priority: defend the target
             new SequenceNode(new List<BaseNode>
             {
                new NodePerceptChaserWithQuaffle(_actor),
                new NodeDefenceTheChaser(_actor),
             }),

             //fifth priority: circle the target
            new SequenceNode(new List<BaseNode>
            {
                new NodeCheckNotCacheQuaffle(_actor),
                new NodeCircleTheTarget(_actor),
            }) 
        });
        return root;
    }
}

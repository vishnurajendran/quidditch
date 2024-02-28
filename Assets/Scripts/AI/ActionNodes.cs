using AgentControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BT
{

    public class NodeFlyInCircle : BaseNode
    {
        private Vector3 circleCenter = Vector3.zero;
        private float radius = 10.0f;
        private float offset = 1.0f;

        public NodeFlyInCircle(AgentController actor_, 
            Vector3 circleCenter,
            float radius = 10.0f)
            : base(actor_)
        {
            this.circleCenter = circleCenter;
            this.radius = radius;
        }
        public override NodeState Process()
        {
            
            Vector3 originDirection = circleCenter - actor.transform.position;
            Vector3 planeDirection = originDirection;
            planeDirection.y = 0.0f;
            Vector3 targetPos = circleCenter + planeDirection.normalized * radius;
            float distance = Vector3.Distance(circleCenter, actor.transform.position);

            
            //Vector3 desiredVector = Vector3.Cross((circleCenter - actor.transform.position).normalized,
            //       Vector3.up);
            //(actor as NPCController).AddKinematicVector(desiredVector);
            
            
            //out of circle
            if (distance > radius)
            {
                Vector3 desiredVector = targetPos - actor.transform.position;
                (actor as NPCController).AddKinematicVector(desiredVector);
            }
            else //in circle
            {
                Vector3 desiredVector = Vector3.Cross((circleCenter - actor.transform.position).normalized,
                    Vector3.up);
                (actor as NPCController).AddKinematicVector(desiredVector);
            }

            state = NodeState.RUNNING;
            return state;
        }
    }

}
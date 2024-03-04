using AgentControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Agents;
using UnityEngine.UIElements;

namespace BT
{

    public class NodeSnitchFloat : BaseNode
    {
        private float floatWeight = 1.0f;
        private float floatSpeed = 3.0f;

        public NodeSnitchFloat(AgentController actor_,
            float weight = 1.0f, float speed = 1.0f)
            : base(actor_)
        {
            this.floatWeight = weight;
            this.floatSpeed = speed;
        }

        public override NodeState Process()
        {
            float currentFloatValue = Mathf.Sin(Time.time * floatSpeed);
            Vector3 scrollY = new Vector3(0f, currentFloatValue * floatWeight, 0f);
            (actor as NPCController).AddKinematicVector(scrollY * 0.2f);

            state = NodeState.RUNNING;
            return state;
        }
    }

    public class NodeSnitchWander : BaseNode
    {
        private float radius = 10.0f;
        private float wanderTimer = 0.0f;
        private float timePeriod = 3.0f;
        private Vector3 target = Vector3.zero;
        private float arrivalRadius = 5.0f;

        public NodeSnitchWander(AgentController actor_,
            float radius = 10.0f)
            : base(actor_)
        {
            this.radius = radius;
        }

        public override NodeState Process()
        {

            wanderTimer += Time.deltaTime;

            if (wanderTimer > timePeriod)
            {
                float randomValX = Random.Range(1.0F, radius) * Util.GetRandomSign();
                float randomValY = Random.Range(1.0F, radius) * Util.GetRandomSign();
                float randomValZ = Random.Range(1.0F, radius) * Util.GetRandomSign();
                target = actor.transform.position + new Vector3(randomValX, randomValY, randomValY);
                wanderTimer = 0.0f;
            }
            else
            {
                float distance = Vector3.Distance(target, actor.transform.position);
                
                if (distance > arrivalRadius)
                {
                    Vector3 desiredVector = (target - actor.transform.position).normalized;
                    (actor as NPCController).AddKinematicVector(desiredVector);
                }
            }

            state = NodeState.RUNNING;
            return state;
        }
    }


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
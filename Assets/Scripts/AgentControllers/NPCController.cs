using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AgentControllers
{
    public class NPCController : AgentController
    {
        [SerializeField] private float floorY;
        [SerializeField] private float ceilY;

        private Vector3 curDirection = Vector3.zero;

        private float CalculateHorizontal()
        {
            return Vector3.Dot(curDirection.normalized, transform.right);
        }

        public void AddKinematicVector(Vector3 kVec, float weight = 1.0f)
        {
            curDirection += kVec * weight;
        }

        public void ResetKinematicVector()
        {
            curDirection = Vector3.zero;
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.Instance.GameStarted)
            {
                _agent.Move(Vector3.zero);
                _agent.SetGraphicRollDirection(0);
                _agent.Boost(false);
                return;
            }

            bool boost = false; //todo: boost speed with decision
            float curHorizontal = CalculateHorizontal();

            //limitation
            if (Mathf.Approximately(transform.position.y, floorY) && curDirection.y < 0)
                curDirection.y = 0;
            else if (Mathf.Approximately(transform.position.y, ceilY) && curDirection.y > 0)
                curDirection.y = 0;

            _agent.Move(curDirection.normalized);
            _agent.SetGraphicRollDirection(curHorizontal);
            _agent.Boost(boost);

            //reset current direction
            curDirection = Vector3.zero;
        }
    }
}

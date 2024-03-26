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

        public void SetKinematicVector(Vector3 kVec)
        {
            curDirection = kVec.normalized;
        }

        public void AddKinematicVector(Vector3 kVec, float weight = 1.0f)
        {
            curDirection += kVec * weight;
        }

        public void ResetKinematicVector()
        {
            curDirection = Vector3.zero;
        }
        
        private float GetInterpolationValue(float curVal, float limitedVal)
        {
            if (curVal > limitedVal)
                return 2.0f;
            return (1.0f + (curVal / limitedVal));
        }


        public bool CheckIsHitByBludger()
        {
            return GetComponent<Role>() && GetComponent<Role>().curColdDownInDizziness > 0.0f;
        }

        public void ProcessCurDirectionInLimitation()
        {
            Vector3 correctDirVect = Vector3.zero;
            Vector3 curPosition = transform.position;

            Vector3 maxTakenForcePoint = GameManager.Instance.GetMaxSpacePointTakenForce();
            Vector3 minTakenForcePoint = GameManager.Instance.GetMinSpacePointTakenForce();
            float takenForceRadius = GameManager.Instance.GetForceDistance();

            float positiveX = curPosition.x - maxTakenForcePoint.x;
            float positiveY = curPosition.y - maxTakenForcePoint.y;
            float positiveZ = curPosition.z - maxTakenForcePoint.z;
            float negativeX = minTakenForcePoint.x - curPosition.x;
            float negativeY = minTakenForcePoint.y - curPosition.y;
            float negativeZ = minTakenForcePoint.z - curPosition.z;

            if(positiveX > 0.0)
            {
                float correctX = GetInterpolationValue(positiveX, takenForceRadius);
                correctDirVect += new Vector3(-correctX, 0.0f, 0.0f);
            }

            if (positiveY > 0.0)
            {
                float correctY = GetInterpolationValue(positiveY, takenForceRadius);
                correctDirVect += new Vector3(0.0f, -correctY, 0.0f);
            }

            if (positiveZ > 0.0)
            {
                float correctZ = GetInterpolationValue(positiveZ, takenForceRadius);
                correctDirVect += new Vector3(0.0f, 0.0f, -correctZ);
            }

            if (negativeX > 0.0)
            {
                float correctX = GetInterpolationValue(negativeX, takenForceRadius);
                correctDirVect += new Vector3(correctX, 0.0f, 0.0f);
            }

            if (negativeY > 0.0)
            {
                float correctY = GetInterpolationValue(negativeY, takenForceRadius);
                correctDirVect += new Vector3(0.0f, correctY, 0.0f);
            }

            if (negativeZ > 0.0)
            {
                float correctZ = GetInterpolationValue(negativeZ, takenForceRadius);
                correctDirVect += new Vector3(0.0f, correctZ, 0.0f);
            }


            //check the dizzy state
            if(CheckIsHitByBludger())
            {
                curDirection = new Vector3(0.0f, -1.0f, 0.0f);
                Debug.Log("current direction:" + curDirection);
            }
            else
            {
                curDirection = (curDirection + correctDirVect).normalized;
            }
        }


        // Update is called once per frame
        void Update()
        {
            if(!GameManager.Instance.GameStarted)
                return;
            
            bool boost = false; //todo: boost speed with decision
            bool slowing = false;
            float curHorizontal = CalculateHorizontal();

            //pre-processing the cur direction vector
            ProcessCurDirectionInLimitation();
            slowing = CheckIsHitByBludger();

            //limitation
            if (Mathf.Approximately(transform.position.y, floorY) && curDirection.y < 0)
                curDirection.y = 0;
            else if (Mathf.Approximately(transform.position.y, ceilY) && curDirection.y > 0)
                curDirection.y = 0;

            _agent.Move(curDirection.normalized);
            _agent.SetGraphicRollDirection(curHorizontal);
            _agent.Boost(boost);
            _agent.Slow(slowing);

            //reset current direction
            curDirection = Vector3.zero;
        }
    }
}

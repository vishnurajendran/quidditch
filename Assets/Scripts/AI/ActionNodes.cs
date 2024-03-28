using AgentControllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Agents;
using UnityEngine.UIElements;
using Teams;
using JetBrains.Annotations;
using Unity.VisualScripting;

namespace BT
{

    public static class ActionUtils
    {
        public static Vector3 CircleFlyDirection(Vector3 circleCenter, Vector3 actorPos, float radius)
        {
            Vector3 originDirection = circleCenter - actorPos;
            Vector3 planeDirection = originDirection;
            planeDirection.y = 0.0f;
            Vector3 targetPos = circleCenter + planeDirection.normalized * radius;
            float distance = Vector3.Distance(circleCenter, actorPos);

            //out of circle
            if (distance > radius)
            {
                Vector3 desiredVector = targetPos - actorPos;
                return desiredVector;
            }
            else //in circle
            {
                Vector3 desiredVector = Vector3.Cross((circleCenter - actorPos).normalized,
                    Vector3.up);
                return desiredVector;
            }
        }

    }

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

    /*
     * The action nodes used in chaser
     */
    public class NodeFlyInCircle : BaseNode
    {
        public NodeFlyInCircle(AgentController actor_)
            : base(actor_)
        {
        }
        public override NodeState Process()
        {
            if (GameManager.Instance.quaffle.GetComponent<Quaffle>().takenChaser == null)
                return NodeState.FAILURE;

            Team teamType = GameManager.Instance.quaffle.GetComponent<Quaffle>().takenChaser.GetComponent<TeamEntity>().MyTeam;
            Vector3 circleCenter = GameManager.Instance.quaffle.GetComponent<Quaffle>().takenChaser.transform.position;

            float radius = actor.GetComponent<Role>().guardeCircleRadius;
            if (teamType != actor.GetComponent<TeamEntity>().MyTeam)
            {
                radius = actor.GetComponent<Role>().attackCircleRadius;
            }

            Vector3 desiredVec = ActionUtils.CircleFlyDirection(circleCenter, actor.transform.position, radius);
            (actor as NPCController).AddKinematicVector(desiredVec);

            state = NodeState.RUNNING;
            return state;
        }
    }

    public class NodeEscapeFromBludger : BaseNode
    {
        public NodeEscapeFromBludger(AgentController actor_)
               : base(actor_)
        {
        }

        public bool ApproximationVector(Vector3 a, Vector3 b)
        {
            float xv = Mathf.Abs(a.x - b.x);
            float yv = Mathf.Abs(a.y - b.y);
            float zv = Mathf.Abs(a.z - b.z);
            return (xv < 0.1f && yv < 0.1f && zv < 0.1f);
        }

        public override NodeState Process()
        {
            List<GameObject> bludges = GameManager.Instance.Bludges;
            float buldgerEscapeRange = actor.GetComponent<Role>().escapeBuldgerRadius;

            List<Transform> escapeObjectsList = new List<Transform>();
            for(int i =0; i < bludges.Count; ++i)
            {
                float distance = Vector3.Distance(actor.transform.position, 
                    bludges[i].transform.position);
                if(distance < buldgerEscapeRange)
                    escapeObjectsList.Add(bludges[i].transform);
            }

            if (escapeObjectsList.Count == 0)
                return NodeState.FAILURE;

            Vector3 escapedVector = Vector3.zero;
            if (escapeObjectsList.Count == 1)
            {
                escapedVector = (actor.transform.position - escapeObjectsList[0].position).normalized;
            }
            else //count == 2
            {
                Vector3 escapeFromBall0 = (actor.transform.position - escapeObjectsList[0].position).normalized;
                Vector3 escapeFromBall1 = (actor.transform.position - escapeObjectsList[1].position).normalized;
                
                if(ApproximationVector(escapeFromBall0 + escapeFromBall1, Vector3.zero))
                {
                    escapedVector = Vector3.Cross(escapeFromBall0, Vector3.up);
                }
                else
                {
                    escapedVector = (escapeFromBall0 + escapeFromBall1).normalized;
                }
            }
            (actor as NPCController).AddKinematicVector(escapedVector);
            return NodeState.FAILURE;
        }
    }


    //is the quaffle in the space?
    public class NodeCheckQuaffleStateSpace : BaseNode
    {
        public NodeCheckQuaffleStateSpace(AgentController actor_)
           : base(actor_)
        {
        }
        public override NodeState Process()
        {
            QuaffleState curState = GameManager.Instance.g_quaffleState;
            bool isChaseValid = GameManager.Instance.quaffle.GetComponent<Quaffle>().IsChaserValidForCurrentBall(actor.gameObject);
            if (curState == QuaffleState.Space && isChaseValid)
                return NodeState.SUCCESS;
            return NodeState.FAILURE;
        }
    }

    public class NodeCheckQuaffleCachedByOthers : BaseNode
    {
        public NodeCheckQuaffleCachedByOthers(AgentController actor_)
           : base(actor_)
        {
        }
        public override NodeState Process()
        {
            QuaffleState curState = GameManager.Instance.g_quaffleState;
            bool isCachedQuaffle = actor.transform.GetComponent<Role>().isCached;
            if (curState != QuaffleState.Space && !isCachedQuaffle)
            {
                //Debug.Log("NodeCheckQuaffleCachedByOthers: true");
                return NodeState.SUCCESS;
            }
            else
            {
                //Debug.Log("NodeCheckQuaffleCachedByOthers: false");
                return NodeState.FAILURE;
            }
        }
    }

    //is the quaffle cached by the other team?
    public class NodeCheckQuaffleStateCachedByOtherTeam : BaseNode
    {
        public NodeCheckQuaffleStateCachedByOtherTeam(AgentController actor_)
           : base(actor_)
        {
        }
        public override NodeState Process()
        {
            QuaffleState curState = GameManager.Instance.g_quaffleState;
            if (curState == QuaffleState.CachedByTeam1)
            {
                if (actor.gameObject.GetComponent<TeamEntity>().MyTeam == Team.Team_1)
                    return NodeState.FAILURE;
                else
                    return NodeState.SUCCESS;
            }
            else if (curState == QuaffleState.CachedByTeam2)
            {
                if (actor.gameObject.GetComponent<TeamEntity>().MyTeam == Team.Team_2)
                    return NodeState.FAILURE;
                else
                    return NodeState.SUCCESS;
            }
            else
                return NodeState.FAILURE;
        }
    }

    //is the quaffle cached by our team?
    public class NodeCheckQuaffleStateCachedByOurTeam : BaseNode
    {
        public NodeCheckQuaffleStateCachedByOurTeam(AgentController actor_)
           : base(actor_)
        {
        }
        public override NodeState Process()
        {
            QuaffleState curState = GameManager.Instance.g_quaffleState;
            if (curState == QuaffleState.CachedByTeam1)
            {
                if (actor.gameObject.GetComponent<TeamEntity>().MyTeam == Team.Team_1)
                    return NodeState.SUCCESS;
                else
                    return NodeState.FAILURE;
            }
            else if (curState == QuaffleState.CachedByTeam2)
            {
                if (actor.gameObject.GetComponent<TeamEntity>().MyTeam == Team.Team_2)
                    return NodeState.SUCCESS;
                else
                    return NodeState.FAILURE;
            }
            else
                return NodeState.FAILURE;
        }
    }

    public class NodeCheckChaserGotQuaffle : BaseNode
    {
        public NodeCheckChaserGotQuaffle(AgentController actor_)
           : base(actor_)
        {
        }
        public override NodeState Process()
        {
            if (actor.gameObject.GetComponent<Role>().isCached == true)
            {
                //Debug.Log("NodeCheckChaserGotQuaffle: true");
                return NodeState.SUCCESS;
            }
            else
            {
                //Debug.Log("NodeCheckChaserGotQuaffle: false");
                return NodeState.FAILURE;
            }
        }
    }

    public class NodeCheckIsClosestToQuaffle : BaseNode
    {
        public NodeCheckIsClosestToQuaffle(AgentController actor_)
            : base(actor_)
        {
        }
        public override NodeState Process()
        {
            List<Transform> friendsChasers = actor.gameObject.GetComponent<Role>().friendChaser;
            Transform QuaffleTransform = GameManager.Instance.quaffle.transform;
            float distance = Vector3.Distance(QuaffleTransform.position, actor.transform.position);
            bool isClosest = true;
            for(int i =0; i < friendsChasers.Count; i++)
            {
                float curDistance = Vector3.Distance(QuaffleTransform.position, friendsChasers[i].position);
                if (curDistance < distance)
                {
                    isClosest = false;
                    //store the closest transform into the context
                    SetRootContext("Closest", friendsChasers[i]);
                }
            }

            if (isClosest)
                return NodeState.SUCCESS;
            return NodeState.FAILURE;
        }
    }

    public class NodeChaseQuaffle : BaseNode
    {
        public NodeChaseQuaffle(AgentController actor_)
          : base(actor_)
        {
        }
        public override NodeState Process()
        {
            Vector3 quafflePosition = GameManager.Instance.quaffle.transform.position;
            Vector3 desiredVector = (quafflePosition - actor.transform.position).normalized;
            //Debug.Log("NodeChaseQuaffle desired position:" + quafflePosition + " desired vector:" + desiredVector);

            (actor as NPCController).AddKinematicVector(desiredVector);
            state = NodeState.RUNNING;
            return state;
        }
    }

    public class NodeCheckCanTakeQuaffle : BaseNode
    {
        public NodeCheckCanTakeQuaffle(AgentController actor_)
        : base(actor_)
        {
        }
        public override NodeState Process()
        {
            if (actor.GetComponent<Role>().cachedQuaffle != null && actor.GetComponent<Role>().isCached == false
                && !actor.GetComponent<Role>().IsInDizzy())
                return NodeState.SUCCESS;
            else
                return NodeState.FAILURE;
        }
    }

    public class NodeTakeQuaffle : BaseNode
    {
        public NodeTakeQuaffle(AgentController actor_)
        : base(actor_)
        {
        }
        public override NodeState Process()
        {
            actor.GetComponent<Role>().TakeQuaffle();
            return NodeState.SUCCESS;
        }
    }

    public class NodeCheckInThrowArea : BaseNode
    {
        public NodeCheckInThrowArea(AgentController actor_)
         : base(actor_)
        {
        }
        public override NodeState Process()
        {
            List<Transform> targetTransforms = actor.GetComponent<CharacterSwitcher>().GetTeamTargets();
            //Debug.Log("current team actor:" + actor.GetComponent<TeamEntity>().MyTeam +  " The target transformation:" + targetTransforms[0].gameObject.tag);
            float throwRange = actor.GetComponent<Role>().throwRadius;
            float distance = Vector3.Distance(targetTransforms[0].position, actor.transform.position);
            if (distance > throwRange)
                return NodeState.FAILURE;
            else 
                return NodeState.SUCCESS;
        }
    }

    public class NodeThrowQuaffleToTarget : BaseNode
    {
        public NodeThrowQuaffleToTarget(AgentController actor_)
         : base(actor_)
        {
        }
        public override NodeState Process()
        {
            List<Transform> targetTransforms = actor.GetComponent<CharacterSwitcher>().GetTeamTargets();
            actor.GetComponent<Role>().PassQuaffle(targetTransforms[0]);
            return NodeState.SUCCESS;
        }
    }

    //Fly to the throw area
    public class NodeGoToThrowArea : BaseNode
    {
        public NodeGoToThrowArea(AgentController actor_)
          : base(actor_)
        {
        }
        public override NodeState Process()
        {
            List<Transform> targetTransforms = actor.GetComponent<CharacterSwitcher>().GetTeamTargets();
            Vector3 desiredVector = (targetTransforms[0].position - actor.transform.position).normalized;
            (actor as NPCController).AddKinematicVector(desiredVector);
            state = NodeState.RUNNING;
            return state;
        }
    }

    /*
    * The action nodes used in bludger
    */
    public class NodeCheckPerceptBludger : BaseNode
    {
        public NodeCheckPerceptBludger(AgentController actor_)
          : base(actor_)
        {
        }
        public override NodeState Process()
        {
            if(actor.GetComponent<Role>().focusBludger != null)
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }

    public class NodeCheckAvailableBeatBludger : BaseNode
    {
        public NodeCheckAvailableBeatBludger(AgentController actor_)
          : base(actor_)
        {
        }
        public override NodeState Process()
        {
            if (actor.GetComponent<Role>().IsBeatAvailable())
            {
                return NodeState.SUCCESS;
            }
            return NodeState.FAILURE;
        }
    }

    public class NodeChaseAvailableBludger : BaseNode
    {
        public NodeChaseAvailableBludger(AgentController actor_)
          : base(actor_)
        {
        }
        public override NodeState Process()
        {
            actor.GetComponent<AnimationController>().HitBallAnimation();
            Vector3 desiredVector = actor.GetComponent<Role>().focusBludger.transform.position - actor.transform.position;
            (actor as NPCController).AddKinematicVector(desiredVector.normalized);
            return NodeState.RUNNING;
        }
    }

    public class NodeBeateBludger : BaseNode
    {
        public NodeBeateBludger(AgentController actor_)
         : base(actor_)
        {
        }

        public Vector3 GetBeatTargetPosition()
        {
            QuaffleState curState = GameManager.Instance.g_quaffleState;
            //cached by other team, beat the bludge to the npc with quaffle
            if ((curState == QuaffleState.CachedByTeam1 && actor.gameObject.GetComponent<TeamEntity>().MyTeam == Team.Team_2) ||
                (curState == QuaffleState.CachedByTeam2 && actor.gameObject.GetComponent<TeamEntity>().MyTeam == Team.Team_1))
            {
                return GameManager.Instance.quaffle.GetComponent<Quaffle>().takenChaser.transform.position;
            }
            //otherwise beat the bludge to any chaser of the other team
            else
            {
                Team enemyTeam = actor.GetComponent<TeamEntity>().GetEnemyTeam();
                List<Transform> enemyChasers = TeamManager.GetChasersOfTeam(enemyTeam);
                //if there is no available enemy chaser, beat the ball into the other side of chasers
                if (enemyChasers.Count == 0)
                {
                    Team friendTeam = actor.GetComponent<TeamEntity>().MyTeam;
                    List<Transform> friendChaser = TeamManager.GetChasersOfTeam(friendTeam);
                    Vector3 gradientPos = Vector3.zero;
                    for(int i = 0; i < friendChaser.Count; i++)
                    {
                        gradientPos += friendChaser[i].position;
                    }
                    float strength = actor.GetComponent<Role>().beatStrength;
                    Vector3 targetPosition = actor.transform.position + (gradientPos - actor.transform.position).normalized * strength;
                    return targetPosition;
                }
                else
                {
                    int randomIndex = Random.Range(0, enemyChasers.Count);
                    return enemyChasers[randomIndex].position;
                }
            }
        }

        public override NodeState Process()
        {
            Vector3 position = GetBeatTargetPosition();
            actor.GetComponent<AnimationController>().ReleaseHitBallAnimation();
            actor.GetComponent<Role>().BeatBludger(position);
            return NodeState.SUCCESS;
        }
    }

    public class NodeFlyInCircleBeater : BaseNode
    {
        public NodeFlyInCircleBeater(AgentController actor_)
            : base(actor_)
        {
        }
        public override NodeState Process()
        {
            Team teamType = actor.GetComponent<TeamEntity>().MyTeam;
            List<Transform> teammates = TeamManager.GetChasersOfTeam(teamType);

            Vector3 centralOrigin = Vector3.zero;
            for(int i = 0; i < teammates.Count; ++i)
            {
                centralOrigin += teammates[i].position;
            }
            centralOrigin /= teammates.Count;

            //for test goal
            if (teammates.Count == 0)
            {
                centralOrigin = new Vector3(10, 8, 10);
            }

            float radius = actor.GetComponent<Role>().guardeRadius;
            Vector3 desiredVector = ActionUtils.CircleFlyDirection(centralOrigin, actor.transform.position, radius);
            (actor as NPCController).AddKinematicVector(desiredVector);

            state = NodeState.RUNNING;
            return state;
        }
    }


}
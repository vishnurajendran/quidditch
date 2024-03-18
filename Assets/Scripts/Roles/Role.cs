using AgentControllers;
using Gameplay;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Teams;
using UnityEngine;
using UnityEngine.UIElements;

public class Role : MonoBehaviour
{
    //Parameters for chaser
    [SerializeField] public float throwRadius = 30.0f;
    [SerializeField] public float attackCircleRadius = 15.0f;
    [SerializeField] public float guardeCircleRadius = 25.0f;
    [SerializeField] public GameObject cachedQuaffle = null;
    [SerializeField] public bool isCached = false;
    [SerializeField] public List<Transform> friendChaser = new List<Transform>();
    [SerializeField] public List<Transform> friendBeater = new List<Transform>();

    //Parameters for beater
    [SerializeField] public GameObject focusBludger = null;
    [SerializeField] public float perceptionRange = 100.0f;
    [SerializeField] public float beatRange = 8.0f;
    [SerializeField] public float guardeRadius = 35.0f;

    public Parabola throwIndicator;
    private GameObject target;

    private PlayerType playerType;

    private void PerceptBludger()
    {
        float minDistance = perceptionRange;
        for(int i =0;  i < GameManager.Instance.Bludges.Count; ++i)
        {
            //todo:optimize the AI logic in here, rightnow we ignore the bludger is in path
            if (!GameManager.Instance.Bludges[i].GetComponent<Bludger>().activeForAttack)
                continue;
            float curDistance = Vector3.Distance(GameManager.Instance.Bludges[i].transform.position, transform.position);
            if(curDistance < minDistance)
            {
                minDistance = curDistance;
                focusBludger = GameManager.Instance.Bludges[i];
            }
        }
        if (minDistance == perceptionRange)
            focusBludger = null;
    }

    private void InitialFriendsInformation()
    {
        List<Transform> friendsTransform = TeamManager.GetPlayersOfTeam(GetComponent<TeamEntity>().MyTeam);
        for(int i =0; i < friendsTransform.Count; i++)
        {
            if (friendsTransform[i] == transform) continue;
            PlayerType curFriendType = friendsTransform[i].GetComponent<TeamEntity>().MyPlayerType;
            if (curFriendType == PlayerType.Chaser)
                friendChaser.Add(friendsTransform[i]);
            else if (curFriendType == PlayerType.Beater)
                friendChaser.Add(friendsTransform[i]);
        }
    }

    public bool IsPlayer()
    {
        return GetComponent<AgentUserController>().enabled;
    }

    // Start is called before the first frame update
    void Start()
    {
        playerType = GetComponent<TeamEntity>().MyPlayerType;
        InitialFriendsInformation();
    }

    public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }

    public Vector3[] GetThrowPath(Vector3 startPoint, Vector3 endPoint, int resolution = 10)
    {
        float distance = (endPoint - startPoint).magnitude;
        var bezierControlPoint = (startPoint + endPoint) * 0.5f + (Vector3.up * distance * 0.3f);

        Vector3[] path = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            var t = (i + 1) / (float)resolution;//range in 0.0f to 1.0f
            path[i] = GetBezierPoint(t, startPoint, bezierControlPoint, endPoint);
        }
        return path;
    }

    public void PassQuaffle()
    {
        Transform selectedTarget = GetComponent<CharacterSwitcher>().selected;
        Debug.Log("PassQuaffle:" + (selectedTarget != null));
        if (selectedTarget != null)
        {
            PassQuaffle(selectedTarget);
        }
    }

    public void PassQuaffle(Transform target)
    {
        if(cachedQuaffle != null && isCached)
        {
            Vector3[] pathPoints = GetThrowPath(transform.position, target.position);
            cachedQuaffle.GetComponent<Quaffle>().SetPathPoints(pathPoints);
            cachedQuaffle = null;
            isCached = false;
        }
    }

    public bool IsBeatAvailable()
    {
        if(focusBludger != null)
        {
            float curDistance = Vector3.Distance(focusBludger.transform.position, this.transform.position);
            if (curDistance < beatRange)
            {
                return true;
            }
        }
        return false;
    }

    public void BeatBludger(Vector3 position)
    {
        if(focusBludger != null)
        {
            Vector3[] pathPoints = GetThrowPath(transform.position, position);
            focusBludger.GetComponent<Bludger>().SetPathPoints(pathPoints);
        }
    }

    //get the quaffle
    public void TakeQuaffle()
    {
        if (cachedQuaffle == null) return;
        isCached = true;
        cachedQuaffle.GetComponent<Quaffle>().takenChaser = gameObject;
        cachedQuaffle.GetComponent<Quaffle>().isCached = true;
    }

    // Update is called once per frame
    void Update()
    {
        //the npc logic
        PerceptBludger();

        if (!IsPlayer()) return;

        //the player logic
        if (playerType == PlayerType.Chaser)
        {
            if (Input.GetKeyUp(KeyCode.F))
            {
                //Debug.Log("Check F Up:");
                if (!isCached && cachedQuaffle != null)
                {
                    TakeQuaffle();
                }
                else if (isCached)
                {
                    PassQuaffle();
                }
            }
        }

        if(playerType == PlayerType.Beater)
        {
            
            if (Input.GetKeyUp(KeyCode.F))
            {
               //todo: player logic
            }
        }

    }
}

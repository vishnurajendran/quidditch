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
    [SerializeField] public GameObject cachedQuaffle = null;
    [SerializeField] public bool isCached = false;
    [SerializeField] public List<Transform> friendChaser = new List<Transform>();
    [SerializeField] public List<Transform> friendBeater = new List<Transform>();


    public Parabola throwIndicator;
    private GameObject target;

    private PlayerType playerType;

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
        if (!IsPlayer()) return;

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
    }
}

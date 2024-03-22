using AgentControllers;
using System.Collections;
using System.Collections.Generic;
using Teams;
using UnityEngine;

public class Bludger : MonoBehaviour
{
    public List<Vector3> pathPoints = new List<Vector3>();
    private int pathIndex = 0;
    public bool activeForAttack = true; //is ready for attack

    private void Start()
    {
       

    }

    private Transform GetClosestPlayer()
    {
        List<Transform> team1Transforms = TeamManager.GetPlayersOfTeam(Team.Team_1);
        List<Transform> team2Transforms = TeamManager.GetPlayersOfTeam(Team.Team_2);

        //Debug.Log("the bludger start function1:" + team1Transforms.Count);
        //Debug.Log("the bludger start function2:" + team2Transforms.Count);


        float minDistance = float.PositiveInfinity;
        Transform res = null;
        for(int i =0; i < team1Transforms.Count; ++i)
        {
            float curDistance = Vector3.Distance(team1Transforms[i].position, transform.position);
            if (curDistance < minDistance)
            {
                res = team1Transforms[i];
                minDistance = curDistance;
            }
        }
        for (int i = 0; i < team2Transforms.Count; ++i)
        {
            float curDistance = Vector3.Distance(team2Transforms[i].position, transform.position);
            if (curDistance < minDistance)
            {
                res = team2Transforms[i];
                minDistance = curDistance;
            }
        }
        return res;
    }


    public void SetPathPoints(Vector3[] _pathPoints)
    {
        activeForAttack = false;
        pathPoints.Add(transform.position);
        for (int i = 0; i < _pathPoints.Length; i++)
        {
            pathPoints.Add(_pathPoints[i]);
        }
        Debug.Log("Current Path Points:" + pathPoints.Count);
    }


    private void SetKinematicVector(Vector3 direction)
    {
        GetComponent<NPCController>().SetKinematicVector(direction.normalized);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("the path points:" + pathPoints.Length);
        if (pathPoints.Count != 0)
        {
            //Debug.Log("the path points:" + pathPoints.Count);
            for (int i = 1; i < pathPoints.Count; ++i)
            {
                Debug.DrawLine(pathPoints[i - 1], pathPoints[i], Color.red);
            }
            if (pathIndex >= pathPoints.Count)
            {
                pathPoints.Clear();
                pathIndex = 0;
                activeForAttack = true;
                return;
            }

            SetKinematicVector((pathPoints[pathIndex] - transform.position));
            if (Vector3.Distance(pathPoints[pathIndex], transform.position) < 8.0f)
            {
                pathIndex = (pathIndex + 1);
            }
        }
        else
        {
            Transform nearestPlayer = GetClosestPlayer();
            Debug.Log("the nearest player vector:" + (nearestPlayer.position - transform.position));
            SetKinematicVector((nearestPlayer.position - transform.position));
        }
    }
}

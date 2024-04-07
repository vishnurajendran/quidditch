using AgentControllers;
using Agents;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Teams;
using UI;
using UnityEngine;

public class GoldenSnich : MonoBehaviour
{
    public GameObject takenSeeker = null;
    public bool isCached = false;
    public GameObject[] pathWayPoints;
    public float wayPointsBuildDistance = 10.0f;
    public int index = 0;
    public float curWaitingTime = 0.0f; 
    public float waitTime = 3.0f;
    public float velocity = 300.0f;
    public float perceptionDistance = 80.0f;

    public List<Transform> seekers = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        velocity = GetComponent<Agent>().MoveSpeed();
    }

    public void Catch(GameObject obj)
    {
        takenSeeker = obj;
        isCached = true;
        GameManager.Instance.GoldenSnitchScored(takenSeeker.GetComponent<TeamEntity>().MyTeam);
    }

    public void UpdateVelocity(int curTimer, int halfSeconds)
    {
        float velocityRate = ((float)(curTimer % halfSeconds) / (float)halfSeconds);
        float baseVelocity = 0.3333f * velocity;
        float motiveVelocity = (velocity - baseVelocity) * velocityRate;
        GetComponent<Agent>().SetMoveSpeed(baseVelocity + motiveVelocity);
    }

    private void UpdateSeekers()
    {
        seekers.Clear();
        List<Transform> team1Seekers = TeamManager.GetSeekerOfTeam(Team.Team_1);
        List<Transform> team2Seekers = TeamManager.GetSeekerOfTeam(Team.Team_2);
        for(int i = 0; i < team1Seekers.Count; i++)
        {
            seekers.Add(team1Seekers[i]);
        }
        for (int i = 0; i < team2Seekers.Count; i++)
        {
            seekers.Add(team2Seekers[i]);
        }
    }

    public bool IsSeekerNearHere()
    {
        if(seekers.Count < 2)
        {
            UpdateSeekers();
        }

        //Debug.Log("Golden Snitch:" + seekers.Count);
        for(int i = 0; i < seekers.Count; ++i)
        {
            float distance = Vector3.Distance(seekers[i].position, this.transform.position);
            if (distance < perceptionDistance)
                return true;
        }
        return false;
    }

    public Vector3 GetEscapeVector()
    {
        List<float> distances = new List<float>();
        float totalDistance = 0.0f;
        for(int i =0; i < seekers.Count; ++i)
        {
            float distance = Vector3.Distance(seekers[i].position, transform.position);
            distances.Add(distance);
            totalDistance += distance;
        }

        Vector3 escapeDir = Vector3.zero;
        for(int i =0; i < seekers.Count; i++)
        {
            escapeDir += (transform.position - seekers[i].position).normalized * (totalDistance / distances[i]);
        }
        if(escapeDir.magnitude < 0.1f)
        {
            return new Vector3(0.0f, 1.0f, 0.0f);
        }
        return escapeDir.normalized;
    }


    // Update is called once per frame
    void Update()
    {
        //is taken by the seeker
        if(takenSeeker != null)
        {
            transform.position = takenSeeker.GetComponent<Role>().quaffleFollowPoint.transform.position;
            return;
        }

        //follow the path
        if(!IsSeekerNearHere())
        {
            Vector3 targetPosition = pathWayPoints[index].transform.position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            GetComponent<NPCController>().SetKinematicVector(direction);

            if (Vector3.Distance(transform.position, targetPosition) < 5.0f)
            {
                index = (index + 1) % pathWayPoints.Length;
            }
        }
        else //escape from the seekers
        {
            Vector3 desiredVec = GetEscapeVector();
            GetComponent<NPCController>().SetKinematicVector(desiredVec);
        }
    }
}

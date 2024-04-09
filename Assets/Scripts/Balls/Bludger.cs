using AgentControllers;
using Agents;
using System.Collections;
using System.Collections.Generic;
using Teams;
using UnityEngine;
using UnityEngine.UIElements;

public class Bludger : MonoBehaviour
{
    public bool activeForAttack = true; //is ready for attack
    public Vector3 beatDir = Vector3.zero;
    public float chaseColdTime = 10.0f;
    public float curColdTime = 0.0f;
    public GameObject previousBeater = null;

    public GameObject hitParticleEffect;

    public void Beat(GameObject beaterObj, Vector3 dirVec)
    {
        beatDir = dirVec;
        previousBeater = beaterObj;
        curColdTime = chaseColdTime;
        GameObject tmp = GameObject.Instantiate(hitParticleEffect, this.transform.position, this.transform.rotation);
        GameObject.Destroy(tmp, 3.0f);
    }

    //if the bludger hit someone, the bludger will ricochet 
    public void HitRicochet(Vector3 posittion)
    {
        beatDir = (this.transform.position - posittion).normalized;
        curColdTime = chaseColdTime;
        GetComponent<Agent>().ResetRigidSpeed(beatDir);
        GameObject tmp = GameObject.Instantiate(hitParticleEffect, this.transform.position, this.transform.rotation);
        GameObject.Destroy(tmp, 3.0f);
    }

    public bool IsThereHasNeedBeat(GameObject beaterNPC)
    {
        if(previousBeater != null && previousBeater.GetComponent<TeamEntity>().MyTeam == beaterNPC.GetComponent<TeamEntity>().MyTeam)
        {
            return false;
        }
        return true;

        /*
        if(previousBeater != null && previousBeater == beaterNPC) 
            return false;
        return true;
        */
    }
    private Transform GetClosestPlayer()
    {
        List<Transform> team1Transforms = TeamManager.GetPlayersOfTeam(Team.Team_1);
        List<Transform> team2Transforms = TeamManager.GetPlayersOfTeam(Team.Team_2);

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

    private void SetKinematicVector(Vector3 direction)
    {
        GetComponent<NPCController>().SetKinematicVector(direction.normalized);
    }

    // Update is called once per frame
    void Update()
    {
        if (curColdTime > 0.0f)
        {
            curColdTime -= Time.deltaTime;
            SetKinematicVector(beatDir);
        }
        else
        {
            if (previousBeater != null) previousBeater = null;
            Transform nearestPlayer = GetClosestPlayer();
            if(nearestPlayer != null)
            {
                SetKinematicVector((nearestPlayer.position - transform.position));
            }            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((other.tag == "Team1" || other.tag == "Team2") && 
            !other.transform.parent.parent.GetComponent<Role>().IsInDizzy())
        {
            Debug.Log("Hit the collider:" + other.name);
            other.transform.parent.parent.GetComponent<Role>().HitByBludger();
            HitRicochet(other.transform.position);
        }
    }


}

using System.Collections;
using System.Collections.Generic;
using Teams;
using UnityEngine;

public class UnityChanTeamIdentify : MonoBehaviour
{
    public GameObject team1sode;
    public GameObject team2sode;
    public GameObject team1shirt;
    public GameObject team2shirt;
    public GameObject[] team1Hair;
    public GameObject[] team2Hair;

    private void Start()
    {
        Team currentPlayerTeam = transform.parent.parent.parent.parent.GetComponent<TeamEntity>().MyTeam;
        SetTeamIdentify(currentPlayerTeam);
    }

    public void SetTeamIdentify(Team teamType)
    {
        if (teamType == Team.Team_1)
        {
            team1sode.SetActive(true);
            team1shirt.SetActive(true);
            team2sode.SetActive(false);
            team2shirt.SetActive(false);
            for (int i = 0; i < team1Hair.Length; ++i)
            {
                team1Hair[i].SetActive(true);
                team2Hair[i].SetActive(false);
            }
        }
        else
        {
            team1sode.SetActive(false);
            team1shirt.SetActive(false);
            team2sode.SetActive(true);
            team2shirt.SetActive(true);
            for (int i = 0; i < team1Hair.Length; ++i)
            {
                team1Hair[i].SetActive(false);
                team2Hair[i].SetActive(true);
            }
        }
    }
}

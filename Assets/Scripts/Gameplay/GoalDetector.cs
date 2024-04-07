using System;
using System.Collections;
using System.Collections.Generic;
using Teams;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] private Team _team;
    [SerializeField] public GameObject beatParticleEffect;

    private Team _otherTeam;
    public void SetTeam(Team team)
    {
        var enumValues = new List<Team>((Team[])Enum.GetValues(typeof(Team)));
        _team = team;
        enumValues.Remove(Team.None);
        enumValues.Remove(_team);
        _otherTeam = enumValues[0];
        
        transform.tag = (_team == Team.Team_1 ? "Team1Target" : "Team2Target");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Quaffle" 
            && (other.gameObject.GetComponent<Quaffle>().takenChaser == null ||
            other.gameObject.GetComponent<Quaffle>().takenChaser.GetComponent<TeamEntity>().MyTeam != _team))
        {
            
            GameManager.Instance.QuaffleScored(_otherTeam);
            other.gameObject.GetComponent<Quaffle>().ResetStatus();
            GameObject tmp = GameObject.Instantiate(beatParticleEffect, this.transform.position, this.transform.rotation);
            GameObject.Destroy(tmp, 2.0f);
            GameManager.Instance.GiveQuaffleToChaser(_team);
        }
    }
}

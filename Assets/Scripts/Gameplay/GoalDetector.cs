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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Quaffle" 
            && (other.gameObject.GetComponent<Quaffle>().takenChaser == null ||
            other.gameObject.GetComponent<Quaffle>().takenChaser.GetComponent<TeamEntity>().MyTeam != _team))
        {
            GameManager.Instance.QuaffleScored(_otherTeam);
            GameObject tmp = GameObject.Instantiate(beatParticleEffect, this.transform.position, this.transform.rotation);
            GameObject.Destroy(tmp, 2.0f);
        }
    }
}

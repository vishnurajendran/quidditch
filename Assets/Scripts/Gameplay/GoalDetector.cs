using System;
using System.Collections;
using System.Collections.Generic;
using Teams;
using UnityEngine;

public class GoalDetector : MonoBehaviour
{
    [SerializeField] private Team _team;

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
        if (other.name.ToLower().Contains("quaffle"))
        {
            GameManager.Instance.QuaffleScored(_otherTeam);
        }
    }
}

using System;
using UnityEngine;

namespace Teams
{
    public class TeamEntity : MonoBehaviour
    {
        [SerializeField] private Team _team;

        public Team MyTeam => _team;

        public void SetTeam(Team team)
        {
            TeamManager.RegisterToTeam(_team, transform);
            _team = team;
            TeamManager.RegisterToTeam(_team, transform);
        }
        
        private void Start()
        {
            TeamManager.RegisterToTeam(_team, transform);
        }

        private void OnDestroy()
        {
            TeamManager.RegisterToTeam(_team, transform);
        }
    }
}
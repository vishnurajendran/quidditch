using System;
using Gameplay;
using UnityEngine;

namespace Teams
{
    public class TeamEntity : MonoBehaviour
    {
        [SerializeField] private Team _team;
        [SerializeField] private PlayerType _playerType;
        
        public Team MyTeam => _team;
        public PlayerType MyPlayerType => _playerType;
        
        public void SetPlayerType(PlayerType playerType)
        {
            _playerType = playerType;
        }
        
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
using System;
using System.Collections.Generic;
using AgentControllers;
using UnityEngine;

namespace Teams
{
    public static class TeamManager
    {
        private static Dictionary<Team, List<Transform>> _playersByTeam;

        private static void TryInitPlayersDict()
        {
            if(_playersByTeam != null)
                return;

            _playersByTeam = new Dictionary<Team, List<Transform>>();
            foreach (var team in (Team[])Enum.GetValues(typeof(Team)))
            {
                _playersByTeam.Add(team, new List<Transform>());
            }
        }
        
        public static void RegisterToTeam(Team team, Transform transform)
        {
            TryInitPlayersDict();
            var list = _playersByTeam[team];
            if(!list.Contains(transform))
                list.Add(transform);
        }
        
        public static void DeRegisterFromTeam(Team team, Transform transform)
        {
            TryInitPlayersDict();
            var list = _playersByTeam[team];
            if(list.Contains(transform))
                list.Remove(transform);
        }

        public static List<Transform> GetPlayersOfTeam(Team team)
        {
            TryInitPlayersDict();
            return _playersByTeam[team];
        }
    }
}
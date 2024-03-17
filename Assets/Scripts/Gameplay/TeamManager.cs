using System;
using System.Collections.Generic;
using AgentControllers;
using UnityEngine;

namespace Teams
{
    public static class TeamManager
    {
        private static Dictionary<Team, List<Transform>> _playersByTeam;
        private static Dictionary<Team, List<Transform>> _targetsByTeam;

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

        private static void TryInitTargetsDict()
        {
            if (_targetsByTeam != null)
                return;

            _targetsByTeam = new Dictionary<Team, List<Transform>>();
            foreach (var team in (Team[])Enum.GetValues(typeof(Team)))
            {
                _targetsByTeam.Add(team, new List<Transform>());
            }

            GameObject[] team1targets = GameObject.FindGameObjectsWithTag("Team1Target");
            GameObject[] team2targets = GameObject.FindGameObjectsWithTag("Team2Target");
            //Debug.Log("TryInitTargetsDict team1 targets:" + team1targets.Length + "  team2 targets:" + team2targets.Length);
            for (int i = 0; i < team1targets.Length; i++)
                _targetsByTeam[Team.Team_1].Add(team1targets[i].transform);
            for(int i = 0;i < team2targets.Length; i++)
                _targetsByTeam[Team.Team_2].Add(team2targets[i].transform);
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

        public static List<Transform> GetTargetsOfTeam(Team team)
        {
            TryInitTargetsDict();
            var list = _targetsByTeam[team];
            return list;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using Teams;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace Gameplay
{
    public class SidesManager : SingletonBehaviour<SidesManager>
    {
        [FormerlySerializedAs("_sidePos_1")] [SerializeField] private Side _side1;
        [FormerlySerializedAs("_sidePos_2")] [SerializeField] private Side _side2;
        
        public void AssignTeams(Team _side1Team, bool side1HasHumanPlayer, Team _side2Team, bool side2HasHumanPlayer, PlayerType humanPlayerType)
        {
            _side1.SetTeam(_side1Team, side1HasHumanPlayer,humanPlayerType);
            _side2.SetTeam(_side2Team,side2HasHumanPlayer,humanPlayerType);
            SpawnPlayers();
        }

        public void SwapSides()
        {
            Side.Swap(_side1, _side2);
        }

        public void ResetPositions()
        {
            _side1.ResetPositions();
            _side2.ResetPositions();
        }
        
        private void SpawnPlayers()
        {
            _side1.SpawnPlayers();
            _side2.SpawnPlayers();
        }
    }
}

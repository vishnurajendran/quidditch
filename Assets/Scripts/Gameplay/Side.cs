using System.Collections.Generic;
using System.Linq;
using AgentControllers;
using Teams;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
    public class Side : MonoBehaviour
    {
        [SerializeField] private Team _team;
        [SerializeField] private List<Transform> _chaserPositions;
        [SerializeField] private List<Transform> _beaterPositions;
        [SerializeField] private Transform _keeperPosition;
        [SerializeField] private Transform _seekerPosition;
        [SerializeField] private Image _symbol;
        [SerializeField] private GameObject _playerPrefab;
        
        private List<Transform> _players;
        private bool _hasHumanPlayer=false;
        private PlayerType _humanPlayerType;
        
        public void SetTeam(Team _team, bool hasHumanPlayer, PlayerType humanPlayerType)
        {
            this._team = _team;
            this._hasHumanPlayer = hasHumanPlayer;
            if(_hasHumanPlayer)
                this._humanPlayerType = humanPlayerType;

            foreach (var goal in GetComponentsInChildren<GoalDetector>())
            {
                goal.SetTeam(_team);
            }
        }
        
        public void SpawnPlayers()
        {
            _players = new List<Transform>();
            //SpawnPlayer(PlayerType.Seeker, new List<Transform>(){_seekerPosition}, _humanPlayerType==PlayerType.Seeker);
            //SpawnPlayer(PlayerType.Keeper, new List<Transform>(){_keeperPosition}, _humanPlayerType==PlayerType.Keeper);
            SpawnPlayer(PlayerType.Chaser, _chaserPositions, _humanPlayerType==PlayerType.Chaser);
            SpawnPlayer(PlayerType.Beater, _beaterPositions, _humanPlayerType==PlayerType.Beater);
        }

        private void SpawnPlayer(PlayerType type, List<Transform> positions, bool hasHumanPlayer)
        {
            int humanPlayerId = hasHumanPlayer?Random.Range(0, positions.Count):-1;
            for (int i = 0; i < positions.Count; i++)
            {
                var go = Instantiate(_playerPrefab, positions[i].position, positions[i].rotation);
                go.name = $"{_team}_{type}_{i+1}";
                var te =  go.GetComponent<TeamEntity>();
                te.SetTeam(_team);
                te.SetPlayerType(type);
                te.UpdateRoleComponent();
                Debug.Log("Initiate Player:" + go.transform.position);
                if (i == humanPlayerId)
                {
                    go.GetComponent<CharacterSwitcher>().SwitchToPlayer(go.transform);
                }
                _players.Add(go.transform);
            }
        }

        
        public void ResetPositions()
        {
            var seeker = _players.SingleOrDefault(a => a.GetComponent<TeamEntity>().MyPlayerType == PlayerType.Seeker);
            var keeper = _players.SingleOrDefault(a => a.GetComponent<TeamEntity>().MyPlayerType == PlayerType.Keeper);
            var chasers = new List<Transform>();
            var beaters = new List<Transform>();
            foreach (var player in _players)
            {
                var te = player.GetComponent<TeamEntity>();
                if(te.MyPlayerType == PlayerType.Beater)
                    beaters.Add(player);
                else if(te.MyPlayerType == PlayerType.Chaser)
                    chasers.Add(player);
            }

            seeker.transform.position = _seekerPosition.position;
            seeker.transform.rotation = _seekerPosition.rotation;
            
            keeper.transform.position = _keeperPosition.position;
            keeper.transform.rotation = _keeperPosition.rotation;

            for (int i = 0; i < chasers.Count; i++)
            {
                chasers[i].transform.position = _chaserPositions[i].position;
                chasers[i].transform.rotation = _chaserPositions[i].rotation;
            }
            
            for (int i = 0; i < chasers.Count; i++)
            {
                chasers[i].transform.position = _chaserPositions[i].position;
                chasers[i].transform.rotation = _chaserPositions[i].rotation;
            }
        }
        
        //Swaps sides.
        public static void Swap(Side side1, Side side2)
        {
            var tempTeam = side2._team;
            var tempHasHumanPlayer = side2._hasHumanPlayer;
            var tempHasHumanPlayerType = side2._humanPlayerType;
            
            var tempPlayers = new List<Transform>(side2._players);
            
            side2._players = new List<Transform>(side1._players);
            side1._players = new List<Transform>(tempPlayers);
            
            side2.SetTeam(side1._team, side1._hasHumanPlayer, side1._humanPlayerType);
            side1.SetTeam(tempTeam, tempHasHumanPlayer, tempHasHumanPlayerType);
        }
    }
}
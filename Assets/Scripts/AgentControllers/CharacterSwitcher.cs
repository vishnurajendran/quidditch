using System.Collections.Generic;
using Teams;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AgentControllers
{
    [RequireComponent(typeof(AgentUserController))]
    public class CharacterSwitcher : MonoBehaviour
    {
        [SerializeField] private float _graceAngleMult=1;
        [SerializeField] private float _lockAngle;
        private List<CharacterSwitcher> characters;
        
        private AgentUserController _userController;
        private NPCController _npController;
        private List<Transform> potentialTargets;
        private Transform _cameraTrf;

        private int selectedIndex = 0;
        public Transform selected = null;
        
        private void Awake()
        {
            _cameraTrf = Camera.main.transform;
            potentialTargets = new List<Transform>();
            _userController = GetComponent<AgentUserController>();
            _npController = GetComponent<NPCController>();
        }

        public List<Transform> GetTeamPlayers()
        {
            return TeamManager.GetPlayersOfTeam(GetComponent<TeamEntity>().MyTeam); 
        }

        public List<Transform> GetTeamTargets()
        {
            return TeamManager.GetTargetsOfTeam(GetComponent<TeamEntity>().GetEnemyTeam());
        }

        public List<Transform> GetOtherTeamTargets()
        {
            Team actorTeamType = GetComponent<TeamEntity>().MyTeam;
            if(actorTeamType == Team.Team_1)
                return TeamManager.GetTargetsOfTeam(Team.Team_2);
            return TeamManager.GetTargetsOfTeam(Team.Team_1);
        }

        private void FixedUpdate()
        {
            if (!_userController.enabled)
                return;

            //clear any that is no longer visible
            var targetsToClear = new List<Transform>();
            foreach (var transform in potentialTargets)
            {
                if(!VisibleToCamera(transform))
                    targetsToClear.Add(transform);
            }

            if (targetsToClear.Count > 0)
            {
                potentialTargets.RemoveAll(a => targetsToClear.Contains(a));
                targetsToClear.Clear();
            }
            
            //Add any that is longer visible
            foreach (var player in GetTeamPlayers())
            {
                if(player == transform)
                    continue;

                if (VisibleToCamera(player))
                {
                    if(!potentialTargets.Contains(player))
                        potentialTargets.Add(player);
                }
            }

            //Also add the targets to the potential targets
            foreach(var target in GetTeamTargets())
            {
                if (VisibleToCamera(target))
                    if (!potentialTargets.Contains(target))
                        potentialTargets.Add(target);
            }

            //Debug.Log("The Team Targets:" + _myTeamTargets.Count);

            if (potentialTargets.Count <= 0)
            {
                selected =  null;
                GameUI.Instance.SetFollowTarget(null);
            }
            else
            {
                if (!potentialTargets.Contains(selected))
                    selected = potentialTargets[Random.Range(0, potentialTargets.Count)];
            }
            GameUI.Instance.SetFollowTarget(selected);
        }

        private void Update()
        {
            if (potentialTargets.Count <= 0)
                return;

            if (Input.mouseScrollDelta.y > 0)
            {
                selected = potentialTargets[(potentialTargets.IndexOf(selected) + 1) % potentialTargets.Count];
                GameUI.Instance.SetFollowTarget(selected);
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                var selectionIndex = potentialTargets.IndexOf(selected) - 1;
                if (selectionIndex < 0)
                    selectionIndex = potentialTargets.Count - 1;
                selected = potentialTargets[selectionIndex];
                GameUI.Instance.SetFollowTarget(selected);
            }

            if (Input.GetMouseButtonDown(1))
            {
                if(potentialTargets.Count > 0)
                {
                    selectedIndex = (selectedIndex + 1) % potentialTargets.Count;
                    //Debug.Log("click right mouse:" + potentialTargets.Count + " selected num:" + selectedIndex);
                    selected = potentialTargets[selectedIndex];
                }
            }
            
            if (Input.GetKeyUp(KeyCode.Q) && selected && selected.GetComponent<TeamEntity>())
            {
                SwitchToPlayer(selected);
            }
        }

        private bool VisibleToCamera(Transform player)
        {
            var angle = GetAngleFromCameraTo(player);
            return angle <= _lockAngle;
        }
        
        private float GetAngleFromCameraTo(Transform target)
        {
            var pos = target.position;
            return Vector3.Angle(_cameraTrf.forward, pos - transform.position);
        }
        
        public void SwitchToPlayer(Transform targetTransform)
        {
            Debug.Log($"Switching Player to {targetTransform.name}");
            var other = targetTransform.GetComponent<CharacterSwitcher>();
            other._npController.enabled = false;
            other._userController.enabled = true;
            other.name += " (Human)";

            if (transform != targetTransform)
            {
                _userController.enabled = false;
                _npController.enabled = true;
                name = name.Replace(" (Human)", "");
                selected = null;
                potentialTargets.Clear();
                GameUI.Instance.SetFollowTarget(null);
            }
        }
    }
}
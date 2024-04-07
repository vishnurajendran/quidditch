using System;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

namespace AgentControllers
{
    
    public class AgentUserController : AgentController
    {
        private Camera _camera;
        [SerializeField] private Transform camTarget;
        [SerializeField] private GameObject helpGUI;
        private void OnEnable()
        {
            helpGUI.SetActive(true);
            
            FindObjectOfType<CinemachineFreeLook>().Follow = camTarget;
            FindObjectOfType<CinemachineTargetGroup>().m_Targets[0].target = camTarget;
            
            GetComponent<BTBeater>().enabled = false;
            GetComponent<BTChaser>().enabled = false;
            GetComponent<BTSeeker>().enabled = false;
            GetComponent<BTKeeper>().enabled = false;
        }

        private void OnDisable()
        {
            helpGUI.SetActive(false);   
        }

        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
            GetComponent<NPCController>().enabled = false;
        }

        //apply dizzy statement to player state
        public bool CheckIsHitByBludger()
        {
            return GetComponent<Role>() && GetComponent<Role>().curColdDownInDizziness > 0.0f;
        }


        private void Update()
        {
            if(!GameManager.Instance.GameStarted)
                return;

            bool slowing = CheckIsHitByBludger();

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            float lift = Input.GetKey(KeyCode.LeftShift) ? 1 : Input.GetKey(KeyCode.LeftControl) ? -1 : 0;
            var boost = Input.GetKey(KeyCode.Space);

            var forward = _camera.transform.forward;
            var right = _camera.transform.right;
            var up = _camera.transform.up;
            
            forward.y = 0;
            right.y = 0;

            var vec = new Vector3(horizontal, 0, vertical);
            vec = Vector3.ClampMagnitude(vec, 1);
            lift *= vec.magnitude; 
            
            var dir = forward * vertical + right * horizontal + up * lift;
            
            _agent.Move(dir);
            _agent.SetGraphicRollDirection(horizontal);
            _agent.Boost(boost);
            _agent.Slow(slowing);
        }
    }
}
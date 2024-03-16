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

        private void OnEnable()
        {
            GameObject.FindObjectOfType<CinemachineFreeLook>().LookAt = camTarget;
            GameObject.FindObjectOfType<CinemachineFreeLook>().Follow = camTarget;
        }

        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
            GetComponent<NPCController>().enabled = false;
        }

        private void Update()
        {
            if (!GameManager.Instance.GameStarted)
            {
                _agent.Move(Vector3.zero);
                _agent.SetGraphicRollDirection(0);
                _agent.Boost(false);
                return;
            }

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
        }
    }
}
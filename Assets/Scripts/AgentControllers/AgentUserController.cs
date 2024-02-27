using System;
using Unity.VisualScripting;
using UnityEngine;

namespace AgentControllers
{
    public class AgentUserController : AgentController
    {
        [SerializeField] private float floorY;
        [SerializeField] private float ceilY;
        
        private Camera _camera;
        protected override void Start()
        {
            base.Start();
            _camera = Camera.main;
        }

        private void Update()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            var lift = Input.GetKey(KeyCode.LeftShift) ? 1 : Input.GetKey(KeyCode.LeftControl) ? -1 : 0;
            var boost = Input.GetKey(KeyCode.Space);

            var forward = _camera.transform.forward;
            var right = _camera.transform.right;
            var up = _camera.transform.up;

            if (horizontal == 0 && vertical == 0)
                lift = 0;
            else if (Mathf.Approximately(transform.position.y, floorY) && lift < 0)
                lift = 0;
            else if (Mathf.Approximately(transform.position.y, ceilY) && lift > 0)
                lift = 0;
            
            forward.y = 0;
            right.y = 0;
            
            var dir = forward * vertical + right * horizontal + up * lift;
            
            _agent.Move(dir);
            _agent.SetGraphicRollDirection(horizontal);
            _agent.Boost(boost);
        }
    }
}
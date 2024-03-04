using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Agents
{
    [RequireComponent(typeof(Rigidbody))]
    public class Agent : MonoBehaviour
    {
        [SerializeField] private float floorY;
        [SerializeField] private float ceilY;
        
        [SerializeField] private Transform _graphicToRoll;
        [SerializeField] private float _graphicToRollLerpMult=3;
        [SerializeField] private float _maxRoll;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _boostMultiplier = 1.5f;
        [SerializeField] private float _lookSpeed;
        
        private Rigidbody _rb;
        private Vector3 _inputVec;
        private float _graphicRollDir;
        private bool _boosting;
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            var maxSpeed = _moveSpeed * (_boosting ? _boostMultiplier : 1);
            _rb.velocity += _inputVec * (maxSpeed * Time.fixedDeltaTime);
            
            if(_rb.velocity == Physics.gravity)
                _rb.velocity += -Physics.gravity * Time.fixedDeltaTime;
            
            _rb.velocity = Vector3.ClampMagnitude(_rb.velocity, maxSpeed);
            if ((_rb.position.y <= floorY && _rb.velocity.y < 0) || (_rb.position.y >= ceilY && _rb.velocity.y > 0))
            {
                _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
                _rb.position = new Vector3(_rb.position.x, Mathf.Clamp(_rb.position.y, floorY, ceilY), _rb.position.z);
            }
        }

        private void Update()
        {
            if (_inputVec != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(_rb.velocity.normalized, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _lookSpeed);
            }

            var roll = Vector3.zero;
            if (_graphicRollDir > 0f)
                roll.z = -_maxRoll;
            else if (_graphicRollDir < 0f)
                roll.z = _maxRoll;
            
            var targetRot = Quaternion.Euler(roll);
            _graphicToRoll.transform.localRotation = Quaternion.Lerp( _graphicToRoll.transform.localRotation, targetRot,
                Time.deltaTime*_graphicToRollLerpMult);
        }

        public void Move(Vector3 move)
        {
            _inputVec = move;
        }

        public void Boost(bool boost)
        {
            _boosting = boost;
        }
        
        public void SetGraphicRollDirection(float dir)
        {
            _graphicRollDir = dir;
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Utils
{
    public class SpatialEffectController : MonoBehaviour
    {
        [SerializeField] 
        private AudioSource _src;
        
        [FormerlySerializedAs("_system")]
        [SerializeField] private ParticleSystem _particleSystem;
        
        [SerializeField] 
        private float maxSpeed;
        
        [SerializeField]
        private AgentControllers.AgentUserController _userController;
        
        
        private Rigidbody _rb;
        
        
        private void Start()
        {
            _rb = GetComponentInParent<Rigidbody>();
            _particleSystem = Camera.main.GetComponentInChildren<ParticleSystem>();
        }

        private void Update()
        {
            var speed = _rb.velocity.magnitude;
            
            if (_userController != null && _userController.enabled)
            {
                Debug.Log(speed);
                var main = _particleSystem.main;
                main.maxParticles = (int)(100 * speed / maxSpeed);
            }

            _src.volume = speed / maxSpeed;
        }
    }
}
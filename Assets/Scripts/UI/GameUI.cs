using UnityEngine;
using Utils;

namespace UI
{
    public class GameUI : SingletonBehaviour<GameUI>
    {
        [SerializeField] private GameObject _selector;
        [SerializeField] private Vector3 _followOffset = new Vector3(0,2,0);
        private Transform _followTarget;
        private Camera _camera;

        private static GameUI _instance;
        
        private void Start()
        {
            _camera = Camera.main;
        }

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
            _selector.SetActive(target != null);
        }
        
        private void Update()
        {
            if (!_followTarget)
                return;

            var uiPos = _camera.WorldToScreenPoint(_followTarget.position + _followOffset);
            _selector.transform.position = uiPos;
        }
    }
}
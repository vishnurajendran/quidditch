using System.Collections;
using Teams;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace UI
{
    public class GameUI : SingletonBehaviour<GameUI>
    {
        [SerializeField] private GameObject _selector;
        [SerializeField] private GameObject _quaffle;
        [SerializeField] private Vector3 _followOffset = new Vector3(0,2,0);
        [SerializeField] private Transform _quaffleTarget;

        [FormerlySerializedAs("_coutdownBG")]
        [Header("Countdown")]
        [SerializeField] private GameObject _zoomMsgBG;
        [FormerlySerializedAs("_countdownText")] [SerializeField] private TMP_Text _zoomMsgText;
        [FormerlySerializedAs("cntMinScale")] [SerializeField] private float zoomMsgMinScale = 0.25f;
        [FormerlySerializedAs("cntMaxScale")] [SerializeField] private float zoomMsgMaxScale = 3f;
        
        private Transform _followTarget;

        private Camera _camera;

        private static GameUI _instance;
        
        private void Start()
        {
            _camera = Camera.main;
            
        }

        /*
        private bool VisibleToCamera(Transform player)
        {
            var angle = GetAngleFromCameraTo(player);
            return angle <= 60;
        }

        private float GetAngleFromCameraTo(Transform target)
        {
            var pos = target.position;
            return Vector3.Angle(Camera.main.transform.forward, pos - transform.position);
        }
        */

        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
            _selector.SetActive(target != null);
            if (_selector.activeSelf)
            {
                if(target.tag == "Team1Target" || target.tag == "Team2Target")
                {
                    _selector.GetComponentInChildren<TMPro.TMP_Text>().text = target.tag;
                }
                else
                {
                    _selector.GetComponentInChildren<TMPro.TMP_Text>().text =
                    target.GetComponent<TeamEntity>().MyPlayerType.ToString();
                }
            }
        }
        
        private void Update()
        {
            /*
            if (VisibleToCamera(_quaffleTarget))
            {
                _quaffle.SetActive(true);
                var quafflePos = _camera.WorldToScreenPoint(_quaffleTarget.position + _followOffset);
                _quaffle.transform.position = quafflePos;
                Debug.Log("UI indicator: " + quafflePos);
            }
            else
            {
                _quaffle.SetActive(false);
            }
            */

            if (!_followTarget)
                return;

            var uiPos = _camera.WorldToScreenPoint(_followTarget.position + _followOffset);
            _selector.transform.position = uiPos;
        }

        public void ShowZoomingMessage(bool show, string msg, float time)
        {
            _zoomMsgBG.SetActive(show);
            _zoomMsgText.text = msg;
            StartCoroutine(ZoomMessageRoutine(time));
        }

        IEnumerator ZoomMessageRoutine(float dur)
        {
            float timeStep = 0;
            float startSize = zoomMsgMinScale;
            float endSize = zoomMsgMaxScale;
            while (timeStep <= 1)
            {
                timeStep += (Time.deltaTime / dur);
                _zoomMsgText.transform.localScale = Vector3.one * Mathf.Lerp(startSize, endSize, timeStep);
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
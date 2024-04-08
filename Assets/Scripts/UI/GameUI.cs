using System;
using System.Collections;
using Gameplay;
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
        [SerializeField] private TMPro.TMP_Text teamTextMessage;
        [SerializeField] private GameObject teamScoreObj;

        [SerializeField] private TMP_Text currentPlayerTypeUI;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameObject gameOverUI;
        [SerializeField] private GameObject winObj;
        [SerializeField] private GameObject loseObj;
        [SerializeField] private GameObject drawObj;
        [SerializeField] private Transform scoreParent;
        [SerializeField] private GameObject winScoreText;
        [SerializeField] private GameObject loseScoreText;
        
        private Transform _followTarget;
        private Camera _camera;
        private Coroutine scoreRoutine;

        private static GameUI _instance;

        private void Awake()
        {
            GameManager.Instance.OnTeamsAssigned += () =>
            {
                teamTextMessage.text = $"You are Team {(GameManager.Instance.PlayerTeam == Team.Team_1 ? 1:2)}";
            };
        }

        private void Start()
        {
            _camera = Camera.main;
            GameManager.Instance.OnGameOver += OnGameOver;
        }

        private void OnDestroy()
        {
            if(GameManager.Instance == null)
                return;
            
            GameManager.Instance.OnGameOver -= OnGameOver;
        }

        private void OnGameOver()
        {
            var score1 = scoreManager.Team1Score;
            var score2 = scoreManager.Team2Score;

            var team1ScoreTextRef = loseScoreText;
            var team2ScoreTextRef = loseScoreText;
            
            gameOverUI.SetActive(true);
            if (score1 > score2)
            {
                //team 1 wins
                bool win = GameManager.Instance.PlayerTeam == Team.Team_1;
                ShowGameOverBanner(win);
                team1ScoreTextRef = winScoreText;
            }
            else if (score2 > score1)
            {
                bool win = GameManager.Instance.PlayerTeam == Team.Team_2;
                ShowGameOverBanner(win);
                team2ScoreTextRef = winScoreText;
            }
            else
            {
                ShowGameDraw();
            }

            ShowScoreText(team1ScoreTextRef, score1, team2ScoreTextRef, score2);
        }

        private void ShowScoreText(GameObject score1TxtObj, int score1, GameObject score2TxtObj, int score2)
        {
            var txt = Instantiate(score1TxtObj, scoreParent);
            txt.SetActive(true);
            txt.GetComponent<TMP_Text>().text = $"Team 1:  {score1}";
            
            var txt2 = Instantiate(score2TxtObj, scoreParent);
            txt2.SetActive(true);
            txt2.GetComponent<TMP_Text>().text = $"Team 2:  {score2}";
        }
        
        private void ShowGameOverBanner(bool win)
        {
            if(win)
                winObj.SetActive(true);
            else
            {
                loseObj.SetActive(true);
            }
            
            if(win)
                AudioManager.Instance.PlayWinVO();
            else
                AudioManager.Instance.PlayLoseVO();
        }
        
        private void ShowGameDraw()
        {
            drawObj.SetActive(true);
        }
        
        public void SetFollowTarget(Transform target)
        {
            _followTarget = target;
            _selector.SetActive(target != null);
            if (_selector.activeSelf)
            {
                if(target.tag == "Team1Target" || target.tag == "Team2Target")
                {
                    _selector.GetComponentsInChildren<TMPro.TMP_Text>()[1].text = target.tag;
                }
                else
                {
                    _selector.GetComponentsInChildren<TMPro.TMP_Text>()[1].text =
                    target.GetComponent<TeamEntity>().MyPlayerType.ToString();
                }
            }
        }
        
        private void Update()
        {
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

        public void TeamScored(Team team)
        {
            if(scoreRoutine != null)
                StopCoroutine(scoreRoutine);
            var tmpText = teamScoreObj.GetComponentInChildren<TMP_Text>();
            tmpText.text = $"TEAM {(team == Team.Team_1 ? 1 : 2)} SCORED!!";
            scoreRoutine = StartCoroutine(TeamScoredRoutine());
        }

        IEnumerator TeamScoredRoutine()
        {
            var cg = teamScoreObj.GetComponent<CanvasGroup>();
            cg.alpha = 0;
            teamScoreObj.transform.localScale = Vector3.one * 0.15f;
            Vector3 smallScale = Vector3.one * 0.15f;
            Vector3 maxScale = Vector3.one * 1.15f;
            float timeStep = 0;
            while (timeStep <= 1)
            {
                timeStep += Time.deltaTime / 0.25f;
                teamScoreObj.transform.localScale = Vector3.Lerp(smallScale, maxScale, timeStep);
                cg.alpha = Mathf.Lerp(0, 1, timeStep);
                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(1);
            timeStep = 0;
            while (timeStep <= 1)
            {
                timeStep += Time.deltaTime / 0.25f;
                teamScoreObj.transform.localScale = Vector3.Lerp(maxScale, smallScale, timeStep);
                cg.alpha = Mathf.Lerp(1, 0, timeStep);
                yield return new WaitForEndOfFrame();
            }
        }

        public void SetCurrenType(PlayerType type)
        {
            currentPlayerTypeUI.text = type.ToString();
        }
    }
}
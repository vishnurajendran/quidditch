using System;
using Teams;
using UnityEngine;

namespace UI
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField, TextArea] private string scoreFormat = "TEAM {0}\n\n<size=60>{1}</size>";
        [SerializeField] private TMPro.TMP_Text _team1ScoreText;
        [SerializeField] private TMPro.TMP_Text _team2ScoreText;
        [SerializeField] private TMPro.TMP_Text _timerText;
        [SerializeField] private int _quaffleScoreIncrement = 10;
        
        private int _team1Score=0;
        private int _team2Score=0;

        private void Awake()
        {
            UpdateScores(Team.Team_1);
            UpdateScores(Team.Team_2);

            GameManager.Instance.OnQuaffleScored += OnQuaffleScored;
            GameManager.Instance.OnTimerUpdate += OnTimerUpdated;
        }

        private void OnQuaffleScored(Team team)
        {
            if (team == Team.Team_1)
                _team1Score += _quaffleScoreIncrement;
            else if (team == Team.Team_2)
                _team2Score += _quaffleScoreIncrement;

            UpdateScores(team);
        }

        private void OnTimerUpdated(TimeSpan timeSpan)
        {
            _timerText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        
        private void UpdateScores(Team team)
        {
            if (team == Team.Team_1)
                _team1ScoreText.text = String.Format(scoreFormat, 1, _team1Score);
            else if (team == Team.Team_2)
                _team2ScoreText.text = String.Format(scoreFormat, 2, _team2Score);
        }
        
    }
}
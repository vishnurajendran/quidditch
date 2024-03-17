using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Teams;
using UI;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

public enum QuaffleState
{
    Space,
    CachedByTeam1,
    CachedByTeam2,
}

public class GameManager : SingletonBehaviour<GameManager>
{
    [SerializeField]
    private PlayerType _playerStartType = PlayerType.Seeker;
    [SerializeField]
    private int gameTimeMinutes = 8;
    [SerializeField]
    private int gameStartCountdown = 3;

    [Header("Testing only"),SerializeField]
    private float _gameTimeScale = 2;

    public QuaffleState g_quaffleState = QuaffleState.Space;
    public GameObject quaffle = null;


    private Team _playerTeam;
    private float timerSeconds;
    
    public Action<Team> OnQuaffleScored;
    public Action<TimeSpan> OnTimerUpdate;
    
    // Indicates if Game Started;
    public bool GameStarted { get; private set; }

    private void Start()
    {
        Time.timeScale = _gameTimeScale;
        StartGame();

        quaffle = GameObject.FindGameObjectWithTag("Quaffle");
    }

    private void Update()
    {
        CheckQuaffleState();
    }

    public void CheckQuaffleState()
    {
        if (quaffle != null)
        {
            if (quaffle.GetComponent<Quaffle>().isCached)
            {
                if (quaffle.GetComponent<Quaffle>().takenChaser.GetComponent<TeamEntity>().MyTeam == Team.Team_1)
                    g_quaffleState = QuaffleState.CachedByTeam1;
                else
                    g_quaffleState = QuaffleState.CachedByTeam2;
            }
            else
                g_quaffleState = QuaffleState.Space;
        }
    }

    public void StartGame()
    {
        timerSeconds = gameTimeMinutes * 60;
        var enumValues = new List<Team>((Team[])Enum.GetValues(typeof(Team)));
        enumValues.Remove(0);
        _playerTeam = enumValues[Random.Range(0, enumValues.Count)];
        
        var side1team = enumValues[Random.Range(0, enumValues.Count)];
        enumValues.Remove(side1team);
        var side2team = enumValues[0];
        
        /*
        SidesManager.Instance.AssignTeams(side1team,side1team==_playerTeam, 
            side2team, side2team == _playerTeam, _playerStartType);
       */

        StartCoroutine(StartGameBeginCountdown());
        StartCoroutine(GameTimer());
    }

    private IEnumerator StartGameBeginCountdown()
    { 
        int countdown = gameStartCountdown;
        yield return new WaitForSeconds(1/Time.timeScale);
        while (countdown > 0)
        {
            GameUI.Instance.ShowZoomingMessage(true, countdown.ToString(), 0.75f/Time.timeScale);
            yield return new WaitForSeconds(1/Time.timeScale);
            countdown -= 1;
        }
        GameUI.Instance.ShowZoomingMessage(true, "START!", 0.75f/Time.timeScale);
        yield return new WaitForSeconds(1/Time.timeScale);
        GameUI.Instance.ShowZoomingMessage(false, "", 0);
        GameStarted = true;
    }
    
    private IEnumerator GameTimer()
    {
        while (true)
        {
            while (GameStarted)
            {
                if(timerSeconds == ((gameTimeMinutes*60)/2))
                {
                    yield return StartCoroutine(HalfTime());
                }
                
                timerSeconds -= 1;
                OnTimerUpdate?.Invoke(TimeSpan.FromSeconds(timerSeconds));
                yield return new WaitForSeconds(1/Time.timeScale);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator HalfTime()
    {
        GameStarted = false;
        yield return new WaitForSeconds(1/Time.timeScale);
        GameUI.Instance.ShowZoomingMessage(true, "HALF TIME", 0.75f/Time.timeScale);
        yield return new WaitForSeconds(1.5f/Time.timeScale);
        GameUI.Instance.ShowZoomingMessage(true, "SIDE CHANGE", 0.75f/Time.timeScale);
        yield return new WaitForSeconds(1);
        SidesManager.Instance.SwapSides();
        SidesManager.Instance.ResetPositions();
        yield return StartCoroutine(StartGameBeginCountdown());
    }

    public void QuaffleScored(Team team)
    {
        OnQuaffleScored?.Invoke(team);
    }
}

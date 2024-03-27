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
    [SerializeField]
    private Vector3 quaffleResetPosition = new Vector3(0, 100.0f, 0);
    [SerializeField]
    private Vector3 minSpacePoint = new Vector3(-100, 0, -100);
    [SerializeField]
    private Vector3 maxSpacePoint = new Vector3(100, 300, 100);
    [SerializeField]
    private float forceDistance = 20.0f;

   


    [Header("Testing only"),SerializeField]
    private float _gameTimeScale = 2;


    [Header("Testing only"), SerializeField]
    private bool _gameEnableSides = true;


    public QuaffleState g_quaffleState = QuaffleState.Space;
    public GameObject quaffle = null;
    public List<GameObject> Bludges = new List<GameObject>();



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
        GameObject[] bludgers = GameObject.FindGameObjectsWithTag("Bludger");
        for(int i =0; i < bludgers.Length; i++)
        {
            Bludges.Add(bludgers[i]);
        }
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
        
        if(_gameEnableSides)
        {
            SidesManager.Instance.AssignTeams(side1team, side1team == _playerTeam,
                    side2team, side2team == _playerTeam, _playerStartType);
        }

        StartCoroutine(StartGameBeginCountdown());
        StartCoroutine(GameTimer());
    }

    public Vector3 GetMaxSpacePoint()
    {
        return maxSpacePoint;
    }

    public Vector3 GetMaxSpacePointTakenForce()
    {
        return maxSpacePoint - new Vector3(forceDistance, forceDistance, forceDistance);
    }

    public float GetForceDistance()
    {
        return forceDistance;
    }

    public Vector3 GetMinSpacePoint()
    {
        return minSpacePoint;
    }

    public Vector3 GetMinSpacePointTakenForce()
    {
        return minSpacePoint + new Vector3(forceDistance, forceDistance, forceDistance);
    }

    public void ResetQuafflePosition()
    {
        quaffle.transform.position = quaffleResetPosition;
        quaffle.GetComponent<Quaffle>().ResetStatus();
    }

    public Vector3 GetQuaffleResetPosition() { return quaffleResetPosition; }

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
        ResetQuafflePosition();
    }
}

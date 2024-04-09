using AgentControllers;
using Gameplay;
using System.Collections.Generic;
using Cinemachine;
using Teams;
using UI;
using Unity.VisualScripting;
using UnityEngine;

public class Role : MonoBehaviour
{
    private const float MIN_DIST_FOR_GOAL = 120;
    
    //Parameters for chaser
    [SerializeField] public float throwRadius = 30.0f;
    [SerializeField] public float attackCircleRadius = 15.0f;
    [SerializeField] public float guardeCircleRadius = 25.0f;
    [SerializeField] public float escapeBuldgerRadius = 35.0f;
    [SerializeField] public GameObject cachedQuaffle = null;
    [SerializeField] public GameObject quaffleFollowPoint = null;
    [SerializeField] public bool isCached = false;
    [SerializeField] public List<Transform> friendChaser = new List<Transform>();
    [SerializeField] public List<Transform> friendBeater = new List<Transform>();

    //Parameters for beater
    [SerializeField] public GameObject focusBludger = null;
    [SerializeField] public float perceptionRange = 100.0f;
    [SerializeField] public float beatRange = 8.0f;
    [SerializeField] public float guardeRadius = 35.0f;
    [SerializeField] public float beatStrength = 30.0f;

    //Parameters for seeker
    [SerializeField] public GameObject cachedGoldenSnitch = null;
    [SerializeField] public bool isCachedGoldenSnich = false;

    //Parameters for keeper
    [SerializeField] public float validRadius = 100.0f;
    [SerializeField] public float rotateRadius = 40.0f;
    [SerializeField] public float passDistance = 60.0f;
    [SerializeField] public float perceptionRadius = 200.0f;
    [SerializeField] public Transform focusChaser = null;

    [SerializeField] private Transform targetGUI;
    [SerializeField] private Transform[] bludgerTargetIdcGUI;
    
    //particle effect
    public GameObject dizzyParticleEffect;
    public GameObject takeBallParticleEffect;
    public GameObject beatBallParticleEffect;

    public float curColdDownInDizziness = 0.0f;
    public float dissinessColdDownTime = 5.0f;
    public int currentTargetIndex = 0;


    public Parabola throwIndicator;
    private GameObject target;

    private PlayerType playerType;

    private Transform selectedTarget;

    public void HitByBludger()
    {
        curColdDownInDizziness = dissinessColdDownTime;
        isCached = false;
        if(cachedQuaffle != null)
        {
            cachedQuaffle.GetComponent<Quaffle>().StopStatus();
            cachedQuaffle = null;
            GetComponent<AnimationController>().NormalState();
        }

        if(IsPlayer())
            AudioManager.Instance.HitByBludger();
    }

    public bool IsInDizzy()
    {
        return curColdDownInDizziness > 0.0f;
    }

    private void PerceptChaserWithQuaffle()
    {
        if (GetComponent<TeamEntity>().MyPlayerType != PlayerType.Keeper) return;
        GameObject takenChaser = GameManager.Instance.quaffle.GetComponent<Quaffle>().takenChaser;
        if (takenChaser == null)
        {
            focusChaser = null;
            return;
        }
           
        if (takenChaser.GetComponent<TeamEntity>().MyTeam == GetComponent<TeamEntity>().MyTeam)
        {
            focusChaser = null;
            return;
        }

        if(Vector3.Distance(takenChaser.transform.position, transform.position) < perceptionRadius)
            focusChaser = takenChaser.transform;
        else
            focusChaser = null;
    }

    private void PerceptBludger()
    {
        focusBludger = null;
        float minDistance = perceptionRange;
        for(int i =0;  i < GameManager.Instance.Bludges.Count; ++i)
        {
            //todo:optimize the AI logic in here, rightnow we ignore the bludger is in path
            if (!GameManager.Instance.Bludges[i].GetComponent<Bludger>().IsThereHasNeedBeat(gameObject))
                continue;
            float curDistance = Vector3.Distance(GameManager.Instance.Bludges[i].transform.position, transform.position);
            if(curDistance < minDistance)
            {
                minDistance = curDistance;
                focusBludger = GameManager.Instance.Bludges[i];
            }
        }
    }

    private void InitialFriendsInformation()
    {
        List<Transform> friendsTransform = TeamManager.GetPlayersOfTeam(GetComponent<TeamEntity>().MyTeam);
        for(int i =0; i < friendsTransform.Count; i++)
        {
            if (friendsTransform[i] == transform) continue;
            PlayerType curFriendType = friendsTransform[i].GetComponent<TeamEntity>().MyPlayerType;
            if (curFriendType == PlayerType.Chaser)
                friendChaser.Add(friendsTransform[i]);
            else if (curFriendType == PlayerType.Beater)
                friendChaser.Add(friendsTransform[i]);
        }
    }

    public bool IsPlayer()
    {
        return GetComponent<AgentUserController>().enabled;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnHalfTime += OnGameHalfTime;
        playerType = GetComponent<TeamEntity>().MyPlayerType;
        InitialFriendsInformation();
        GetComponent<AnimationController>().InitRoles(playerType);
    }

    private void OnGameHalfTime()
    {
        if (isCached)
        {
            cachedQuaffle = null;
            isCached = false;
            GetComponent<AnimationController>().NormalState();
            GameManager.Instance.ResetQuafflePosition();
        }
    }
    
    public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
    {
        return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
    }

    public Vector3[] GetThrowPath(Vector3 startPoint, Vector3 endPoint, int resolution = 10)
    {
        float distance = (endPoint - startPoint).magnitude;
        var bezierControlPoint = (startPoint + endPoint) * 0.5f + (Vector3.up * (distance * 0.3f));

        Vector3[] path = new Vector3[resolution];
        for (int i = 0; i < resolution; i++)
        {
            var t = (i + 1) / (float)resolution;//range in 0.0f to 1.0f
            path[i] = GetBezierPoint(t, startPoint, bezierControlPoint, endPoint);
        }
        return path;
    }

    public void PassQuaffleByAnimation()
    {
        if (cachedQuaffle != null && isCached)
        {
            selectedTarget = GetComponent<CharacterSwitcher>().selected;
            GetComponent<AnimationController>().ThrowBall();
        }
    }

    public void PassQuaffle()
    {
        Debug.Log("PassQuaffle:" + (selectedTarget != null));

        if(selectedTarget == null)
            return;
        
        bool isTarget = selectedTarget.GetComponent<GoalDetector>() == null;
        //added an additional check for distance here
        if (selectedTarget != null && Vector3.Distance(transform.position, selectedTarget.position) <= MIN_DIST_FOR_GOAL)
        {
            PassQuaffle(selectedTarget, isTarget);
        }
    }

    public void PassQuaffle(Transform target, bool isTarget = false)
    {
        PassQuaffle(target.position, isTarget);
    }

    public void PassQuaffle(Vector3 position, bool isTarget = false)
    {
        if (cachedQuaffle != null && isCached)
        {
            Vector3[] pathPoints = GetThrowPath(transform.position, position);
            cachedQuaffle.GetComponent<Quaffle>().SetPathPoints(pathPoints, gameObject, isTarget);
            cachedQuaffle = null;
            isCached = false;
        }
    }

    public bool IsBeatAvailable()
    {
        if(focusBludger != null)
        {
            float curDistance = Vector3.Distance(focusBludger.transform.position, this.transform.position);
            if (curDistance < beatRange)
            {
                return true;
            }
        }
        return false;
    }

    public void BeatBludger(Vector3 position)
    {
        if(focusBludger != null)
        {
            Vector3 dirVector = (position - transform.position).normalized;
            focusBludger.GetComponent<Bludger>().Beat(gameObject, dirVector);
            GameObject tmpObj = GameObject.Instantiate(beatBallParticleEffect, focusBludger.transform.position, new Quaternion());
            GameObject.Destroy(tmpObj, 2.0f);
        }
    }

    public void ActiveBeatBludger()
    {
        if(focusBludger != null)
        {
            //check the current beat range
            if (IsBeatAvailable())
            {
                //if selected an enemy, then beat the ball to the enemy
                if(GetComponent<CharacterSwitcher>().selected != null && GetComponent<CharacterSwitcher>().selected.GetComponent<TeamEntity>() &&
                    GetComponent<CharacterSwitcher>().selected.GetComponent<TeamEntity>().MyTeam != GetComponent<TeamEntity>().MyTeam)
                {
                    Vector3 target = GetComponent<CharacterSwitcher>().selected.position;
                    Vector3 dirVector = (target - transform.position).normalized;
                    focusBludger.GetComponent<Bludger>().Beat(gameObject, dirVector);
                }
                else
                {
                    Vector3 dirVector = (focusBludger.transform.position - transform.position).normalized;
                    focusBludger.GetComponent<Bludger>().Beat(gameObject, dirVector);
                }
            }
        }
    }

    //get the quaffle
    public void TakeQuaffle()
    {
        if (cachedQuaffle == null) return;
        if (cachedQuaffle.GetComponent<Quaffle>().isCached) return;
        isCached = true;
        cachedQuaffle.GetComponent<Quaffle>().Cache(gameObject);
        currentTargetIndex = Random.Range(0, 3);
        GetComponent<AnimationController>().CatchTheBall();
        GameObject tmpObj = GameObject.Instantiate(takeBallParticleEffect, cachedQuaffle.transform.position, new Quaternion());
        Destroy(tmpObj, 2.0f);
    }

    //get the golden snitch
    public void TakeGoldenSnitch()
    {
        if(cachedGoldenSnitch == null) return;
        isCachedGoldenSnich = true;
        cachedGoldenSnitch.GetComponent<GoldenSnich>().Catch(gameObject);
        GetComponent<AnimationController>().CatchTheBall();
        GameObject tmpObj = Instantiate(takeBallParticleEffect, cachedGoldenSnitch.transform.position, new Quaternion());
        Destroy(tmpObj, 2.0f);
    }
    
    
    // Update is called once per frame
    void Update()
    {
        if(curColdDownInDizziness > 0.0f)
        {
            curColdDownInDizziness -= Time.deltaTime;
            dizzyParticleEffect.SetActive(true);
        }
        else
        {
            dizzyParticleEffect.SetActive(false);
        }
            
        //the npc logic
        PerceptBludger();
        PerceptChaserWithQuaffle();

        if (!IsPlayer()) return;
        
        //the player logic
        if (playerType == PlayerType.Chaser || playerType == PlayerType.Keeper)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                //Debug.Log("Check F Up:");
                if (!isCached && cachedQuaffle != null)
                {
                    TakeQuaffle();
                }
                else if (isCached)
                {
                    PassQuaffleByAnimation();
                }
            }
        }

        if(playerType == PlayerType.Beater)
        {
            //be poised to beat the bludger
            if (Input.GetMouseButton(0))
            {
                GetComponent<AnimationController>().HitBallAnimation();
            }
            
            //hit the bludger
            if(Input.GetMouseButtonUp(0))
            {
                GetComponent<AnimationController>().ReleaseHitBallAnimation();
                ActiveBeatBludger();
            }
        }

        if(playerType == PlayerType.Seeker)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isCachedGoldenSnich && cachedGoldenSnitch != null)
                {
                    TakeGoldenSnitch();
                }
            }
        }
        
        ShowHelpGUI();
        UpdateTargetGUI();
    }

    private void ShowHelpGUI()
    {
        if (!GameManager.Instance.GameStarted)
        {
            GameUI.Instance.ShowHelpUI(false, "");
            return;
        }
            
        if (playerType == PlayerType.Chaser)
        {
            if (cachedQuaffle == null)
            {
                GameUI.Instance.ShowHelpUI(false, "");
                return;
            }

            if (!isCached)
            {
                GameUI.Instance.ShowHelpUI(true, "F to Take Quaffle");
                return;
            }

            var quaffle = cachedQuaffle.GetComponent<Quaffle>();
            if (quaffle.takenChaser != null && quaffle.takenChaser != gameObject)
            {
                GameUI.Instance.ShowHelpUI(false, "");
                return;
            }
            
            if (Vector3.Distance(cachedQuaffle.transform.position, transform.position) <= 0.75f)
            {
                GameUI.Instance.ShowHelpUI(true, "F to Take Quaffle");
                return;
            }
            
            var target = GetComponent<CharacterSwitcher>().selected;
            if (target == null)
            {
                GameUI.Instance.ShowHelpUI(false, "");
                return;
            }

            var isGoal = target.GetComponent<GoalDetector>() != null;
            if (isGoal && Vector3.Distance(transform.position, target.position) <= MIN_DIST_FOR_GOAL)
            {
                GameUI.Instance.ShowHelpUI(true, "F to Goal!");
                return;
            }

            var teamE = target.GetComponent<TeamEntity>();
            if (teamE == null)
            {
                GameUI.Instance.ShowHelpUI(false, "");
                return;
            }
            
            var isSelectedTargetChaser = target.GetComponent<TeamEntity>().MyPlayerType == PlayerType.Chaser;
            if(isSelectedTargetChaser)
            {
                GameUI.Instance.ShowHelpUI(true, "F to Pass!");
                return;
            }
            
            GameUI.Instance.ShowHelpUI(false, "");
        }
        else if (playerType == PlayerType.Seeker)
        {
            if (cachedGoldenSnitch != null)
            {
                GameUI.Instance.ShowHelpUI(true, "F to Catch!");
            }
        }
        else if (playerType == PlayerType.Beater)
        {
            if (!IsBeatAvailable())
            {
                GameUI.Instance.ShowHelpUI(false, "");
                return;
            }
            
            GameUI.Instance.ShowHelpUI(true, "F to Beat Bludger!");
        }
        else
        {
            GameUI.Instance.ShowHelpUI(false, "");
        }
    }
    
    private void UpdateTargetGUI()
    {
        targetGUI.gameObject.SetActive(playerType != PlayerType.Beater);
        
        Transform target=null;
        if (playerType == PlayerType.Seeker)
        {
            target = GameManager.Instance.goldenSnitch.transform;
        }
        else if (playerType == PlayerType.Chaser || playerType == PlayerType.Keeper)
        {
            target = GameManager.Instance.quaffle.transform;
        }

        if (target != null)
        {
            var forward = target.position - transform.position;
            targetGUI.forward = forward.normalized;
        }

        foreach (GameObject bludger in GameManager.Instance.Bludges)
        {
            var bForward = bludger.transform.position - transform.position;
            bludgerTargetIdcGUI[GameManager.Instance.Bludges.IndexOf(bludger)].forward = bForward.normalized;
        }
    }
}

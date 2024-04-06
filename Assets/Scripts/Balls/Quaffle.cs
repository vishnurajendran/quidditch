using AgentControllers;
using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Teams;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class Quaffle : MonoBehaviour
{
    public GameObject takenChaser = null;
    public bool isCached = false;
    public float fallingVelocity = 2.0f;
    public float velocity = 50.0f;

    public bool isToTarget = false;
    public GameObject lastThrowingNPC = null;
    public List<Vector3> pathPoints = new List<Vector3>();
    private int pathIndex = 0;

    private Vector3 originPos = GameManager.Instance.GetQuaffleResetPosition();

    private void Start()
    {
        originPos = GameManager.Instance.GetQuaffleResetPosition();
        this.transform.position = originPos;
    }

    public void ResetStatus()
    {
        isToTarget = false;
        pathPoints.Clear();
        pathIndex = 0;
        originPos = GameManager.Instance.GetQuaffleResetPosition();
        Debug.Log("originPos:" + originPos);
        lastThrowingNPC = null;
    }

    public void StopStatus()
    {
        isToTarget = false;
        pathPoints.Clear();
        originPos = transform.position;
        pathIndex = 0;
        lastThrowingNPC = null;
        isCached = false;
        takenChaser = null;
    }

    public void SetPathPoints(Vector3[] _pathPoints, GameObject throwingNPC, bool isToTarget_ = false)
    {
        isToTarget = isToTarget_;
        lastThrowingNPC = throwingNPC;
        takenChaser = null;
        isCached = false;
        pathPoints.Add(transform.position);
        for(int i =0; i < _pathPoints.Length; i++)
        {
            pathPoints.Add(_pathPoints[i]);
        }
        Debug.Log("Current Path Points:" + pathPoints.Count);
    }

    public void Cache(GameObject takenNPC)
    {
        isToTarget = false;
        isCached = true;
        lastThrowingNPC = null;
        takenChaser = takenNPC;
        pathPoints.Clear();
        pathIndex = 0;
    }

    public bool IsChaserValidForCurrentBall(GameObject chaser)
    {
        //friend sent it to the target
        if (pathPoints.Count != 0 && lastThrowingNPC != null && lastThrowingNPC.gameObject.tag == chaser.tag && isToTarget)
            return false;

        //there is no need for the chaser to cache the ball throwed by himself
        if (pathPoints.Count != 0 && lastThrowingNPC == chaser)
            return false;
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCached)
        {
            //Debug.Log("the path points:" + pathPoints.Length);
            if (pathPoints.Count != 0)
            {

                //Debug.Log("the path points:" + pathPoints.Count);
                for (int i = 1; i < pathPoints.Count; ++i)
                {
                    Debug.DrawLine(pathPoints[i-1], pathPoints[i], Color.red);
                }
                if (pathIndex >= pathPoints.Count) {
                    StopStatus();
                    return;
                }

                transform.position = Vector3.MoveTowards(transform.position,
                    pathPoints[pathIndex], velocity * Time.deltaTime);
                if (Vector3.Distance(pathPoints[pathIndex], transform.position) < 0.00001f)
                {
                    pathIndex = (pathIndex + 1);
                }
            }
            else
            {
                //Debug.Log("current position:" + transform.position + " origin position:" + originPos);
                //if (originPos == GameManager.Instance.GetQuaffleResetPosition())
                //    originPos = transform.position;
                float sinValue = Mathf.Sin(Time.fixedTime);
                Vector3 newPos = originPos;
                newPos.y += sinValue;
                transform.position = newPos;
            }
        }
        else
        {
            //float after the path finish
            transform.position = takenChaser.GetComponent<Role>().quaffleFollowPoint.transform.position;
            //originPos = GameManager.Instance.GetQuaffleResetPosition();
        }
    }
}

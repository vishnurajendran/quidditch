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
    public float velocity = 10.0f;

    public List<Vector3> pathPoints = new List<Vector3>();
    private int pathIndex = 0;

    private Vector3 originPos = Vector3.zero;
    public void SetPathPoints(Vector3[] _pathPoints)
    {
        takenChaser = null;
        isCached = false;
        pathPoints.Add(transform.position);
        for(int i =0; i < _pathPoints.Length; i++)
        {
            pathPoints.Add(_pathPoints[i]);
        }
        Debug.Log("Current Path Points:" + pathPoints.Count);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCached)
        {
            //Debug.Log("the path points:" + pathPoints.Length);
            if (pathPoints.Count != 0)
            {

                Debug.Log("the path points:" + pathPoints.Count);

                for (int i = 1; i < pathPoints.Count; ++i)
                {
                    Debug.DrawLine(pathPoints[i-1], pathPoints[i], Color.red);
                }
                if (pathIndex >= pathPoints.Count) {
                    pathPoints.Clear();
                    pathIndex = 0;
                    return;
                }

                transform.position = Vector3.MoveTowards(transform.position,
                    pathPoints[pathIndex], 50.0f * Time.deltaTime);
                if (Vector3.Distance(pathPoints[pathIndex], transform.position) < 0.00001f)
                {
                    pathIndex = (pathIndex + 1);
                }
            }
            else
            {
                if (originPos == Vector3.zero)
                    originPos = transform.position;
                float sinValue = Mathf.Sin(Time.fixedTime);
                Vector3 newPos = originPos;
                newPos.y += sinValue;
                transform.position = newPos;
            }
        }
        else
        {
            //float after the path finish
            transform.position = takenChaser.transform.position + takenChaser.transform.right;
            originPos = Vector3.zero;
        }
    }
}

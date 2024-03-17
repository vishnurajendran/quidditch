using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parabola : MonoBehaviour
{
    public GameObject point;
    private float initialHight = 0;                  //射线开始发射的初始高度
    public float initialVelocity = 0;                //初始速度
    private float velocity_Horizontal, velocity_Vertical;  //水平分速度和垂直分速度
    private float includeAngle = 0;                  //与水平方向的夹角
    private float totalTime = 0;                     //抛出到落地的总时间
    private float timeStep = 0;                      //时间步长

    private LineRenderer line;
    [SerializeField] private float lineWidth = 0.07f;
    [SerializeField] private Material lineMaterial;
    private RaycastHit hits;

    [Range(2, 200)] public int line_Accuracy = 10;   //射线的精度（拐点的个数)
    private float grivaty = 9.8f;
    private int symle = 1;                           //确定上下的符合
    private Vector3 parabolaPos = Vector3.zero;      //抛物线的坐标
    private Vector3 lastCheckPos, currentCheckPos;   //上一个和当前一个监测点的坐标
    private Vector3 checkPointPosition;              //监测点的方向向量
    private Vector3[] checkPointPos;                 //监测点的坐标数组
    private float timer = 0;                         //累计时间
    private int lineCount = 0;

    private Transform startPoint;

    //GameObject point;
    private void Start()
    {
        startPoint = transform;
        //point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        if (!this.GetComponent<LineRenderer>())
        {
            line = this.gameObject.AddComponent<LineRenderer>();
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.material = lineMaterial;
        }
    }
    private void Update()
    {
        if (startPoint == null)
        {
            return;
        }
        Calculation_parabola();
    }
    private void Calculation_parabola()
    {
        velocity_Horizontal = initialVelocity * Mathf.Cos(includeAngle);
        velocity_Vertical = initialVelocity * Mathf.Sin(includeAngle);
        initialHight = Mathf.Abs(startPoint.transform.position.y);
        float time_1 = velocity_Vertical / grivaty;
        float time_2 = Mathf.Sqrt((time_1 * time_1) + (2 * initialHight) / grivaty);
        totalTime = time_1 + time_2;
        timeStep = totalTime / line_Accuracy;
        includeAngle = Vector3.Angle(startPoint.forward, Vector3.ProjectOnPlane(startPoint.forward, Vector3.up)) * Mathf.Deg2Rad;
        symle = (startPoint.position + startPoint.forward).y > startPoint.position.y ? 1 : -1;

        if (checkPointPos == null || checkPointPos.Length != line_Accuracy)
        {
            checkPointPos = new Vector3[line_Accuracy];
        }
        for (int i = 0; i < line_Accuracy; i++)
        {
            if (i == 0)
            {
                lastCheckPos = startPoint.position - startPoint.forward;
            }
            parabolaPos.z = velocity_Horizontal * timer;
            parabolaPos.y = velocity_Vertical * timer * symle + (-grivaty * timer * timer) / 2;
            currentCheckPos = startPoint.position + Quaternion.AngleAxis(startPoint.eulerAngles.y, Vector3.up) * parabolaPos;
            checkPointPosition = currentCheckPos - lastCheckPos;
            lineCount = i + 1;
            if (Physics.Raycast(lastCheckPos, checkPointPosition, out hits, checkPointPosition.magnitude + 3))
            {
                checkPointPosition = hits.point - lastCheckPos;
                checkPointPos[i] = hits.point;

                point.SetActive(true);
                point.transform.position = hits.point;
                point.transform.localScale = Vector3.one / 3;
                point.transform.GetComponent<MeshRenderer>().material.color = Color.red;
                if (hits.transform == null)
                {
                    point.SetActive(false);
                }
            }
            checkPointPos[i] = currentCheckPos;
            lastCheckPos = currentCheckPos;
            timer += timeStep;
        }
        line.positionCount = lineCount;
        line.SetPositions(checkPointPos);
        timer = 0;
    }

}

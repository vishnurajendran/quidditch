using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class TPSCamera : MonoBehaviour
{
    public float distanceChangeSpeed = 50.0f;
    public float rotateSpeed = 300.0f;
    public float focusCameraOffset = 1.0f;
    public float pitchMaxLimit = 89.0f;
    public float pitchMinLimit = 10.0f;
    public float distanceNormal = 15.0f;
    public float distanceFocus = 3.0f;

    public float rotationAngleX = 0.0f;
    public float rotationAngleY = 0.0f;
    float distance = 0.0f;
    Vector3 yawAxis = Vector3.zero;
    Vector3 pitchAxis = Vector3.zero;
    Vector3 defaultForward = Vector3.zero;

    public GameObject target;

    private void Start()
    {
        yawAxis = -Physics.gravity.normalized;
        defaultForward = Vector3.ProjectOnPlane((transform.position - target.transform.position), yawAxis).normalized;
        pitchAxis = Vector3.Cross(yawAxis, Vector3.ProjectOnPlane(defaultForward, yawAxis));

    }

    void LateUpdate()
    {

        float mouseXMove = Input.GetAxis("Mouse X");
        float mouseYMove = Input.GetAxis("Mouse Y");
        rotationAngleX += mouseXMove * rotateSpeed * Time.smoothDeltaTime;
        rotationAngleX = rotationAngleX % 360;
        rotationAngleY += mouseYMove * rotateSpeed * Time.smoothDeltaTime;
        rotationAngleY = Mathf.Clamp(rotationAngleY, -60.0f, 60.0f);


        Quaternion horizental = Quaternion.AngleAxis(rotationAngleX, yawAxis);
        Quaternion vertical = Quaternion.AngleAxis(rotationAngleY, pitchAxis);

        Vector3 cameraForward = horizental * vertical * defaultForward;
        Vector3 targetPosition = target.transform.position;

        float curDistance = distanceNormal;
        if (Input.GetMouseButton(1))
        {
            curDistance = distanceFocus;
        }
        
        distance = Mathf.Lerp(distance, curDistance, Time.deltaTime * distanceChangeSpeed);
        Vector3 cameraPosition = targetPosition + cameraForward * distance;


        transform.position = cameraPosition;
        transform.LookAt(target.transform);
    }
}

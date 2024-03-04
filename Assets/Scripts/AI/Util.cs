using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static bool CheckSector(float angle, float length, Transform origin, Transform target)
    {
        float distance = Vector3.Distance(origin.position, target.position);
        if (distance > length) return false;
        Vector3 forwardVec = origin.forward.normalized;
        Vector3 directionVec = (target.position - origin.position).normalized;
        float tmp = Mathf.Clamp(Vector3.Dot(forwardVec.normalized, directionVec.normalized), -0.99f, 0.99f);
        float currentAngle = Mathf.Acos(tmp) * Mathf.Rad2Deg;
        if (currentAngle <= angle * 0.5f)
            return true;
        return false;
    }

    public static bool CheckSectorOnlyAngle(float angle, Transform origin, Transform target)
    {
        Vector3 forwardVec = origin.forward.normalized;
        Vector3 directionVec = (target.position - origin.position).normalized;
        float currentAngle = Mathf.Acos(Vector3.Dot(forwardVec.normalized, directionVec.normalized)) * Mathf.Rad2Deg;
        if (currentAngle <= angle * 0.5f)
            return true;
        return false;
    }

    public static bool IsInFront(Transform origin, Transform target)
    {
        Vector3 direction = (target.position - origin.position).normalized;
        Vector3 front = origin.forward.normalized;
        return Vector3.Dot(direction, front) > 0;
    }

    public static bool IsInFront(Transform origin, Transform target, Vector3 frontVec)
    {
        Vector3 direction = (target.position - origin.position).normalized;
        Vector3 front = frontVec;
        return Vector3.Dot(direction, front) > 0;
    }

    public static bool IsInRight(Transform origin, Transform target)
    {
        Vector3 direction = (target.position - origin.position).normalized;
        Vector3 right = origin.right.normalized;
        return Vector3.Dot(direction, right) > 0;
    }
    public static int GetRandomSign()
    {
        float val = Random.Range(-1.0f, 1.0f);
        int sigVal = 1;
        if (val < 0.0f) sigVal = -1;
        return sigVal;
    }
    public static bool IsInRight(Vector3 origin, Vector3 target, Vector3 point)
    {
        Vector3 AB = target - origin;
        Vector3 AC = point - origin;
        var crossVal = Vector3.Cross(AB, AC);
        if (crossVal.y > 0)
        {
            return true;
        }
        return false;
    }

}

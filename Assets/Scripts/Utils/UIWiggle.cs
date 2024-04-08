using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UIWiggle : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine(Wiggle());
    }

    IEnumerator Wiggle()
    {
        while (true)
        {
            float timeStep = 0;
            Vector3 current = transform.localPosition;
            Vector3 next = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), 0);
            while (timeStep <= 1)
            {
                timeStep += Time.deltaTime / 1.5f;
                transform.localPosition = Vector3.Lerp(current, next, timeStep);
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }
    }
}

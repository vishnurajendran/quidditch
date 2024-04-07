using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceManager : MonoBehaviour
{

    public Animator[] audienceAnimators;

    // Start is called before the first frame update
    void Start()
    {
        audienceAnimators = GetComponentsInChildren<Animator>();
    }

    public void React(float applaudRate, float celebrateRate)
    {
        int animatorCount = audienceAnimators.Length;

        for(int i =0; i < animatorCount; ++i)
        {
            int randomCatIndex = Random.Range(1, 11);
            int applaudLimitation = (int)(applaudRate * 10);
            if(randomCatIndex <= applaudLimitation)
            {
                int randomIndex = Random.Range(1, 3);
                audienceAnimators[i].SetInteger("applause", randomIndex);
            }
            else
            {
                int randomIndex = Random.Range(1, 4);
                audienceAnimators[i].SetInteger("celerbrate", randomIndex);
            }  
        }
    }

    public void Celerbrate()
    {
        React(0.3f, 0.7f);
    }

    public void Applaud()
    {
        React(0.7f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

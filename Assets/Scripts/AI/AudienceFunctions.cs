using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudienceFunctions : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ResetApplause()
    {
        animator.SetInteger("applause", 0);
    }

    public void ResetCelerbrate()
    {
        animator.SetInteger("celerbrate", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

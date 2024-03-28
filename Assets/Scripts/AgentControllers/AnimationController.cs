using Gameplay;
using System.Collections;
using System.Collections.Generic;
using Teams;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Animator animator;
    public IKController ikController;
    public GameObject beaterUtils;

    public void InitRoles(PlayerType type)
    {
        if (type == PlayerType.Beater)
            animator.SetBool("Beater", true);
        else
            beaterUtils.SetActive(false);
        ikController.SetPlayerType(type);
    }

    public void HitBallAnimation()
    {
        animator.SetBool("Hit", true);
    }

    public void NormalState()
    {
        ikController.SetNormalIKState();
    }


    public void CatchTheBall()
    {
        ikController.SetCacheBallIKState();
    }

    public void ThrowBall()
    {
        ikController.ikActive = false;
        ikController.SetNormalIKState();
        animator.SetBool("Throw", true);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

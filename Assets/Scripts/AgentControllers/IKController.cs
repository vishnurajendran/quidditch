using Gameplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
    public Transform SweepHoldingPoint = null;
    public Transform CacheBallPoint = null;
    public Transform LookForwardPoint = null;
    public Transform leftHandObj = null;
    public Transform rightHandObj = null;
    private Animator avatar;
    public bool ikActive = true;
    private PlayerType playerType = PlayerType.Chaser;

    public void SetPlayerType(PlayerType playerType_)
    {
        this.playerType = playerType_;
    }

    public void PassQuaffle()
    {
        transform.parent.parent.parent.parent.GetComponent<Role>().PassQuaffle();
    }

    public void OpenIKControl()
    {
        avatar.SetBool("Hit", false);
        avatar.SetBool("Throw", false);
        ikActive = true;
        SetNormalIKState();
    }

    public void SetNormalIKState()
    {
        leftHandObj = SweepHoldingPoint;
        rightHandObj = SweepHoldingPoint;
    }

    public void SetCacheBallIKState()
    {
        leftHandObj = SweepHoldingPoint;
        rightHandObj = CacheBallPoint;  
    }

    void Start()
    {
        avatar = GetComponent<Animator>();
        SetNormalIKState();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (avatar == null)
            return;


        if (ikActive)
        {
            if(playerType == PlayerType.Chaser)
            {
                avatar.SetLookAtWeight(1.0f);
                if (LookForwardPoint != null)
                {
                    avatar.SetLookAtPosition(LookForwardPoint.position);
                }

                avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
                avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);
                avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, 1.0f);
                avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, 1.0f);

                //
                if (leftHandObj != null)
                {
                    avatar.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    avatar.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
                if (rightHandObj != null)
                {
                    avatar.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    avatar.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }
            }
            else if(playerType == PlayerType.Beater)
            {
                avatar.SetLookAtWeight(1.0f);
                if (LookForwardPoint != null)
                {
                    avatar.SetLookAtPosition(LookForwardPoint.position);
                }
               
                avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
            else
            {
                avatar.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                avatar.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                avatar.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                avatar.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
            }
        }


          

    }

}

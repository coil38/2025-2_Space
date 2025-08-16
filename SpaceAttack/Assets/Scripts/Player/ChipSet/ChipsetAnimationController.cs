using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChipsetAnimationController : MonoBehaviour
{
    private Animator animator;

    public void SetAnimator(Animator _animator)
    {
        animator = _animator;
    }

    public void PlayAttackAnimation(PlayerAniInfo aniInfo)
    {
        switch (aniInfo.type)
        {
            case AniType.Trrigger:
                animator.SetTrigger(aniInfo.name);
                break;

            case AniType.Bool:
                animator.SetBool(aniInfo.name, true);
                break;

        }

        //재생속도 설정
        animator.SetFloat("speed", aniInfo.speed);
    }
}

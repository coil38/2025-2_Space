using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndParamBehaviour : StateMachineBehaviour
{
    public string parameter = "IsAttacking";

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //애니메이션이 실행하기 전 실행조건 초기화
        animator.SetBool(parameter, false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEndParamBehaviour : StateMachineBehaviour
{
    public static bool isEndAttack = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //애니메이션이 실행하기 전 실행조건 초기화
        isEndAttack = true;
    }
}

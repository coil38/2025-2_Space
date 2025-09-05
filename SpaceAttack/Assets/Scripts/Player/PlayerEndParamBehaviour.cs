using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEndParamBehaviour : StateMachineBehaviour
{
    public bool useInAttack = false;

    public static bool isEndAttack = false;
    public static bool isResetingAni = false;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //애니메이션이 실행하기 전 실행조건 초기화
        if(useInAttack) isEndAttack = true;
        
        isResetingAni = true;
    }
}

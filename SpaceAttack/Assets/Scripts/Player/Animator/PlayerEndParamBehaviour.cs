using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEndParamBehaviour : StateMachineBehaviour
{
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMoveAniCondition.EndAni();
    }
}

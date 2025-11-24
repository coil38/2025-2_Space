using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillCoreActivation : SkillType   //시전시간(발사: 애니메이션 후, 실행) O | 공격시간 X | 플레이어 대기시간(쿨타임) O
{
    //private float speedUpValue = 1.3f;
    //private float damageValue = 1.3f;               //임시
    //private float criticalValue = 1.3f;

    private bool isUsingBuff;
    public override void OnEnable()
    {
        unLockedNumber = 3;
        chipsetCompID = 106;
        base.OnEnable();
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();

        if (isUsingBuff)
        {
            isUsingBuff = false;

            Debug.Log("버프사용 종료");

            //플레이어 속도 초기화
            //플레이어 공격력 초기화
            //플레이어 치명타 초기화
        }
    }

    public override void CheckUse(Vector3 currentPos)
    {
        if (!isUnLocked) return;                                      //해금여부에 따른 스킬 사용 여부

        if (PlayerInputController.skill3Action.triggered)
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;

            if (coolTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            //물약 마시는 사운드
            //물약 마시는 애니메이션
            coolTimer.Start();         //쿨타임 시작
            Use();                     //즉시 사용 처리
        }
    }

    public override void Use()
    {
        Debug.Log("버프사용 시작");

        isUsingBuff = true;
        //플레이어 속도 증가
        //플레이어 공격력 증가
        //플레이어 치명타 증가

    }
}

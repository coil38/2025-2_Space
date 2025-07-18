using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillCoreActivation : SkillType
{
    private float speedUpValue = 1.3f;
    private float damageValue = 1.3f; //임시
    private float criticalValue = 1.3f;

    private bool isUsingBuff;
    public override void OnEnable()
    {
        attackTime = 10f;
        playerWaitTime = 0.2f;
        coolTime = 25f;
        coolTimer = new Timer(coolTime);
        w_AttackTimer = new Timer(playerWaitTime);
        s_AttackTimer = new Timer(attackTime);
    }

    public override void UpdateInfo()
    {
        s_AttackTimer.Update();

        if (isUsingBuff && !s_AttackTimer.IsRunning())
        {
            isUsingBuff = false;

            Debug.Log("버프사용 종료");

            //플레이어 속도 초기화
            //플레이어 공격력 초기화
            //플레이어 치명타 초기화
        }
    }

    public override void CheckAttack(Vector3 currentPos)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (coolTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            //물약 마시는 사운드
            //물약 마시는 애니메이션

            TimeSystem.s_w_AttackTimer = w_AttackTimer;
            TimeSystem.s_w_AttackTimer.Start();                 //다음 공격 전 대기 체크 시작

            coolTimer.Start();         //쿨타임 시작

            Attack();
        }
    }

    public override void Attack()
    {
        s_AttackTimer.Start();

        Debug.Log("버프사용 시작");

        isUsingBuff = true;
        //플레이어 속도 증가
        //플레이어 공격력 증가
        //플레이어 치명타 증가

    }
}

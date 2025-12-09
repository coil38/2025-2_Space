using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSkillCoreActivation : SkillType   //시전시간(발사: 애니메이션 후, 실행) O | 공격시간 X | 플레이어 대기시간(쿨타임) O
{
    [SerializeField] private ParticleSystem TriggerEffect;
    [SerializeField] private ParticleSystem AuraEffect;

    float skilldamageRate = 1.6f;
    float criticalRate = 1.8f;
    float speedRate = 1.5f;
    float lifeTime = 6f;

    public override void OnEnable()
    {
        unLockedNumber = 3;
        chipsetCompID = 106;
        base.OnEnable();
    }

    public override void UpdateInfo()
    {
        base.UpdateInfo();
    }

    public override void CheckUse(Vector3 currentPos)
    {
        //if (!isUnLocked) return;                                      //해금여부에 따른 스킬 사용 여부

        if (PlayerInputController.skill3Action.triggered)
        {
            if (PlayerTimeSystem.w_SkillTimer != null)
                if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;

            if (coolTimer.IsRunning()) return; //다음 공격 대기 체크 실행중, 리턴

            PlayerSoundManager.PlaySwordSkill3();  //이펙트 사용 사운드
            coolTimer.Start();         //쿨타임 시작
            Use();                     //즉시 사용 처리
        }
    }

    public override void Use()
    {
        Debug.Log("버프적용");

        TriggerEffect.Play();
        AuraEffect.Play();

        PlayerStatus.normalDamage *= skilldamageRate;           //플레이어 공격력 증가
        PlayerStatus.criticalRate *= criticalRate;              //플레이어 치명타 피해 증가
        PlayerStatus.m_speed *= speedRate;                      //플레이어 속도 증가

        TimerEvent.Add(lifeTime, OffSkillBuff);                 //버트 종료 타이머 이벤트 넣기
    }

    private void OffSkillBuff()
    {
        Debug.Log("버프풀림");
        AuraEffect.Stop();

        PlayerStatus.normalDamage /= skilldamageRate;           //플레이어 공격력 초기화
        PlayerStatus.criticalRate /= criticalRate;              //플레이어 치몇타 피해 초기화
        PlayerStatus.m_speed /= speedRate;                      //플레이어 속도 증가
    }
}

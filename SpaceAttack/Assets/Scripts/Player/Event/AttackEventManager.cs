using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEventManager
{
    public static event Action<AttackContext> OnAttackStarted;
    public static event Action<AttackContext> OnAttackFinished;
    public static void RaiseAttack(AttackContext context)
    {
        OnAttackStarted?.Invoke(context);

        if (context.IsReattack)
            Attack(context);
        Attack(context);

        if (context.IsReattack) 
            DamageEffectManager.instance.ShowDoubleAttack(context.target.transform.position + context.target.transform.up * 0.5f);
        Camera.main.GetComponent<CameraFallow>().CameraShack();                     //카메라 흔들림 연출

        OnAttackFinished?.Invoke(context);
    }

    private static void Attack(AttackContext context)
    {
        ChipAttackType type = context.attackType;
        Vector3 dir = context.attackDir;
        GameObject target = context.target;


        EventManager.relicEvent.OnPlayerAttackEvent();                              //플레이어 모든 공격 이벤트 실행
        if (type == ChipAttackType.Skill)
            EventManager.relicEvent.OnPlayerSkillAttackEvent();                     //플레이어 스킬 공격 이벤트 실행


        bool isCritical = false;
        AttackInfo info = new AttackInfo(CalculateAttackDamage(context), dir);      //공격 정보 생성

        float criDamage = info.CheckAndSetCritical(
            info.damage, 
            PlayerStatus.criticalChanceRate + context.addedCritChanceRate, 
            PlayerStatus.criticalRate + context.addedCritRate
            );

        if (criDamage > info.damage) isCritical = true;                             //크리티컬 처리
        info.damage = criDamage;                                                    //크리데미지 적용

        if (isCritical)
            EventManager.relicEvent.OnCriticalEvent();                              //크리티컬 성공 이벤트 실행

        Vector3 effectPos = target.transform.position + target.transform.up * 0.5f;
        if (DamageEffectManager.instance != null)                                   //데미지 이펙트 적용
        {
            if(!context.IsReattack)
                DamageEffectManager.instance.ShowDamage(effectPos, (int)criDamage, false, isCritical);
        }

        target.SendMessage("ApplyDamage", info);                                    //공격 함수

        if (target.gameObject != null) return;
        if (target.GetComponent<EnemyBase>() != null)
            if (target.GetComponent<EnemyBase>().isDead) return;
        EventManager.relicEvent.OnKilledEnemyEvent();                           //적 처치 성공 이벤트 실행
    }

    private static float CalculateAttackDamage(AttackContext context)            //데미지 계산 함수 (크리티컬 미포함)
    {
        float value = 0;
        if (context.attackType == ChipAttackType.Skill)
            value += context.skillDamageRateSume;
        else if (context.attackType == ChipAttackType.Weapon)
            value += context.weaponDamagekRateSume;

        return (PlayerStatus.normalDamage * (1 + context.attackRateSume)) * context.damageRateSume * context.damageRate * value;
    }

    public static void InitialEvent()
    {
        OnAttackFinished = null;
        OnAttackStarted = null;
    }
}

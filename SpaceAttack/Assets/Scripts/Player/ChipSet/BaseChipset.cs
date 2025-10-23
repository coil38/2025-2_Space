using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChipDataModifiers
{
    public float s_attackRate;     // == Sume_attackRate
    public float s_damageRate = 1;
    public float s_weaponDamageRate = 1;
    public float s_skillDamageRate = 1;

    public float s_coolDownRate;
    public float s_attackTimeRate;
}

public class BaseChipset : ChipSetType
{
    ChipDataModifiers r_dataModifiers = new ChipDataModifiers();

    public override void SetCorrectionValue(object obj, PlayerEvent e)  //레벨업용 보정치 데이터 저장 함수
    {
        if (e.correctablility)  //공격력 보정치 주입
            r_dataModifiers.s_attackRate += e.damageCorrection;

        if (e.unlockability)  //스킬 해금
        {
            int unlockNum = e.skillNumber;
            foreach (var skill in skills)
            {
                if (skill.unLockedNumber == unlockNum)  //해금
                    skill.isUnLocked = true;
            }

            //UI스킬 해금 연출
        }
    }

    public override void SetChipAttackRate(object obj, PlayerEvent e)
    {
        if (e.isEquip) r_dataModifiers.s_attackRate += e.attackRate;
        else r_dataModifiers.s_attackRate -= e.attackRate;

        LogUtil.Log($"공격력 배율 변경, 현재 배율: {r_dataModifiers.s_attackRate}, 증가여부: {e.isEquip}, 변경 수치: {e.attackRate}");
    }
    public override void SetChipDamageRate(object obj, PlayerEvent e)
    {
        if (e.isEquip) r_dataModifiers.s_damageRate *= 1 + e.damageRate;
        else r_dataModifiers.s_damageRate /= 1 + e.damageRate;

        LogUtil.Log($"데미지 배율 변경, 현재 배율: {r_dataModifiers.s_damageRate}, 증가여부: {e.isEquip}, 변경 수치: {e.damageRate}");
    }
    public override void SetChipWeaponDamageRate(object obj, PlayerEvent e)
    {
        if (e.isEquip) r_dataModifiers.s_weaponDamageRate *= 1 + e.weaponDamageRate;
        else r_dataModifiers.s_weaponDamageRate /= 1 + e.weaponDamageRate;

        LogUtil.Log($"데미지(무기) 배율 변경, 현재 배율: {r_dataModifiers.s_weaponDamageRate}, 증가여부: {e.isEquip}, 변경 수치: {e.weaponDamageRate}");
    }
    public override void SetChipSkillDamageRate(object obj, PlayerEvent e)
    {
        if (e.isEquip) r_dataModifiers.s_skillDamageRate *= 1 + e.skillDamageRate;
        else r_dataModifiers.s_skillDamageRate /= 1 + e.skillDamageRate;

        LogUtil.Log($"데미지(스킬) 배율 변경, 현재 배율: {r_dataModifiers.s_skillDamageRate}, 증가여부: {e.isEquip}, 변경 수치: {e.skillDamageRate}");
    }

    public override void SetCoolDownRate(object obj, PlayerEvent e)  //칩셋 쿨타임 감소 데이터 저장 함수
    {
        if (e.isEquip) r_dataModifiers.s_coolDownRate += e.coolDownRate;
        else r_dataModifiers.s_coolDownRate -= e.coolDownRate;
        UpdateCoolDown();
    }
    public override void SetAttackTime(object obj, PlayerEvent e)
    {
        if(e.isEquip) r_dataModifiers.s_attackTimeRate += e.attackTimeRate;
        else r_dataModifiers.s_attackTimeRate -= e.attackTimeRate;
        UpdateAttackSpeed();
    }
    private void UpdateAttackSpeed()
    {
        weapon.attackTime = weapon.normalAttackTime * (1 - r_dataModifiers.s_attackTimeRate);
        LogUtil.Log($"기본공격 기본 공속: {weapon.normalAttackTime}, 변경된 공속: {weapon.attackTime}, 공속배율: {r_dataModifiers.s_attackTimeRate}");
        foreach (var skill in skills)
        {
            skill.attackTime = skill.normalAttackTime * (1 - r_dataModifiers.s_attackTimeRate);
            LogUtil.Log($"스킬 기본 공속: {skill.normalAttackTime}, 변경된 공속: {skill.attackTime}, 공속배율: {r_dataModifiers.s_attackTimeRate}");
        }
    }

    private void UpdateCoolDown()
    {
        foreach (var skill in skills)
        {
            skill.coolTime = skill.normalCoolTime * (1 - r_dataModifiers.s_coolDownRate);
            LogUtil.Log($"스킬 기본 쿨타임: {skill.normalCoolTime}, 변경된 스킬 쿨타임: {skill.coolTime}, 쿨타임비율: {r_dataModifiers.s_coolDownRate}");
        }
    }

    public override void Attack(GameObject target, float damageRate, Vector3 dir, float addedCriChanceRate, float addedCriRate, ChipAttackType type)   //스킬, 무기 공용 공격함수
    {
        if (target == null) return;
        if (target.GetComponent<EnemyBase>() != null)
            if (target.GetComponent<EnemyBase>().isDead) return;                                         //만약 타겟이 사망했다면 반환처리

        AttackContext ctx = new AttackContext(
            target,
            damageRate,
            dir,
            type,
            addedCriRate,
            addedCriChanceRate,
            r_dataModifiers.s_attackRate,
            r_dataModifiers.s_damageRate,
            r_dataModifiers.s_weaponDamageRate,
            r_dataModifiers.s_skillDamageRate
            );
        AttackEventManager.RaiseAttack(ctx);
        //Debug.Log(target.name + "에게 공격함");
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(SetEvent());
    }

    protected virtual void OnDisable()
    {
        PlayerEvent.correctionEventHandler -= SetCorrectionValue;           //레벨업(공격력, 스킬) 보정 이벤트 구독 해지
        PlayerEvent.chipAttackEventHandler -= SetChipAttackRate;            //공격력 보정 이벤트 구독 해지
        PlayerEvent.chipDamageEventHandler -= SetChipDamageRate;            //데미지 보정 이벤트 구독 해지
        PlayerEvent.chipWeaponDamageEventHandler -= SetChipWeaponDamageRate;//기본 공격 데미지 보정 이벤트 구독 해지
        PlayerEvent.chipSkillDamageEventHandler -= SetChipSkillDamageRate;  //스킬 데미지 보정 이벤트 구독 해지
        PlayerEvent.chipcoolDownEventHandler -= SetCoolDownRate;            //쿨타임 배율 이벤트 구독해지
        PlayerEvent.chipattackTimeDownHandler -= SetAttackTime;             //공격 시간 감소 이벤트 구독 해지
    }

    private IEnumerator SetEvent()
    {
        yield return new WaitUntil(() => EventManager.playerEvent != null);
        PlayerEvent.correctionEventHandler += SetCorrectionValue;            //레벨업(공격력, 스킬) 보정 이벤트 구독
        PlayerEvent.chipAttackEventHandler += SetChipAttackRate;             //공격력 보정 이벤트 구독
        PlayerEvent.chipDamageEventHandler += SetChipDamageRate;             //데미지 보정 이벤트 구독
        PlayerEvent.chipWeaponDamageEventHandler += SetChipWeaponDamageRate; //기본 공격 데미지 보정 이벤트 구독
        PlayerEvent.chipSkillDamageEventHandler += SetChipSkillDamageRate;   //스킬 데미지 보정 이벤트 구독
        PlayerEvent.chipcoolDownEventHandler += SetCoolDownRate;             //쿨타임 배율 이벤트 구독
        PlayerEvent.chipattackTimeDownHandler += SetAttackTime;              //공격 시간 감소 이벤트 구독
    }
}

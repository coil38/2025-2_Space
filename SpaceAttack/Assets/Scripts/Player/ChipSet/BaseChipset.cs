using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseChipset : ChipSetType
{
    private List<float> attackRates = new List<float>();        //공격 데미지 리스트
    private List<float> damageRates = new List<float>();        //피해 데미지 리스트
    private List<float> coolDownRates = new List<float>();      //쿨 다운 리스트

    private List<float> skillDamageRates = new List<float>();
    private List<float> weaponDamageRates = new List<float>();

    private float attackRateSume;
    private float damageRateSume;
    private float weaponDamageRateSume;
    private float skillDamageRateSume;

    public override void SetCorrectionValue(object obj, PlayerEvent e)  //레벨업용 보정치 데이터 저장 함수
    {
        if (e.correctablility)  //공격력 보정치 주입
            SetAttackRateValue(true, e.damageCorrection);

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

    public override void SetRelicAttackValue(object obj, PlayerEvent e)  //유물의 공격 보정치 데이터 저장 함수
    {
        SetAttackRateValue(e.isEquip, e.damageRate);
    }

    public override void SetCoolDownValue(object obj, PlayerEvent e)  //칩셋 쿨타임 감소 데이터 저장 함수
    {
        if (e.isEquip) coolDownRates.Add(e.coolDownRate);
        else coolDownRates.Remove(e.coolDownRate);
        UpdateCoolDown();
    }

    private void UpdateCoolDown()
    {
        float _coolDownRate = 0f;
        foreach (var rate in coolDownRates)
        {
            _coolDownRate += rate;
        }
        foreach (var skill in skills)
        {
            skill.coolTime = skill.normalCoolTime - skill.normalCoolTime * _coolDownRate;
            LogUtil.Log($"스킬 기본 쿨타임: {skill.normalCoolTime}, 변경된 스킬 쿨타임: {skill.coolTime}, 쿨타임비율: {_coolDownRate}");
        }
    }

    public override void Attack(GameObject target, float damageRate, Vector3 dir, ChipAttackType type)   //스킬, 무기 공용 공격함수
    {
        if (target.GetComponent<EnemyBase>() != null)
            if (target.GetComponent<EnemyBase>().isDead) return;                                         //만약 타겟이 사망했다면 반환처리

        AttackContext ctx = new AttackContext(target, damageRate, dir, type, attackRateSume, damageRateSume, weaponDamageRateSume, skillDamageRateSume);
        AttackEventManager.RaiseAttack(ctx);
        //Debug.Log(target.name + "에게 공격함");
    }

    protected virtual void OnEnable()
    {
        StartCoroutine(SetEvent());
    }

    protected virtual void OnDisable()
    {
        PlayerEvent.correctionEventHandler -= SetCorrectionValue;  //공격력, 스킬 보정 이벤트 구독 해지
        PlayerEvent.relicAttackEventHandler -= SetRelicAttackValue; //유물으로 인한 공격력 보정 이벤트 구독 해지
        PlayerEvent.coolDownEventHandler -= SetCoolDownValue;       //쿨타임 감소 이벤트 구독해제
    }

    private IEnumerator SetEvent()
    {
        yield return new WaitUntil(() => EventManager.playerEvent != null);
        PlayerEvent.correctionEventHandler += SetCorrectionValue;  //공격력, 스킬 보정 이벤트 구독
        PlayerEvent.relicAttackEventHandler += SetRelicAttackValue; //유물으로 인한 공격력 보정 이벤트 구독
        PlayerEvent.coolDownEventHandler += SetCoolDownValue;       //쿨타임 감소 이벤트 구독해제
    }

    private void SetDamageRateValue(bool isAdd, float value)
    {
        if (isAdd)
        {
            damageRates.Add(value);
            damageRateSume += value;
        }
        else
        {
            damageRates.Remove(value);
            damageRateSume -= value;
        }
    }
    private void SetAttackRateValue(bool isAdd, float value)
    {
        if (isAdd)
        {
            attackRates.Add(value);
            attackRateSume += value;
        }
        else
        {
            attackRates.Remove(value);
            attackRateSume -= value;
        }
    }
    private void SetSkillDamageRateValue(bool isAdd, float value)
    {
        if (isAdd)
        {
            skillDamageRates.Add(value);
            skillDamageRateSume += value;
        }
        else
        {
            skillDamageRates.Remove(value);
            skillDamageRateSume -= value;
        }
    }
    private void SetWeaponDamageRateValue(bool isAdd, float value)
    {
        if (isAdd)
        {
            weaponDamageRates.Add(value);
            weaponDamageRateSume += value;
        }
        else
        {
            weaponDamageRates.Remove(value);
            weaponDamageRateSume -= value;
        }
    }
}

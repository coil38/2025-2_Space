using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseChipset : ChipSetType
{
    private Queue<float> n_DamageRates = new Queue<float>();    //레벨 데미지 보정 리스트
    private List<float> attackDamageRates = new List<float>();  //피해 데미지 리스트
    private List<float> coolDownRates = new List<float>();      //쿨 다운 리스트
    public override void SetCorrectionValue(object obj, PlayerEvent e)
    {
        if (e.correctablility)  //공격력 보정치 주입
        {
            n_DamageRates.Enqueue(e.damageCorrection);
            UpdateAttackDamage();
        }

        if (e.unlockability)  //스킬 해금
        {
            int unlockNum = e.skillNumber;
            foreach (var skill in skills)
            {
                if (skill.unLockedNumber == unlockNum)  //해금
                {
                    skill.canUse = true;
                }
            }

            //UI스킬 해금 연출
        }
    }

    public override void SetRelicAttackValue(object obj, PlayerEvent e)  //유물의 공격 보정치 할당
    {
        if (e.isEquip)  //장착
        {
            attackDamageRates.Add(e.damageRate);
        }
        else  //해제
        {
            attackDamageRates.Remove(e.damageRate);
        }
        UpdateAttackDamage();
    }

    public override void SetCoolDownValue(object obj, PlayerEvent e)  //칩셋 쿨타임 감소 적용
    {
        if (e.isEquip)
        {
            coolDownRates.Add(e.coolDownRate);
        }
        else
        {
            coolDownRates.Remove(e.coolDownRate);
        }
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

    private void UpdateAttackDamage()
    {
        float n_DamageRate = 1f;
        float attackDamageRate = 1f;
        foreach (var rate in n_DamageRates)
        {
            n_DamageRate += rate;
        }
        foreach (var rate in attackDamageRates)  //피해 데미지 누적 덧셉
        {
            attackDamageRate += rate;
        }
        weapon.damage = (PlayerStatus.normalDamage * n_DamageRate) * weapon.damageRate * attackDamageRate;  //공격력 연산
        LogUtil.Log($"대상: {gameObject.name}, 총 공격력: {weapon.damage}, 누적 공격수치: {n_DamageRate}, 누적 피해 데미지: {attackDamageRate}");
        foreach (var skill in skills)
            skill.damage = (PlayerStatus.normalDamage * n_DamageRate) * skill.damageRate * attackDamageRate;
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
}

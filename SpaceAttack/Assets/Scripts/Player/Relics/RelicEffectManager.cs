using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicEffectManager : MonoBehaviour
{
    private static Dictionary<int, Action<bool, RelicInfo>> relicEffectMap;

    private void Awake()
    {
        relicEffectMap = new Dictionary<int, Action<bool, RelicInfo>>()   //유물 <ID, 장착여부>
        {
            { 100,  AttackRateBoost},
            { 101, C_ChanceToHeal},
            { 102, CritChanceBoost},
            { 104, EvasionRateBoost},
            { 105, MoveSpeedBoost},
            { 106, RelicDropRateBoost},
            { 107, SkillCoolDecrease}

        };
    }

    public static void ApplyRelicEffect(RelicSO relicSO, bool isEquip)     //해당 유물의 모든 효과실행 함수
    {
        foreach (int effectId in relicSO.relicEffects)   //유물 효과ID받기
        {
            if (relicEffectMap.TryGetValue(effectId, out var action))  //알맞은 유물ID 찾기
            {
                RelicInfo info = Array.Find(relicSO.relicInfos, p => p.id == effectId);   //유물 정보 찾기
                if (info == null)
                    LogUtil.LogError($"유물ID_{effectId}에 맞는 유물효과정보를 찾을 수 없습니다. {relicSO.name}유물을 확인해주세요");

                action.Invoke(isEquip, info);
            }
        }
    }

    private void AttackRateBoost(bool isEquip, RelicInfo info)         //100 - 공격력 n% 상승.
    {
        if (isEquip)
        {
            EventManager.playerEvent.SetRelicAttackValue(isEquip, info.n);
            LogUtil.Log("공격력 상승 + 정보 번호" + info.id);
        }
        else
        {
            EventManager.playerEvent.SetRelicAttackValue(isEquip, info.n);
            LogUtil.Log("공격력 상승효과 취소");
        }
    }

    private RelicInfo healToChanceInfo;
    private void C_ChanceToHeal(bool isEquip, RelicInfo info)          //101 - 피해를 받을 시 n%확률로 하트 z칸을 회복.
    {
        healToChanceInfo = info;
        if (isEquip) PlayerStatus.playerHitEvent += ChanceToHeal;
        else PlayerStatus.playerHitEvent -= ChanceToHeal;
    }
    private void ChanceToHeal()
    {
        RelicInfo info = healToChanceInfo;
        float randomValue = UnityEngine.Random.value;
        LogUtil.Log($"회복확률:{info.n}, 회복값: {info.z}, 랜덤값; {randomValue}");
        if (randomValue <= info.n)
        {
            LogUtil.Log("체력회복 성공");
            PlayerStatus.AddHp(info.z);  //플레이어 체력 회복
        }
        else LogUtil.Log("체력회복 실패");
    }

    private void CritChanceBoost(bool isEquip, RelicInfo info)           //102 - 치명타 확률이 n% 상승
    {
        if (isEquip)
        {
            PlayerStatus.criticalChanceRate *= (1 + info.n);
            LogUtil.Log($"치명타율 상승, 현재 치명타율: {PlayerStatus.criticalChanceRate}");
        }
        else
        {
            PlayerStatus.criticalChanceRate /= (1 + info.n);
            LogUtil.Log($"치명타율 상승 해제, 현재 치명타율: {PlayerStatus.criticalChanceRate}");
        }
    }

    private void EvasionRateBoost(bool isEquip, RelicInfo info)           //104 - 회피 확률이 n% 증가
    {
        if (isEquip)
        {
            PlayerStatus.missRate *= (1 + info.n);  //회피율 상승
            LogUtil.Log($"회피율 상승, 현재 회피율: {PlayerStatus.missRate}");
        }
        else
        {
            PlayerStatus.missRate /= (1 + info.n);
            LogUtil.Log($"회피율 상승 해제, 현재 회피율: {PlayerStatus.missRate}");
        }
    }

    private void MoveSpeedBoost(bool isEquip, RelicInfo info)              //105 - 이동 속도가 n% 증가
    {
        if (isEquip)
        {
            PlayerStatus.m_speed += (PlayerStatus.m_defultSpeed * info.n);
            LogUtil.Log($"이속 상승, 현재 이속: {PlayerStatus.m_speed}");
        }
        else
        {
            PlayerStatus.m_speed -= (PlayerStatus.m_defultSpeed * info.n);
            LogUtil.Log($"이속 상승 해제, 현재 이속: {PlayerStatus.m_speed}");
        }
    }

    private void RelicDropRateBoost(bool isEquip, RelicInfo info)            //106 - 오염된 프로세스 드랍 확률이 n% 증가
    {
        if (isEquip)
        {
            LogUtil.Log($"칩셋 드롭확률 상승, 상승 수치: {info.n}");
            DropRelicSystem.dropRate += info.n;
        }
        else
        {
            LogUtil.Log($"칩셋 드롭확률 상승 해제, 해제 수치: {info.n}");
            DropRelicSystem.dropRate -= info.n;
        }
    }

    private void SkillCoolDecrease(bool isEquip, RelicInfo info)              //107 - 스킬 재사용 대기 시간이 n% 감소
    {
        if (isEquip)
        {
            EventManager.playerEvent.SetCoolDownValue(isEquip, info.n);
            LogUtil.Log("칩스킬 쿨타임 감소");
        }
        else
        {
            EventManager.playerEvent.SetCoolDownValue(isEquip, info.n);
            LogUtil.Log("스킬 쿨타임 감소 해제");
        }
    }
}

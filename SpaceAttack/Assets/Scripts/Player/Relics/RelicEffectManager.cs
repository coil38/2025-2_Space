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
            {100, AttackRateBoost }, {101, MoreAttackChance}, { 102, CritChanceBoost},
            {103,  EvasionRateBoost}, {104, MoveSpeedBoost}, {105, WeaknessAnalyzer},
            {106, SkillCoolDecrease }, {107, AttackSpeedUp}, {108, CritDamageBoost},
            {109,  MaxHeart}, {110, BloodPower }, {111, GlassCannon},
            {112,  SkillDamageBosst}, {113, LifeToSkillPower}, {114, SkillCoolincrease},
            {115,  Damageincrease}, {116, GambleHit}, {117, MoveSpeedDecrease},
            {118,  LastReserve}, {119, FailSafe}, {120, OverloadBackflow},
            {121, BaseAttackDecrease }, {122, CritChanceDecrease}, {123, AttackSpeedDecrease},
            {124, SkillDamageDecrease }, {125, CritDamageDecrease}, {126, MaxHeartDecrease},
            {127,  InvincibleTimeAfterHitDecrease}, {128, DamageFromEliteBossPercent},
            {129,  DashDistanceDecrease}, {130, ItemDropChanceDecrease}, {131, ExpGainDecrease},
            {132,  BaseAttackSpeedOnHit}, {133, AttackSpeedOnKill}, {134, SkillDamageNextAttack},
            {135, CritDamageAttackPowerOnCrit }, {136, MaxHeartShieldOnStart}, {137, ShieldIfNoHit},
            {138, IgnoreEliteBossDamageChance }, {139, DashDistanceincrease}, {140, DashDistanceDamage},
            {141,  MoveSpeedDashBoost}, {142, SkillCooldownSlowWave}, {143, ItemDropBonusReward },
            {144,  ExpGainincrease}, {145, CorruptionOnLevelup}, {146, OverLoadCore},
            {147, Execute }, {148, NoHitShieldHeart}, {149, Resurrection}, {150, AutoCleanse},
            {151, CounterCore }, {152, DebuffImmunity}, {153, RelicDropRateBoost}, {154, CorruptionIncrease}
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

    //----------------------------------------------------------------------------------------------------------------------------------------

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

    private void MoreAttackChance(bool isEquip, RelicInfo info)         //101 - 모든 공격이 n% 확률로 데미지가 1 번 더 적용됩니다. (다단히트 공격은 각 타격마다 개별 적용)
    {
        if (isEquip)
        {

        }
        else
        {

        }
    }

    //private void OnMoreAttackChance()
    private void CritChanceBoost(bool isEquip, RelicInfo info) //102 - 치명타 확률이 n% 상승한다.
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
    private void EvasionRateBoost(bool isEquip, RelicInfo info) //103 - 회피 확률이 n% 증가합니다.
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
    private void MoveSpeedBoost(bool isEquip, RelicInfo info)   //104 - 이동 속도가 n% 증가합니다.
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
    private void WeaknessAnalyzer(bool isEquip, RelicInfo info) //105 - 적에게 n회 공격을 적중시킬 때마다, z초간 피해량이 y% 증가하는 '분석 완료' 상태가 됩니다. (중첩 w번)
    {

    }
    private void SkillCoolDecrease(bool isEquip, RelicInfo info)//106 - 모든 스킬 재사용 대기 시간이 n% 감소합니다.
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
    private void AttackSpeedUp(bool isEquip, RelicInfo info)    //107 - 공격 속도가 n% 증가합니다.
    {

    }
    private void CritDamageBoost(bool isEquip, RelicInfo info)  //108 - 치명타 피해량이 n% 증가합니다.
    {

    }
    private void MaxHeart(bool isEquip, RelicInfo info)         //109 - 최대 하트가 n칸 증가합니다.
    {

    }
    private void BloodPower(bool isEquip, RelicInfo info)       //110 - 잃은 하트 n칸 당 공격력이 z% 증가하지만, 최대 하트가 y칸 감소합니다.
    {

    }
    private void GlassCannon(bool isEquip, RelicInfo info)      //111 - 모든 공격의 피해량이 n% 증가합니다.
    {

    }
    private void SkillDamageBosst(bool isEquip, RelicInfo info) //112 - 스킬 피해량이 n% 증가합니다.
    {

    }
    private void LifeToSkillPower(bool isEquip, RelicInfo info) //113 - 스킬 사용 시 모든 하트 중 n 칸 소모하며, 해당 스킬의 피해량이 z% 증가합니다.
    {

    }
    private void SkillCoolincrease(bool isEquip, RelicInfo info) //114 - 스킬 재사용 대기시간이 n% 증가합니다.
    {

    }
    private void Damageincrease(bool isEquip, RelicInfo info)    //115 - 받는 모든 피해가 n배로 증가합니다.
    {

    }
    private void GambleHit(bool isEquip, RelicInfo info)         //116 - 공격 시 n% 확률로 z%의 피해를, y% 확률로 w%의 피해를 줍니다.
    {

    }
    private void MoveSpeedDecrease(bool isEquip, RelicInfo info) //117 - 이동 속도가 n% 감소합니다.
    {

    }
    private void LastReserve(bool isEquip, RelicInfo info)       //118 - 최대 하트가 n칸으로 고정되지만, z초마다 보호막 하트 y개를 얻습니다 (최대 w칸 중첩)
    {

    }
    private void FailSafe(bool isEquip, RelicInfo info)          //119 - 하트가 n칸일 때 공격 속도와 이동 속도가 z% y분간 증가하지만, 그 상태에서는 체력 회복이 불가능해집니다. (스테이지 당 w번)
    {

    }
    private void OverloadBackflow(bool isEquip, RelicInfo info)  //120 - 최대 하트가 n칸으로 고정 되는 대신 하트 잃을 때 마다 z% 확률로 하트 y칸을 회복한다.
    {

    }
    private void BaseAttackDecrease(bool isEquip, RelicInfo info)//121 - 기본 공격력이 n%감소합니다.
    {

    }
    private void CritChanceDecrease(bool isEquip, RelicInfo info)//122 - 치명타 확률이 n% 감소한다.
    {

    }
    private void AttackSpeedDecrease(bool isEquip, RelicInfo info)//123 - 공격 속도가 n% 감소합니다.
    {

    }
    private void SkillDamageDecrease(bool isEquip, RelicInfo info)//124 - 스킬 피해랴이 n% 감소합니다.
    {

    }
    private void CritDamageDecrease(bool isEquip, RelicInfo info) //125 - 치명타 데미지가 n% 감소합니다.
    {

    }
    private void MaxHeartDecrease(bool isEquip, RelicInfo info)   //126 - 최대 하트가 n칸 감소합니다.
    {

    }
    private void InvincibleTimeAfterHitDecrease(bool isEquip, RelicInfo info)//127 - 피격 후 발생하는 무적 시간이 n% 감소합니다.
    {

    }
    private void DamageFromEliteBossPercent(bool isEquip, RelicInfo info)//128 - 엘리트/보스 몬스터에게 받는 피해가 n%증가합니다.
    {

    }
    private void DashDistanceDecrease(bool isEquip, RelicInfo info) //129 - 대시의 이동 거리가 n% 감소합니다.
    {

    }
    private void ItemDropChanceDecrease(bool isEquip, RelicInfo info)//130 - 적 처치 시 아이템 획득 확률이 n% 감소합니다.
    {

    }
    private void ExpGainDecrease(bool isEquip, RelicInfo info)       //131 - 획득하는 경험치가 n% 감소합니다.
    {

    }
    private void BaseAttackSpeedOnHit(bool isEquip, RelicInfo info)  //132 - 공격 시 n% 확률로 z초간 공격 속도가 y% 증가 합니다.
    {

    }
    private void AttackSpeedOnKill(bool isEquip, RelicInfo info)     //133 - 적 처치 시 n초간 공격 속도가 추가로 z% 증가합니다.
    {

    }
    private void SkillDamageNextAttack(bool isEquip, RelicInfo info)     //134 - 스킬 사용 시 다음 기본 공격의 피해량이 n% 증가합니다.
    {

    }
    private void CritDamageAttackPowerOnCrit(bool isEquip, RelicInfo info)//135 - 치명타 발동 시 n초간 공격력이 z% 증가합니다.
    {

    }
    private void MaxHeartShieldOnStart(bool isEquip, RelicInfo info)      //136 - 스테이지 시작 시 하트 n칸 만큼의 보호막을 얻습니다.
    {

    }
    private void ShieldIfNoHit(bool isEquip, RelicInfo info)              //137 - n초 동안 피격당하지 않으면, 최대 하트 z칸 만큼의 보호막을 얻습니다.
    {

    }
    private void IgnoreEliteBossDamageChance(bool isEquip, RelicInfo info)//138 - 엘리트/보스 몬스터의 공격에 피격 시, n% 확률로 피해를 무시합니다.
    {

    }
    private void DashDistanceincrease(bool isEquip, RelicInfo info)       //139 - 대시의 이동 거리가 n% 증가합니다.
    {

    }
    private void DashDistanceDamage(bool isEquip, RelicInfo info)         //140 - 대시 중 통과하는 적에게 기본 공격력의 n% 피해를 줍니다.
    {

    }
    private void MoveSpeedDashBoost(bool isEquip, RelicInfo info)         //141 - 대시 사용 후 n초간 이동 속도가 추가로 z% 증가합니다.
    {

    }
    private void SkillCooldownSlowWave(bool isEquip, RelicInfo info)      //142 - 스킬 사용 시 주변에 일반/엘리트 몬스터를 느리게 하는 냉기 파동이 방출됩니다.
    {

    }
    private void ItemDropBonusReward(bool isEquip, RelicInfo info)        //143 - 적 처치 시 아이템 획득 확률이 n% 증가하고 z% 확률로 추가 보상을 획득합니다.
    {

    }
    private void ExpGainincrease(bool isEquip, RelicInfo info)             //144 - 획득하는 경험치가 n% 증가합니다.
    {

    }
    private void CorruptionOnLevelup(bool isEquip, RelicInfo info)         //145 - 레벨 업 시 증가하는 오염도 게이지가 n 추가로 증가합니다.
    {

    }
    private void OverLoadCore(bool isEquip, RelicInfo info)                //146 - 스킬 사용 시 다음 n회의 기본 공격이 강화되어 z% 피해 데미지가 증가합니다.
    {

    }
    private void Execute(bool isEquip, RelicInfo info)                     //147 - 체력이 n% 이하인 일반 몬스터를 즉시 처치합니다. (엘리트/보스 몬스터에게는 적용되지 않음)
    {

    }
    private void NoHitShieldHeart(bool isEquip, RelicInfo info)            //148 - n초 동안 피격당하지 않으면, 추가 하트의 보호막이 z개 추가 됩니다. (최대 y개)
    {

    }
    private void Resurrection(bool isEquip, RelicInfo info)                //149 - 죽었을 경우 하트 n개를 생성되며, z초간 무적 상태가 됩니다. (게임 진행 간 y회)
    {

    }
    private void AutoCleanse(bool isEquip, RelicInfo info)                 //150 - 하트를 잃을때 마다 n%확률로 부여된 해로운 효과 z개를 제거합니다.
    {

    }
    private void CounterCore(bool isEquip, RelicInfo info)                 //151 - 하트를 읽을때 마다 n% 확률로 zm범위의 몬스터들을 경직 시킵니다. (보스 몬스터에게는 적용되지 않음)
    {

    }
    private void DebuffImmunity(bool isEquip, RelicInfo info)              //152 - 구속을 제외한 모든 디버프에 면역 상태가 됩니다.
    {

    }
    private void RelicDropRateBoost(bool isEquip, RelicInfo info)          //153 - 오염된 프로세스 드랍 확률이 n% 증가합니다.
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
    private void CorruptionIncrease(bool isEquip, RelicInfo info)          //154 - 플레이어 오염도 수치가 n 증가합니다.
    {

    }

}

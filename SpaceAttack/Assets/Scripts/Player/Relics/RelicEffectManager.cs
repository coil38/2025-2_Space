using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicEffectManager : MonoBehaviour
{
    private static Dictionary<int, Type> relicEffectMap;
    private static Dictionary<int, RelicEffectType[]> relicEffects = new Dictionary<int, RelicEffectType[]>();

    private void Awake()
    {
        relicEffectMap = new Dictionary<int, Type>()   //유물 <ID, 장착여부>
        {
            {100, typeof(AttackRateBoost) }, {101, typeof(MoreAttackChance)}, { 102, typeof(CritChanceBoost)},
            {103,  typeof(EvasionRateBoost)}, {104, typeof(MoveSpeedBoost)}, {105, typeof(WeaknessAnalyzer)},
            {106, typeof(SkillCoolDecrease) }, {107, typeof(AttackSpeedUp)}, {108, typeof(CritDamageBoost)},
            {109,  typeof(MaxHeart)}, {110, typeof(BloodPower) }, {111, typeof(GlassCannon)},
            {112,  typeof(SkillDamageBosst)}, {113, typeof(LifeToSkillPower)}, {114, typeof(SkillCoolincrease)},
            {115,  typeof(Damageincrease)}, {117, typeof(GambleHit)}, {118, typeof(MoveSpeedDecrease)},
            {119,  typeof(LastReserve)}, {120, typeof(FailSafe)}, {121, typeof(OverloadBackflow)},
            {122, typeof(BaseAttackDecrease) }, {123, typeof(CritChanceDecrease)}, {124, typeof(AttackSpeedDecrease)},
            {125, typeof(SkillDamageDecrease) }, {126, typeof(CritDamageDecrease)}, {127, typeof(MaxHeartDecrease)},
            {128,  typeof(InvincibleTimeAfterHitDecrease)}, {129, typeof(DamageFromEliteBossPercent)},
            {130,  typeof(DashDistanceDecrease)}, {131, typeof(ItemDropChanceDecrease)}, {132, typeof(ExpGainDecrease)},
            {133,  typeof(BaseAttackSpeedOnHit)}, {134, typeof(AttackSpeedOnKill)}, {135, typeof(SkillDamageNextAttack)},
            {136, typeof(CritDamageAttackPowerOnCrit) }, {137, typeof(MaxHeartShieldOnStart)}, {138, typeof(ShieldIfNoHit)},
            {139, typeof(IgnoreEliteBossDamageChance) }, {140, typeof(DashDistanceincrease)}, {141, typeof(DashDistanceDamage)},
            {142,  typeof(MoveSpeedDashBoost)}, {143, typeof(SkillCooldownSlowWave)}, {144, typeof(ItemDropBonusReward) },
            {145,  typeof(ExpGainincrease)}, {146, typeof(CorruptionOnLevelup)}, {147, typeof(OverLoadCore)},
            {148, typeof(Execute) }, {149, typeof(NoHitShieldHeart)}, {150, typeof(Resurrection)}, {151, typeof(AutoCleanse)},
            {152, typeof(CounterCore) }, {153, typeof(DebuffImmunity)}, {154, typeof(RelicDropRateBoost)}, {155, typeof(CorruptionIncrease)}
        };
    }

    public static void ApplyRelicEffect(RelicSO relicSO, bool isEquip, int relicInstanceId)     //해당 유물의 모든 효과실행 함수
    {
        int effectCount = 0;
        RelicEffectType[] effects = new RelicEffectType[relicSO.relicEffects.Length];

        if (!relicEffects.TryGetValue(relicInstanceId, out var instance))
            relicEffects.Add(relicInstanceId, new RelicEffectType[relicSO.relicEffects.Length]);//새로운 인스턴스 일 경우, 효과 배열 생성


        foreach (int effectId in relicSO.relicEffects)   //유물 효과ID받기
        {
            RelicInfo info = Array.Find(relicSO.relicInfos, p => p.id == effectId);   //유물 정보 찾기
            if (info == null)
                LogUtil.LogError($"유물ID_{effectId}에 맞는 유물효과정보를 찾을 수 없습니다. {relicSO.name}유물을 확인해주세요");

            if (relicEffects.TryGetValue(relicInstanceId, out var instances))
            {
                if(isEquip)
                {
                    if (relicEffectMap.TryGetValue(effectId, out var value))  //알맞은 유물효과 인스턴스 찾기
                    {
                        effects[effectCount] = (RelicEffectType)Activator.CreateInstance(value);
                        effects[effectCount].Excute(isEquip, info);
                        relicEffects[relicInstanceId] = effects;
                        LogUtil.Log($"장착여부: {isEquip}, 인스턴스번호: {relicInstanceId}, 유물이름: {relicSO.relicName}");
                    }
                }
                else
                {
                    instances[effectCount].Excute(isEquip, info);
                    LogUtil.Log($"장착여부: {isEquip} 인스턴스번호: {relicInstanceId}, 유물이름: {relicSO.relicName}");
                }
            }
            effectCount++;
        }
        if (!isEquip) relicEffects.Remove(relicInstanceId);            //장착해제일 경우, 기록 데이터 제거
    }

    //--------------------------------------------------------------각 유물 효과 내부 코드------------------------------------------------------------------
    private class AttackRateBoost : RelicEffectType                        //100 - 공격력 n% 상승.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetChipAttackRate(isEquip, info.n);
            LogUtil.Log($"공격력 변경");
        }
    }

    private class MoreAttackChance : RelicEffectType                    //101 - 모든 공격이 n% 확률로 데미지가 1 번 더 적용됩니다. (다단히트 공격은 각 타격마다 개별 적용)
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) AttackEventManager.OnAttackStarted += OnMoreAttackChance;
            else AttackEventManager.OnAttackStarted -= OnMoreAttackChance;
        }

        private void OnMoreAttackChance(AttackContext cotext)
        {
            LogUtil.Log("다단계 공격 시도");
            float randomValue = UnityEngine.Random.value;
            if (randomValue <= relicInfo.n)
            {
                LogUtil.Log("다단계 공격 성공");
                cotext.IsReattack = true;
            }
        }
    }

    private class CritChanceBoost: RelicEffectType          //102 - 치명타 확률이 n% 상승한다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.criticalChanceRate += info.n;
            else PlayerStatus.criticalChanceRate -= info.n;
            LogUtil.Log($"치명타율 변경, 현재 치명타율: {PlayerStatus.criticalChanceRate}");
        }
    }

    private class EvasionRateBoost : RelicEffectType           //103 - 회피 확률이 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.missRate += info.n;  //회피율 상승
            else PlayerStatus.missRate -= info.n;
            LogUtil.Log($"회피율 변경, 현재 회피율: {PlayerStatus.missRate}");
        }
    }
    private class MoveSpeedBoost : RelicEffectType               //104 - 이동 속도가 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.m_speed += (DataManager.instance.i_speed * info.n);
            else PlayerStatus.m_speed -= (DataManager.instance.i_speed * info.n);
            LogUtil.Log($"이속 변경, 현재 이속: {PlayerStatus.m_speed}");
        }
    }
    private class WeaknessAnalyzer : RelicEffectType            //105 - 적에게 n회 공격을 적중시킬 때마다, z초간 피해량이 y% 증가하는 '분석 완료' 상태가 됩니다. (중첩 w번)
    {
        private int analyzerCount = 0;
        private int analyzerCumCount = 0;

        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.playerAttckEvent += OnWeaknessAnalyzer;
            else
            {
                RelicEvent.playerAttckEvent -= OnWeaknessAnalyzer;
                analyzerCount = 0;
                analyzerCumCount = 0;
                TimerEvent.RemoveAll(OffWeaknessAnalyzerBuff);
            }
        }
        private void OnWeaknessAnalyzer()
        {
            if (analyzerCumCount >= relicInfo.w) return;

            if (analyzerCount >= relicInfo.n)
            {
                if (DamageEffectManager.instance) 
                    DamageEffectManager.instance.ShowWeaknessAnalyzer(true, analyzerCumCount + 1);

                LogUtil.Log($"적에게 {relicInfo.n}회 공격을 적중시킬 때마다, {relicInfo.z}초간 피해량이 {relicInfo.y}% 증가");
                EventManager.playerEvent.SetChipDamageRate(true, relicInfo.y);   //피해량 증가 적용
                TimerEvent.Add(relicInfo.z, OffWeaknessAnalyzerBuff);  //일정시간후, 자동 피해량 감소
                analyzerCount = 0;
                analyzerCumCount++;
            }
            analyzerCount++;
        }

        private void OffWeaknessAnalyzerBuff()
        {
            if (DamageEffectManager.instance)
                DamageEffectManager.instance.ShowWeaknessAnalyzer(false);
            EventManager.playerEvent.SetChipDamageRate(false, relicInfo.y);
            analyzerCumCount--;
        }
    }

    private class SkillCoolDecrease : RelicEffectType               //106 - 모든 스킬 재사용 대기 시간이 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetCoolDownRate(isEquip, info.n);
            LogUtil.Log("칩스킬 쿨타임 변경");
        }
    }

    private class AttackSpeedUp : RelicEffectType               //107 - 공격 속도가 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetAttackTimeRate(isEquip, info.n);
        }
    }

    private class CritDamageBoost : RelicEffectType               //108 - 치명타 피해량이 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.criticalRate += info.n;
            else PlayerStatus.criticalRate -= info.n;
        }
    }
    private class MaxHeart : RelicEffectType                //109 - 최대 하트가 n칸 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            PlayerStatus.ChangeMaxHp(isEquip, (int)info.n);
        }
    }

    private class BloodPower : RelicEffectType               //110 - 잃은 하트 n칸 당 공격력이 z% 증가하지만, 최대 하트가 y칸 감소합니다.
    {
        private int bloodPowerAmount = 0;
        private int bloodPowerCount = 0;
        private int losedHp = 0;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            PlayerStatus.ChangeMaxHp(!isEquip, (int)(info.y * 2));
            if (isEquip)
            {
                RelicEvent.playerLoseHpEvent += OnBloodPower;
                losedHp = PlayerStatus.GetLosedHp();
            }
            else
            {
                RelicEvent.playerLoseHpEvent -= OnBloodPower;
                PlayerStatus.RecoverLosedHp(losedHp);                       //잃어버린 체력 되돌리기
                bloodPowerAmount = 0;
                EventManager.playerEvent.SetChipAttackRate(false, relicInfo.z * bloodPowerCount);
                bloodPowerCount = 0;
            }
            
            LogUtil.Log(info.y);
        }

        private void OnBloodPower(int amount)
        {
            bloodPowerAmount += amount;
            if (bloodPowerAmount / 2 >= relicInfo.n)
            {
                LogUtil.Log($"누적 피해 : {bloodPowerAmount}, 공격력 수치: {relicInfo.z}, 받은 데미지: {amount}");
                EventManager.playerEvent.SetChipAttackRate(true, relicInfo.z);
                bloodPowerAmount -= (int)(relicInfo.n * 2);
                bloodPowerCount++;
            }
        }
    }
    private class GlassCannon : RelicEffectType               //111 - 모든 공격의 피해량이 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetChipDamageRate(isEquip, info.n);
        }
    }

    private class SkillDamageBosst : RelicEffectType               //112 - 스킬 피해량이 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetChipSkillDamageRate(isEquip, info.n);
        }
    }
    private class LifeToSkillPower : RelicEffectType               //113 - 스킬 사용 시 모든 하트 중 n 칸 소모하며, 해당 스킬의 피해량이 z% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                RelicEvent.playerUseSkillEvent += LoseHp_LifeToSkillPower;
                AttackEventManager.OnAttackStarted += AddSkillDamage_LifeToSkillPower;
            }
            else
            {
                RelicEvent.playerUseSkillEvent -= LoseHp_LifeToSkillPower;
                AttackEventManager.OnAttackStarted -= AddSkillDamage_LifeToSkillPower;
            }
        }
        private void LoseHp_LifeToSkillPower()
        {
            if (PlayerStatus.Instance != null)
                PlayerStatus.Instance.ReduceHp((int)(relicInfo.n * 2));
        }
        private void AddSkillDamage_LifeToSkillPower(AttackContext context)
        {
            if (context.attackType == ChipAttackType.Skill)
                context.damageRateSume *= (1 + relicInfo.z);
        }
    }
    private class SkillCoolincrease : RelicEffectType               //114 - 스킬 재사용 대기시간이 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetCoolDownRate(!isEquip, info.n);
        }
    }
    private class Damageincrease : RelicEffectType               //115 - 받는 모든 피해가 n배로 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            float value = Math.Max(DataManager.instance.i_hitRate, info.n);
            if (isEquip) PlayerStatus.hitRate = value;
            else PlayerStatus.hitRate = DataManager.instance.i_hitRate;
        }
    }
    private class GambleHit : RelicEffectType               //117 - 공격 시 n% 확률로 z배의 피해를, y% 확률로 w배의 피해를 줍니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) AttackEventManager.OnAttackStarted += OnGamleHit;
            else AttackEventManager.OnAttackStarted -= OnGamleHit;
        }
        private void OnGamleHit(AttackContext cotext)
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue <= relicInfo.n)
            {
                cotext.damageRateSume *= relicInfo.z;
            }
            else
            {
                cotext.damageRateSume *= relicInfo.w;
            }
        }
    }
    private class MoveSpeedDecrease : RelicEffectType               //118 - 이동 속도가 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.m_speed -= DataManager.instance.i_speed * info.n;
            else PlayerStatus.m_speed += DataManager.instance.i_speed * info.n;
        }
    }
    private class LastReserve : RelicEffectType               //119 - 최대 하트가 n칸으로 고정되지만, z초마다 보호막 하트 y개를 얻습니다 (최대 w칸 중첩)
    {
        private int lastReserveCumCount = 0;
        private int losedHp = 0;
        private int changedMaxHp = 0;
        private bool isAdd;
        private int addedShild = 0;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                changedMaxHp = Mathf.Abs((int)info.n - PlayerStatus.m_maxhp);
                isAdd = info.n > PlayerStatus.m_maxhp;
                PlayerStatus.ChangeMaxHp(isAdd, changedMaxHp);
                if (!isAdd) losedHp = PlayerStatus.losedHp;
                PlayerStatus.maxHpFixing = true;

                TimerEvent.Add(info.z, OnLastReserve);
            }
            else
            {
                PlayerStatus.maxHpFixing = false;
                PlayerStatus.ChangeMaxHp(!isAdd, changedMaxHp);
                PlayerStatus.RecoverLosedHp(losedHp);
                PlayerStatus.ChangeShildHp(false, addedShild);

                TimerEvent.Remove(OnLastReserve);
            }
        }
        private void OnLastReserve()
        {
            PlayerStatus.ChangeShildHp(true, (int)(relicInfo.y * 2));
            addedShild += (int)(relicInfo.y * 2);
            lastReserveCumCount++;
            if (lastReserveCumCount < relicInfo.w)
                TimerEvent.Add(relicInfo.z, OnLastReserve);
        }
    }

    private class FailSafe : RelicEffectType               //120 - 하트가 n칸일 때 공격 속도와 이동 속도가 z% y분간 증가하지만, 그 상태에서는 체력 회복이 불가능해집니다. (스테이지 당 w번)
    {
        private int failSafeCumCount = 0;
        private bool isRunning;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if(isEquip)
            {
                OnFailSafe(0);
                RelicEvent.playerLoseHpEvent += OnFailSafe;
                RelicEvent.startStageEvent += InitialFailSafe;
            }
            else
            {
                RelicEvent.playerLoseHpEvent -= OnFailSafe;
                RelicEvent.startStageEvent -= InitialFailSafe;
                TimerEvent.Remove(OffFailSafe);
                if(isRunning) OffFailSafe();
            }
        }
        private void OnFailSafe(int amount)
        {
            if (failSafeCumCount >= relicInfo.w) return;

            if (isRunning) return;
            if (PlayerStatus.m_hp / 2 + (PlayerStatus.m_hp % 2 == 1 ? 1 : 0) <= relicInfo.n)
            {
                PlayerStatus.cannotHealing = true;
                EventManager.playerEvent.SetAttackTimeRate(true, relicInfo.z);
                PlayerStatus.m_speed += DataManager.instance.i_speed * relicInfo.z;
                TimerEvent.Add(relicInfo.y * 60, OffFailSafe);

                isRunning = true;
                failSafeCumCount++;
            }
        }
        private void InitialFailSafe()
        {
            failSafeCumCount = 0;
        }
        private void OffFailSafe()
        {
            EventManager.playerEvent.SetAttackTimeRate(false, relicInfo.z);
            PlayerStatus.m_speed -= DataManager.instance.i_speed * relicInfo.z;
            PlayerStatus.cannotHealing = false;
            isRunning = false;
        }
    }
    private class OverloadBackflow : RelicEffectType               //121 - 최대 하트가 n칸으로 고정 되는 대신 하트 잃을 때 마다 z% 확률로 하트 y칸을 회복한다.
    {
        private int losedHp;
        private int changedMaxHp;
        bool isAdding;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                isAdding = info.n * 2 >= PlayerStatus.m_maxhp;
                changedMaxHp = Math.Abs(PlayerStatus.m_maxhp - (int)info.n * 2);
                PlayerStatus.ChangeMaxHp(isAdding, changedMaxHp);
                losedHp = PlayerStatus.GetLosedHp();
                RelicEvent.playerLoseHpEvent += OnOverloadBackflow;
            }
            else
            {
                PlayerStatus.ChangeMaxHp(!isAdding, changedMaxHp);
                PlayerStatus.RecoverLosedHp(losedHp);
                RelicEvent.playerLoseHpEvent -= OnOverloadBackflow;
            }
        }
        private void OnOverloadBackflow(int amount)
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue <= relicInfo.z) PlayerStatus.AddHp((int)(relicInfo.y * 2));
        }
    }
    private class BaseAttackDecrease : RelicEffectType               //122 - 기본 공격력이 n%감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetChipAttackRate(!isEquip, info.n);
        }
    }
    private class CritChanceDecrease : RelicEffectType               //123 - 치명타 확률이 n% 감소한다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.criticalChanceRate = Math.Max(0, PlayerStatus.criticalChanceRate - info.n);
            else PlayerStatus.criticalChanceRate += info.n;
        }
    }
    private class AttackSpeedDecrease : RelicEffectType              //124 - 공격 속도가 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetAttackTimeRate(!isEquip, info.n);
        }
    }
    private class SkillDamageDecrease : RelicEffectType               //125 - 스킬 피해량이 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            EventManager.playerEvent.SetChipSkillDamageRate(!isEquip, info.n);
        }
    }
    private class CritDamageDecrease : RelicEffectType               //126 - 치명타 데미지가 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.criticalRate = Math.Max(0, PlayerStatus.criticalRate - info.n);
            else PlayerStatus.criticalRate +=  info.n;
        }
    }
    private class MaxHeartDecrease : RelicEffectType               //127 - 최대 하트가 n칸 감소합니다.
    {
        private int losedHp;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            PlayerStatus.ChangeMaxHp(!isEquip, (int)(info.n * 2));
            if (isEquip) losedHp = PlayerStatus.GetLosedHp();
            else PlayerStatus.RecoverLosedHp(losedHp);
        }
    }
    private class InvincibleTimeAfterHitDecrease : RelicEffectType               //128 - 피격 후 발생하는 무적 시간이 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerTimeSystem.SetStunTimer(PlayerTimeSystem.m_stunTime - DataManager.instance.i_m_stunTime * info.n);
            else PlayerTimeSystem.SetStunTimer(PlayerTimeSystem.m_stunTime + DataManager.instance.i_m_stunTime * info.n);
        }
    }
    private class DamageFromEliteBossPercent : RelicEffectType               //129 - 엘리트/보스 몬스터에게 받는 피해가 n%증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                RelicEvent.playerHitEventStart += OnDamageFromEliteBossPercentInfo;
                RelicEvent.playerHitEventEnd += OffDamageFromEliteBossPercentInfo;
            }
            else
            {
                RelicEvent.playerHitEventStart -= OnDamageFromEliteBossPercentInfo;
                RelicEvent.playerHitEventEnd -= OffDamageFromEliteBossPercentInfo;
            }
        }
        private void OnDamageFromEliteBossPercentInfo(int damage, GameObject attacker)
        {
            if (attacker == null) return;
            if (attacker.CompareTag("Boss") || attacker.CompareTag("Elite"))
            {
                PlayerStatus.hitRate += relicInfo.n;
            }

        }
        private void OffDamageFromEliteBossPercentInfo()
        {
            PlayerStatus.hitRate -= relicInfo.n;
        }
    }
    private class DashDistanceDecrease : RelicEffectType               //130 - 대시의 이동 거리가 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.m_DashDistance -= DataManager.instance.i_DashDistance * info.n;
            else PlayerStatus.m_DashDistance += DataManager.instance.i_DashDistance * info.n;
        }
    }
    private class ItemDropChanceDecrease : RelicEffectType               //131 - 적 처치 시 아이템 획득 확률이 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);

        }
    }
    private class ExpGainDecrease : RelicEffectType               //132 - 획득하는 경험치가 n% 감소합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) DropEXPSystem.dropExpCount -= (int)(DropEXPSystem.i_dropExpCount * info.n);
            else DropEXPSystem.dropExpCount += (int)(DropEXPSystem.i_dropExpCount * info.n);
        }
    }
    private class BaseAttackSpeedOnHit : RelicEffectType                //133 - 공격 시 n% 확률로 z초간 공격 속도가 y% 증가 합니다.
    {
        bool isAttackSpeedUped;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.playerAttckEvent += OnBaseAttackSpeedOnHit;
            else
            {
                RelicEvent.playerAttckEvent -= OnBaseAttackSpeedOnHit;
                TimerEvent.Remove(OffBaseAttackSpeedOnHit);
                if (isAttackSpeedUped) OffBaseAttackSpeedOnHit();
            }
        }
        private void OnBaseAttackSpeedOnHit()
        {
            if (isAttackSpeedUped) return;

            float randomValue = UnityEngine.Random.value;
            if (randomValue <= relicInfo.n)
            {
                isAttackSpeedUped = true;
                if (DamageEffectManager.instance != null)
                    DamageEffectManager.instance.ShowAttackSpeedUp(isAttackSpeedUped);
                EventManager.playerEvent.SetAttackTimeRate(true, relicInfo.y); //공격 속도 상승
                TimerEvent.Add(relicInfo.z, OffBaseAttackSpeedOnHit);
            }
        }
        private void OffBaseAttackSpeedOnHit()
        {
            EventManager.playerEvent.SetAttackTimeRate(false, relicInfo.y); //공격속도 감소
            isAttackSpeedUped = false;
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowAttackSpeedUp(isAttackSpeedUped);
        }
    }
    private class AttackSpeedOnKill : RelicEffectType               //134 - 적 처치 시 n초간 공격 속도가 추가로 z% 증가합니다.
    {
        bool isAttackSpeedUped;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.killedEnemyEvent += OnAttackSpeedOnKill;
            else
            {
                RelicEvent.killedEnemyEvent -= OnAttackSpeedOnKill;
                TimerEvent.Remove(OffOnAttackSpeedOnKill);
                if (isAttackSpeedUped) OffOnAttackSpeedOnKill();
            }
        }
        private void OnAttackSpeedOnKill()
        {
            if (isAttackSpeedUped) return;

            isAttackSpeedUped = true;
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowAttackSpeedUp(isAttackSpeedUped);
            EventManager.playerEvent.SetAttackTimeRate(true, relicInfo.z); //공격 속도 상승
            TimerEvent.Add(relicInfo.n, OffOnAttackSpeedOnKill);
        }
        private void OffOnAttackSpeedOnKill()
        {
            isAttackSpeedUped = false;
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowAttackSpeedUp(isAttackSpeedUped);
            EventManager.playerEvent.SetAttackTimeRate(false, relicInfo.z); //공격속도 감소
        }
    }
    private class SkillDamageNextAttack : RelicEffectType               //135 - 스킬 사용 시 다음 기본 공격의 피해량이 n% 증가합니다.
    {
        private bool isUpgraded = false;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                RelicEvent.playerUseSkillEvent += OnSkillDamageNextAttack;
                AttackEventManager.OnAttackStarted += CheckSkillDamageNextAttack;
            }
            else
            {
                RelicEvent.playerUseSkillEvent -= OnSkillDamageNextAttack;
                AttackEventManager.OnAttackStarted -= CheckSkillDamageNextAttack;
            }
        }
        private void OnSkillDamageNextAttack()
        {
            isUpgraded = true;
        }
        private void CheckSkillDamageNextAttack(AttackContext context)
        {
            if (!isUpgraded) return;

            if (context.attackType == ChipAttackType.Weapon)
                context.weaponDamagekRateSume *= ( 1+ relicInfo.n);
            isUpgraded = false;
        }
    }
    private class CritDamageAttackPowerOnCrit : RelicEffectType               //136 - 치명타 발동 시 n초간 공격력이 z% 증가합니다.
    {
        bool attackUp;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.criticalEvent += OnCritDamageAttackPowerOnCrit;
            else
            {
                RelicEvent.criticalEvent -= OnCritDamageAttackPowerOnCrit;
                TimerEvent.Remove(OffCritDamageAttackPowerOnCrit);
                if (attackUp) OffCritDamageAttackPowerOnCrit();
            }
        }
        private void OnCritDamageAttackPowerOnCrit()
        {
            if (attackUp) return;

            attackUp = true;
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowAttackValueUp(attackUp);
            EventManager.playerEvent.SetChipAttackRate(true, relicInfo.z);
            TimerEvent.Add(relicInfo.n, OffCritDamageAttackPowerOnCrit);
        }
        private void OffCritDamageAttackPowerOnCrit()
        {
            attackUp = false;
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowAttackValueUp(attackUp);
            EventManager.playerEvent.SetChipAttackRate(false, relicInfo.z);
        }
    }
    private class MaxHeartShieldOnStart : RelicEffectType               //137 - 스테이지 시작 시 하트 n칸 만큼의 보호막을 얻습니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.startStageEvent += OnMaxHeartShieldOnStart;
            else RelicEvent.startStageEvent -= OnMaxHeartShieldOnStart;
        }
        private void OnMaxHeartShieldOnStart()
        {
            PlayerStatus.ChangeShildHp(true, (int)relicInfo.n);
        }
    }
    private class ShieldIfNoHit : RelicEffectType               //138 - n초 동안 피격당하지 않으면, 최대 하트 z칸 만큼의 보호막을 얻습니다.
    {
        private int addedShield;
        private bool isAdded;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                RelicEvent.playerHitEventStart += OffShieldIfNoHit;
                TimerEvent.Add(info.n, OnShieldIfNoHit);
            }
            else
            {
                RelicEvent.playerHitEventStart -= OffShieldIfNoHit;
                PlayerStatus.ChangeShildHp(false, addedShield);
                TimerEvent.Remove(OnShieldIfNoHit);

                if (addedShield > 0 && DamageEffectManager.instance != null)
                    DamageEffectManager.instance.ShowGetShild(false);
            }
        }
        private void OnShieldIfNoHit()  //보호막 생성 함수
        {
            isAdded = true;
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowGetShild(true);
            PlayerStatus.ChangeShildHp(true, (int)(relicInfo.z * 2));
            addedShield += (int)(relicInfo.z * 2);
        }
        private void OffShieldIfNoHit(int damage, GameObject attacker)  //보호막 생성 취소 후, 다시 생성 함수
        {
            if (isAdded) return;
            TimerEvent.Remove(OnShieldIfNoHit);
            TimerEvent.Add(relicInfo.n, OnShieldIfNoHit);
        }
    }
    private class IgnoreEliteBossDamageChance : RelicEffectType               //139 - 엘리트/보스 몬스터의 공격에 피격 시, n% 확률로 피해를 무시합니다.
    {
        float currentHitRate;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                RelicEvent.playerHitEventStart += OnIgnoreEliteBossDamageChance;
                RelicEvent.playerHitEventEnd += OffIgnoreEliteBossDamageChance;
            }
            else
            {
                RelicEvent.playerHitEventStart -= OnIgnoreEliteBossDamageChance;
                RelicEvent.playerHitEventEnd -= OffIgnoreEliteBossDamageChance;
            }
        }
        private void OnIgnoreEliteBossDamageChance(int damage, GameObject attacker)
        {
            if (attacker == null) return;

            if (attacker.CompareTag("Boss") || attacker.CompareTag("Elite"))
            {
                float randomValue = UnityEngine.Random.value;
                if (randomValue <= relicInfo.n)
                {
                    currentHitRate = PlayerStatus.hitRate;
                    PlayerStatus.hitRate = 0;
                    if (DamageEffectManager.instance != null)
                        DamageEffectManager.instance.ShowMiss();
                }
            }
        }
        private void OffIgnoreEliteBossDamageChance()
        {
            if (currentHitRate == 0) return;

            PlayerStatus.hitRate = currentHitRate;
        }
    }
    private class DashDistanceincrease : RelicEffectType               //140 - 대시의 이동 거리가 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerStatus.m_DashDistance += DataManager.instance.i_DashDistance * info.n;
            else PlayerStatus.m_DashDistance -= DataManager.instance.i_DashDistance * info.n;
        }
    }
    private class DashDistanceDamage : RelicEffectType                //141 - 대시 중 통과하는 적에게 기본 공격력의 n% 피해를 줍니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            //////////////////보류!!!!!!!!!!!!!!!!!!!!111
        }
    }
    private class MoveSpeedDashBoost : RelicEffectType               //142 - 대시 사용 후 n초간 이동 속도가 추가로 z% 증가합니다.
    {
        bool isAdded;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.dashEvent += OnMoveSpeedDashBoost;
            else RelicEvent.dashEvent -= OnMoveSpeedDashBoost;
        }
        private void OnMoveSpeedDashBoost()
        {
            if (isAdded) return;
            isAdded = true;
            if (DamageEffectManager.instance)
                DamageEffectManager.instance.ShowSpeedUp(isAdded);
            PlayerStatus.m_speed += DataManager.instance.i_speed * relicInfo.z;
            TimerEvent.Add(relicInfo.n, OffMoveSpeedDashBoostInfo);
        }
        private void OffMoveSpeedDashBoostInfo()
        {
            isAdded = false;
            if (DamageEffectManager.instance)
                DamageEffectManager.instance.ShowSpeedUp(isAdded);
            PlayerStatus.m_speed -= DataManager.instance.i_speed * relicInfo.z;
        }
    }
    private class SkillCooldownSlowWave : RelicEffectType               //143 - 스킬 사용 시 주변에 일반/엘리트 몬스터를 느리게 하는 냉기 파동이 방출됩니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            //////////////////보류!!!!!!!!!!!!!!!!!!!!111
        }
    }
    private class ItemDropBonusReward : RelicEffectType               //144 - 적 처치 시 아이템 획득 확률이 n% 증가하고 z% 확률로 추가 보상을 획득합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            //////////////////보류!!!!!!!!!!!!!!!!!!!!111
        }
    }
    private class ExpGainincrease : RelicEffectType               //145 - 획득하는 경험치가 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) DropEXPSystem.dropExpCount += (int)(DropEXPSystem.i_dropExpCount * info.n);
            else DropEXPSystem.dropExpCount -= (int)(DropEXPSystem.i_dropExpCount * info.n);
        }
    }
    private class CorruptionOnLevelup : RelicEffectType               //146 - 레벨 업 시 증가하는 오염도 게이지가 n 추가로 증가합니다.
    {
        private int addedDarkMatCount;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) PlayerEvent.levelUpEventHandler += OnCorruptionOnLevelup;
            else
            {
                PlayerEvent.levelUpEventHandler -= OnCorruptionOnLevelup;
                PlayerStatus.ChangeMaxDarkMatCount(false, addedDarkMatCount);
                addedDarkMatCount = 0;
            }
        }
        private void OnCorruptionOnLevelup(object obj, PlayerEvent e)
        {
            PlayerStatus.ChangeMaxDarkMatCount(true, (int)relicInfo.n);
            addedDarkMatCount += (int)relicInfo.n;
        }
    }
    private class OverLoadCore : RelicEffectType               //147 - 스킬 사용 시 다음 n회의 기본 공격이 강화되어 z% 피해 데미지가 증가합니다.
    {
        private int overLoadCoreCount;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                RelicEvent.playerUseSkillEvent += OnOverLoadCore;
                AttackEventManager.OnAttackStarted += OnOverLoadCore2;
            }
            else
            {
                RelicEvent.playerUseSkillEvent -= OnOverLoadCore;
                AttackEventManager.OnAttackStarted -= OnOverLoadCore2;
                overLoadCoreCount = 0;
            }
        }
        private void OnOverLoadCore()  //스킬 사용시, 강공 가능 상태로 전환
        {
            if (overLoadCoreCount > 0) return;
            overLoadCoreCount = (int)relicInfo.n;
        }
        private void OnOverLoadCore2(AttackContext context) //기본 공격시, 강공 적용
        {
            if (overLoadCoreCount <= 0) return;
            if (context.attackType == ChipAttackType.Weapon)
            {
                context.damageRate *= (1 + relicInfo.z);
                overLoadCoreCount--;
            }
        }
    }
    private class Execute : RelicEffectType               //148 - 체력이 n% 이하인 일반 몬스터를 즉시 처치합니다. (엘리트/보스 몬스터에게는 적용되지 않음)
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            //////////////////보류!!!!!!!!!!!!!!!!!!!!111
        }
    }
    private class NoHitShieldHeart : RelicEffectType               //149 - n초 동안 피격당하지 않으면, 추가 하트의 보호막이 z개 추가 됩니다. (최대 y개)
    {
        int noHitShieldHeartCum;
        int addedShildCount;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip)
            {
                RelicEvent.playerHitEventStart += OffNoHitShieldHeart;
                TimerEvent.Add(info.n, OnNoHitShieldHeart);
            }
            else
            {
                RelicEvent.playerHitEventStart -= OffNoHitShieldHeart;
                PlayerStatus.ChangeShildHp(false, addedShildCount);
                TimerEvent.RemoveAll(OnNoHitShieldHeart);
            }
        }
        private void OnNoHitShieldHeart()  //보호막 생성 함수
        {
            if (noHitShieldHeartCum >= relicInfo.y) return;
            PlayerStatus.ChangeShildHp(true, (int)(relicInfo.z * 2));
            addedShildCount += (int)(relicInfo.z * 2);

            TimerEvent.Add(relicInfo.n, OnNoHitShieldHeart);
            noHitShieldHeartCum++;
        }
        private void OffNoHitShieldHeart(int damage, GameObject attacker)  //보호막 생성 취소 후, 다시 생성 함수
        {
            TimerEvent.RemoveAll(OnNoHitShieldHeart);
            TimerEvent.Add(relicInfo.n, OnNoHitShieldHeart);
        }
    }
    private class Resurrection : RelicEffectType               //150 - 죽었을 경우 하트 n개를 생성되며, z초간 무적 상태가 됩니다. (게임 진행 간 y회)
    {
        private int resurrectionCount;
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.playerDeadEvent += OnResurrection;
            else RelicEvent.playerDeadEvent -= OnResurrection;
        }
        private void OnResurrection()
        {
            if (resurrectionCount >= relicInfo.y) return;

            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowResurrection();

            PlayerStatus.Instance.isDead = false;
            PlayerStatus.AddHp((int)(relicInfo.n * 2));
            PlayerTimeSystem.SetAndStartInvincibilityTimer(relicInfo.z);
            resurrectionCount++;
        }
    }
    private class AutoCleanse : RelicEffectType               //151 - 하트를 잃을때 마다 n%확률로 부여된 해로운 효과 z개를 제거합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.playerLoseHpEvent += OnAutoCleanseInfo;
            else RelicEvent.playerLoseHpEvent -= OnAutoCleanseInfo;
        }
        private void OnAutoCleanseInfo(int damage)
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue <= relicInfo.n)
            {
                //해로운 효과 삭제
            }
        }
    }
    private class CounterCore : RelicEffectType               //152 - 하트를 읽을때 마다 n% 확률로 zm범위의 몬스터들을 경직 시킵니다. (보스 몬스터에게는 적용되지 않음)
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) RelicEvent.playerLoseHpEvent += OnCounterCore;
            else RelicEvent.playerLoseHpEvent -= OnCounterCore;
        }
        private void OnCounterCore(int damage)
        {
            float randomValue = UnityEngine.Random.value;
            if (randomValue <= relicInfo.n)
            {
                // zm범위의 몬스터들을 경직 시킵니다
            }
        }
    }
    private class DebuffImmunity : RelicEffectType               //153 - 구속을 제외한 모든 디버프에 면역 상태가 됩니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            //보류~~
        }
    }
    private class RelicDropRateBoost : RelicEffectType               //154 - 오염된 프로세스 드랍 확률이 n% 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            if (isEquip) DropRelicSystem.dropRate += info.n;
            else DropRelicSystem.dropRate -= info.n;
            LogUtil.Log($"칩셋 드롭확률 변경, 변경 수치: {info.n}");
        }
    }
    private class CorruptionIncrease : RelicEffectType               //155 - 플레이어 오염도 수치가 n 증가합니다.
    {
        public override void Excute(bool isEquip, RelicInfo info)
        {
            base.Excute(isEquip, info);
            PlayerStatus.ChangeMaxDarkMatCount(isEquip, (int)info.n);
        }
    }
}

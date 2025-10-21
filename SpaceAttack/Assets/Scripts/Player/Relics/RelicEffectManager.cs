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
            {115,  Damageincrease}, {117, GambleHit}, {118, MoveSpeedDecrease},
            {119,  LastReserve}, {120, FailSafe}, {121, OverloadBackflow},
            {122, BaseAttackDecrease }, {123, CritChanceDecrease}, {124, AttackSpeedDecrease},
            {125, SkillDamageDecrease }, {126, CritDamageDecrease}, {127, MaxHeartDecrease},
            {128,  InvincibleTimeAfterHitDecrease}, {129, DamageFromEliteBossPercent},
            {130,  DashDistanceDecrease}, {131, ItemDropChanceDecrease}, {132, ExpGainDecrease},
            {133,  BaseAttackSpeedOnHit}, {134, AttackSpeedOnKill}, {135, SkillDamageNextAttack},
            {136, CritDamageAttackPowerOnCrit }, {137, MaxHeartShieldOnStart}, {138, ShieldIfNoHit},
            {139, IgnoreEliteBossDamageChance }, {140, DashDistanceincrease}, {141, DashDistanceDamage},
            {142,  MoveSpeedDashBoost}, {143, SkillCooldownSlowWave}, {144, ItemDropBonusReward },
            {145,  ExpGainincrease}, {146, CorruptionOnLevelup}, {147, OverLoadCore},
            {148, Execute }, {149, NoHitShieldHeart}, {150, Resurrection}, {151, AutoCleanse},
            {152, CounterCore }, {153, DebuffImmunity}, {154, RelicDropRateBoost}, {155, CorruptionIncrease}
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

    //--------------------------------------------------------------각 유물 효과 내부 코드------------------------------------------------------------------

    private void AttackRateBoost(bool isEquip, RelicInfo info)         //100 - 공격력 n% 상승.
    {
        EventManager.playerEvent.SetChipAttackRate(isEquip, info.n);
        LogUtil.Log($"공격력 변경" );
    }

    RelicInfo moreAttackChanceInfo;
    private void MoreAttackChance(bool isEquip, RelicInfo info)         //101 - 모든 공격이 n% 확률로 데미지가 1 번 더 적용됩니다. (다단히트 공격은 각 타격마다 개별 적용)
    {
        moreAttackChanceInfo = info;
        if (isEquip) AttackEventManager.OnAttackStarted += OnMoreAttackChance;
        else AttackEventManager.OnAttackStarted -= OnMoreAttackChance;
    }

    private void OnMoreAttackChance(AttackContext cotext)
    {
        LogUtil.Log("다단계 공격 시도");
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= moreAttackChanceInfo.n)
        {
            LogUtil.Log("다단계 공격 성공");
            cotext.IsReattack = true;
        }
    }
    private void CritChanceBoost(bool isEquip, RelicInfo info) //102 - 치명타 확률이 n% 상승한다.
    {
        if (isEquip) PlayerStatus.criticalChanceRate += info.n;
        else PlayerStatus.criticalChanceRate -= info.n;
        LogUtil.Log($"치명타율 변경, 현재 치명타율: {PlayerStatus.criticalChanceRate}");
    }

    private void EvasionRateBoost(bool isEquip, RelicInfo info) //103 - 회피 확률이 n% 증가합니다.
    {
        if (isEquip) PlayerStatus.missRate += info.n;  //회피율 상승
        else PlayerStatus.missRate -= info.n;
        LogUtil.Log($"회피율 변경, 현재 회피율: {PlayerStatus.missRate}");
    }

    private void MoveSpeedBoost(bool isEquip, RelicInfo info)   //104 - 이동 속도가 n% 증가합니다.
    {
        if (isEquip) PlayerStatus.m_speed += (DataManager.instance.i_speed * info.n);
        else PlayerStatus.m_speed -= (DataManager.instance.i_speed * info.n);
        LogUtil.Log($"이속 변경, 현재 이속: {PlayerStatus.m_speed}");
    }

    private int analyzerCount = 0;
    private int analyzerCumCount = 0;
    private RelicInfo weaknessAnalyzerInfo;
    private void WeaknessAnalyzer(bool isEquip, RelicInfo info) //105 - 적에게 n회 공격을 적중시킬 때마다, z초간 피해량이 y% 증가하는 '분석 완료' 상태가 됩니다. (중첩 w번)
    {
        weaknessAnalyzerInfo = info;
        if (isEquip) RelicEvent.playerAttckEvent += OnWeaknessAnalyzer;
        else
        {
            RelicEvent.playerAttckEvent -= OnWeaknessAnalyzer;
            analyzerCount = 0;
            analyzerCumCount = 0;
        }
    }
    private void OnWeaknessAnalyzer()
    {
        if (analyzerCumCount >= weaknessAnalyzerInfo.w) return;

        if (analyzerCount >= weaknessAnalyzerInfo.n)
        {
            LogUtil.Log($"적에게 {weaknessAnalyzerInfo.n}회 공격을 적중시킬 때마다, {weaknessAnalyzerInfo.z}초간 피해량이 {weaknessAnalyzerInfo.y}% 증가");
            EventManager.playerEvent.SetChipDamageRate(true, weaknessAnalyzerInfo.y);   //피해량 증가 적용
            TimerEvent.Add(weaknessAnalyzerInfo.z, OffWeaknessAnalyzerBuff);  //일정시간후, 자동 피해량 감소
            analyzerCount = 0;
            analyzerCumCount++;
        }
        analyzerCount++;
    }

    private void OffWeaknessAnalyzerBuff()
    {
        EventManager.playerEvent.SetChipDamageRate(false, weaknessAnalyzerInfo.y);
        analyzerCumCount--;
    }

    private void SkillCoolDecrease(bool isEquip, RelicInfo info)//106 - 모든 스킬 재사용 대기 시간이 n% 감소합니다.
    {
        EventManager.playerEvent.SetCoolDownRate(isEquip, info.n);
        LogUtil.Log("칩스킬 쿨타임 변경");
    }

    private void AttackSpeedUp(bool isEquip, RelicInfo info)    //107 - 공격 속도가 n% 증가합니다.
    {
        EventManager.playerEvent.SetAttackTimeRate(isEquip, info.n);
    }
    private void CritDamageBoost(bool isEquip, RelicInfo info)  //108 - 치명타 피해량이 n% 증가합니다.
    {
        if (isEquip) PlayerStatus.criticalRate += info.n;
        else PlayerStatus.criticalRate -= info.n;
    }
    private void MaxHeart(bool isEquip, RelicInfo info)         //109 - 최대 하트가 n칸 증가합니다.
    {
        PlayerStatus.ChangeMaxHp(isEquip, (int) info.n);
    }

    private int bloodPowerAmount = 0;
    private int bloodPowerCount = 0;
    private RelicInfo bloodPowerInfo;
    private void BloodPower(bool isEquip, RelicInfo info)       //110 - 잃은 하트 n칸 당 공격력이 z% 증가하지만, 최대 하트가 y칸 감소합니다.
    {
        bloodPowerInfo = info;
        if (isEquip) RelicEvent.playerLoseHpEvent += OnBloodPower;
        else
        {
            RelicEvent.playerLoseHpEvent -= OnBloodPower;
            PlayerStatus.RecoverLosedHp();                       //잃어버린 체력 되돌리기
            bloodPowerAmount = 0;
            EventManager.playerEvent.SetChipAttackRate(false, bloodPowerInfo.z * bloodPowerCount);
            bloodPowerCount = 0;
        }
        PlayerStatus.ChangeMaxHp(!isEquip, (int)(info.y * 2));
        LogUtil.Log(info.y);
    }
    private void OnBloodPower(int amount)
    {
        bloodPowerAmount += amount;
        if (bloodPowerAmount / 2 >= bloodPowerInfo.n)
        {
            LogUtil.Log($"누적 피해 : {bloodPowerAmount}, 공격력 수치: {bloodPowerInfo.z}, 받은 데미지: {amount}");
            EventManager.playerEvent.SetChipAttackRate(true, bloodPowerInfo.z);
            bloodPowerAmount -= (int)(bloodPowerInfo.n * 2);
            bloodPowerCount++;
        }
    }
    private void GlassCannon(bool isEquip, RelicInfo info)      //111 - 모든 공격의 피해량이 n% 증가합니다.
    {
        EventManager.playerEvent.SetChipDamageRate(isEquip, info.n);
    }
    private void SkillDamageBosst(bool isEquip, RelicInfo info) //112 - 스킬 피해량이 n% 증가합니다.
    {
        EventManager.playerEvent.SetChipSkillDamageRate(isEquip, info.n);
    }

    private RelicInfo lifeToSkillPowerInfo;
    private void LifeToSkillPower(bool isEquip, RelicInfo info) //113 - 스킬 사용 시 모든 하트 중 n 칸 소모하며, 해당 스킬의 피해량이 z% 증가합니다.
    {
        lifeToSkillPowerInfo = info;
        if (isEquip) AttackEventManager.OnAttackStarted += OnLifeToSkillPower;
        else AttackEventManager.OnAttackStarted -= OnLifeToSkillPower;
    }
    private void OnLifeToSkillPower(AttackContext context)
    {
        if (context.attackType == ChipAttackType.Skill)
        {
            if (PlayerStatus.Instance != null)
                PlayerStatus.Instance.ReduceHp((int) lifeToSkillPowerInfo.n);
            context.damageRateSume *= (1 + lifeToSkillPowerInfo.z);
        }
    }
    private void SkillCoolincrease(bool isEquip, RelicInfo info) //114 - 스킬 재사용 대기시간이 n% 증가합니다.
    {
        EventManager.playerEvent.SetCoolDownRate(!isEquip, info.n);
    }
    private void Damageincrease(bool isEquip, RelicInfo info)    //115 - 받는 모든 피해가 n배로 증가합니다.
    {
        float value = Math.Max(DataManager.instance.i_hitRate, info.n);
        if (isEquip) PlayerStatus.hitRate = value;
        else PlayerStatus.hitRate = DataManager.instance.i_hitRate;
    }
    private RelicInfo gambleHit;
    private void GambleHit(bool isEquip, RelicInfo info)         //117 - 공격 시 n% 확률로 z배의 피해를, y% 확률로 w배의 피해를 줍니다.
    {
        gambleHit = info;
        if (isEquip) AttackEventManager.OnAttackStarted += OnGamleHit;
        else AttackEventManager.OnAttackStarted -= OnGamleHit;
    }
    private void OnGamleHit(AttackContext cotext)
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= gambleHit.n)
        {
            cotext.damageRateSume *= gambleHit.z;
        }
        else
        {
            cotext.damageRateSume *= gambleHit.w;
        }
    }
    private void MoveSpeedDecrease(bool isEquip, RelicInfo info) //118 - 이동 속도가 n% 감소합니다.
    {
        if (isEquip) PlayerStatus.m_speed -= DataManager.instance.i_speed * info.n;
        else PlayerStatus.m_speed += DataManager.instance.i_speed * info.n;
    }

    private RelicInfo lastReserveInfo;
    private int lastReserveCumCount = 0;
    private void LastReserve(bool isEquip, RelicInfo info)       //119 - 최대 하트가 n칸으로 고정되지만, z초마다 보호막 하트 y개를 얻습니다 (최대 w칸 중첩)
    {
        lastReserveInfo = info;
        if (isEquip)
        {
            PlayerStatus.ChangeMaxHp(info.n > PlayerStatus.m_maxhp, Mathf.Abs((int)info.n - PlayerStatus.m_maxhp));
            PlayerStatus.maxHpFixing = true;
            TimerEvent.Add(info.z, OnLastReserve);
        }
    }
    private void OnLastReserve()
    {
        PlayerStatus.ChangeShildHp(true, (int)lastReserveInfo.y);
        lastReserveCumCount++;
        if (lastReserveCumCount < lastReserveInfo.w)
            TimerEvent.Add(lastReserveInfo.z, OnLastReserve);
    }

    private RelicInfo failSafeInfo;
    private int failSafeCumCount = 0;
    private void FailSafe(bool isEquip, RelicInfo info)          //120 - 하트가 n칸일 때 공격 속도와 이동 속도가 z% y분간 증가하지만, 그 상태에서는 체력 회복이 불가능해집니다. (스테이지 당 w번)
    {
        failSafeInfo = info;
        if (isEquip)
        {
            OnFailSafe(0);
            RelicEvent.playerLoseHpEvent += OnFailSafe;
            RelicEvent.startStageEvent += InitialFailSafe;
        }
        else
        {
            RelicEvent.playerLoseHpEvent -= OnFailSafe;
            RelicEvent.startStageEvent -= InitialFailSafe;
        }
    }
    private void OnFailSafe(int amount)
    {
        if (failSafeCumCount >= failSafeInfo.w) return;

        if (PlayerStatus.m_hp / 2 + (PlayerStatus.m_hp % 2 == 1 ? 1:0) <= failSafeInfo.n)
        {
            PlayerStatus.cannotHealing = true;
            //공격속도 증가
            PlayerStatus.m_speed += DataManager.instance.i_speed * failSafeInfo.z;
            TimerEvent.Add(failSafeInfo.y * 60, OffFailSafe);

            failSafeCumCount++;
        }
    }
    private void InitialFailSafe()
    {
        failSafeCumCount = 0;
    }
    private void OffFailSafe()
    {
        //공격속도 감소
        PlayerStatus.m_speed -= DataManager.instance.i_speed * failSafeInfo.z;
        PlayerStatus.cannotHealing = false;
    }

    private RelicInfo overloadBackflowInfo;
    private void OverloadBackflow(bool isEquip, RelicInfo info)  //121 - 최대 하트가 n칸으로 고정 되는 대신 하트 잃을 때 마다 z% 확률로 하트 y칸을 회복한다.
    {
        if (isEquip)
        {
            PlayerStatus.ChangeMaxHp(info.n >= PlayerStatus.m_maxhp, Math.Abs(PlayerStatus.m_maxhp - (int)info.n));
            RelicEvent.playerLoseHpEvent += OnOverloadBackflow;
        }
        else
        {
            PlayerStatus.RecoverLosedHp();
            RelicEvent.playerLoseHpEvent -= OnOverloadBackflow;
        }
    }
    private void OnOverloadBackflow(int amount)
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue < overloadBackflowInfo.z) PlayerStatus.AddHp((int) overloadBackflowInfo.y);
    }

    private void BaseAttackDecrease(bool isEquip, RelicInfo info)//122 - 기본 공격력이 n%감소합니다.
    {
        EventManager.playerEvent.SetChipAttackRate(!isEquip, info.n);
    }
    private void CritChanceDecrease(bool isEquip, RelicInfo info)//123 - 치명타 확률이 n% 감소한다.
    {
        if (isEquip) PlayerStatus.criticalChanceRate -= DataManager.instance.i_criticalChanceRate * info.n;
        else PlayerStatus.criticalChanceRate += DataManager.instance.i_criticalChanceRate * info.n;
    }
    private void AttackSpeedDecrease(bool isEquip, RelicInfo info)//124 - 공격 속도가 n% 감소합니다.
    {
        EventManager.playerEvent.SetAttackTimeRate(!isEquip, info.n);
    }
    private void SkillDamageDecrease(bool isEquip, RelicInfo info)//125 - 스킬 피해랴이 n% 감소합니다.
    {
        EventManager.playerEvent.SetChipSkillDamageRate(!isEquip, info.n);
    }
    private void CritDamageDecrease(bool isEquip, RelicInfo info) //126 - 치명타 데미지가 n% 감소합니다.
    {
        if (isEquip) PlayerStatus.criticalRate -= DataManager.instance.i_criticalRate * info.n;
        else PlayerStatus.criticalRate += DataManager.instance.i_criticalRate * info.n;
    }
    private void MaxHeartDecrease(bool isEquip, RelicInfo info)   //127 - 최대 하트가 n칸 감소합니다.
    {
        PlayerStatus.ChangeMaxHp(!isEquip, (int) info.n);
        if(!isEquip) PlayerStatus.RecoverLosedHp();
    }
    private void InvincibleTimeAfterHitDecrease(bool isEquip, RelicInfo info)//128 - 피격 후 발생하는 무적 시간이 n% 감소합니다.
    {
        if (isEquip) PlayerTimeSystem.SetStunTimer(PlayerTimeSystem.m_stunTime - DataManager.instance.i_m_stunTime * info.n);
        else PlayerTimeSystem.SetStunTimer(PlayerTimeSystem.m_stunTime + DataManager.instance.i_m_stunTime * info.n);
    }

    private RelicInfo damageFromEliteBossPercentInfo;
    private bool isAddDamageFromEliteBossPercent = false;
    private void DamageFromEliteBossPercent(bool isEquip, RelicInfo info)//129 - 엘리트/보스 몬스터에게 받는 피해가 n%증가합니다.
    {
        damageFromEliteBossPercentInfo = info;
        if (isEquip) RelicEvent.playerHitEventStart += OnDamageFromEliteBossPercentInfo;
        else
        {
            RelicEvent.playerHitEventStart -= OnDamageFromEliteBossPercentInfo;
            if (isAddDamageFromEliteBossPercent)
            {
                PlayerStatus.hitRate -= DataManager.instance.i_hitRate * damageFromEliteBossPercentInfo.n;
                isAddDamageFromEliteBossPercent = false;
            }
        }
    }
    private void OnDamageFromEliteBossPercentInfo(int damage, GameObject attacker)
    {
        if (attacker == null) return;
        if (attacker.gameObject.CompareTag("Boss") || attacker.gameObject.CompareTag("Elite"))
        {
            PlayerStatus.hitRate += DataManager.instance.i_hitRate * damageFromEliteBossPercentInfo.n;
            isAddDamageFromEliteBossPercent = true;
        }
        else
        {
            PlayerStatus.hitRate -= DataManager.instance.i_hitRate * damageFromEliteBossPercentInfo.n;
            isAddDamageFromEliteBossPercent = false;
        }
    }

    private void DashDistanceDecrease(bool isEquip, RelicInfo info) //130 - 대시의 이동 거리가 n% 감소합니다.
    {
        if (isEquip) PlayerStatus.m_DashDistance -= DataManager.instance.i_DashDistance * info.n;
        else PlayerStatus.m_DashDistance += DataManager.instance.i_DashDistance * info.n;
    }
    private void ItemDropChanceDecrease(bool isEquip, RelicInfo info)//131 - 적 처치 시 아이템 획득 확률이 n% 감소합니다.
    {
        ////////////////보류!!!!!!!!!!!!!!!!!!!!!!!1
    }
    private void ExpGainDecrease(bool isEquip, RelicInfo info)       //132 - 획득하는 경험치가 n% 감소합니다.
    {
        if (isEquip) DropEXPSystem.dropExpCount -= (int)(DropEXPSystem.i_dropExpCount * info.n);
        else DropEXPSystem.dropExpCount += (int)(DropEXPSystem.i_dropExpCount * info.n);
    }

    private RelicInfo baseAttackSpeedOnHitInfo;
    private void BaseAttackSpeedOnHit(bool isEquip, RelicInfo info)  //133 - 공격 시 n% 확률로 z초간 공격 속도가 y% 증가 합니다.
    {
        baseAttackSpeedOnHitInfo = info;
        if (isEquip) RelicEvent.playerAttckEvent += OnBaseAttackSpeedOnHit;
        else RelicEvent.playerAttckEvent -= OnBaseAttackSpeedOnHit;
    }
    private void OnBaseAttackSpeedOnHit()
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= baseAttackSpeedOnHitInfo.n)
        {
            EventManager.playerEvent.SetAttackTimeRate(true, baseAttackSpeedOnHitInfo.y); //공격 속도 상승
            TimerEvent.Add(baseAttackSpeedOnHitInfo.z, OffBaseAttackSpeedOnHit);
        }
    }
    private void OffBaseAttackSpeedOnHit()
    {
        EventManager.playerEvent.SetAttackTimeRate(false, baseAttackSpeedOnHitInfo.y); //공격속도 감소
    }

    private RelicInfo attackSpeedOnKillInfo;
    private void AttackSpeedOnKill(bool isEquip, RelicInfo info)     //134 - 적 처치 시 n초간 공격 속도가 추가로 z% 증가합니다.
    {
        attackSpeedOnKillInfo = info;
        if (isEquip) RelicEvent.killedEnemyEvent += OnAttackSpeedOnKill;
        else RelicEvent.killedEnemyEvent -= OnAttackSpeedOnKill;
    }
    private void OnAttackSpeedOnKill()
    {
        EventManager.playerEvent.SetAttackTimeRate(true, attackSpeedOnKillInfo.z); //공격 속도 상승
        TimerEvent.Add(attackSpeedOnKillInfo.n, OffOnAttackSpeedOnKill);
    }
    private void OffOnAttackSpeedOnKill()
    {
        EventManager.playerEvent.SetAttackTimeRate(false, attackSpeedOnKillInfo.z); //공격속도 감소
    }

    private RelicInfo skillDamageNextAttackInfo;
    private bool isUpgraded = false;
    private void SkillDamageNextAttack(bool isEquip, RelicInfo info)     //135 - 스킬 사용 시 다음 기본 공격의 피해량이 n% 증가합니다.
    {
        skillDamageNextAttackInfo = info;
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
            context.weaponDamagekRateSume += skillDamageNextAttackInfo.n;
    }
    private RelicInfo critDamageAttackPowerOnCritInfo;
    private void CritDamageAttackPowerOnCrit(bool isEquip, RelicInfo info)//136 - 치명타 발동 시 n초간 공격력이 z% 증가합니다.
    {
        critDamageAttackPowerOnCritInfo = info;
        if (isEquip) RelicEvent.criticalEvent += OnCritDamageAttackPowerOnCrit;
        else RelicEvent.criticalEvent -= OnCritDamageAttackPowerOnCrit;
    }
    private void OnCritDamageAttackPowerOnCrit()
    {
        EventManager.playerEvent.SetChipAttackRate(true, critDamageAttackPowerOnCritInfo.z);
        TimerEvent.Add(critDamageAttackPowerOnCritInfo.n, OffCritDamageAttackPowerOnCrit);
    }
    private void OffCritDamageAttackPowerOnCrit()
    {
        EventManager.playerEvent.SetChipAttackRate(false, critDamageAttackPowerOnCritInfo.z);
    }

    private RelicInfo maxHeartShieldOnStartInfo;
    private void MaxHeartShieldOnStart(bool isEquip, RelicInfo info)      //137 - 스테이지 시작 시 하트 n칸 만큼의 보호막을 얻습니다.
    {
        maxHeartShieldOnStartInfo = info;
        if (isEquip) RelicEvent.startStageEvent += OnMaxHeartShieldOnStart;
        else RelicEvent.startStageEvent -= OnMaxHeartShieldOnStart;
    }
    private void OnMaxHeartShieldOnStart()
    {
        PlayerStatus.ChangeShildHp(true, (int)maxHeartShieldOnStartInfo.n);
    }

    private RelicInfo shieldIfNoHitInfo;
    private void ShieldIfNoHit(bool isEquip, RelicInfo info)              //138 - n초 동안 피격당하지 않으면, 최대 하트 z칸 만큼의 보호막을 얻습니다.
    {
        shieldIfNoHitInfo = info;
        if (isEquip)
        {
            RelicEvent.playerHitEventStart += OffShieldIfNoHit;
            TimerEvent.Add(info.n, OnShieldIfNoHit);
        }
        else RelicEvent.playerHitEventStart -= OffShieldIfNoHit;
    }
    private void OnShieldIfNoHit()  //보호막 생성 함수
    {
        PlayerStatus.ChangeMaxHp(true, (int)shieldIfNoHitInfo.z);
    }
    private void OffShieldIfNoHit(int damage, GameObject attacker)  //보호막 생성 취소 후, 다시 생성 함수
    {
        TimerEvent.Remove(OnShieldIfNoHit);
        TimerEvent.Add(shieldIfNoHitInfo.n, OnShieldIfNoHit);
    }

    private RelicInfo ignoreEliteBossDamageChanceInfo;
    private float currentHitRate;
    private void IgnoreEliteBossDamageChance(bool isEquip, RelicInfo info)//139 - 엘리트/보스 몬스터의 공격에 피격 시, n% 확률로 피해를 무시합니다.
    {
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
        currentHitRate = PlayerStatus.hitRate;
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= ignoreEliteBossDamageChanceInfo.n)
            PlayerStatus.hitRate = 0;
    }
    private void OffIgnoreEliteBossDamageChance()
    {
        PlayerStatus.hitRate = currentHitRate;
    }

    private void DashDistanceincrease(bool isEquip, RelicInfo info)       //140 - 대시의 이동 거리가 n% 증가합니다.
    {
        if (isEquip) PlayerStatus.m_DashDistance += DataManager.instance.i_DashDistance * info.n;
        else PlayerStatus.m_DashDistance -= DataManager.instance.i_DashDistance * info.n;
    }
    private void DashDistanceDamage(bool isEquip, RelicInfo info)         //141 - 대시 중 통과하는 적에게 기본 공격력의 n% 피해를 줍니다.
    {
        //////////////////보류!!!!!!!!!!!!!!!!!!!!111
    }

    private RelicInfo moveSpeedDashBoostInfo;
    private void MoveSpeedDashBoost(bool isEquip, RelicInfo info)         //142 - 대시 사용 후 n초간 이동 속도가 추가로 z% 증가합니다.
    {
        moveSpeedDashBoostInfo = info;
        if (isEquip) RelicEvent.dashEvent += OnMoveSpeedDashBoost;
        else RelicEvent.dashEvent -= OnMoveSpeedDashBoost;
    }
    private void OnMoveSpeedDashBoost()
    {
        PlayerStatus.m_speed += DataManager.instance.i_speed * moveSpeedDashBoostInfo.z;
        TimerEvent.Add(moveSpeedDashBoostInfo.n, OffMoveSpeedDashBoostInfo);
    }
    private void OffMoveSpeedDashBoostInfo()
    {
        PlayerStatus.m_speed -= DataManager.instance.i_speed * moveSpeedDashBoostInfo.z;
    }

    private void SkillCooldownSlowWave(bool isEquip, RelicInfo info)      //143 - 스킬 사용 시 주변에 일반/엘리트 몬스터를 느리게 하는 냉기 파동이 방출됩니다.
    {
        //////////////////보류!!!!!!!!!!!!!!!!!!!!111
    }
    private void ItemDropBonusReward(bool isEquip, RelicInfo info)        //144 - 적 처치 시 아이템 획득 확률이 n% 증가하고 z% 확률로 추가 보상을 획득합니다.
    {
        //////////////////보류!!!!!!!!!!!!!!!!!!!!111
    }
    private void ExpGainincrease(bool isEquip, RelicInfo info)             //145 - 획득하는 경험치가 n% 증가합니다.
    {
        if (isEquip) DropEXPSystem.dropExpCount += (int)(DropEXPSystem.i_dropExpCount * info.n);
        else DropEXPSystem.dropExpCount -= (int)(DropEXPSystem.i_dropExpCount * info.n);
    }

    private RelicInfo corruptionOnLevelupInfo;
    private int addedDarkMatCount;
    private void CorruptionOnLevelup(bool isEquip, RelicInfo info)         //146 - 레벨 업 시 증가하는 오염도 게이지가 n 추가로 증가합니다.
    {
        corruptionOnLevelupInfo = info;
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
        PlayerStatus.ChangeMaxDarkMatCount(true, (int)corruptionOnLevelupInfo.n);
        addedDarkMatCount += (int)corruptionOnLevelupInfo.n;
    }

    private RelicInfo overLoadCoreInfo;
    private int overLoadCoreCount;
    private void OverLoadCore(bool isEquip, RelicInfo info)                //147 - 스킬 사용 시 다음 n회의 기본 공격이 강화되어 z% 피해 데미지가 증가합니다.
    {
        overLoadCoreInfo = info;
        if (isEquip)
        {
            RelicEvent.playerUseSkillEvent += OnOverLoadCore;
            AttackEventManager.OnAttackStarted += OnOverLoadCore2;
        }
        else
        {
            RelicEvent.playerUseSkillEvent += OnOverLoadCore;
            AttackEventManager.OnAttackStarted += OnOverLoadCore2;
            overLoadCoreCount = 0;
        }
    }
    private void OnOverLoadCore()  //스킬 사용시, 강공 가능 상태로 전환
    {
        overLoadCoreCount = (int)overLoadCoreInfo.n;
    }
    private void OnOverLoadCore2(AttackContext context) //기본 공격시, 강공 적용
    {
        if (overLoadCoreCount <= 0) return;
        if (context.attackType == ChipAttackType.Weapon)
        {
            context.damageRate *= (1 + overLoadCoreInfo.z);
            overLoadCoreCount--;
        }
    }

    private void Execute(bool isEquip, RelicInfo info)                     //148 - 체력이 n% 이하인 일반 몬스터를 즉시 처치합니다. (엘리트/보스 몬스터에게는 적용되지 않음)
    {
        //////////////////보류!!!!!!!!!!!!!!!!!!!!111
    }

    private RelicInfo noHitShieldHeartInfo;
    private int noHitShieldHeartCount;
    private void NoHitShieldHeart(bool isEquip, RelicInfo info)            //149 - n초 동안 피격당하지 않으면, 추가 하트의 보호막이 z개 추가 됩니다. (최대 y개)
    {
        shieldIfNoHitInfo = info;
        if (isEquip)
        {
            RelicEvent.playerHitEventStart += OffNoHitShieldHeart;
            TimerEvent.Add(info.n, OnNoHitShieldHeart);
        }
        else RelicEvent.playerHitEventStart -= OffNoHitShieldHeart;
    }
    private void OnNoHitShieldHeart()  //보호막 생성 함수
    {
        if (noHitShieldHeartCount >= noHitShieldHeartInfo.y) return;
        PlayerStatus.ChangeMaxHp(true, (int)noHitShieldHeartInfo.z);
        noHitShieldHeartCount++;
    }
    private void OffNoHitShieldHeart(int damage, GameObject attacker)  //보호막 생성 취소 후, 다시 생성 함수
    {
        TimerEvent.Remove(OnShieldIfNoHit);
        TimerEvent.Add(noHitShieldHeartInfo.n, OnShieldIfNoHit);
    }

    private RelicInfo resurrectionInfo;
    private int resurrectionCount;
    private void Resurrection(bool isEquip, RelicInfo info)                //150 - 죽었을 경우 하트 n개를 생성되며, z초간 무적 상태가 됩니다. (게임 진행 간 y회)
    {
        resurrectionInfo = info;
        if (isEquip) RelicEvent.playerDeadEvent += OnResurrection;
        else RelicEvent.playerDeadEvent -= OnResurrection;
    }
    private void OnResurrection()
    {
        if (resurrectionCount >= resurrectionInfo.y) return;

        PlayerStatus.Instance.isDead = false;
        PlayerStatus.AddHp((int)resurrectionInfo.n);
        resurrectionCount++;
    }

    private RelicInfo autoCleanseInfo;
    private void AutoCleanse(bool isEquip, RelicInfo info)                 //151 - 하트를 잃을때 마다 n%확률로 부여된 해로운 효과 z개를 제거합니다.
    {
        autoCleanseInfo = info;
        if (isEquip) RelicEvent.playerLoseHpEvent += OnAutoCleanseInfo;
        else RelicEvent.playerLoseHpEvent -= OnAutoCleanseInfo;
    }
    private void OnAutoCleanseInfo(int damage)
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= autoCleanseInfo.n)
        {
            //해로운 효과 삭제
        }
    }

    private RelicInfo counterCoreInfo;
    private void CounterCore(bool isEquip, RelicInfo info)                 //152 - 하트를 읽을때 마다 n% 확률로 zm범위의 몬스터들을 경직 시킵니다. (보스 몬스터에게는 적용되지 않음)
    {
        counterCoreInfo = info;
        if (isEquip) RelicEvent.playerLoseHpEvent += OnCounterCore;
        else RelicEvent.playerLoseHpEvent -= OnCounterCore;
    }
    private void OnCounterCore(int damage)
    {
        float randomValue = UnityEngine.Random.value;
        if (randomValue <= autoCleanseInfo.n)
        {
            // zm범위의 몬스터들을 경직 시킵니다
        }
    }
    private void DebuffImmunity(bool isEquip, RelicInfo info)              //153 - 구속을 제외한 모든 디버프에 면역 상태가 됩니다.
    {
        //보류~~
    }
    private void RelicDropRateBoost(bool isEquip, RelicInfo info)          //154 - 오염된 프로세스 드랍 확률이 n% 증가합니다.
    {
        if (isEquip) DropRelicSystem.dropRate += info.n;
        else DropRelicSystem.dropRate -= info.n;
        LogUtil.Log($"칩셋 드롭확률 변경, 변경 수치: {info.n}");
    }
    private void CorruptionIncrease(bool isEquip, RelicInfo info)          //155 - 플레이어 오염도 수치가 n 증가합니다.
    {
        PlayerStatus.ChangeMaxDarkMatCount(isEquip, (int)info.n);
    }

}

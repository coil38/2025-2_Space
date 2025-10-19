using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicEvent
{
    public static event Action<int> playerLoseHpEvent;     //플레이어 체력 잃음 O
    public static event Action<int> playerHitEvent;        //플레이어 피격 O
    public static event Action playerAttckEvent;           //플레이어 모든 공격 O
    public static event Action playerUseSkillEvent;        //플레이어 스킬 사용 O
    public static event Action playerSkillAttackEvent;     //플레이어 공격 스킬 사용 O
    public static event Action killedEnemyEvent;           //플레이어 적 처치 O
    public static event Action dashEvent;                  //대쉬            O
    public static event Action dashHittedEnemyEvent;       //대쉬중 적을 스침
    public static event Action playerDeadEvent;            //플레이어 사망    O
    public static event Action startStageEvent;            //스테이지 시작    
    public static event Action criticalEvent;              //크리티컬 성공    O

    public void OnPlayerHitEvent(int damageAmount)
    {
        playerHitEvent?.Invoke(damageAmount);
    }

    public void OnPlayerAttackEvent()
    {
        playerAttckEvent?.Invoke();
    }

    public void OnPlayerUseSkillEvent()
    {
        playerUseSkillEvent?.Invoke();
    }
    public void OnPlayerSkillAttackEvent()
    {
        playerSkillAttackEvent?.Invoke();
    }

    public void OnKilledEnemyEvent()
    {
        killedEnemyEvent?.Invoke();
    }

    public void OnDashEvent()
    {
        dashEvent?.Invoke();
    }

    public void OnDashHittedEnemyEvent()
    {
        dashHittedEnemyEvent?.Invoke();
    }
    public void OnPlayerDeadEvent()
    {
        playerDeadEvent?.Invoke();
    }
    public void OnStartStageEvent()
    {
        startStageEvent?.Invoke();
    }
    public void OnCriticalEvent()
    {
        criticalEvent?.Invoke();
    }

    public void OnPlayerLoseHp(int amount)
    {
        playerLoseHpEvent?.Invoke(amount);
    }

    public static void InitializeEvent()
    {
        playerHitEvent = null;
        playerAttckEvent = null;
        playerUseSkillEvent = null;
        playerSkillAttackEvent = null;
        killedEnemyEvent = null;
        dashEvent = null;
        dashHittedEnemyEvent = null;
        playerDeadEvent = null;
        startStageEvent = null;
        criticalEvent = null;
    }
}

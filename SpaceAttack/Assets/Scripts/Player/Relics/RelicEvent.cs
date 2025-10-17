using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicEvent
{
    public static event EventHandler<RelicEvent> playerHitEvent;        //플레이어 피격 O
    public static event EventHandler<RelicEvent> playerAttckEvent;      //플레이어 모든 공격 O
    public static event EventHandler<RelicEvent> playerUseSkillEvent;   //플레이어 스킬 사용 O
    public static event EventHandler<RelicEvent> playerSkillAttackEvent;//플레이어 공격 스킬 사용 O
    public static event EventHandler<RelicEvent> killedEnemyEvent;      //플레이어 적 처치 O
    public static event EventHandler<RelicEvent> dashEvent;             //대쉬            O
    public static event EventHandler<RelicEvent> dashHittedEnemyEvent;  //대쉬중 적을 스침
    public static event EventHandler<RelicEvent> playerDeadEvent;       //플레이어 사망    O
    public static event EventHandler<RelicEvent> startStageEvent;       //스테이지 시작    
    public static event EventHandler<RelicEvent> criticalEvent;         //크리티컬 성공    O

    public void OnPlayerHitEvent()
    {
        playerHitEvent?.Invoke(this, EventManager.relicEvent);
    }

    public void OnPlayerAttackEvent()
    {
        playerAttckEvent?.Invoke(this, EventManager.relicEvent);
    }

    public void OnPlayerUseSkillEvent()
    {
        playerUseSkillEvent?.Invoke(this, EventManager.relicEvent);
    }
    public void OnPlayerSkillAttackEvent()
    {
        playerSkillAttackEvent?.Invoke(this, EventManager.relicEvent);
    }

    public void OnKilledEnemyEvent()
    {
        killedEnemyEvent?.Invoke(this, EventManager.relicEvent);
    }

    public void OnDashEvent()
    {
        dashEvent?.Invoke(this, EventManager.relicEvent);
    }

    public void OnDashHittedEnemyEvent()
    {
        dashHittedEnemyEvent?.Invoke(this, EventManager.relicEvent);
    }
    public void OnPlayerDeadEvent()
    {
        playerDeadEvent?.Invoke(this, EventManager.relicEvent);
    }
    public void OnStartStageEvent()
    {
        startStageEvent?.Invoke(this, EventManager.relicEvent);
    }
    public void OnCriticalEvent()
    {
        criticalEvent?.Invoke(this, EventManager.relicEvent);
    }
}

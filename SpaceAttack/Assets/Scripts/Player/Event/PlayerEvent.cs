using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerEvent
{
    private EventHandler<PlayerEvent> _correctionEventHandler;  //플레이어 레벨 보정치 이벤트

    public event EventHandler<PlayerEvent> correctionEventHandler
    {
        add { _correctionEventHandler += value; }
        remove{ _correctionEventHandler -= value; }
    }

    private EventHandler<PlayerEvent> _levelEventHandler;  //플레이어 레벨정보 얻기 이벤트

    public event EventHandler<PlayerEvent> levelEventHandler
    {
        add { _levelEventHandler += value; }
        remove { _levelEventHandler -= value; }
    }

    private EventHandler<PlayerEvent> _relicAttackEventHandler;  //유물 공격력 수치 변경 이벤트
    public event EventHandler<PlayerEvent> relicAttackEventHandler
    {
        add { _relicAttackEventHandler += value; }
        remove { _relicAttackEventHandler -= value; }
    }

    private EventHandler<PlayerEvent> _coolDownEventHandler;  //유물 공격력 수치 변경 이벤트
    public event EventHandler<PlayerEvent> coolDownEventHandler
    {
        add { _coolDownEventHandler += value; }
        remove { _coolDownEventHandler -= value; }
    }

    public LevelDatabaseSO levelDatabase;   //레벨 보정관련 데이터 베이스


    public bool correctablility;
    public float damageCorrection;
    public int heartCorrection;
    public float speedCorrection;
    public bool unlockability;
    public int skillNumber;

    public void FindCorectionValue(int _level)
    {
        correctablility = false;
        damageCorrection = 0;
        heartCorrection = 0;
        speedCorrection = 0;
        unlockability = false;
        skillNumber = 0;

        //정보전달
        LevelSO levelSO = levelDatabase.GetLevelByLevel(_level);
        if (levelSO != null)
        {
            if (levelSO.correctability)  //플레이어 스텟보정
            {
                correctablility = levelSO.correctability;
                damageCorrection = levelSO.damageCorrection;
                heartCorrection = levelSO.heartCorrection;
                speedCorrection = levelSO.speedCorrection;
            }

            if (levelSO.unlockability)  //스킬 해금
            {
                unlockability = levelSO.unlockability;
                skillNumber = levelSO.unlockedSkill;
            }

            _correctionEventHandler?.Invoke(this, EventManager.playerEvent);   //이벤트 호출
        }
    }

    public int maxEXP;
    public int nextMaxEXP;

    public void FindMaxExpValue(int _level)
    {
        maxEXP = 0;
        nextMaxEXP = 0;
        if (levelDatabase.maxLevel < _level)  //최대레벨 예외처리
        {
            LogUtil.LogWarning($"{levelDatabase.maxLevel} 레벨의 이상의 레벨은 존재하지 않습니다. 입력받은 레벨: {_level}");
        }
        else
        {
            maxEXP = levelDatabase.GetMaxExp(_level);  //해당 레벨의 최대경험치양 찾기
        }

        if (levelDatabase.maxLevel < _level + 1)  //최대레벨 예외처리
        {
            LogUtil.LogWarning($"{levelDatabase.maxLevel} 레벨의 이상의 레벨은 존재하지 않습니다. 입력받은 레벨: {_level + 1}");
        }
        else
        {
            nextMaxEXP = levelDatabase.GetMaxExp(_level + 1);  //해당 레벨의 최대경험치양 찾기
        }

        _levelEventHandler?.Invoke(this, EventManager.playerEvent);   //최대레벨을 찾는 이벤트 호출
    }

    public bool isEquip;      //장착여부
    public float damageRate;  //데미지률
    public void SetRelicAttackValue(bool _isEquip, float _damageRate)
    {
        //LogUtil.Log("작동한다.");
        isEquip = _isEquip;
        damageRate = _damageRate;
        _relicAttackEventHandler?.Invoke(this, EventManager.playerEvent);
    }

    public float coolDownRate;  //쿨타임 감소률
    public void SetCoolDownValue(bool _isEquip, float _coolDownRate)
    {
        isEquip = _isEquip;
        coolDownRate = _coolDownRate;
        _coolDownEventHandler?.Invoke(this, EventManager.playerEvent);
    }
}
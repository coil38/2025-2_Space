using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerEvent
{
    private static EventHandler<PlayerEvent> _correctionEventHandler;  //플레이어 레벨 보정치 이벤트

    public static event EventHandler<PlayerEvent> correctionEventHandler
    {
        add { _correctionEventHandler += value; }
        remove{ _correctionEventHandler -= value; }
    }

    private static EventHandler<PlayerEvent> _levelUpEventHandler;  //플레이어 레벨정보 얻기 이벤트

    public static event EventHandler<PlayerEvent> levelUpEventHandler
    {
        add { _levelUpEventHandler += value; }
        remove { _levelUpEventHandler -= value; }
    }
     
    private static EventHandler<PlayerEvent> _chipAttackEventHandler;  //칩셋 공격력 배율 변경 이벤트
    public static event EventHandler<PlayerEvent> chipAttackEventHandler
    {
        add { _chipAttackEventHandler += value; }
        remove { _chipAttackEventHandler -= value; }
    }

    private static event EventHandler<PlayerEvent> _chipDamageEventHandler;  //칩셋 데미지 배율 변경 이벤트
    public static event EventHandler<PlayerEvent> chipDamageEventHandler
    {
        add { _chipDamageEventHandler += value; }
        remove { _chipDamageEventHandler -= value; }
    }

    private static event EventHandler<PlayerEvent> _chipWeaponDamageEventHandler;  //칩셋 기본공격 데미지 배율 변경 이벤트
    public static event EventHandler<PlayerEvent> chipWeaponDamageEventHandler
    {
        add { _chipWeaponDamageEventHandler += value; }
        remove { _chipWeaponDamageEventHandler -= value; }
    }

    private static event EventHandler<PlayerEvent> _chipSkillDamageEventHandler;  //칩셋 스킬 데미지 배율 변경 이벤트
    public static event EventHandler<PlayerEvent> chipSkillDamageEventHandler
    {
        add { _chipSkillDamageEventHandler += value; }
        remove { _chipSkillDamageEventHandler -= value; }
    }

    private static EventHandler<PlayerEvent> _chipcoolDownEventHandler;  //칩셋 스킬 쿨타임 배율 변경 이벤트
    public static event EventHandler<PlayerEvent> chipcoolDownEventHandler
    {
        add { _chipcoolDownEventHandler += value; }
        remove { _chipcoolDownEventHandler -= value; }
    }

    private static EventHandler<PlayerEvent> _chipattackTimeDownHandler;  //칩셋 공격속도 배율 변경 이벤트
    public static event EventHandler<PlayerEvent> chipattackTimeDownHandler
    {
        add { _chipattackTimeDownHandler += value; }
        remove { _chipattackTimeDownHandler -= value; }
    }

    public LevelDatabaseSO levelDatabase;   //레벨 보정관련 데이터 베이스


    public bool correctablility;
    public float damageCorrection;
    public int heartCorrection;
    public float darkMatCountCorrection;
    public bool unlockability;
    public int skillNumber;

    public void FindCorectionValue(int _level)
    {
        correctablility = false;
        damageCorrection = 0;
        heartCorrection = 0;
        darkMatCountCorrection = 0;
        unlockability = false;
        skillNumber = 0;

        //정보전달
        if (levelDatabase == null)
            levelDatabase = DataManager.instance._levelDatabase;

        LevelSO levelSO = levelDatabase.GetLevelByLevel(_level);
        if (levelSO != null)
        {
            if (levelSO.correctability)  //플레이어 스텟보정
            {
                correctablility = levelSO.correctability;
                damageCorrection = levelSO.damageCorrection;
                heartCorrection = levelSO.heartCorrection;
                darkMatCountCorrection = levelSO.darkMatCountCorrection;
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

    public void LevelUp(int _level)
    {
        maxEXP = 0;
        nextMaxEXP = 0;

        if (levelDatabase == null)
            levelDatabase = DataManager.instance._levelDatabase;

        if (levelDatabase.maxLevel < _level)  //최대레벨 예외처리
        {
            LogUtil.LogWarning($"{levelDatabase.maxLevel} 레벨의 이상의 레벨은 존재하지 않습니다. 입력받은 레벨: {_level}");
        }
        else
        {
            maxEXP = levelDatabase.GetMaxExp(_level);  //해당 레벨의 최대경험치양 찾기
            //LogUtil.Log($"현재 레벨: {_level}, 다음 최대경험치: {maxEXP}");
        }

        if (levelDatabase.maxLevel < _level + 1)  //최대레벨 예외처리
        {
            LogUtil.LogWarning($"{levelDatabase.maxLevel} 레벨의 이상의 레벨은 존재하지 않습니다. 입력받은 레벨: {_level + 1}");
        }
        else
        {
            nextMaxEXP = levelDatabase.GetMaxExp(_level + 1);  //해당 레벨의 최대경험치양 찾기
        }

        _levelUpEventHandler?.Invoke(this, EventManager.playerEvent);   //최대레벨을 찾는 이벤트 호출
    }

    public bool isEquip;      //장착여부
    public float attackRate;  //데미지률
    public void SetChipAttackRate(bool _isEquip, float _attackRate)
    {
        //LogUtil.Log("작동한다.");
        isEquip = _isEquip;
        attackRate = _attackRate;
        _chipAttackEventHandler?.Invoke(this, EventManager.playerEvent);
    }

    public float damageRate;
    public void SetChipDamageRate(bool _isEquip, float _damageRate)
    {
        isEquip = _isEquip;
        damageRate = _damageRate;
        _chipDamageEventHandler?.Invoke(this, EventManager.playerEvent);
    }

    public float weaponDamageRate;
    public void SetChipWeaponDamageRate(bool _isEquip, float _weaponDamageRate)
    {
        isEquip = _isEquip;
        weaponDamageRate = _weaponDamageRate;
        _chipWeaponDamageEventHandler?.Invoke(this, EventManager.playerEvent);
    }

    public float skillDamageRate;
    public void SetChipSkillDamageRate(bool _isEquip, float _skillDamageRate)
    {
        isEquip = _isEquip;
        skillDamageRate = _skillDamageRate;
        _chipSkillDamageEventHandler?.Invoke(this, EventManager.playerEvent);
    }

    public float coolDownRate;  //쿨타임 감소률
    public void SetCoolDownRate(bool _isEquip, float _coolDownRate)
    {
        isEquip = _isEquip;
        coolDownRate = _coolDownRate;
        _chipcoolDownEventHandler?.Invoke(this, EventManager.playerEvent);
    }

    public float attackTimeRate;
    public void SetAttackTimeRate(bool _isEquip, float _attackTimeRate)
    {
        isEquip = _isEquip;
        attackTimeRate = _attackTimeRate;
        _chipattackTimeDownHandler?.Invoke(this, EventManager.playerEvent);
    }

    public static void Initialize()
    {
        _chipAttackEventHandler = null;
        _chipDamageEventHandler = null;
        _chipWeaponDamageEventHandler = null;
        _chipSkillDamageEventHandler = null;
        _chipcoolDownEventHandler = null;
    }
}
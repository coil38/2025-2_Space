using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FindCorectionValueEvent
{
    private EventHandler<FindCorectionValueEvent> _correctionEventHandler;

    public event EventHandler<FindCorectionValueEvent> correctionEventHandler
    {
        add
        {
            _correctionEventHandler += value;
        }
        remove
        {
            _correctionEventHandler -= value;
        }
    }

    private EventHandler<FindCorectionValueEvent> _levelEventHandler;

    public event EventHandler<FindCorectionValueEvent> levelEventHandler
    {
        add
        {
            _levelEventHandler += value;
        }
        remove
        {
            _levelEventHandler -= value;
        }
    }

    public LevelDatabaseSO levelDatabase;   //레벨 보정관련 데이터 베이스

    public bool correctablility;
    public int damageCorrection;
    public int heartCorrection;
    public int speedCorrection;
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

            _correctionEventHandler.Invoke(this, EventManager.f_CorrectionValueEvent);   //이벤트 호출
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

        _levelEventHandler.Invoke(this, EventManager.f_CorrectionValueEvent);   //최대레벨을 찾는 이벤트 호출
    }
}
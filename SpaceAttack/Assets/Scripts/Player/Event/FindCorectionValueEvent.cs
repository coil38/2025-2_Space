using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FindCorectionValueEvent
{
    private EventHandler<FindCorectionValueEvent> _eventHandler;

    public event EventHandler<FindCorectionValueEvent> eventHandler
    {
        add
        {
            _eventHandler += value;
        }
        remove
        {
            _eventHandler -= value;
        }
    }

    public string test;

    public void FindCorectionValue(int _level)
    {
        ClearData();

        test = "찾는 중...";

        //정보전달
        _eventHandler.Invoke(this, EventManager.f_CorrectionValueEvent);   //이벤트 호출
    }

    private void ClearData()
    {
        //이전에 사용한 정보들 초기화
    }
}
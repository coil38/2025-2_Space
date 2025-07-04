using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerEvent
{
    public int parm_1 = 1;   //전달하고 싶은 매개변수1
    public int parm_2 = 1;   //전달하고 싶은 매개변수1

    private EventHandler<TimerEvent> _eventHandler;
    public event EventHandler<TimerEvent> eventHandler
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

    public void StartEvent(TimerEvent e)
    {
        _eventHandler.Invoke(this, e);
    }
}

public class Timer
{
    private float duration;        //지속시간
    private float remainingTimer;  //남은 시간
    private bool isRunning;        //진행 중 여부 확인
    public TimerEvent timerEvent { get; set; }   //타이머 이벤트 변수 - 외부에서 이벤트 구독 및 해지용

    public Timer(float _duration)
    {
        timerEvent = new TimerEvent();  //생성과 동시에 이벤트클래스도 생성 - 생성자이기 때문에 게임 초기화시, 전부 저절로 사라짐

        duration = _duration;
        remainingTimer = _duration;
        isRunning = false;
    }

    public void Start()             //타이머 시작
    {
        remainingTimer = duration;
        isRunning = true;
    }

    public void Update()             //시작될 시, 실시간으로 시간을 체크하는 주체
    {
        if (isRunning)
        {
            remainingTimer -= duration;
            if (remainingTimer < 0)
            {
                if(timerEvent != null)
                    timerEvent.StartEvent(timerEvent);  //이벤트 실행

                isRunning = false;
                remainingTimer = 0;
            }
        }
    }

    public bool IsRunning()            //현재 실행 여부확인
    {
        return isRunning;
    }

    public float GetRemainingTimer()   //남은 시간확인
    {
        return remainingTimer;
    }

    public void Reset()                 //초화 및 강제 종료함수
    {
        remainingTimer = duration;
        isRunning = false;
    }
}

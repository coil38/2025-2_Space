using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float duration;        //지속시간
    private float remainingTimer;  //남은 시간
    private bool isRunning;        //진행 중 여부 확인
    private bool isEndTimer;       //타이머 종료 여부

    public Timer(float _duration)
    {
        duration = _duration;
        remainingTimer = _duration;
        isRunning = false;
        isEndTimer = false;
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
            remainingTimer -= Time.deltaTime;
            if (remainingTimer < 0)
            {
                isRunning = false;
                remainingTimer = 0;
                isEndTimer = true;   //타이머 종료 활성화
            }
        }
        else
        {
            isEndTimer = false;
        }
    }

    public bool IsRunning()            //현재 실행 여부확인
    {
        return isRunning;
    }

    public float GetRemainingTime()   //남은 시간확인
    {
        return remainingTimer;
    }

    public void Reset()                 //초화 및 강제 종료함수
    {
        remainingTimer = duration;
        isRunning = false;
    }

    public bool IsEndTimer()   //타이머가 시작하고 종료할 때, 한번 실행됨
    {
        return isEndTimer;
    }
}

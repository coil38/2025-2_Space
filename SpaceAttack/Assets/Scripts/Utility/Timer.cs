using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float duration;        //지속시간
    private float remainingTime;  //남은 시간
    private bool isRunning;        //진행 중 여부 확인
    private bool isEndTimer;       //타이머 종료 여부

    public Timer(float _duration)
    {
        duration = _duration;
        remainingTime = 0;
        isRunning = false;
        isEndTimer = false;
    }

    public void Start()             //타이머 시작
    {
        remainingTime = duration;
        isRunning = true;
    }

    public void Update()             //시작될 시, 실시간으로 시간을 체크하는 주체
    {
        if (isRunning)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime < 0)
            {
                isRunning = false;
                remainingTime = 0;
                isEndTimer = true;   //타이머 종료 활성화
            }
        }
        else
        {
            isEndTimer = false;
        }
    }

    public void ChangeDuration(float _duration)
    {
        if (isRunning) return;   //타이머가 실행중일 경우, 처리 안됨

        duration = _duration;
        remainingTime = _duration;
        isRunning = false;
        isEndTimer = false;
    }

    public bool IsRunning()            //현재 실행 여부확인
    {
        return isRunning;
    }

    public float GetRemainingTime()   //남은 시간확인
    {
        return remainingTime;
    }

    public void Reset()                 //초화 및 강제 종료함수
    {
        remainingTime = duration;
        isRunning = false;
    }

    public bool IsEndTimer()   //타이머가 시작하고 종료할 때, 한번 실행됨
    {
        return isEndTimer;
    }
}

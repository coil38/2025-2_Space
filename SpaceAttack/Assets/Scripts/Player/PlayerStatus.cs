using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    //플레이어 상태값--------------------------------------------

    public float m_speed = 5f;
    public float m_movementSmooth = 0.5f;
    public float m_DashForce = 3f;

    [HideInInspector] public bool isInvincibility = false;
    [HideInInspector] public bool isStuned = false;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isDead = false;

    //타이머 생성값--------------------------------------------

    public Timer stunTimer = new Timer(0.3f);
    public Timer invincibilityTimer = new Timer(0.2f);
    public Timer dashTimer = new Timer(0.5f);

    public TimerEvent e = new TimerEvent();
    void Start()                                    // 각각의 타이머변수의 델리게이트에 체인(구독)
    {
        stunTimer.timerEvent.eventHandler += StartDash;
        invincibilityTimer.timerEvent.eventHandler += StartInvincibility;
        stunTimer.timerEvent.eventHandler += StartStun;
    }

    void Update()
    {
        stunTimer.Update();
        invincibilityTimer.Update();
        dashTimer.Update();

        //각각의 상태 실행여부값 할당
        isDashing = dashTimer.IsRunning();
        isInvincibility = invincibilityTimer.IsRunning();
        isStuned = stunTimer.IsRunning();
    }

    private void StartDash(object o, TimerEvent e)    //대시 시작시 실행 할 함수
    {
        Debug.Log("대쉬시작");
    }

    void StartStun(object o, TimerEvent e)   //스턴 시작 시, 실행시킬 함수
    {
        Debug.Log("스턴시작");
    }

    void StartInvincibility(object o, TimerEvent e)   //무적 시작 될시, 실행시킬 함수
    {
        Debug.Log("무적시작");
    }
}

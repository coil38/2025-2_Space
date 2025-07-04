using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    //플레이어 상태값--------------------------------------------
    [Header("PlayerInfo")]
    public float m_hp = 5f;              //체력
    public float m_speed = 5f;           //이동 속도
    public float m_DashDruation = 2.2f;  //대쉬 거리

    [Header("PlayerTimeInfo")]
    public float m_DashTime = 0.1f;      //대쉬 시간
    public float m_stunTime = 0.1f;       //스턴 시간
    public float m_invincibilityTime = 0.1f;  //대쉬이후 무적 시간
    public float w_DashTime = 0.3f;           //다음 대쉬까지 대기 시간


    [HideInInspector] public bool isInvincibility = false;
    [HideInInspector] public bool isStuned = false;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isDead = false;

    //타이머 생성값--------------------------------------------

    public Timer stunTimer;                       //피격 이후 스턴(무적)시간
    public Timer invincibilityTimer;              //대쉬 이후 무적시간
    public Timer deshTimer;                       //대쉬 시간
    public Timer w_dashTimer;                     //대쉬 대기 시간

    void Start()                                    // 각각의 타이머변수의 델리게이트에 체인(구독)
    {
        stunTimer = new Timer(m_stunTime);
        invincibilityTimer = new Timer(m_invincibilityTime);
        deshTimer = new Timer(m_DashTime);
        w_dashTimer = new Timer(w_DashTime);
    }

    void Update()
    {
        stunTimer.Update();
        invincibilityTimer.Update();
        deshTimer.Update();
        w_dashTimer.Update();

        //각각의 상태 실행여부값 할당
        isDashing = w_dashTimer.IsRunning();
        isInvincibility = invincibilityTimer.IsRunning();
        isStuned = stunTimer.IsRunning();

    }

    public void ApplyDamage(float damage)
    {
        if (isInvincibility || isStuned) return;   //대쉬 무적 상태 혹은 스턴 상태일 때, 피격 안됨

        m_hp -= damage;

        if (m_hp <= 0)
        {
            //플레이어 사망 연출 시작
        }
        else
        {
            //피격 사운드
            stunTimer.Start();   //스턴 타이머 시작
            //스턴 연출 시작
        }
    }
}

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

    [HideInInspector] public bool isInvincibility = false;
    [HideInInspector] public bool isStuned = false;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isAttacking = false;

    public bool m_FacingRight {get; private set;}

    private void Start()
    {
        m_FacingRight = true;
    }
    void Update()
    {
        //각각의 상태 실행여부값 할당
        isDashing = TimeSystem.w_dashTimer.IsRunning();
        isInvincibility = TimeSystem.invincibilityTimer.IsRunning();
        isStuned = TimeSystem.stunTimer.IsRunning();
        isAttacking = TimeSystem.w_AttackTimer.IsRunning();
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
            TimeSystem.stunTimer.Start();   //스턴 타이머 시작
            //스턴 연출 시작
        }
    }

    public void Flip()  //좌우반전 로직
    {
        m_FacingRight = !m_FacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}

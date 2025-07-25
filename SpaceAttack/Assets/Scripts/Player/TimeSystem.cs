using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSystem : MonoBehaviour
{
    [Header("PlayerTimeInfo")]
    public static float m_DashTime = 0.1f;       //대쉬 시간
    public static float m_stunTime = 0.3f;       //스턴 시간
    public static float m_invincibilityTime = 0.3f;  //대쉬이후 무적 시간
    public static float w_DashTime = 0.3f;           //다음 대쉬까지 대기 시간

    public static Timer stunTimer;                       //피격 이후 스턴(무적)시간
    public static Timer invincibilityTimer;              //대쉬 이후 무적시간
    public static Timer deshTimer;                       //대쉬 시간
    public static Timer w_dashTimer;                     //대쉬 대기 시간

    //무기별 공격 대기
    public static Timer w_w_AttackTimer;                  //공격 이후 대기시간

    //스킬 대기
    public static Timer s_w_AttackTimer;

    void Start()
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

        if(w_w_AttackTimer != null) 
            w_w_AttackTimer.Update();

        if(s_w_AttackTimer != null)
            s_w_AttackTimer.Update();
    }
}

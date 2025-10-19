using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTimeSystem : MonoBehaviour
{
    [Header("PlayerTimeInfo")]
    public static float m_DashTime = 0.2f;       //대쉬 시간
    public static float m_stunTime = 0.3f;       //스턴 시간
    public static float m_invincibilityTime = 0.3f;  //대쉬이후 무적 시간
    public static float c_DashTime = 3f;              //대쉬 쿨타임

    public static Timer stunTimer;                       //피격 이후 스턴(무적)시간
    public static Timer invincibilityTimer;              //대쉬 이후 무적시간
    public static Timer deshTimer;                       //대쉬 시간
    public static Timer w_dashTimer;                     //대쉬 대기 시간
    public static Timer c_dashTimer;                     //대쉬 쿨타임

    //무기별 공격 대기
    public static Timer w_BaseAttackTimer;                  //공격 이후 대기시간

    //스킬 대기
    public static Timer w_SkillTimer;

    void Start()
    {
        stunTimer = new Timer(m_stunTime);
        invincibilityTimer = new Timer(m_invincibilityTime);
        deshTimer = new Timer(m_DashTime);
        c_dashTimer = new Timer(c_DashTime);
    }

    void Update()
    {
        stunTimer.Update();
        invincibilityTimer.Update();
        deshTimer.Update();
        w_dashTimer.Update();
        c_dashTimer.Update();

        if(w_BaseAttackTimer != null) 
            w_BaseAttackTimer.Update();

        if(w_SkillTimer != null)
            w_SkillTimer.Update();
    }
}

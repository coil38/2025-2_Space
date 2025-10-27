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
    public static Timer c_dashTimer;                     //대쉬 쿨타임

    //무기별 공격 대기
    public static Timer w_BaseAttackTimer;                  //공격 이후 대기시간

    //스킬 대기
    public static Timer w_SkillTimer;
    private static Dictionary<float, Timer> skillTimers = new Dictionary<float, Timer>();
    private static Dictionary<float, Timer> weaponTimers = new Dictionary<float, Timer>();

    public static void SetChipTimer(float time, ChipAttackType type) //스킬, 기본공격 타이머 설정 함수
    {
        if (type == ChipAttackType.Weapon)
        {
            if (weaponTimers.TryGetValue(time, out Timer timer)) w_BaseAttackTimer = timer;
            else
            {
                LogUtil.Log($"새로운 공격 타이머 저장: {time}");
                w_BaseAttackTimer = new Timer(time);
                weaponTimers.Add(time, w_BaseAttackTimer);
            }
        }
        else if (type == ChipAttackType.Skill)
        {
            if (skillTimers.TryGetValue(time, out var value)) w_SkillTimer = value;
            else
            {
                w_SkillTimer = new Timer(time);
                skillTimers.Add(time, w_SkillTimer);
            }
        }
    }
    public static void SetStunTimer(float time)  //스턴 타이머 설정 함수
    {
        float temp = m_stunTime;
        m_stunTime = time;
        stunTimer = new Timer(time);
        LogUtil.Log($"기존 스턴시간: {temp}, 변경된 스턴 시간: {m_stunTime}");
    }

    public static void SetAndStartInvincibilityTimer(float time)
    {
        m_invincibilityTime = time;
        invincibilityTimer = new Timer(time);
        invincibilityTimer.Start();
    }

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
        c_dashTimer.Update();

        if(w_BaseAttackTimer != null) 
            w_BaseAttackTimer.Update();

        if(w_SkillTimer != null)
            w_SkillTimer.Update();
    }
}

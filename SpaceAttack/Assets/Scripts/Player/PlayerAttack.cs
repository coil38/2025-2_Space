using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerStatus playerState;
    private Rigidbody rb;

    private WeaponType weaponType;        //의존성 주입
    public WeaponType WeaponType
    {
        set { weaponType = value; }
    }

    private SkillType[] skillTypes;      //의존성 주입
    public SkillType[] SkillTypes
    {
        set { skillTypes = value; }
    }

    void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(playerState.isDead || Time.timeScale == 0) return;   //사망시, 입력 안됨

        UpdateSkillAndWeaponInfo();
        CheckWeaponAttack();;
        CheckSkillAttack();
    }

    private void UpdateSkillAndWeaponInfo()
    {
        if (weaponType == null || skillTypes == null) return;

        weaponType.UpdateInfo();
        foreach (var skill in skillTypes)
            skill.UpdateInfo();
    }

    private void CheckWeaponAttack()
    {
        if(PlayerUIManager.instance != null)
            if(PlayerUIManager.instance.isInventorySlotButtonClick) return;   //인벤토리 슬롯 클릭일 경우, 반환

        if (PlayerTimeSystem.w_SkillTimer != null)
            if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;  //현재 공격중일 경우, 반환

        if (PlayerTimeSystem.stunTimer != null)
            if (PlayerTimeSystem.stunTimer.IsRunning()) return; //스턴 상태일 경우, 반환

        if (PlayerTimeSystem.deshTimer != null)
            if (PlayerTimeSystem.deshTimer.IsRunning()) return;  //데쉬 도중일 경우, 반환

        if (weaponType == null) return;  //현재 보유 중 무기가 없을 시, 반환

        //무기 시스템 연결
        weaponType.CheckUse(transform.position);

        if ((playerState.m_FacingRight && weaponType.attackDirection.x > 0) || (!playerState.m_FacingRight && weaponType.attackDirection.x < 0))  //공격방향과 현재방향이 불일치 경우
        {
            if (weaponType.isAttacking)
            {
                //Debug.Log("실행된다2.");
                playerState.Flip();
            }
        }

        if (weaponType.isAttackMoving)  //무기이동 실행
            rb.MovePosition(weaponType.attackMovePos);
    }

    private void CheckSkillAttack()
    {
        if (PlayerTimeSystem.w_BaseAttackTimer != null)
            if (PlayerTimeSystem.w_BaseAttackTimer.IsRunning()) return;  //현재 기본공격 실행중일 경우, 반환

        if (PlayerTimeSystem.deshTimer != null)
            if (PlayerTimeSystem.deshTimer.IsRunning()) return;  //데쉬 도중일 경우, 반환

        if (PlayerTimeSystem.stunTimer != null)
            if (PlayerTimeSystem.stunTimer.IsRunning()) return; //스턴 상태일 경우, 반환

        if (skillTypes == null) return; //현재 보유 중 스킬이 없을 시, 반환

        //스킬 시스템 연결
        foreach (var skill in skillTypes)
        {
            skill.CheckUse(transform.position);

            if ((playerState.m_FacingRight && skill.attackDirection.x > 0) || (!playerState.m_FacingRight && skill.attackDirection.x < 0))  //공격방향과 현재방향이 불일치 경우
            {
                if (skill.isAttacking)
                {
                    playerState.Flip();
                }
            }

            if (skill.isAttackMoving)  //무기이동 실행
                rb.MovePosition(skill.attackMovePos);
        }
    }
}


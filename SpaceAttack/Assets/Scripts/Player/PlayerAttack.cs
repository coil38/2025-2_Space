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

    //public SkillType[] skills;         //테스용
    //public WeaponType weapon;
    //public bool notUseTestMode;

    void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(playerState.isDead) return;   //사망시, 입력 안됨

        CheckWeaponAttack();
        CheckSkillAttack();
    }

    private void CheckWeaponAttack()
    {
        if (PlayerTimeSystem.w_SkillTimer != null)
            if (PlayerTimeSystem.w_SkillTimer.IsRunning()) return;  //현재 공격중일 경우, 반환

        if (weaponType == null) return;  //현재 보유 중 무기가 없을 시, 반환

        //무기 시스템 연결
        weaponType.CheckAttack(transform.position);

        if ((playerState.m_FacingRight && weaponType.attackDirection.x > 0) || (!playerState.m_FacingRight && weaponType.attackDirection.x < 0))  //공격방향과 현재방향이 불일치 경우
        {
            if (weaponType.isAttacking)
            {
                //Debug.Log("실행된다2.");
                playerState.Flip();
            }
        }

        weaponType.UpdateInfo();

        if (weaponType.isAttackMoving)  //무기이동 실행
            rb.MovePosition(weaponType.attackMovePos);
    }

    private void CheckSkillAttack()
    {
        if (PlayerTimeSystem.w_BaseAttackTimer != null)
            if (PlayerTimeSystem.w_BaseAttackTimer.IsRunning()) return;  //현재 스킬 실행중일 경우, 반환

        if(skillTypes == null) return; //현재 보유 중 스킬이 없을 시, 반환

        //스킬 시스템 연결
        foreach (var skill in skillTypes)
        {
            skill.CheckAttack(transform.position);

            if ((playerState.m_FacingRight && skill.attackDirection.x > 0) || (!playerState.m_FacingRight && skill.attackDirection.x < 0))  //공격방향과 현재방향이 불일치 경우
            {
                if (skill.isAttacking)
                {
                    playerState.Flip();
                }
            }
            skill.UpdateInfo();

            if (skill.isAttackMoving)  //무기이동 실행
                rb.MovePosition(skill.attackMovePos);
        }
    }

    public void SetAnimationObj(string name, bool par1, int par2, float par3)
    {

    }
}


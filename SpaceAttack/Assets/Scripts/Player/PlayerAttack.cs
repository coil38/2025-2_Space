using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerStatus playerState;
    private Rigidbody rb;

    private WeaponType weaponType;        //무기 임시 테스트용_의존성 주입
    public WeaponType WeaponType
    {
        set { weaponType = value; }
    }

    public WeaponType weapon;            //임시로 추가


    private SkillType[] skillTypes;      //스킬 임시 테스트용_의존성 주입
    public SkillType[] SkillTypes
    {
        set { skillTypes = value; }
    }
    //public SkillType[] skills;

    public bool notUseTestMode;

    void Start()
    {
        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody>();

        if (!notUseTestMode)            //무기 시스템 연결
        {
            WeaponType = weapon;   //의존성 주입(임시)
            TimeSystem.w_w_AttackTimer = weapon.w_AttackTimer;  //대기시간 설정
            weaponType.attackAnimator = GetComponent<Animator>();  //애니메이터 전달
        }

        //스킬 시스템 연결
        //SkillTypes = skills;

        //if (skillTypes.Length <= 0) return;
        //TimeSystem.s_w_AttackTimer = skillTypes[0].s_AttackTimer;  //대기시간 설정

        //foreach (var skill in skillTypes)
        //{
        //    skill.attackAnimator = GetComponent<Animator>();
        //    skill.lineRenderer = GetComponent<LineRenderer>();
        //}

        //LineRenderer lineRenderer = GetComponent<LineRenderer>();
        //lineRenderer.enabled = false;
        //lineRenderer.alignment = LineAlignment.TransformZ;
        //lineRenderer.useWorldSpace = true;
        //lineRenderer.transform.right = Vector3.up;
    }
    void Update()
    {
        if(playerState.isDead) return;   //사망시, 입력 안됨

        CheckWeaponAttack();
        CheckSkillAttack();
    }

    private void CheckWeaponAttack()
    {
        if (TimeSystem.s_w_AttackTimer != null)
            if (TimeSystem.s_w_AttackTimer.IsRunning()) return;

        if (weaponType == null) return;  //현재 보유 중 무기가 없을 시, 반환

        //무기 시스템 연결
        weaponType.CheckAttack(transform.position);

        if ((playerState.m_FacingRight && weaponType.attackDirection.x < 0) || (!playerState.m_FacingRight && weaponType.attackDirection.x > 0))  //공격방향과 현재방향이 불일치 경우
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
        if (TimeSystem.w_w_AttackTimer != null)
            if (TimeSystem.w_w_AttackTimer.IsRunning()) return;

        if(skillTypes == null) return; //현재 보유 중 스킬이 없을 시, 반환

        //스킬 시스템 연결
        foreach (var skill in skillTypes)
        {
            skill.CheckAttack(transform.position);

            if ((playerState.m_FacingRight && skill.attackDirection.x < 0) || (!playerState.m_FacingRight && skill.attackDirection.x > 0))  //공격방향과 현재방향이 불일치 경우
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
}


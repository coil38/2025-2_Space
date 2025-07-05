using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private WeaponType weaponType;
    public WeaponType WeaponType
    {
        set { weaponType = value; }
    }

    private PlayerStatus playerState;
    private Rigidbody2D rb;
    //임시 테스트용_의존성 주입
    public WeaponSword weaponSword;

    private bool c_isAttacking;
    void Start()
    {
        WeaponType = weaponSword;   //의존성 주입(임시)

        playerState = GetComponent<PlayerStatus>();
        rb = GetComponent<Rigidbody2D>();

        weaponType.attackAnimator = GetComponent<Animator>();  //애니메이터 전달
        weaponType.SetInfo();
    }

    void Update()
    {
        weaponType.CheckAttack((Vector2)transform.position);

        if ((playerState.m_FacingRight && weaponType.attackDirection.x < 0) || (!playerState.m_FacingRight && weaponType.attackDirection.x > 0))  //공격방향과 현재방향이 불일치 경우
        {
            if (weaponType.isAttacking)
            {
                playerState.Flip();
                Debug.Log("공격 실행된다");
            }
        }

        weaponType.UpdateInfo();

        if (weaponType.isAttackMoving)  //무기이동 실행
        {
            rb.MovePosition(weaponType.attackMovePos);
        }
    }
}

public class AttackInfo      //공격 정보 전달용 클래스
{
    public float damage;
    public Vector2 attackDirection;
}
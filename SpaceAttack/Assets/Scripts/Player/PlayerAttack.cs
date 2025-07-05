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

    //임시 테스트용_의존성 주입
    public WeaponSword weaponSword;
    void Start()
    {
        WeaponType = weaponSword;   //의존성 주입(임시)
        weaponSword.attackAnimator = GetComponent<Animator>();  //애니메이터 전달
    }

    void Update()
    {
        weaponType.CheckAttack((Vector2)transform.position);
    }
}

public class AttackInfo      //공격 정보 전달용 클래스
{
    public float damage;
    public Vector2 attackDirection;
}
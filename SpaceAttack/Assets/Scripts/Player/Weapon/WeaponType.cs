using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponType : MonoBehaviour, IAttack, ICheckAttack
{
    //임시로 플레이어에 할당
    public event Action<PlayerAniInfo> weaponAniDelegate;  //애니메이션 실행 또는 공격본활성화를 위한 델리게이트
    public Vector3 attackDirection { get; protected set; }  //공격 사거리

    protected Vector3 _currentPos;                          //현재 위치
    public bool isAttacking { get; protected set; }         //공격 여부

    public Vector3 attackMovePos { get; protected set; }    //공격 이동 위치 변수
    public bool isAttackMoving { get; protected set; }      //공격 이동 여부
    public Timer w_AttackTimer { get; protected set; }      //다음 공격 대기 타이머
    public Timer m_AttackTimer { get; protected set; }        //공격 이동 타이머(원거리용)
    public Timer r_AttackTimer { get; protected set; }        //공격 애니메이션 대기

    //--------------------------------------------------------------------------------------------------

    //레이어 설정
    protected LayerMask planLayer;   //바닥감지용 리이어 마스크
    protected LayerMask enemyLayer;  //적감지용 레이어 마스크
    protected LayerMask wallLayer;   //벽감지용 레이어 마스크

    //내부 기능
    public float damage { get; set; }
    public float damageRate { get; protected set; }

    public abstract void CheckAttack(Vector3 currentPos);

    public abstract void Attack();

    public virtual void OnEnable()  //공용 레이어 설정
    {
        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        enemyLayer |= (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("DestructableObject")) | (1 << LayerMask.NameToLayer("Boss"));
        wallLayer |= 1 << LayerMask.NameToLayer("Wall");
    }

    public abstract void UpdateInfo();

    protected void PlayAniMation(PlayerAniInfo info)
    {
        weaponAniDelegate.Invoke(info);
    }

}
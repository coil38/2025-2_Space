using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponType : MonoBehaviour, IAttack, ICheckAttack
{
    //임시로 플레이어에 할당
    public Animator attackAnimator { get; set; }            //공격 내부에서 쓸 애니메이터
    public Vector3 attackDirection { get; protected set; }  //공격 사거리

    protected Vector3 _currentPos;                          //현재 위치
    public bool isAttacking { get; protected set; }         //공격 여부

    public Vector3 attackMovePos { get; protected set; }    //공격 이동 위치 변수
    public bool isAttackMoving { get; protected set; }      //공격 이동 여부
    public Timer w_AttackTimer { get; protected set; }      //다음 공격 대기 타이머
    public Timer m_AttackTimer { get; protected set; }        //공격 이동 타이머(원거리용)
    public Timer r_AttackTimer { get; protected set; }        //공격 애니메이션 대기

    public abstract void CheckAttack(Vector3 currentPos);

    public abstract void Attack();

    //Start 대신으로 임시 지정
    public abstract void OnEnable();
    //Update 대신으로 임시 지정
    public abstract void UpdateInfo();

}
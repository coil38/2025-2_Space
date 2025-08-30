using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillType : MonoBehaviour, IAttack, ICheckAttack
{
    public float damage { get;  set; }                       //공격력
    public float initialDamage { get; protected set; }       //초기 데미지
    public float damageRate {  get; protected set; }         //데미지 비율
    public float mass { get; protected set; }                //공격무기 질량
    public float attackDistance { get; protected set; }      //공격 거리
    public float attackWidth { get; protected set; }         //공격 너비
    public float attackTime { get; protected set; }          //공격 시간
    public float r_AttackTime { get; protected set; }      //공격 대기 시간
    public float coolTime { get; protected set; }            //쿨타임
    public Timer coolTimer { get; protected set; }           //쿨타임 타이머
    public int unLockedNumber { get; protected set; }      //해금 대상 번호
    public bool canUse { get; set; }                        //사용 가능 여부
    //public Sprite generateSprit { get; set; }      //장판 스프라이트 이미지

    //------------------------------------------------------------------------------------------------------

    //임시로 플레이어에 할당

    public event Action<PlayerAniInfo> skillAniDelegate;  //애니메이션 실행 또는 공격본활성화를 위한 델리게이트
    public Animator attackAnimator { get; set; }            //공격 내부에서 쓸 애니메이터
    public LineRenderer lineRenderer { get; set; }          //공격 범위 표시
    public Vector3 attackDirection { get; protected set; }  //공격 방향

    protected Vector3 _currentPos;                          //현재 위치
    public bool isAttacking { get; protected set; }         //공격 여부

    public Vector3 attackMovePos { get; protected set; }    //공격 이동 위치 변수
    public bool isAttackMoving { get; protected set; }      //공격 이동 여부
    public Timer w_AttackTimer { get; protected set; }      //다음 공격 후, 플레이어 대기 타이머 !! --> w_AttackTimer와 s_AttackTimer가 같을 경우, s_AttackTimer만 사용
    public Timer s_AttackTimer { get; protected set; }      //공격 Lerp용 타이머


    public abstract void CheckAttack(Vector3 currentPos);

    public abstract void Attack();

    public abstract void OnEnable();

    public abstract void UpdateInfo();
}

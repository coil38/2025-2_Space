using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillType : MonoBehaviour, IAttack, ICheckAttack
{
    public float damage { get;  set; }                       //공격력
    public float damageRate {  get; protected set; }         //데미지 비율
    public float mass { get; protected set; }                //공격무기 질량
    public float attackDistance { get; protected set; }      //공격 거리
    public float attackWidth { get; protected set; }         //공격 너비
    public float attackTime { get; protected set; }          //공격 시간
    public float r_AttackTime { get; protected set; }      //공격 대기 시간
    public Timer coolTimer { get; protected set; }           //쿨타임 타이머
    public int unLockedNumber { get; protected set; }      //해금 대상 번호
    public bool canUse { get; set; }                        //사용 가능 여부
    //public Sprite generateSprit { get; set; }      //장판 스프라이트 이미지

    protected float _coolTime;                              //쿨타임
    public float normalCoolTime { get; protected set; }     //기본 쿨타임
    public float coolTime
    {
        get { return _coolTime; }
        set
        {
            _coolTime = value;
            coolTimer = new Timer(value);                   //쿨타입값 변경시, 인스턴스 변경
        }
    }

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

    //----------------------------------------------------------------------------------------

    //레이어

    protected LayerMask planLayer;   //바닥감지용 리이어 마스크
    protected LayerMask wallLayer;   //벽감지용 레이어 마스크
    protected LayerMask enemyLayer;  //적감지용 레이어 마스크

    public abstract void CheckAttack(Vector3 currentPos);

    public abstract void Attack();

    public virtual void OnEnable()  //공용 레이어 설정
    {
        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        wallLayer |= 1 << LayerMask.NameToLayer("Wall");
        enemyLayer |= (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("DestructableObject")) | (1<< LayerMask.NameToLayer("Boss"));
    }

    public abstract void UpdateInfo();
}

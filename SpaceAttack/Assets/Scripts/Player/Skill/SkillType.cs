using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class SkillType : MonoBehaviour
{
    //외부에서 접근하는 변수들
    public int unLockedNumber { get; protected set; }      //해금 대상 번호
    public bool isUnLocked { get; set; }                   //사용 가능 여부
    public float normalCoolTime { get { return chipCompSO.coolTime[0]; } }
    public float normalAttackTime { get { return chipCompSO.attackTime[0]; } }
    public float coolTime
    {
        get { return _coolTime; }
        set
        {
            _coolTime = value;
            coolTimer = new Timer(value);                   //쿨타입값 변경시, 인스턴스 변경
        }
    }
    public float attackTime
    {
        get { return m_attackTime; }
        set
        {
            //LogUtil.Log($"공격시간은 : {value * 1.1f}");
            PlayerTimeSystem.SetChipTimer(value * 1.1f, ChipAttackType.Skill);
            readyAttackTime = value * 0.5f;
            m_attackTime = value;

            projectileMoveTime = value;
        }
    }

    public event Action<PlayerAniInfo> skillAniDelegate;  //애니메이션 실행 또는 공격본활성화를 위한 델리게이트
    public Animator attackAnimator { get; set; }            //공격 내부에서 쓸 애니메이터

    public LineRenderer lineRenderer { get; set; }          //공격 범위 표시
    public Vector3 attackDirection { get; protected set; }  //공격 방향
    public bool isAttacking { get; protected set; }         //공격 여부

    public Vector3 attackMovePos { get; protected set; }    //공격 이동 위치 변수
    public bool isAttackMoving { get; protected set; }      //공격 이동 여부
    public Timer s_AttackTimer { get; protected set; }      //공격 Lerp용 타이머

    //내부에서만 접근가능한 변수들

    protected BaseChipset chipset;        //부모 칩셋타입

    protected int chipsetCompID = 103;
    protected float[] damageRates;
    protected float[] coolTimes;
    protected float[] addedCritRates;
    protected float[] addedCritChanceRates;
    protected float[] attackTimes;
    protected float[] attackDistances;

    protected float damageRate;            //데미지 비율
    protected float addedCritRate;
    protected float addedCritChanceRate;
    protected float attackDistance;      //공격 거리
    protected float attackWidth;         //공격 너비
    private float m_attackTime;          //공격 시간
    protected float readyAttackTime;     //공격 준비 시간
    protected Timer coolTimer;           //쿨타임 타이머
    private float _coolTime;             //쿨타임
    protected Vector3 _currentPos;       //현재 위치
    protected Vector3 detectSize;        //감지범위
    private ChipsetComponentSO chipCompSO;  //칩셋구성SO
    private float _projectileMoveTime;

    private VisualAttackRange visualAttackRange;   //바닥 공격범위
    private VisualAttackRange[] visualAttackRanges;  //바닥 공격범위
    protected float projectileMoveTime     //발사체 이동시간
    {
        set
        {
            _projectileMoveTime = value;
            //LogUtil.Log($"발사체 이동 타이머설정, 설정시간: {value}");
            p_MoveTimer = new Timer(value);
        }
    }
    protected Timer p_MoveTimer;             //발사체 이동타이머

    protected float _attackDistance;
    protected float _attackTime;

    //레이어
    protected LayerMask planLayer;   //바닥감지용 리이어 마스크
    protected LayerMask wallLayer;   //벽감지용 레이어 마스크
    protected LayerMask enemyLayer;  //적감지용 레이어 마스크

    public abstract void CheckUse(Vector3 currentPos);

    public virtual void Use()
    {
        EventManager.relicEvent.OnPlayerUseSkillEvent();      //스킬 사용 이벤트 실행
    }

    public virtual void OnEnable()  //공용 레이어 설정
    {
        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        wallLayer |= 1 << LayerMask.NameToLayer("Wall");
        enemyLayer |= (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("DestructableObject")) | (1<< LayerMask.NameToLayer("Boss"));

        if(chipset == null)
            chipset = gameObject.GetComponent<BaseChipset>();

        chipCompSO = DataManager.instance._chipCompDatabase.GetChipsetComponentByID(chipsetCompID);
        damageRates = chipCompSO.damageRate;
        coolTimes = chipCompSO.coolTime;
        addedCritRates = chipCompSO.addedCritRate;
        addedCritChanceRates = chipCompSO.addedCritChanceRate;
        attackTimes = chipCompSO.attackTime;
        attackDistances = chipCompSO.attackRange;

        if (chipCompSO.damageRate.Length == 1)
        {
            damageRate = chipCompSO.damageRate[0];
            coolTime = chipCompSO.coolTime[0];
            addedCritChanceRate = chipCompSO.addedCritChanceRate[0];
            addedCritRate = chipCompSO.addedCritRate[0];
            attackTime = chipCompSO.attackTime[0];
            attackDistance = chipCompSO.attackRange[0];
        }

        //LogUtil.Log($"기술 이름: {chipCompSO.chipsetCpname}, 공격비율: {damageRate}, 쿨타임: {coolTime}, 추가 치확: {addedCritChanceRate}, 추가 치피: {addedCritRate}, 공격시간: {attackTime}, 공격 거리: {attackDistance}");
    }

    public virtual void UpdateInfo()
    {
        coolTimer?.Update();
        p_MoveTimer?.Update();
    }

    protected void PlayAniMation(PlayerAniInfo info)
    {
        skillAniDelegate?.Invoke(info);
    }

    protected Vector3 GetAttackDirection(Vector3 currentPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);   //마우스 위치 받기
        Vector3 mousePos = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, planLayer))
            mousePos = hit.point;
        mousePos.y = currentPos.y;

        return (mousePos - _currentPos).normalized;   //플레이어 기준 마우스 방향 얻기
    }

    protected void OnVisualAttackRange(Vector3 genPos, float distance, float width, Vector3 dir, float lifeTime)
    {
        if (visualAttackRange == null)
        {
            GameObject obj = Instantiate(DataManager.instance.VisualAttackRange);
            visualAttackRange = obj.GetComponent<VisualAttackRange>();
        }

        visualAttackRange.OnAttackRange(genPos, distance, width, dir, lifeTime);
    }

    protected void OnVisualAttackRanges(Vector3[] genPos, float distance, float width, Vector3[] dir, float lifeTime)
    {
        if (visualAttackRanges == null || visualAttackRanges.Length < dir.Length)
        {
            visualAttackRanges = new VisualAttackRange[dir.Length];

            for (int i = 0; i < visualAttackRanges.Length; i++)
            {
                GameObject obj = Instantiate(DataManager.instance.VisualAttackRange);
                visualAttackRanges[i] = obj.GetComponent<VisualAttackRange>();
            }
        }

        for(int i = 0; i < dir.Length; i++)
            visualAttackRanges[i].OnAttackRange(genPos[i], distance, width, dir[i], lifeTime);
    }
}

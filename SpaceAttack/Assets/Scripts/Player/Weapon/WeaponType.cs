using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponType : MonoBehaviour, IUse, ICheckUse
{
    //외부에서 접근하는 변수들

    public event Action<PlayerAniInfo> weaponAniDelegate;  //애니메이션 실행 또는 공격본활성화를 위한 델리게이트
    public Vector3 attackDirection { get; protected set; }  //공격 방향
    public bool isAttacking { get; protected set; }         //공격 여부
    public Vector3 attackMovePos { get; protected set; }    //공격 이동 위치 변수
    public bool isAttackMoving { get; protected set; }      //공격 이동 여부
    public float normalAttackTime { get { return chipCompSO.attackTime[0]; } }
    public float attackTime
    {
        get { return _attackTime; }
        set
        {
            //LogUtil.Log($"공격시간은 : {value * 1.1f}");
            PlayerTimeSystem.SetChipTimer(value * 1.1f, ChipAttackType.Weapon);
            readyAttackTime = value * 0.5f;
            _attackTime = value;
        }
    }

    //내부 변수들

    protected BaseChipset chipset;                          //부모 칩셋타입

    protected Vector3 _currentPos;                          //현재 위치
    protected int chipsetCompID = 103;
    protected float coolTime;
    protected float addedCritRate;
    protected float addedCritChanceRate;
    protected float attackDistance;
    protected float readyAttackTime;
    protected float damageRate;
    private float _attackTime;
    private ChipsetComponentSO chipCompSO;
    
    //레이어 설정
    protected LayerMask planLayer;   //바닥감지용 리이어 마스크
    protected LayerMask enemyLayer;  //적감지용 레이어 마스크
    protected LayerMask wallLayer;   //벽감지용 레이어 마스크

    public abstract void CheckUse(Vector3 currentPos);

    public abstract void Use();

    public virtual void OnEnable()  //공용 레이어 설정
    {
        planLayer |= 1 << LayerMask.NameToLayer("Plan");
        enemyLayer |= (1 << LayerMask.NameToLayer("Enemy")) | (1 << LayerMask.NameToLayer("DestructableObject")) | (1 << LayerMask.NameToLayer("Boss"));
        wallLayer |= 1 << LayerMask.NameToLayer("Wall");

        if (chipset == null)
            chipset = gameObject.GetComponent<BaseChipset>();

        chipCompSO = DataManager.instance._chipCompDatabase.GetChipsetComponentByID(chipsetCompID);
        damageRate = chipCompSO.damageRate[0];
        coolTime = chipCompSO.coolTime[0];
        addedCritChanceRate = chipCompSO.addedCritChanceRate[0];
        addedCritRate = chipCompSO.addedCritRate[0];
        attackTime = chipCompSO.attackTime[0];
        attackDistance = chipCompSO.attackRange[0];
    }

    public abstract void UpdateInfo();

    protected void PlayAniMation(PlayerAniInfo info)
    {
        weaponAniDelegate?.Invoke(info);
    }

}
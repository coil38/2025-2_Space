using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGuidedArrow : MonoBehaviour
{
    private Rigidbody rb;
    private ChipSetType chipset;      //칩셋
    private ChipAttackType chipsetAttackType;
    private GameObject target;
    private GameObject beforeHittedTarget;

    private Vector3 attackDirection;  //이동 방향
    //private float moveDistance;       //최대 이동 거리
    private float damageRate;         //데미지 비율
    private float addedCritChanceRate;
    private float addedCritRate;

    private bool startTracking = false;       //추적 시작
    private float trackingSpeed;              //추적 속도

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!startTracking || target == null) return;

        Vector3 moveDir = (target.transform.position - transform.position).normalized;
        Quaternion quaternion = Quaternion.LookRotation(moveDir, Vector3.up);
        Debug.Log($"이동 방향: {moveDir}, 타겟 위치: {target.transform.position}");

        transform.rotation = Quaternion.Slerp(transform.rotation, quaternion, 30f * Time.deltaTime);
        transform.position += moveDir * trackingSpeed * Time.deltaTime;
    }

    public void Fire(Vector3 dir, float speed, float damageRate, float dis, float addedCriChanceRate, float addedCriRate, ChipSetType chipset, ChipAttackType chipsetAttackType, 
        GameObject target, GameObject beforeHittedTarget)  //이동 위치, 이동방향, 이동 속도, 공격력
    {
        if (rb == null)
            rb = gameObject.GetComponent<Rigidbody>();

        startTracking = true;

        attackDirection = dir;
        trackingSpeed = speed;
        this.chipset = chipset;
        this.chipsetAttackType = chipsetAttackType;
        this.target = target;
        this.beforeHittedTarget = beforeHittedTarget;
        this.damageRate = damageRate;
        this.addedCritRate = addedCriRate;
        this.addedCritChanceRate = addedCriChanceRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == beforeHittedTarget) return;   //전에 피격한 대상일 경우, 반환처리

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            chipset.Attack(other.gameObject, damageRate, attackDirection, addedCritChanceRate, addedCritRate, chipsetAttackType);
            Destroy(gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("DestructableObject"))
        {
            AttackInfo info = new AttackInfo(PlayerStatus.normalDamage * damageRate, attackDirection, 1, gameObject);
            other.SendMessage("ApplyDamage", info);
            Destroy(gameObject);
        }
        else if (!other.gameObject.CompareTag("Arrow") && !other.gameObject.CompareTag("Player"))
        {
            LogUtil.Log("파괴파괴 " + other.gameObject.tag);
            Destroy(gameObject);
        }
    }
}

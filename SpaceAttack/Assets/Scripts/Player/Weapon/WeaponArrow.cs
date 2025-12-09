using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class WeaponArrow : MonoBehaviour
{
    private Rigidbody rb;
    private ChipSetType chipset;      //칩셋
    private ChipAttackType chipsetAttackType;


    private Vector3 startPos;         //시작위치
    private Vector3 attackDirection;  //이동 방향
    private float moveDistance;       //최대 이동 거리
    private float damageRate;         //데미지 비율
    private float addedCritChanceRate;
    private float addedCritRate;

    private Action<GameObject> hitEvent;

    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    void Update()
    {
        float dis = Vector3.Distance(transform.position, startPos);
        if (dis > moveDistance) Destroy(gameObject);
    }
    public void SetEvent(Action<GameObject> action)
    {
        hitEvent += action;
    }

    public void Fire(Vector3 dir, float speed, float damageRate, float dis, float addedCriChanceRate, float addedCriRate, ChipSetType chipset, ChipAttackType chipsetAttackType)  //이동 위치, 이동방향, 이동 속도, 공격력
    {
        if (rb == null)
            rb = gameObject.GetComponent<Rigidbody>();

        rb.AddForce(dir * speed, ForceMode.Impulse);
        attackDirection = dir;
        moveDistance = dis;
        this.chipset = chipset;
        this.chipsetAttackType = chipsetAttackType;
        this.damageRate = damageRate;
        this.addedCritRate = addedCriRate;
        this.addedCritChanceRate = addedCriChanceRate;
    }

    private void OnTriggerEnter(Collider other)
    {
        //LogUtil.Log($"충돌대상: {other.gameObject.tag}");

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Boss"))
        {
            chipset.Attack(other.gameObject, damageRate, attackDirection, addedCritChanceRate, addedCritRate, chipsetAttackType);
            HitAndDestroy(other.gameObject);
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("DestructableObject"))
        {
            chipset.Attack(other.gameObject, damageRate, attackDirection, addedCritChanceRate, addedCritRate, chipsetAttackType);
            HitAndDestroy(other.gameObject);
        }
        else if (!other.gameObject.CompareTag("Arrow") && !other.gameObject.CompareTag("Player"))
        {
            //LogUtil.Log("파괴파괴 " + other.gameObject.tag);
            HitAndDestroy(other.gameObject);
        }
    }

    private void HitAndDestroy(GameObject hittedObj)
    {
        hitEvent?.Invoke(hittedObj);
        Destroy(gameObject);
    }
}

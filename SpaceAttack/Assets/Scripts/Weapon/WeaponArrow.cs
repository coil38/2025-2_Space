using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponArrow : MonoBehaviour
{
    private Rigidbody rb;

    private bool isFired = false;
    private Vector3 startPos;
    private float damage;
    private Vector3 attackDirection;
    private float fireDistance;
    void OnEnable()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isFired)
        {
            float dis = Vector3.Distance(transform.position, startPos);
            if (dis > fireDistance)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Fire(Vector3 _startPos, Vector3 dir, float speed, float _damage, float dis)  //이동 위치, 이동방향, 이동 속도, 공격력
    {
        if (rb == null)
        {
            Debug.Log("없음");
        }
        rb.AddForce(dir * speed, ForceMode.Impulse);
        startPos = _startPos;
        damage = _damage;
        attackDirection = dir;
        fireDistance = dis;
        isFired = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            AttackInfo attackInfo = new AttackInfo();
            attackInfo.damage = damage;
            attackInfo.attackDirection = attackDirection;

            collision.gameObject.SendMessage("ApplyDamage", attackInfo);
            Debug.Log("화살 공격");

            Destroy(gameObject);
        }
        else if(collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Destroy(gameObject);
        }
    }
}

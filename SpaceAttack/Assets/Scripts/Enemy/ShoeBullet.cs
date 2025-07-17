using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeBullet : MonoBehaviour
{
    [Header("총알 정보")]
    public float damage = 1f;
    public float lifetime = 3f;      
    public float knockback = 2f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
    }
    public void Init(Vector3 velocity)
    {
        rb.velocity = velocity;

        bool isFacingRight = velocity.x > 0;

        // BillBoard 스크립트에 좌우 방향 전달
        BillBoard bb = GetComponent<BillBoard>();
        if (bb != null)
        {
            bb.SetFacingRight(isFacingRight);
        }
    }

    private void OnTriggerEnter(Collider other)   //플레이어 찾기
    {
        if (other.CompareTag("Player"))
        {
            PlayerStatus ps = other.GetComponent<PlayerStatus>();
            if (ps)
            {
                AttackInfo info = new AttackInfo
                {
                    damage = damage,
                    attackDirection = transform.forward * knockback
                };
                ps.ApplyDamage(info);
            }
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}

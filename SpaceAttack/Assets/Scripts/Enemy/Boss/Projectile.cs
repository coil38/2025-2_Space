using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Projectile : MonoBehaviour
{
    public float speed = 5f;          // 초기 속도
    public float damage = 1f;         // 데미지
    public float bounceForce = 2f;    // 초기 튀는 힘
    public float bounceDuration = 1.5f; // 통통 튀는 지속 시간
    private float timer;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        timer = 0f;
    }

    public void Init(Vector3 direction)
    {
        Vector3 vel = direction.normalized * speed;
        vel.y = 0f; // 초기에는 Y축 속도 0
        rb.velocity = vel;
    }

    void Update()
    {
        timer += Time.deltaTime;

        // bounceDuration 이후에는 더 이상 Y축 튀지 않게
        if (timer >= bounceDuration)
        {
            Vector3 vel = rb.velocity;
            vel.y = 0f;       // Y축 속도 고정
            rb.velocity = vel;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // 플레이어 데미지 적용
            AttackInfo info = new AttackInfo
            {
                damage = damage,
                attackDirection = rb.velocity.normalized,
                attacker = this.gameObject
            };
            PlayerStatus.Instance.ApplyDamage(info);

            // 플레이어와 충돌하면 총알 삭제
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            // 벽에 부딪히면 삭제
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Plan"))
        {
            // 바닥에 닿으면 튀는 효과 (Y축만)
            if (timer < bounceDuration)
            {
                Vector3 vel = rb.velocity;
                vel.y = bounceForce;
                rb.velocity = vel;
            }
        }
    }
}

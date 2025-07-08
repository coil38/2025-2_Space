using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackCooldown = 3f;
    private float timer;
    private Animator anim;

    public Transform player; // 👈 플레이어 위치 지정 필요

    void Start()
    {
        anim = GetComponent<Animator>();
        timer = attackCooldown;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Attack();
            timer = attackCooldown;
        }
    }

    void Attack()
    {
        anim.SetTrigger("Attack");

        if (player != null)
        {
            Vector2 baseDir = (player.position - transform.position).normalized;

            // 방향 회전 함수 사용
            Vector2[] directions = new Vector2[]
            {
            baseDir,
            RotateVector(baseDir, 90),
            RotateVector(baseDir, -90),
            -baseDir // 반대 방향
            };

            foreach (Vector2 dir in directions)
            {
                GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                proj.GetComponent<Projecttile>().Init(dir);
            }
        }
    }
    Vector2 RotateVector(Vector2 v, float angleDegrees)
    {
        float rad = angleDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(
            v.x * cos - v.y * sin,
            v.x * sin + v.y * cos
        ).normalized;
    }
}

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
        if (player == null) return;

        // 1. 보스 -> 플레이어 방향
        Vector3 toPlayer = (player.position - transform.position).normalized;

        // 2. Y축을 기준으로 회전할 4방향 벡터 만들기
        Vector3[] directions = new Vector3[4];
        directions[0] = toPlayer;                           // 플레이어 방향
        directions[1] = Quaternion.Euler(0, 90, 0) * toPlayer;  // 오른쪽
        directions[2] = Quaternion.Euler(0, -90, 0) * toPlayer; // 왼쪽
        directions[3] = Quaternion.Euler(0, 180, 0) * toPlayer; // 뒤쪽

        // 3. 총알 생성
        foreach (Vector3 dir in directions)
        {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            proj.GetComponent<Projectile>().Init(dir); // Init에서 방향 세팅
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("스킬 프리팹")]
    public GameObject redZonePrefab;
    public GameObject whiteZonePrefab;
    public GameObject projectilePrefab;

    [Header("장판 크기 설정")]
    public float safeZoneScale = 3f;       // 안전장판 크기
    public float minSafeZoneDistance = 4f; // 안전장판끼리 최소 거리
    public float bossSafeRadius = 5f;      // 보스 주변 금지 반경
    public int maxAttempts = 50; 

   [Header("생성 설정")]
    public int safeZoneCount = 3;   // 흰색 장판 개수 (몇 개만 생성)
    public float spawnHeight = 0.1f;

    private Transform planArea;
    public float attackCooldown = 3f;
    private float timer;
    private Animator anim;

    public Transform player; // 👈 플레이어 위치 지정 필요

    void Start()
    {
        GameObject plan = GameObject.FindGameObjectWithTag("Plan");
        if (plan != null) planArea = plan.transform;
        anim = GetComponent<Animator>();
        timer = attackCooldown;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // 테스트: 점프 공격 실행
            DoJumpAttack();

            // 기존 총알 공격
            // Attack();

            timer = attackCooldown;
        }
    }

    //총알 어택
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


    //점프 공격
    public void DoJumpAttack()
    {
        if (planArea == null) return;

        Vector3 size = planArea.localScale;
        Vector3 center = planArea.position;

        GameObject red = Instantiate(redZonePrefab, center + Vector3.up * spawnHeight, Quaternion.Euler(90, 0, 0));
        red.transform.localScale = new Vector3(size.x, size.y, size.z);

        DamageZone dz = red.GetComponent<DamageZone>();
        List<GameObject> safeZones = new List<GameObject>();

        for (int i = 0; i < safeZoneCount; i++)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < maxAttempts)
            {
                attempts++;

                float halfSafe = safeZoneScale / 2f;
                float randX = Random.Range(-size.x / 2f + halfSafe, size.x / 2f - halfSafe);
                float randZ = Random.Range(-size.z / 2f + halfSafe, size.z / 2f - halfSafe);
                Vector3 safePos = new Vector3(center.x + randX, center.y + spawnHeight + 0.05f, center.z + randZ);

                if (Vector3.Distance(safePos, transform.position) < bossSafeRadius)
                    continue;

                bool overlap = false;
                foreach (var zone in safeZones)
                {
                    if (zone == null) continue;
                    if (Vector3.Distance(safePos, zone.transform.position) < minSafeZoneDistance)
                    {
                        overlap = true;
                        break;
                    }
                }
                if (overlap) continue;

                GameObject safe = Instantiate(whiteZonePrefab, safePos, Quaternion.Euler(90, 0, 0));
                safe.transform.localScale = new Vector3(safeZoneScale, safeZoneScale, 1);
                safeZones.Add(safe);

                placed = true;
            }
        }
        if (dz != null)
            dz.SetSafeZones(safeZones);
    }
}


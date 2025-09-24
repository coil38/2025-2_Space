using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyBase
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
    public int safeZoneCount = 3;   // 흰색 장판 개수
    public float spawnHeight = 0.1f;

    [Header("쿨타임")]
    public float attackCooldown = 20f;
    private float timer;

    private Transform planArea;
    private Animator anim;

    public Transform player;


    protected override void Start()
    {
        base.Start();
        anim = GetComponent<Animator>();
        GameObject plan = GameObject.FindGameObjectWithTag("Plan");
        if (plan != null) planArea = plan.transform;
        timer = attackCooldown;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            // BossAttack();
            StartJumpAttack();
            timer = attackCooldown;
        }
    }

    void StartJumpAttack()
    {
        anim.SetTrigger("JumpUp");
        StartCoroutine(JumpRoutine());
    }

    IEnumerator JumpRoutine()
    {
        yield return new WaitForSeconds(3.2f); 

        anim.SetTrigger("LandImpact");
    }

    public override void ApplyDamage(AttackInfo attackInfo)
    {
        if (isDead || !canBeHit) return;

        // 체력 감소
        hp -= attackInfo.damage;

        // 히트 사운드 재생
        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        // 사망 체크
        if (hp <= 0 && !isDead)
        {
            isDead = true;
            OnDeathAction?.Invoke(this);
            OnDeath();

            animator.SetBool("Dead", true);
            rb.velocity = Vector3.zero;

            if (deathMarkPrefab != null && footPosition != null)
                Instantiate(deathMarkPrefab, footPosition.position, Quaternion.identity);

            Destroy(gameObject, 1f);
        }
        else
        {
            animator.SetTrigger("Hit");
        }
    }

    //총알 어택
    void BossAttack()
    {
        if (player == null) return;

        Vector3 toPlayer = (player.position - transform.position).normalized;

        Vector3[] directions = new Vector3[4];
        directions[0] = toPlayer;
        directions[1] = Quaternion.Euler(0, 90, 0) * toPlayer;
        directions[2] = Quaternion.Euler(0, -90, 0) * toPlayer;
        directions[3] = Quaternion.Euler(0, 180, 0) * toPlayer;

        foreach (Vector3 dir in directions)
        {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            proj.GetComponent<Projectile>().Init(dir);
        }
    }

    //점프 공격
    public void DoJumpAttack()
    {
        if (planArea == null) return;

        Vector3 size = planArea.localScale;
        Vector3 center = planArea.position;

        // 빨간 장판 생성
        GameObject red = Instantiate(redZonePrefab, center + Vector3.up * spawnHeight, Quaternion.Euler(90, 0, 0));
        red.transform.localScale = new Vector3(size.x, size.y, size.z);

        DamageZone dz = red.GetComponent<DamageZone>();
        List<GameObject> safeZones = new List<GameObject>();

        // 흰 장판 여러 개 생성
        for (int i = 0; i < safeZoneCount; i++)
        {
            bool placed = false;
            int attempts = 0;

            while (!placed && attempts < maxAttempts)
            {
                attempts++;

                float halfSafe = safeZoneScale / 2f;
                float randX = Random.Range(-size.x / 2f + halfSafe, size.x / 2f - halfSafe);
                float randY = Random.Range(-size.y / 2f + halfSafe, size.y / 2f - halfSafe);
                Vector3 safePos = new Vector3(center.x + randX, center.y + spawnHeight + 0.05f, center.z + randY);

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

    protected override void OnDeath()
    {
        base.OnDeath(); // ✅ 기본 드롭 로직 실행
        // 보스 전용 추가 연출 넣고 싶으면 여기서 구현
        Debug.Log("보스 처치됨!");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : EnemyBase
{
    [Header("스킬 프리팹")]
    public GameObject redZonePrefab;
    public GameObject whiteZonePrefab;
    public GameObject projectilePrefab;
    public GameObject CoinPrefab;
    public GameObject CoinZonePrefab;
    public GameObject canPrefab; 

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

    [Header("보스 사운드")]
    public AudioClip coinAttackSound;
    public AudioClip WariningSound;
    public AudioClip bossjumpdownSound;
    public AudioClip bossCanAttackSound;

    public Transform player;
    float margin = 1f;

    public Transform headTransform; // 보스 머리 위치


    protected override void Start()
    {

        Collider col = GetComponent<Collider>();
        base.Start();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        GameObject plan = GameObject.FindGameObjectWithTag("Plan");
        if (plan != null) planArea = plan.transform;


        timer = attackCooldown;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            BossAttack();
            //StartJumpAttack();
            //StartCoinRain();
            //LaunchCansCrossAttack();
            timer = attackCooldown;
        }
    }

    void StartJumpAttack()
    {
        anim.SetTrigger("JumpUp");
        StartCoroutine(JumpRoutine());
    }

    void StartCoinRain()
    {
        anim.SetTrigger("CoinAttack");
    }

    IEnumerator JumpRoutine()
    {
        yield return new WaitForSeconds(3.2f); 

        anim.SetTrigger("LandImpact");
    }

    public override void ApplyDamage(AttackInfo attackInfo)
    {
        if (isDead) return;

        // 체력 감소
        hp -= attackInfo.damage;

        // 히트 사운드 재생
        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        StartCoroutine(HitFlash());

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
    }

    //총알 어택
    public void BossAttack()
    {
        if (player == null) return;

        // 애니메이션 실행
        animator.SetTrigger("FireAttack");
    }

  
    public void FireProjectile()
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

    //실행 함수
    public void DoCoinRainAttack()
    {
        CoinRainAttack(20); // 동전 갯수 설정
    }

    //코인 떨어지는 로직
    public void CoinRainAttack(int coinCount = 10)
    {
        if (planArea == null) return;

        Vector3 size = planArea.localScale;
        Vector3 center = planArea.position;

        GameObject boss = GameObject.FindGameObjectWithTag("Boss");
        Vector2 bossXZ = boss != null ? new Vector2(boss.transform.position.x, boss.transform.position.z) : Vector2.negativeInfinity;
        float bossRadius = 3f; // 원하는 보스 주변 안전 거리

        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < coinCount; i++)
        {
            Vector3 groundPos;
            int safety = 0;

            do
            {
                float randX = Random.Range(-size.x / 2f + margin, size.x / 2f - margin);
                float randY = Random.Range(-size.y / 2f + margin, size.y / 2f - margin);
                groundPos = new Vector3(center.x + randX, center.y, center.z + randY);

                safety++;
                if (safety > 50) break;

            }
            while (
                   (boss != null && Vector2.Distance(new Vector2(groundPos.x, groundPos.z), bossXZ) < bossRadius) ||
                    usedPositions.Exists(p => Vector3.Distance(p, groundPos) < 2.0f)
                   );

            usedPositions.Add(groundPos);

            // 경고 장판
            GameObject warning = Instantiate(
                CoinZonePrefab,
                groundPos + Vector3.up * 0.15f,
                Quaternion.Euler(90, 0, 0)
            );
            warning.transform.localScale = new Vector3(2f, 2f, 1f);

            // 코인 생성
            Vector3 spawnPos = groundPos + Vector3.up * 10f;
            GameObject coin = Instantiate(CoinPrefab, spawnPos, Quaternion.identity);

            CoinProject cp = coin.GetComponent<CoinProject>();
            cp.Init(warning);
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

    //캔 던지기
    public void LaunchCansCrossAttack()
    {
        if (planArea == null || canPrefab == null || headTransform == null) return;

        StartCoroutine(LaunchCansRoutine());
    }

    private IEnumerator LaunchCansRoutine()
    {
        Vector3 size = planArea.localScale;
        Vector3 center = planArea.position;

        int waveCount = 4;   // 3번 반복
        int cansPerWave = 10; // 한 번에 3개

        for (int wave = 0; wave < waveCount; wave++)
        {
            for (int i = 0; i < cansPerWave; i++)
            {
                float randX = Random.Range(-size.x / 2f, size.x / 2f);
                float randY = Random.Range(-size.y / 2f, size.y / 2f); 

                Vector3 targetPos = new Vector3(center.x + randX, center.y, center.z + randY);

                GameObject can = Instantiate(canPrefab, headTransform.position, Quaternion.identity);
                can.GetComponent<CanProjectile>().Init(targetPos);
            }

            yield return new WaitForSeconds(1f); 
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath(); 
        // 보스 전용 추가 연출 넣고 싶으면 여기서 구현
        Debug.Log("보스 처치됨!");
    }



    //여긴 보스 사운드로 채울거임
    public void BossWariningSound()
    {
        if (audioSource != null && WariningSound!= null)
        {
            audioSource.PlayOneShot(WariningSound);
        }
    }

    public void BossCanAttackSound()
    {
        if (audioSource != null && bossCanAttackSound != null)
        {
            audioSource.PlayOneShot(bossCanAttackSound);
        }
    }

    public void PlayCoinAttackSound()
    {
        if (audioSource != null && coinAttackSound != null)
        {
            audioSource.PlayOneShot(coinAttackSound);
        }
    }

    public void BossJumpDownSound()
    {
        if (audioSource != null && bossjumpdownSound != null)
        {
            audioSource.PlayOneShot(bossjumpdownSound);
        }
    }
}

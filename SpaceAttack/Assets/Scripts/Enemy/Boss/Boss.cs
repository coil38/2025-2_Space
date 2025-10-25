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

    [Header("쿨타임 설정")]
    public float bossAttackCooldown = 10f;
    public float coinRainCooldown = 15f;
    public float canAttackCooldown = 20f;
    public float jumpAttackCooldown = 25f;
    public float summonMinionCooldown = 30f;

    private float bossAttackTimer;
    private float coinRainTimer;
    private float canAttackTimer;
    private float jumpAttackTimer;
    private float summonMinionTimer;

    [Header("장판 크기 설정(보스 스킬 쿵)")]
    public float safeZoneScale = 3f;       // 안전장판 크기
    public float minSafeZoneDistance = 4f; // 안전장판끼리 최소 거리
    public float bossSafeRadius = 5f;      // 보스 주변 금지 반경
    public int maxAttempts = 50;
    public int safeZoneCount = 3;   // 흰색 장판 개수
    public float spawnHeight = 0.1f;

    [Header("보스 스킬(퉤)")]
    public Transform planArea;
    public GameObject[] minionPrefabs;         // 소환할 잡몹 종류들
    public int minionCount = 5;                // 한 번에 소환할 마리 수
    public float minDistanceBetweenMinions = 2f; // 서로 너무 가까이 안 소환되게
    public Transform mouthPoint; // 보스 입 위치
    public float shootForce = 8f; // 잡몹이 날아가는 힘
    public float upwardForce = 3f; // 살짝 위로 날아가게
    public float spreadAngle = 60f; // 퍼지는 각도

    [Header("보스 사운드")]
    public AudioClip coinAttackSound;
    public AudioClip WariningSound;
    public AudioClip bossjumpdownSound;
    public AudioClip bossCanAttackSound;
    public AudioClip bosscancrossSound;
    public AudioClip[] hitSounds;
    public AudioClip phaseTransitionSound;
    public AudioClip spitSound;
    public AudioClip dieSound;


    //변수 선언들
    public Transform player;
    float margin = 1f;
    private Animator anim;
    public Transform headTransform; // 보스 머리 위치
    private bool isLaunchingCans = false;
    private int hitCount = 0;
    private Collider bossCollider;
    private List<GameObject> summonedMinions = new List<GameObject>();
    public int currentPhase = 1;
    private bool isTransitioningPhase = false;
    private bool isUsingSkill = false; // 스킬 중복 방지;
    private bool isSpitting = false;
    public Transform bodyCenter;
    public GameObject scrapExplosionPrefab;
    protected override void Start()
    {
        base.Start();
        bossCollider = GetComponent<Collider>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        GameObject plan = GameObject.FindGameObjectWithTag("Plan");
        if (plan != null) planArea = plan.transform;

        // 초기 쿨타임 세팅
        bossAttackTimer = bossAttackCooldown;
        coinRainTimer = coinRainCooldown;
        canAttackTimer = canAttackCooldown;
        jumpAttackTimer = jumpAttackCooldown;
        summonMinionTimer = summonMinionCooldown;
    }


    void Update()
    {
        if (isDead) return;

        // 쿨타임 감소
        bossAttackTimer -= Time.deltaTime;
        coinRainTimer -= Time.deltaTime;
        canAttackTimer -= Time.deltaTime;
        jumpAttackTimer -= Time.deltaTime;
        summonMinionTimer -= Time.deltaTime;

        // 페이즈 구분
        if (!isTransitioningPhase)
        {
            if (hp > 3500 && currentPhase != 1)
            {
                currentPhase = 1;
            }
            else if (hp <= 3500 && hp > 1500 && currentPhase != 2)
            {
                StartCoroutine(PhaseTransition(2));
                return;
            }
            else if (hp <= 1500 && currentPhase != 3)
            {
                StartCoroutine(PhaseTransition(3));
                return;
            }
        }

        //  페이즈 전환 중이면 패턴 스킬 호출 안함
        if (isTransitioningPhase) return;

        if (currentPhase == 1) Phase1Pattern();
        else if (currentPhase == 2) Phase2Pattern();
        else Phase3Pattern();
    }

    //페이지 컨트롤러
    void Phase1Pattern()
    {
        List<System.Action> availableSkills = new List<System.Action>();

        if (bossAttackTimer <= 0f) availableSkills.Add(() => { BossAttack(); bossAttackTimer = bossAttackCooldown; });
        if (coinRainTimer <= 0f) availableSkills.Add(() => { StartCoinRain(); coinRainTimer = coinRainCooldown; });
        if (jumpAttackTimer <= 0f) availableSkills.Add(() => { StartJumpAttack(); jumpAttackTimer = jumpAttackCooldown; });
        
        TryUseRandomSkill(availableSkills);
    }

    void Phase2Pattern()
    {
        List<System.Action> availableSkills = new List<System.Action>();

        if (bossAttackTimer <= 0f) availableSkills.Add(() => { BossAttack(); bossAttackTimer = bossAttackCooldown; });
        if (coinRainTimer <= 0f) availableSkills.Add(() => { StartCoinRain(); coinRainTimer = coinRainCooldown; });
        if (canAttackTimer <= 0f) availableSkills.Add(() => { LaunchCansCrossAttack(); canAttackTimer = canAttackCooldown; });
        if (jumpAttackTimer <= 0f) availableSkills.Add(() => { StartJumpAttack(); jumpAttackTimer = jumpAttackCooldown; });

        TryUseRandomSkill(availableSkills);
    }
    void Phase3Pattern()
    {
        List<System.Action> availableSkills = new List<System.Action>();

        if (bossAttackTimer <= 0f) availableSkills.Add(() => { BossAttack(); bossAttackTimer = bossAttackCooldown; });
        if (coinRainTimer <= 0f) availableSkills.Add(() => { StartCoinRain(); coinRainTimer = coinRainCooldown; });
        if (canAttackTimer <= 0f) availableSkills.Add(() => { LaunchCansCrossAttack(); canAttackTimer = canAttackCooldown; });
        if (jumpAttackTimer <= 0f) availableSkills.Add(() => { StartJumpAttack(); jumpAttackTimer = jumpAttackCooldown; });
        if (summonMinionTimer <= 0f) availableSkills.Add(() => { StartSpitMinions(); summonMinionTimer = summonMinionCooldown; });

        TryUseRandomSkill(availableSkills);
    }
    void TryUseRandomSkill(List<System.Action> availableSkills)
    {
        // 페이즈 전환 중이면 스킬 안쓰도록
        if (isUsingSkill || availableSkills.Count == 0 || isTransitioningPhase) return;

        System.Action chosenSkill = availableSkills[Random.Range(0, availableSkills.Count)];
        StartCoroutine(UseSkillRoutine(chosenSkill));
    }
    IEnumerator UseSkillRoutine(System.Action skillAction)
    {
        isUsingSkill = true;
        skillAction.Invoke();

        yield return new WaitForSeconds(3f);

        float skillDelay = Random.Range(3f, 5f);
        yield return new WaitForSeconds(skillDelay);

        isUsingSkill = false;
    }

    private IEnumerator PhaseTransition(int nextPhase)
    {
        isTransitioningPhase = true; // 시작

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = true;

        Camera mainCam = Camera.main;
        CameraFallow camFollow = mainCam.GetComponent<CameraFallow>();
        if (camFollow != null)
            camFollow.LockCamera(true);

        Vector3 originalPos = mainCam.transform.position;
        Quaternion originalRot = mainCam.transform.rotation;

        BossIntroController intro = FindObjectOfType<BossIntroController>();
        Transform bossCamPoint = intro != null ? intro.bossCameraPoint : null;



        Vector3 zoomTargetPos = bossCamPoint ? bossCamPoint.position : transform.position + transform.forward * 5f + Vector3.up * 3f;
        Quaternion zoomTargetRot = bossCamPoint ? bossCamPoint.rotation : Quaternion.LookRotation(transform.position - mainCam.transform.position);

        float zoomDuration = 0.8f;
        float t = 0f;

        Animator anim = GetComponent<Animator>();
        
        if (anim != null)
        {
            anim.SetTrigger("Angry");
        }

        if (audioSource != null && phaseTransitionSound != null)
            audioSource.PlayOneShot(phaseTransitionSound);
      
        while (t < zoomDuration)
        {
            t += Time.deltaTime;
            float lerp = t / zoomDuration;
            mainCam.transform.position = Vector3.Lerp(originalPos, zoomTargetPos, lerp);
            mainCam.transform.rotation = Quaternion.Lerp(originalRot, zoomTargetRot, lerp);
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        // 다음 페이즈 스킬 실행
        if (nextPhase == 2)
        {
            LaunchCansCrossAttack();
            bossAttackTimer += 15f;
            coinRainTimer += 15f;
            canAttackTimer += 15f;
            jumpAttackTimer += 15f;
            summonMinionTimer += 15f;
        }
        else if (nextPhase == 3)
        {
            StartSpitMinions();
             bossAttackTimer += 15f;
            coinRainTimer += 15f;
            canAttackTimer += 15f;
            jumpAttackTimer += 15f;
            summonMinionTimer += 15f;
        }
        // 플레이어 쪽으로 복귀
        if (PlayerStatus.Instance != null && camFollow != null)
        {
            t = 0f;
            Transform player = PlayerStatus.Instance.transform;
            Vector3 targetPos = player.position + camFollow.cameraDir.normalized * camFollow.cameraDis;
            Quaternion targetRot = Quaternion.Euler(camFollow.cameraRot);

            while (t < zoomDuration)
            {
                t += Time.deltaTime;
                float lerp = t / zoomDuration;
                mainCam.transform.position = Vector3.Lerp(zoomTargetPos, targetPos, lerp);
                mainCam.transform.rotation = Quaternion.Slerp(zoomTargetRot, targetRot, lerp);
                yield return null;
            }
        }

        if (camFollow != null)
            camFollow.LockCamera(false);

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = false;

        currentPhase = nextPhase; 
        isTransitioningPhase = false; 
  
           BossHealthUI ui = FindObjectOfType<BossHealthUI>();
    if (ui != null)
        ui.UpdatePhaseIndicators();
    }

    public override void ApplyDamage(AttackInfo attackInfo)
    {
        if (isDead) return;

        hp -= attackInfo.damage;
        hitCount++;

        if (hitCount % 2 == 0 && audioSource != null && hitSounds != null && hitSounds.Length > 0)
        {
            int rand = Random.Range(0, hitSounds.Length);
            audioSource.PlayOneShot(hitSounds[rand]);
        }

        StartHitFlash();

        if (scrapExplosionPrefab != null && bodyCenter != null)
        {
            GameObject scrap = Instantiate(scrapExplosionPrefab, bodyCenter.position, Quaternion.identity);
            Destroy(scrap, 0.5f); // 1초 후 삭제
        }

        // 사망 처리
        if (hp <= 0 && !isDead)
        {
            isDead = true;
            OnDeathAction?.Invoke(this);
            OnDeath();
            
            rb.velocity = Vector3.zero;

            if (deathMarkPrefab != null && footPosition != null)
                Instantiate(deathMarkPrefab, footPosition.position, Quaternion.identity);

            Destroy(gameObject, 3f);
        }
    }

    public void FireProjectile()
    {
        if (player == null) return;

        Vector3 toPlayer = (player.position - transform.position).normalized;

        Vector3[] directions = new Vector3[4];
        directions[0] = toPlayer; // 정면
        directions[1] = Quaternion.Euler(0, 90, 0) * toPlayer; // 오른쪽
        directions[2] = Quaternion.Euler(0, -90, 0) * toPlayer; // 왼쪽
        directions[3] = Quaternion.Euler(0, 180, 0) * toPlayer; // 뒤쪽

        foreach (Vector3 dir in directions)
        {
            GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            proj.GetComponentInChildren<VFXSlashHitBox>().SetDirection(dir);
        }
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

    public void OnBossLand()
    {
        // 카메라 쉐이크 실행
        BossCameraShake shake = Camera.main.GetComponent<BossCameraShake>();
        if (shake != null)
        {
            StartCoroutine(shake.Shake(0.5f, 0.4f)); 
        }
    }
    private IEnumerator WaitAndLaunchCans()
    {
        yield return new WaitForSeconds(3f); // 애니메이션 타이밍 조절
        yield return StartCoroutine(LaunchCansRoutine());

        // 다 끝나면 다시 가능하게
        isLaunchingCans = false;
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

    public void SummonMinionsSkill()
    {
        if (planArea == null || minionPrefabs.Length == 0 || mouthPoint == null)
            return;

        for (int i = 0; i < minionCount; i++)
        {
            GameObject randomMinion = minionPrefabs[Random.Range(0, minionPrefabs.Length)];
            GameObject minion = Instantiate(randomMinion, mouthPoint.position, Quaternion.identity);

            summonedMinions.Add(minion);

            float randomY = Random.Range(0f, 360f);
            Vector3 randomDir = Quaternion.Euler(0, randomY, 0) * mouthPoint.forward;

            Rigidbody rb = minion.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 force = randomDir.normalized * shootForce + Vector3.up * upwardForce;
                rb.AddForce(force, ForceMode.Impulse);
            }

            EnemyBase enemy = minion.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.canDetectPlayer = false;
                StartCoroutine(EnableEnemyAfterLanding(enemy, 4f));
            }
        }
    }

    private IEnumerator EnableEnemyAfterLanding(EnemyBase enemy, float waitAfterLand)
    {
        Rigidbody rb = enemy.GetComponent<Rigidbody>();
        if (rb == null) yield break;

        while (Mathf.Abs(rb.velocity.y) > 0.1f)
            yield return null;

        yield return new WaitForSeconds(waitAfterLand);

        if (enemy != null)
            enemy.canDetectPlayer = true;
    }
    //보스 죽음 처리
    protected override void OnDeath()
    {
        base.OnDeath();

        foreach (GameObject minion in summonedMinions)
        {
            if (minion != null)
            {
                Destroy(minion);
            }
        }
        summonedMinions.Clear();

        if (audioSource != null && dieSound != null)
            audioSource.PlayOneShot(dieSound);
        anim.SetTrigger("Dead");
    }
    //콜리더 on/off
    public void DisableCollider()
    {
        if (bossCollider != null)
            bossCollider.enabled = false;
    }

    public void EnableCollider()
    {
        if (bossCollider != null)
            bossCollider.enabled = true;
    }
    //이벤트 관련 함수
    public void StartJumpAttack()
    {
        anim.SetTrigger("JumpUp");
        StartCoroutine(JumpRoutine());
    }

    public void StartCoinRain()
    {
        anim.SetTrigger("CoinAttack");
    }
    IEnumerator JumpRoutine()
    {
        yield return new WaitForSeconds(3.2f);

        anim.SetTrigger("LandImpact");
    }
    public void OnSpitEvent()
    {

        if (audioSource != null && spitSound != null)
            audioSource.PlayOneShot(spitSound);

        SummonMinionsSkill();

        isSpitting = false;
    }

    public void BossAttack()
    {
        if (player == null) return;

        // 애니메이션 실행
        animator.SetTrigger("FireAttack");
    }

    public void LaunchCansCrossAttack()
    {
        if (isLaunchingCans) return; // 이미 실행 중이면 무시
        if (planArea == null || canPrefab == null || headTransform == null) return;

        isLaunchingCans = true;

        anim.SetTrigger("CanBlast");
        // 코루틴 시작
        StartCoroutine(WaitAndLaunchCans());
    }
    public void StartSpitMinions()
    {
        if (isSpitting) return;

        isSpitting = true;
        anim.SetTrigger("Spit");
    }

    public void DoCoinRainAttack()
    {
        CoinRainAttack(45); // 동전 갯수 설정
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

    public void BossCanCrossSound()
    {
        if (audioSource != null && bosscancrossSound != null)
        {
            audioSource.PlayOneShot(bosscancrossSound);
        }
    }

    public void BossSpitSound()
    {
        if (audioSource != null && spitSound != null)
        {
            audioSource.PlayOneShot(spitSound);
        }
    }

    public void BossDieSound()
    {
        if (audioSource != null && dieSound != null)
        {
            audioSource.PlayOneShot(dieSound);
        }
    }

}

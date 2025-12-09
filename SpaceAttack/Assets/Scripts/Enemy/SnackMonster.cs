using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnackMonster : EnemyBase
{
    [Header("스낵 몬스터 설정")]
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolChangeTime = 3f;

    [Header("먹기 관련 설정")]
    [SerializeField] private float eatRadius = 2f;
    [SerializeField] private float eatDuration = 3f;         // 먹는 시간 총합
    [SerializeField] private float damageInterval = 0.5f;    // HP 깎는 간격
    [SerializeField] private int damagePerTick = 1;       // 한 번에 깎는 HP 양
    [SerializeField] private GameObject eatRangeVisualPrefab;
    [SerializeField] private float eatCooldown = 2f;  // 뱉고 나서 다시 먹기까지 대기시간
    private float lastEatTime = -Mathf.Infinity;


    [Header("사운드 클립들")]
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip eatClip;
    [SerializeField] private AudioClip spitClip;


    //상태변수들
    private float patrolTimer;
    private bool isChasing;
    private bool isEating = false;
    private GameObject eatRangeVisualInstance;
    private Vector3 lastPosition;
    private bool isPaused = false;
    private Transform playerTransform;
    private PlayerStatus playerStatusScript;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();

        ChooseNewPatrolPoint();
        patrolTimer = patrolChangeTime;
    }

    private void Update()
    {
        if (isDead || isHit || isEating || isPaused) return;

        if (!isChasing)
        {
            patrolTimer -= Time.deltaTime;
            if (patrolTimer <= 0f)
            {
                ChooseNewPatrolPoint();
                patrolTimer = patrolChangeTime;
            }
        }

        Patrol();
    }

    private void FixedUpdate()
    {
        if (isDead || isHit || isEating || isPaused || !canDetectPlayer)
        {
            SetIsWalking(false);
            return;
        }

        if (!isChasing)
        {
            MoveToRigidbody(patrolTarget, patrolSpeed);
        }
        else
        {
            Chase();
        }

        // 이전 위치와 현재 위치 차이로 속도 판단 (월드 좌표 기준)
        float movedDistance = Vector3.Distance(transform.position, lastPosition);
        bool isWalking = movedDistance > 0.01f; // 너무 작은 이동은 무시

        SetIsWalking(isWalking);

        lastPosition = transform.position; // 위치 갱신
    }
    //움직임
    private void SetIsWalking(bool walking)
    {
        if (animator.GetBool("isWalking") != walking)
        {
            animator.SetBool("isWalking", walking);

            if (walking)
            {
                PlayWalkSound();
            }
            else
            {
                StopSound();
            }
        }
    }
    //주변 배회 상태 자식
    private void ChooseNewPatrolPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 3f;
        patrolTarget = transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);
    }
    //플레이어 감지
    protected override void OnPlayerDetected(Transform player)
    {
        if (!canDetectPlayer || isEating) return;
        {
            isChasing = true;
            attackTarget = player;
        }
    }
    //플레이어가 박스 안에 있는가?
    private bool IsPlayerInEatBox()
    {
        Vector3 center = transform.position + (isFacingRight ? transform.right : -transform.right) * (eatRadius / 2);
        Vector3 halfExtents = new Vector3(eatRadius / 2, 1f, 1.5f); 

        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask("Player"));
        foreach (Collider hit in hits)
        {
            if (hit.transform == attackTarget)
                return true;
        }
        return false;
    }
    //주변 배회 상태
    private void Chase()
    {
        if (!canDetectPlayer) return;

        if (attackTarget == null)
        {
            isChasing = false;
            ChooseNewPatrolPoint();
            patrolTimer = patrolChangeTime;
            return;
        }

        float eatStartDistance = eatRadius + 0.8f;
        float stopDistance = 3f;

        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);

        if (distanceToTarget <= eatStartDistance && Time.time - lastEatTime >= eatCooldown && IsPlayerInEatBox())
        {
            StartCoroutine(EatPlayerRoutine());
            return;
        }

        if (distanceToTarget > DetectRadius * 1.2f)
        {
            isChasing = false;
            attackTarget = null;
            ChooseNewPatrolPoint();
            patrolTimer = patrolChangeTime;
            return;
        }

        if (distanceToTarget > stopDistance)
        {
            MoveToRigidbody(attackTarget.position, chaseSpeed);
        }
        else
        {
            rb.velocity = Vector2.zero; 

            float dirX = (attackTarget.position - transform.position).x;
            Flip(dirX);
        }
    }


    protected override void OnDeath()
    {
        if (eatRangeVisualInstance != null)
        {
            Destroy(eatRangeVisualInstance);
            eatRangeVisualInstance = null;
        }


        if (isEating)
        {
            StopAllCoroutines(); 
            isEating = false;

            if (playerTransform != null)
            {
                // 플레이어 상태 초기화
                SpriteRenderer sr = playerTransform.GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = true;

                playerTransform.SetParent(null);

                if (playerStatusScript != null)
                {
                    playerStatusScript.isBeingEaten = false;
                    playerStatusScript.isRooted = false;
                }

                Vector3 forwardOffset = (isFacingRight ? Vector3.right : Vector3.left) * 2f + Vector3.up * 1f;
                playerTransform.position = transform.position + forwardOffset;

                playerTransform = null;
                playerStatusScript = null;
            }
        }

        // 상태 초기화
        isChasing = false;
        rb.velocity = Vector3.zero;

        base.OnDeath(); 
    }

    //먹기 코드
    private IEnumerator EatPlayerRoutine()
    {
        isEating = true;
        animator.SetTrigger("isEating");
        isChasing = false;
        rb.velocity = Vector3.zero;

      
        playerTransform = attackTarget;
        playerStatusScript = playerTransform.GetComponent<PlayerStatus>();

     
        if (eatRangeVisualPrefab != null && eatRangeVisualInstance == null)
        {
            eatRangeVisualInstance = Instantiate(eatRangeVisualPrefab);  
            eatRangeVisualInstance.SetActive(false);

            Vector3 visualOffset = (isFacingRight ? transform.right : -transform.right) * (eatRadius / 2);
            visualOffset += Vector3.up * 0.1f;  
            eatRangeVisualInstance.transform.position = transform.position + visualOffset;

            eatRangeVisualInstance.transform.localScale = new Vector3(eatRadius, 3.5f, eatRadius * 4f);

            eatRangeVisualInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            eatRangeVisualInstance.SetActive(true);
        }

        yield return new WaitForSeconds(1.5f);
        PlayEatSound();
        if (!IsPlayerInEatBox())
        {
            Debug.Log("먹기 실패: 플레이어가 범위 밖에 있음");

            isEating = false;
            isChasing = false;
            rb.velocity = Vector3.zero;

            if (eatRangeVisualInstance != null)
            {
                Destroy(eatRangeVisualInstance);
                eatRangeVisualInstance = null;
            }

            animator.SetBool("isWalking", false); 
            animator.SetTrigger("toIdle"); 
            ChooseNewPatrolPoint(); 
            patrolTimer = patrolChangeTime;

            yield break;
        }
        if (eatRangeVisualInstance != null)
        {
            eatRangeVisualInstance.SetActive(false); 
        }
        SpriteRenderer sr = playerTransform.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }

        if (playerStatusScript != null)
        {
            playerStatusScript.isBeingEaten = true;
        }

        if (playerStatusScript != null)
            playerStatusScript.isRooted = true;   //플레이어 속박처리

        playerTransform.localPosition = transform.position;

        float elapsed = 0f;

        while (elapsed < eatDuration)
        {
            if (playerStatusScript != null)
            {
                AttackInfo attackInfo = new AttackInfo
                {
                    damage = damagePerTick,
                    attackDirection = Vector3.zero,
                    attacker = this.gameObject
                };
                playerStatusScript.ApplyDamage(attackInfo);

                Debug.Log("실행된다.");
            }

            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }
        if (playerStatusScript != null)
        {
            playerStatusScript.isRooted = false;   //플레이어 속박 해제 처리

            // 먹기 끝나고 무적 해제
            playerStatusScript.isBeingEaten = false;
        }

        Vector3 forwardOffset = (isFacingRight ? Vector3.right : Vector3.left) * 3f + Vector3.up * 0.1f;
        playerTransform.position = transform.position + forwardOffset;

        if (eatRangeVisualInstance != null)
        {
            Destroy(eatRangeVisualInstance);
            eatRangeVisualInstance = null;
        }

        isEating = false;
      

        attackTarget = null;
        isChasing = false;     
        rb.velocity = Vector3.zero;

        
        animator.SetTrigger("spit");
        PlaySpitSound();
        ChooseNewPatrolPoint();
        patrolTimer = patrolChangeTime;
        if (sr != null)
        {
            sr.enabled = true;
        }

        isPaused = true;
        yield return new WaitForSeconds(3f);
        isPaused = false;


        attackTarget = playerTransform;
        isChasing = true;
        lastEatTime = Time.time;

       
    }
    //움직임
    private void MoveToRigidbody(Vector3 targetPos, float speed)
    {
        Vector3 direction = targetPos - rb.position;
        direction.y = 0f;

        if (direction.magnitude < 0.1f)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + move);

        Flip(move.x);
    }
    //스낵 몬스터 사운드 
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void PlayWalkSound()
    {
        PlayClip(walkClip, loop: true);
    }

    public void PlayEatSound()
    {
        PlayClip(eatClip, loop: false);
    }

    public void PlaySpitSound()
    {
        PlayClip(spitClip, loop: false);
    }

    private void PlayClip(AudioClip clip, bool loop)
    {
        if (clip == null) return;

        if (audioSource.isPlaying)
            audioSource.Stop();

        audioSource.loop = loop;
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void StopSound()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}

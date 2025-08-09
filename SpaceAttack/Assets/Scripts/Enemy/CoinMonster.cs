using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinMonsterState
{
    Patrol,         // 주변을 둘러봄
    Chase,          // 플레이어 추적
    ExplodeReady,   // 불붙고 천천히 따라감
    Exploding       // 폭발 실행
}

public class CoinMonster : EnemyBase
{
    [Header("코인 몬스터 설정")]
    public float explodeReadySpeed = 1f;
    public float explosionDelay = 2f;
    public int explosionDamage = 5;
    public float explosionKnockbackForce = 5f;
    private float rollCooldown = 0.4f;  // 한 번 굴고 나서 다시 굴 수 있는 최소 시간
    private float rollTimer = 0f;

    [Header("사운드 설정")]
    public AudioClip rollSound;
    [SerializeField] private AudioSource chargeSource;
    [SerializeField] private AudioSource explosionSource;

    [SerializeField] private AudioClip chargeSound;
    [SerializeField] private AudioClip explosionSound;

    [Header("이동 속도 설정")]
    public float patrolSpeed = 1.5f;  // 느린 속도
    public float chaseSpeed = 3f;     

    [Header("폭발 조건 및 범위")]
    public float triggerDistance = 1f;       // 플레이어가 붙으면 폭발 준비 시작 
    public float explodeDistance = 3f;       // 폭뎀범위

   
    private Transform player;
    private CoinMonsterState state = CoinMonsterState.Patrol;
    private float explodeTimer = 0f;


    //patrol 기반 함수
    private Vector3 patrolDirection;
    private float patrolMoveTime = 1.5f;      
    private float patrolIdleTime = 2f;         
    private float patrolTimer = 0f;
    private bool isPatrolling = true;

    private Vector3 lastMoveDirection = Vector3.zero;
    //상태변수
    private bool isRolling = false;
    private bool hasExploded = false;

    private SpriteRenderer spriteRenderer;
    private Rigidbody rb1;

    //폭발반경범위 시각화
    public GameObject explodeRangeVisual;

    protected override void Start()
    {
        base.Start();
        state = CoinMonsterState.Patrol;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        SetNextPatrol();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        rb1 = GetComponent<Rigidbody>();
        rb1.useGravity = true;
        rb1.isKinematic = false;

        if (explodeRangeVisual != null)
        {
            float baseRadius = 0.013f;  
            float scale = explodeDistance / baseRadius;

            explodeRangeVisual.transform.localScale = new Vector3(scale, scale, scale);
            explodeRangeVisual.SetActive(false);
        }
    }

    //다음 상태 배회
    private void SetNextPatrol()
    {
        isPatrolling = Random.value > 0.5f; 
        patrolTimer = 0f;
        patrolDirection = Random.insideUnitSphere;
        patrolDirection.y = 0f;

    }


    //주변 배회 상태 코드
    protected override void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (isPatrolling)
        {
            MoveRolling(patrolDirection, patrolSpeed);
            if (patrolTimer >= patrolMoveTime)
            {
                isPatrolling = false;
                patrolTimer = 0f;
            }
        }
        else
        {

            if (patrolTimer >= patrolIdleTime)
            {
                isPatrolling = true;
                patrolTimer = 0f;

                patrolDirection = Random.insideUnitSphere;
                patrolDirection.y = 0f;
            }
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, DetectRadius, playerLayer);
        if (hits.Length > 0)
        {
            OnPlayerDetected(hits[0].transform);
        }
    }


    private void Update()
    {
        if (rollTimer > 0f)
            rollTimer -= Time.deltaTime;

        if (isDead) return;

        switch (state)
        {
            case CoinMonsterState.Patrol:
                Patrol();
                break;

            case CoinMonsterState.Chase:
                ChasePlayer();
                break;

            case CoinMonsterState.ExplodeReady:
                ExplodeReady();
                break;
        }
    }

    //구르기가 끝났나?
    public void OnRollDone()
    {
        isRolling = false;
    }

    //구르기 코드
    private void MoveRolling(Vector3 direction, float speed)
    {
        if (rb == null) return;

        Vector3 move = direction.normalized * speed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        if (direction.x != 0)
            spriteRenderer.flipX = direction.x < 0;

        if (rollTimer > 0f) return;

        if (Mathf.Sign(direction.x) != Mathf.Sign(lastMoveDirection.x) || !isRolling)
        {
            if (direction.x < 0)
                animator.SetTrigger("RollLeft");
            else
                animator.SetTrigger("RollRight");

            isRolling = true;
            rollTimer = rollCooldown;             

        }

        lastMoveDirection = direction;
    }

    //데미지 주기
    public override void ApplyDamage(AttackInfo attackInfo)
    {
        if (isDead || !canBeHit) return;

        hp -= attackInfo.damage;

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (hp <= 0)
        {
            base.ApplyDamage(attackInfo);

            state = CoinMonsterState.Patrol;  
            hasExploded = true;

            if (explodeRangeVisual != null)
                explodeRangeVisual.SetActive(false);

            return;
        }

        rb.velocity = Vector3.zero;
        rb.AddForce(attackInfo.attackDirection * 0.5f, ForceMode.Impulse);

        if (state == CoinMonsterState.ExplodeReady)
        {
            return;
        }

        StartCoroutine(HitProcess());
    }


    //플레이어 감지
    protected override void OnPlayerDetected(Transform detectedPlayer)
    {
        player = detectedPlayer;
        state = CoinMonsterState.Chase;
    }


    //플레이어쫒기
    private void ChasePlayer()
    {

        animator.SetBool("isChasing", true);
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < triggerDistance)
        {
            state = CoinMonsterState.ExplodeReady;
            animator.SetTrigger("StartCry");  
            explodeTimer = 0f;
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        MoveRolling(direction, chaseSpeed);

    }

    //폭발 준비
    private void ExplodeReady()
    {


        if (player == null) return;

        if (explodeRangeVisual != null)
            explodeRangeVisual.SetActive(true);

        explodeTimer += Time.deltaTime;
        canBeHit = false;

        if (explodeTimer >= explosionDelay)
        {
            animator.SetTrigger("Explode");
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (!hasExploded && info.IsName("Coin_explode") && info.normalizedTime >= 1f)
        {
            Explode(); 
        }
    }
    
    //폭발 코드
    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explodeRangeVisual != null)
            Destroy(explodeRangeVisual); 

        Collider[] cols = Physics.OverlapSphere(transform.position, explodeDistance, attackLayer);
        foreach (var col in cols)
        {
            Vector3 dir = (col.transform.position - transform.position).normalized;
            AttackInfo info = new AttackInfo(explosionDamage, dir * explosionKnockbackForce);
            col.SendMessage("ApplyDamage", info, SendMessageOptions.DontRequireReceiver);
        }
        foreach (var sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.enabled = false;
        }
    }

    //코인몬스터 소리
    public void PlayRollSound()
    {
        if (audioSource != null && rollSound != null)
        {
            if (audioSource.isPlaying)
                audioSource.Stop(); // 기존 소리 끊기

            audioSource.clip = rollSound;
            audioSource.Play();
        }
    }

    public void OnChargeStart()
    {

        if (chargeSource != null && chargeSound != null)
        {
            chargeSource.clip = chargeSound;
            chargeSource.Play();
        }
    }


    public void OnExplode()
    {

        if (chargeSource != null)
            chargeSource.Stop();

        if (explosionSource != null && explosionSound != null)
        {
            explosionSource.PlayOneShot(explosionSound);
        }

        Explode();
    }
}

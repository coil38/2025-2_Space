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
    public float chaseSpeed = 3f;
    public float explodeReadySpeed = 1f;
    public float explosionDelay = 2f;
    public float explosionDamage = 5f;
    public float explosionKnockbackForce = 5f;

    [Header("폭발 조건 및 범위")]
    public float triggerDistance = 1f;       // 플레이어가 붙으면 폭발 준비 시작 
    public float explodeDistance = 3f;       // 폭뎀범위
    
    
    private Transform player;
    private CoinMonsterState state = CoinMonsterState.Patrol;
    private float explodeTimer = 0f;


    //patrol 기반 함수
    private Vector3 patrolDirection;
    private float patrolChangeInterval = 3f;
    private float patrolTime = 0f;
    private float patrolMoveTime = 1.5f;       // 한 번 이동하는 시간
    private float patrolIdleTime = 2f;         // 정지하는 시간
    private float patrolTimer = 0f;
    private bool isPatrolling = true;
    private float patrolWaitTime = 0f;
    private float patrolWaitCounter = 0f;

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

        rb1 = GetComponent<Rigidbody>();
        rb1.useGravity = true;
        rb1.isKinematic = false;

        if (explodeRangeVisual != null)
        {
            float baseRadius = 0.09f;  
            float scale = explodeDistance / baseRadius;

            explodeRangeVisual.transform.localScale = new Vector3(scale, scale, 1f);
            explodeRangeVisual.SetActive(false);
        }
    }

    private void SetNextPatrol()
    {
        isPatrolling = Random.value > 0.5f; 
        patrolTime = 0f;
        patrolDirection = Random.insideUnitSphere;
        patrolDirection.y = 0f;

        patrolWaitTime = Random.Range(1f, 2f); 
        patrolWaitCounter = 0f;
    }

    protected override void Patrol()
    {
        patrolTimer += Time.deltaTime;

        if (isPatrolling)
        {
            // 이동 상태
            MoveRolling(patrolDirection, chaseSpeed);
            animator.SetBool("StartRoll", true);

            if (patrolTimer >= patrolMoveTime)
            {
                isPatrolling = false;
                patrolTimer = 0f;
                animator.SetBool("StartRoll", false);
            }
        }
        else
        {
            animator.SetBool("StartRoll", false);

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
    private void MoveRolling(Vector3 direction, float speed)
    {
        if (rb == null) return;

        Vector3 move = direction.normalized * speed * Time.deltaTime;
        rb.MovePosition(rb.position + move);

        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }

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
            return;
        }

        rb.velocity = Vector3.zero;
        rb.AddForce(attackInfo.attackDirection * 0.5f, ForceMode.Impulse);

        AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
        bool isExplodeOrCryPlaying = animState.IsName("Coin_explode") || animState.IsName("StartCry");

        if (state == CoinMonsterState.ExplodeReady && isExplodeOrCryPlaying)
        {
            return;
        }

        StartCoroutine(HitProcess());
    }

    protected override void OnPlayerDetected(Transform detectedPlayer)
    {
        player = detectedPlayer;
        state = CoinMonsterState.Chase;
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < triggerDistance)
        {
            state = CoinMonsterState.ExplodeReady;
            animator.SetTrigger("StartCry");  
            animator.SetBool("StartRoll", false); 
            explodeTimer = 0f;
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        MoveRolling(direction, chaseSpeed);

        // 애니메이션 제어
        animator.SetBool("StartRoll", true);      
        animator.SetBool("StopRolling", false);   
    }

    private void ExplodeReady()
    {
        if (player == null) return;

        if (explodeRangeVisual != null)
            explodeRangeVisual.SetActive(true);

        explodeTimer += Time.deltaTime;

        animator.SetBool("StartRoll", false);  
        animator.SetBool("IsMoving", false);

        if (!hasExploded && explodeTimer >= explosionDelay)
        {
            animator.SetTrigger("Explode");  
            hasExploded = true;
        }

        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        if (hasExploded && info.IsName("Coin_explode") && info.normalizedTime >= 1f)
        {
            Explode();
        }
    }
    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        if (explodeRangeVisual != null)
            explodeRangeVisual.SetActive(false);

        Collider[] cols = Physics.OverlapSphere(transform.position, explodeDistance, attackLayer);
        foreach (var col in cols)
        {
            Vector3 dir = (col.transform.position - transform.position).normalized;
            AttackInfo info = new AttackInfo(explosionDamage, dir * explosionKnockbackForce);
            col.SendMessage("ApplyDamage", info, SendMessageOptions.DontRequireReceiver);
        }

        Destroy(gameObject);
    }
}

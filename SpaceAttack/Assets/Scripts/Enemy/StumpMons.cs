using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpMons : EnemyBase
{
    [Header("패트롤 설정")]
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolChangeTime = 3f;

    [Header("공격 설정")]
    [SerializeField] private float attackRadius = 6f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject darkMatterPrefab;

    private float patrolTimer;
    private bool isChasing;
    private bool isAttacking = false;
    private float lastAttackTime = -Mathf.Infinity;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        ChooseNewPatrolPoint();
        patrolTimer = patrolChangeTime;
    }

    private void Update()
    {
        if (isDead)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (isHit)
        {
            isAttacking = false;
            rb.velocity = Vector3.zero;
            return;
        }

        if (isAttacking)
        {
            rb.velocity = Vector3.zero;
            SetIsWalking(false);
            if (attackTarget != null)
            {
                float dirX = (attackTarget.position - transform.position).x;
                Flip(dirX); 
            }
            return;
        }

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
        if (isDead || isHit || !canDetectPlayer || isAttacking)
        {
            rb.velocity = Vector3.zero;
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
    }

    private void SetIsWalking(bool walking)
    {
        if (animator.GetBool("isWalking") != walking)
        {
            animator.SetBool("isWalking", walking);
        }
    }

    private void ChooseNewPatrolPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 3f;
        patrolTarget = transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);
    }

    protected override void OnPlayerDetected(Transform player)
    {
        if (!canDetectPlayer) return;
        isChasing = true;
        attackTarget = player;
    }

    protected override void Patrol()
    {
        if (!canDetectPlayer) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, DetectRadius, playerLayer);
        if (hits.Length > 0)
        {
            OnPlayerDetected(hits[0].transform);
            return;
        }

        if (!isChasing)
            MoveToRigidbody(patrolTarget, patrolSpeed);
    }

    private void Chase()
    {
        if (attackTarget == null)
        {
            isChasing = false;
            ChooseNewPatrolPoint();
            patrolTimer = patrolChangeTime;
            return;
        }

        float distance = Vector3.Distance(transform.position, attackTarget.position);

        if (distance > DetectRadius * 1.2f)
        {
            isChasing = false;
            attackTarget = null;
            ChooseNewPatrolPoint();
            patrolTimer = patrolChangeTime;
            return;
        }

        if (distance > attackRadius)
        {
            MoveToRigidbody(attackTarget.position, chaseSpeed);
        }
        else
        {
            rb.velocity = Vector3.zero;
            float dirX = (attackTarget.position - transform.position).x;
            Flip(dirX);

            TryRangedAttack();
        }
    }

    private void TryRangedAttack()
    {
        if (isHit || isAttacking) return;
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (attackTarget == null) return;

        StartCoroutine(RangedAttackRoutine());
    }

    public void OnFireDarkMatter()
    {
        if (attackTarget == null || darkMatterPrefab == null || firePoint == null) return;

        GameObject projectile = Instantiate(darkMatterPrefab, firePoint.position, Quaternion.identity);
        DarkMatter dm = projectile.GetComponent<DarkMatter>();
        if (dm != null)
        {
            dm.Launch(attackTarget.position);
        }
    }
    private IEnumerator RangedAttackRoutine()
    {
        isAttacking = true;
        rb.velocity = Vector3.zero;
        SetIsWalking(false);

        animator.SetTrigger("Attack");

        lastAttackTime = Time.time;

        float attackDuration = 2f; 
        float elapsed = 0f;
        while (elapsed < attackDuration)
        {
            if (isHit) 
            {
                isAttacking = false;
                yield break;
            }

            rb.velocity = Vector3.zero;

            if (attackTarget != null)
            {
                float dirX = (attackTarget.position - transform.position).x;
                Flip(dirX);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
    }

    private void MoveToRigidbody(Vector3 targetPos, float speed)
    {
        Vector3 direction = targetPos - rb.position;
        direction.y = 0f;

        if (direction.magnitude < 0.1f)
        {
            rb.velocity = Vector3.zero;
            SetIsWalking(false);
            return;
        }

        Vector3 move = direction.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);
        SetIsWalking(true);
        Flip(move.x);
    }
}

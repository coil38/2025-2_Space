using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockMons : EnemyBase
{
    [Header("펀치 몬스터 설정")]
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolChangeTime = 3f;

    [Header("공격 관련 설정")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;

    private float patrolTimer;
    private bool isChasing;
    private bool isAttacking = false;
    private Transform playerTransform;
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

        if (isAttacking && attackTarget != null)
        {
            float dirX = (attackTarget.position - transform.position).x;
            Flip(dirX);
        }

        if (isAttacking) return;  

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
        if (isDead || isHit || isAttacking || !canDetectPlayer)
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

        if (distance > attackRange)
        {
            MoveToRigidbody(attackTarget.position, chaseSpeed);
        }
        else
        {
            rb.velocity = Vector3.zero;

            float dirX = (attackTarget.position - transform.position).x;
            Flip(dirX);

            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (isHit) return;
        if (Time.time - lastAttackTime < attackCooldown || isAttacking) return;

        isAttacking = true;
        rb.velocity = Vector3.zero;
        animator.SetTrigger("Attack"); 
        lastAttackTime = Time.time;
    }

    public void OnAttackHit()
    {
        if (attackTarget == null) return;
        if (Vector3.Distance(transform.position, attackTarget.position) > attackRange + 1.5f) return;

        PlayerStatus player = attackTarget.GetComponent<PlayerStatus>();
        if (player != null)
        {
            AttackInfo attackInfo = new AttackInfo
            {
                damage = damage,
                attackDirection = (attackTarget.position - transform.position).normalized,
                attacker = this.gameObject
            };
            player.ApplyDamage(attackInfo);
        }
    }

    public void OnAttackEnd()
    {
        StartCoroutine(AttackPauseRoutine());
    }

    private IEnumerator AttackPauseRoutine()
    {
        float t = 0f;
        while (t < 0.7f)
        {
            if (isHit)
            {
                isAttacking = false;
                yield break;
            }
            t += Time.deltaTime;
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
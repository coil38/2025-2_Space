using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StumpMons : EnemyBase
{
    [Header("공격 설정")]
    [SerializeField] private float attackRadius = 6f;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject darkMatterPrefab;

    [Header("그루터기 사운드 설정")]
    [SerializeField] private AudioSource sfxSource;   
    [SerializeField] private AudioClip chargeClip;
    [SerializeField] private AudioClip fireClip;

    private bool isAttacking = false;
    private float lastAttackTime = -Mathf.Infinity;

    private enum State { Idle, Attack }
    private State currentState = State.Idle;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();

    }

    private void Update()
    {
        if (isDead || isHit)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        // 공격 중에는 상태 유지하며 항상 방향 회전
        if (currentState == State.Attack && attackTarget != null)
        {
            float dirX = (attackTarget.position - transform.position).x;
            Flip(dirX);
        }

        DetectPlayerAndAttack();
    }

    private void FixedUpdate()
    {
        // 항상 제자리
        rb.velocity = Vector3.zero;
    }

    private void DetectPlayerAndAttack()
    {
        if (!canDetectPlayer) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, DetectRadius, playerLayer);
        if (hits.Length == 0)
        {
            attackTarget = null;
            return;
        }

        attackTarget = hits[0].transform;

        // 시선 회전
        float dirX = (attackTarget.position - transform.position).x;
        Flip(dirX);

        float distance = Vector3.Distance(transform.position, attackTarget.position);
        if (distance > attackRadius) return;

        TryRangedAttack();
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
            dm.Launch(attackTarget.position);
    }

    private IEnumerator RangedAttackRoutine()
    {
        isAttacking = true;
        currentState = State.Attack;   // ⬅ 공격 상태 진입
        rb.velocity = Vector3.zero;

        animator.SetTrigger("Attack");
        lastAttackTime = Time.time;

        float attackDuration = 2f;
        float elapsed = 0f;

        while (elapsed < attackDuration)
        {
            if (isHit) break;

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
        currentState = State.Idle;   
    }
    public void OnChargeSound()
    {
        if (sfxSource != null && chargeClip != null)
            sfxSource.PlayOneShot(chargeClip);
    }

    public void OnFireSound()
    {
        if (sfxSource != null && fireClip != null)
            sfxSource.PlayOneShot(fireClip);
    }
}

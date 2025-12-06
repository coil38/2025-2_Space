using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldDollMons : EnemyBase
{
    [Header("순찰 설정")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolChangeTime = 3f;

    private float patrolTimer;

    // =============== 이동 상태 ===============
    private enum MoveState { Idle, MoveStart, MoveLoop, MoveStop }
    private MoveState moveState = MoveState.Idle;

    private bool isMoveStarting = false;

    // =============== 전투 상태 추가 ===============
    private enum State { Patrol, AttackReady, Attack, Escape }
    private State state = State.Patrol;

    [Header("공격 설정")]
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private GameObject yarnPrefab;
    [SerializeField] private GameObject redYarnPrefab;
    [SerializeField] private int yarnCount = 3;

    private float lastAttackTime = -999f;
    private float escapeTimer;
    private bool firstAttackDone = false;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
        patrolTimer = patrolChangeTime;
        ChooseNewPatrolPoint();

        lastAttackTime = Time.time; 
    }


    private void Update()
    {
        if (isDead || isHit) return;

        LookAtTargetDuringAttack(); 

        switch (state)
        {
            case State.Patrol:
                PatrolUpdate();
                break;

            case State.AttackReady:
                break;

            case State.Attack:
                break;

            case State.Escape:
                EscapeUpdate();
                break;
        }
    }

    private void FixedUpdate()
    {

        if (isDead || isHit)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (state == State.Patrol)
            MoveToPatrolTarget();

        if (state == State.Escape)
            EscapeMove();
    }

    private void PatrolUpdate()
    {
        base.Patrol();

        patrolTimer -= Time.deltaTime;

        if (attackTarget != null)
        {
            float dist = Vector3.Distance(transform.position, attackTarget.position);

            if (dist <= attackDistance)
            {
                EnterAttackReady();
                return;
            }
        }

        if (patrolTimer <= 0f)
        {
            ChooseNewPatrolPoint();
            patrolTimer = patrolChangeTime;
        }
    }

    private void LookAtTargetDuringAttack()
    {
        if (attackTarget == null) return;

        if (state == State.AttackReady || state == State.Attack)
        {
            Vector3 dir = attackTarget.position - transform.position;
            Flip(dir.x);
        }
    }

    protected override void OnPlayerDetected(Transform player)
    {
        attackTarget = player;
    }

    private void ChooseNewPatrolPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 6f;
        patrolTarget = transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);
        SetMoveState(MoveState.MoveStart);
    }

    private void MoveToPatrolTarget()
    {
        if (moveState == MoveState.Idle)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        Vector3 direction = patrolTarget - transform.position;
        direction.y = 0;

        float dist = direction.magnitude;

        if (dist < 0.8f)
        {
            rb.velocity = Vector3.zero;

            if (moveState != MoveState.MoveStop)
                SetMoveState(MoveState.MoveStop);

            return;
        }
        if (IsFrontBlocked(direction.normalized))
            direction = GetSideDirection(direction.normalized);

        rb.MovePosition(rb.position + direction.normalized * patrolSpeed * Time.fixedDeltaTime);

        Flip(direction.x);

        if (moveState == MoveState.MoveStart && isMoveStarting) return;

        if (moveState != MoveState.MoveLoop)
            SetMoveState(MoveState.MoveLoop);
    }

    private void SetMoveState(MoveState newState)
    {
        if (moveState == newState) return;
        moveState = newState;

        switch (moveState)
        {
            case MoveState.MoveStart:
                isMoveStarting = true;
                animator.SetTrigger("MoveStart");
                StartCoroutine(MoveStartToLoop());
                break;

            case MoveState.MoveLoop:
                animator.SetBool("MoveLoop", true);
                break;

            case MoveState.MoveStop:
                animator.SetBool("MoveLoop", false);
                animator.SetTrigger("MoveStop");
                StartCoroutine(StopThenIdle());
                break;

            case MoveState.Idle:
                rb.velocity = Vector3.zero;
                animator.SetBool("MoveLoop", false);
                break;
        }
    }

    private IEnumerator MoveStartToLoop()
    {
        yield return new WaitForSeconds(0.2f);
        isMoveStarting = false;

        if (moveState == MoveState.MoveStart)
            SetMoveState(MoveState.MoveLoop);
    }

    private IEnumerator StopThenIdle()
    {
        yield return new WaitForSeconds(0.2f);

        if (moveState == MoveState.MoveStop)
            SetMoveState(MoveState.Idle);
    }

    private void EnterAttackReady()
    {
        state = State.AttackReady;

        rb.velocity = Vector3.zero;
        SetMoveState(MoveState.Idle);

        animator.SetTrigger("AttackReady");

    }

    public void OnSpawnYarn()
    {
        SpawnYarns();
    }

    public void OnAttackEnd()
    {
        EnterEscape();
    }

    private void SpawnYarns()
    {
        if (attackTarget == null) return;

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float spreadDistance = 1.5f;
        float forwardOffset = 1f;

        Vector3[] offsets =
        {
        (-right * spreadDistance) + forward * forwardOffset,
        forward * (forwardOffset + 0.2f),
        (right * spreadDistance) + forward * forwardOffset
    };

        float[] speeds = { 7f, 5f, 3f };

        for (int i = 0; i < 3; i++)
        {
            bool isRed = Random.value <= 0.25f;
            GameObject prefab = isRed ? redYarnPrefab : yarnPrefab;

            Vector3 spawnPos = transform.position + Vector3.up * 0.5f + offsets[i];

            Vector3 dir = attackTarget.position - transform.position;
            dir.y = 0;
            Quaternion rot = Quaternion.LookRotation(dir);

            GameObject yarn = Instantiate(prefab, spawnPos, rot);

            SilkThread proj = yarn.GetComponent<SilkThread>();
            if (proj != null)
            {
                float finalDamage = isRed ? damage * 2f : damage;
                float speed = speeds[i];   

                proj.Init(attackTarget, finalDamage, isRed, speed);
            }
        }
    }

    // ESCAPE
    private void EnterEscape()
    {
        state = State.Escape;

        escapeTimer = 3f;

        animator.SetBool("MoveLoop", false);

        animator.SetTrigger("Escape");
        animator.SetBool("EscapeLoop", true);  

        rb.velocity = Vector3.zero;
    }

    private void EscapeUpdate()
    {
        escapeTimer -= Time.deltaTime;

        if (escapeTimer <= 0f)
        {
            animator.SetBool("EscapeLoop", false);  

            lastAttackTime = Time.time;
            state = State.Patrol;

            ChooseNewPatrolPoint();
            SetMoveState(MoveState.MoveStart);
        }
    }

    private void EscapeMove()
    {
        if (!attackTarget) return;

        Vector3 dir = (transform.position - attackTarget.position).normalized;
        dir.y = 0;

        if (IsFrontBlocked(dir))
            dir = GetSideDirection(dir);

        rb.MovePosition(rb.position + dir * patrolSpeed * Time.fixedDeltaTime);

        Flip(dir.x);
    }

    private bool IsFrontBlocked(Vector3 dir)
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, dir.normalized);
        float checkDist = 1.0f;

        return Physics.Raycast(ray, checkDist, LayerMask.GetMask("Wall", "DestructableObject"));
    }

    private Vector3 GetSideDirection(Vector3 dir)
    {
        Vector3 left = new Vector3(-dir.z, 0, dir.x);   
        Vector3 right = new Vector3(dir.z, 0, -dir.x);  

        if (!IsFrontBlocked(left)) return left;
        if (!IsFrontBlocked(right)) return right;

        return -dir;  
    }
}

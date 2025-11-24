using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minipollution : EnemyBase
{
    private enum State { Idle, Patrol, Chase, AttackReady, Dash, Escape }
    private State currentState = State.Idle;

    [Header("이동 관련")]
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3f;
    [SerializeField] private float patrolChangeTime = 3f;
    private float patrolTimer;
    [SerializeField] private float escapeSpeed = 6f;


    [Header("공격 관련")]
    [SerializeField] private float attackReadyTime = 3f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.4f;
    [SerializeField] private float attackCooldown = 5f;

    private bool isAttacking = false;
    private float lastAttackTime = -Mathf.Infinity;
    private bool canDamagePlayer = false;



    private Vector3 dashDirection;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();

        ChooseNewPatrolPoint();
        patrolTimer = patrolChangeTime;

        currentState = State.Patrol;
    }

    private void Update()
    {
        if (isDead || isHit) return;

        switch (currentState)
        {
            case State.Patrol:
                PatrolUpdate();
                break;

            case State.Chase:
                ChaseUpdate();
                break;

            case State.AttackReady:
                AttackReadyRotate();
                break;

            case State.Escape:
                EscapeUpdate();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isDead || isHit) return;

        switch (currentState)
        {
            case State.Patrol:
                MoveToRigidbody(patrolTarget, patrolSpeed);
                break;

            case State.Chase:
                MoveToRigidbody(attackTarget.position, chaseSpeed);
                break;

            case State.Dash:
                rb.MovePosition(transform.position + dashDirection * dashSpeed * Time.fixedDeltaTime);
                break;

            case State.Escape:
                EscapeMove();
                break;
        }
    }

    private void EscapeUpdate()
    {
        if (attackTarget == null)
            return; 
    }

    // -------- Patrol & Chase --------

    private void PatrolUpdate()
    {
        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0f)
        {
            ChooseNewPatrolPoint();
            patrolTimer = patrolChangeTime;
        }

        Patrol(); 
    }

    private void ChaseUpdate()
    {
        if (!attackTarget)
        {
            SwitchState(State.Patrol);
            return;
        }

        float dist = Vector3.Distance(transform.position, attackTarget.position);

        if (dist <= attackDistance && Time.time > lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(AttackReadyRoutine());
        }
    }

    protected override void OnPlayerDetected(Transform player)
    {
        attackTarget = player;
        SwitchState(State.Chase);
    }

    private void ChooseNewPatrolPoint()
    {
        Vector2 rand = Random.insideUnitCircle * 3f;
        patrolTarget = transform.position + new Vector3(rand.x, 0, rand.y);
    }

    private void MoveToRigidbody(Vector3 targetPos, float speed)
    {
        Vector3 dir = targetPos - rb.position;
        dir.y = 0;

        if (dir.magnitude < 0.1f)
        {
            rb.velocity = Vector3.zero;
            animator.SetBool("isWalking", false);
            return;
        }

        animator.SetBool("isWalking", true);
        rb.MovePosition(rb.position + dir.normalized * speed * Time.fixedDeltaTime);
        Flip(dir.x);
    }

    // ------- Attack Ready ---------

    private IEnumerator AttackReadyRoutine()
    {
        isAttacking = true;
        rb.velocity = Vector3.zero;

        animator.SetTrigger("AttackReady");
        SwitchState(State.AttackReady);

        float timer = attackReadyTime;
        while (timer > 0 && attackTarget != null && !isDead)
        {
            AttackReadyRotate(); 
            timer -= Time.deltaTime;
            yield return null;
        }

        if (attackTarget != null)
        {
            dashDirection = (attackTarget.position - transform.position).normalized;
        }

        StartCoroutine(DashRoutine());
    }

    private void AttackReadyRotate()
    {
        if (!attackTarget) return;

        Vector3 dir = attackTarget.position - transform.position;
        Flip(dir.x);
    }

    // ------- Dash --------

    private IEnumerator DashRoutine()
    {
        animator.SetTrigger("Dash");
        SwitchState(State.Dash);

        float timer = dashDuration;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        lastAttackTime = Time.time;
        isAttacking = false;

        SwitchState(State.Escape);

        canDetectPlayer = false;

        float escapeTimer = 4f; 
        while (escapeTimer > 0)
        {
            escapeTimer -= Time.deltaTime;
            yield return null;
        }

        canDetectPlayer = true;

        if (attackTarget != null)
        {
            SwitchState(State.Chase);
        }
        else
        {
            SwitchState(State.Patrol);
        }
    }

    private void EscapeMove()
    {
        if (!attackTarget) return;

        Vector3 opposite = (transform.position - attackTarget.position).normalized;
        Flip(opposite.x);

        rb.MovePosition(transform.position + opposite * escapeSpeed * Time.fixedDeltaTime);
    }


    private void SwitchState(State newState)
    {
        currentState = newState;

        if (newState == State.Patrol || newState == State.Chase)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }

        else if (newState == State.Escape)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }

    public void OnDashStartEvent()
    {
        canDamagePlayer = true;
        StartCoroutine(DashDamageRoutine());
    }
    private IEnumerator DashDamageRoutine()
    {
        while (canDamagePlayer)
        {
            if (attackTarget != null)
            {
                float dist = Vector3.Distance(transform.position, attackTarget.position);

                if (dist < 1.2f)  
                {
                    PlayerStatus player = attackTarget.GetComponent<PlayerStatus>();
                    if (player != null)
                    {
                        AttackInfo info = new AttackInfo
                        {
                            damage = damage,
                            attackDirection = dashDirection,
                            attacker = gameObject
                        };

                        player.ApplyDamage(info);

                        canDamagePlayer = false;   
                    }
                }
            }

            yield return null;  
        }
    }

    public void OnDashEndEvent()
    {
        canDamagePlayer = false;
    }

    protected override bool CanPlayHitAnimation()
    {
        return !(currentState == State.AttackReady ||
                 currentState == State.Dash);
    }

    public override void ApplyDamage(AttackInfo attackInfo)
    {
        if (currentState == State.Dash) // Dash만 무적
            return;

        base.ApplyDamage(attackInfo);
    }

}

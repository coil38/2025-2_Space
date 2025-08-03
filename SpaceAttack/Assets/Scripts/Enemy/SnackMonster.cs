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
    [SerializeField] private float damagePerTick = 1f;       // 한 번에 깎는 HP 양
    [SerializeField] private GameObject eatRangeVisualPrefab;
    [SerializeField] private float eatCooldown = 2f;  // 뱉고 나서 다시 먹기까지 대기시간
    private float lastEatTime = -Mathf.Infinity;

    private float patrolTimer;
    private bool isChasing;
    private bool isEating = false;
    private GameObject eatRangeVisualInstance;


    private bool isPaused = false;

    private Transform playerTransform;
    private PlayerStatus playerStatusScript;

    protected override void Start()
    {
        base.Start();
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
        if (isDead || isHit || isEating || isPaused) return;

        if (!isChasing)
        {
            MoveToRigidbody(patrolTarget, patrolSpeed);
        }
        else
        {
            Chase();
        }
    }

    private void ChooseNewPatrolPoint()
    {
        Vector2 randomOffset = Random.insideUnitCircle * 3f;
        patrolTarget = transform.position + new Vector3(randomOffset.x, 0, randomOffset.y);
    }

    protected override void OnPlayerDetected(Transform player)
    {
        if (!isEating)
        {
            isChasing = true;
            attackTarget = player;
        }
    }
    private bool IsPlayerInEatBox()
    {
        Vector3 center = transform.position + (isFacingRight ? transform.right : -transform.right) * (eatRadius / 2);
        Vector3 halfExtents = new Vector3(eatRadius / 2, 1f, 2.5f); 

        Collider[] hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity, LayerMask.GetMask("Player"));
        foreach (Collider hit in hits)
        {
            if (hit.transform == attackTarget)
                return true;
        }
        return false;
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

        float distanceToTarget = Vector3.Distance(transform.position, attackTarget.position);

        if (distanceToTarget <= eatRadius && Time.time - lastEatTime >= eatCooldown && IsPlayerInEatBox())
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

        MoveToRigidbody(attackTarget.position, chaseSpeed);
    }

    private IEnumerator EatPlayerRoutine()
    {
        isEating = true;
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

            eatRangeVisualInstance.transform.localScale = new Vector3(eatRadius, 3f, eatRadius * 4f);

            eatRangeVisualInstance.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

            eatRangeVisualInstance.SetActive(true);
        }

        yield return new WaitForSeconds(1.5f);

        if (!IsPlayerInEatBox())
        {
            isEating = false;
            if (eatRangeVisualInstance != null) Destroy(eatRangeVisualInstance);
            yield break;
        }


        playerTransform.SetParent(transform);
        playerTransform.localPosition = Vector3.up * 0.5f;

        if (playerStatusScript != null)
            playerStatusScript.DisableMovement();

        float elapsed = 0f;

        while (elapsed < eatDuration)
        {
            if (playerStatusScript != null)
            {
                AttackInfo attackInfo = new AttackInfo
                {
                    damage = damagePerTick,
                    attackDirection = Vector3.zero
                };
                playerStatusScript.ApplyDamage(attackInfo);
            }

            yield return new WaitForSeconds(damageInterval);
            elapsed += damageInterval;
        }

        if (playerStatusScript != null)
            playerStatusScript.EnableMovement();

        playerTransform.SetParent(null);

        Vector3 forwardOffset = (isFacingRight ? Vector3.right : Vector3.left) * 3f + Vector3.up * 1f;
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


        ChooseNewPatrolPoint();
        patrolTimer = patrolChangeTime;


        isPaused = true;
        yield return new WaitForSeconds(3f);
        isPaused = false;


        attackTarget = playerTransform;
        isChasing = true;
        lastEatTime = Time.time;
    }

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

}
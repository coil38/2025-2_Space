using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]



public abstract class EnemyBase : MonoBehaviour
{
    [Header("공통 속성")]
    public float hp = 10f;
    public float attackDistance = 2f;
    public float detectAngle = 155f;
    public float damage = 1f;
    public float attackDuration = 0.5f;

    [Header("공통 탐지 설정")]
    [SerializeField]
    private float detectRadius = 5f;  

    protected Rigidbody rb;
    [SerializeField] protected Animator animator;

    protected bool isDead;
    protected bool isHit;

    private float baseScaleX;

    protected Vector3 _currentPos;
    protected Vector3 attackDirection;
    protected LayerMask playerLayer;
    protected LayerMask attackLayer;


    [Header("공통 주변탐색 설정")]
    protected Vector3 patrolTarget;
    protected float patrolMoveTime = 2f;
    protected float patrolIdleTime = 1f;
    protected float patrolTimer = 0f;
    protected bool isPatrolMoving = false;

    [SerializeField] private float hitInvincibleTime = 0.4f;  // 피격 후 무적
    private bool canBeHit = true;

    protected float DetectRadius => detectRadius;
    protected virtual void OnPlayerDetected(Transform player) { }

    protected virtual void Start()
    {
        baseScaleX = transform.localScale.x;
        rb = GetComponent<Rigidbody>();

        if (animator == null)
            Debug.LogError($"[EnemyBase] Animator is not assigned on {gameObject.name}");

        playerLayer |= (1 << LayerMask.NameToLayer("Player"));
        attackLayer |= (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("DestructableObject"));

        StartCoroutine(EnemyPattern());
    }
    protected virtual void Patrol()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, DetectRadius, playerLayer);
        if (hits.Length > 0)
        {
            OnPlayerDetected(hits[0].transform); 
            return;
        }

        patrolTimer += Time.deltaTime;

        if (isPatrolMoving)
        {
            animator.SetBool("IsMoving", true);
            MoveTo(patrolTarget, 1.5f); // 기본 속도

            if (Vector3.Distance(transform.position, patrolTarget) < 0.5f || patrolTimer > patrolMoveTime)
            {
                isPatrolMoving = false;
                patrolTimer = 0f;
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);

            if (patrolTimer > patrolIdleTime)
            {
                isPatrolMoving = true;
                patrolTimer = 0f;

                Vector3 randomDirection = Random.insideUnitSphere;
                randomDirection.y = 0;
                patrolTarget = transform.position + randomDirection.normalized * Random.Range(3f, 6f);
            }
        }
    }
    protected void MoveTo(Vector3 target, float speed)
    {
        Vector3 direction = (target - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        Flip(direction.x); //좌우반전
    }

    protected virtual IEnumerator EnemyPattern()
    {
        float timer = 0f;
        float timer2 = 0f;
        float attackTime = attackDuration;

        while (true)
        {
            timer += Time.deltaTime / attackTime;
            if (timer > 1 && !isHit)
            {
                CheckAttack();
                timer = 0f;
            }

            if (isHit)
            {
                timer2 += Time.deltaTime / 0.5f;
                if (timer2 > 1)
                {
                    isHit = false;
                    timer2 = 0f;
                }
            }
            yield return null;
        }
    }

    protected virtual void CheckAttack() { }
    protected virtual void Attack() { }

    public virtual void ApplyDamage(AttackInfo attackInfo)
    {
        if (isDead || !canBeHit) return;

        hp -= attackInfo.damage;

        if (hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                animator.SetBool("Dead", true);
                rb.velocity = Vector3.zero;
                rb.AddForce(attackInfo.attackDirection, ForceMode.Impulse);
                Destroy(gameObject, 1f);
            }
        }
        else
        {
            rb.AddForce(attackInfo.attackDirection * 0.5f, ForceMode.Impulse);
            StartCoroutine(HitProcess(attackInfo.attackDirection));
        }
    }
    private IEnumerator HitProcess(Vector3 dir)
    {
        canBeHit = false;
        isHit = true;

        animator.SetTrigger("Hit"); 

        rb.AddForce(dir * 0.5f, ForceMode.Impulse);

        yield return new WaitForSeconds(hitInvincibleTime);

        isHit = false;
        canBeHit = true;
    }

    protected void Flip(float moveX)
    {
        if (moveX < -0.01f)
        {
            transform.localScale = new Vector3(-baseScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (moveX > 0.01f)
        {
            transform.localScale = new Vector3(baseScaleX, transform.localScale.y, transform.localScale.z);
        }
    }
}
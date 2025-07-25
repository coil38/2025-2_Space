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

    [SerializeField] protected Transform visualTransform;

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
    
    [Header("공통 피격후 경직 시간")]
    [SerializeField] protected float hitInvincibleTime = 0.4f;  
    private bool canBeHit = true;

    [Header("죽은 흔적 설정")]
    [SerializeField] private GameObject deathMarkPrefab;
    [SerializeField] private Transform footPosition;

    [Header("공통 사운드")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hitSound;

    protected PlayerStatus playerStatus;
    protected float DetectRadius => detectRadius;
    protected virtual void OnPlayerDetected(Transform player) { }

    protected Transform attackTarget;

    protected virtual void Start()
    {
        if (visualTransform != null)
            baseScaleX = visualTransform.localScale.x;
        else
            Debug.LogError("[EnemyBase] visualTransform is not assigned!");

        rb = GetComponent<Rigidbody>();

        if (animator == null)
            Debug.LogError($"[EnemyBase] Animator is not assigned on {gameObject.name}");

        playerLayer |= (1 << LayerMask.NameToLayer("Player"));
        attackLayer |= (1 << LayerMask.NameToLayer("Player")) | (1 << LayerMask.NameToLayer("DestructableObject"));

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            playerStatus = playerObj.GetComponent<PlayerStatus>();
        else
            Debug.LogError("[EnemyBase] Player object with tag 'Player' not found!");

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
        float attackTime = attackDuration;

        while (true)
        {
            timer += Time.deltaTime / attackDuration;
            if (timer > 1 && !isHit)
            {
                CheckAttack();
                timer = 0f;
            }

            yield return null;
        }
    }

    protected virtual void CheckAttack()
    {
        if (playerStatus == null)
        {
            Debug.LogWarning("playerStatus가 null입니다.");
            return;
        }
        if (playerStatus.isDead || isHit || isDead)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, attackDistance, playerLayer);
        if (hits.Length == 0)
            return;

        Transform player = hits[0].transform;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= attackDistance)
        {
            attackTarget = player;

            Attack();
        }
    }
    protected virtual void Attack() { }

    public virtual void ApplyDamage(AttackInfo attackInfo)
    {
        if (isDead || !canBeHit) return;

        hp -= attackInfo.damage;

        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        if (hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                animator.SetBool("Dead", true);
                rb.velocity = Vector3.zero;
                rb.AddForce(attackInfo.attackDirection, ForceMode.Impulse);

                if (deathMarkPrefab != null && footPosition != null)
                {
                    Vector3 spawnPos = footPosition.position;
                    Instantiate(deathMarkPrefab, spawnPos, Quaternion.identity);
                }

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

        isHit = true;

        animator.SetTrigger("Hit"); 

        rb.AddForce(dir * 0.5f, ForceMode.Impulse);

        yield return new WaitForSeconds(hitInvincibleTime);

        isHit = false;

    }

    protected void Flip(float moveX)
    {
        if (visualTransform == null) return;

        if (moveX < -0.01f)
        {
            visualTransform.localScale = new Vector3(-Mathf.Abs(baseScaleX),
                                                     visualTransform.localScale.y,
                                                     visualTransform.localScale.z);
        }
        else if (moveX > 0.01f)
        {
            visualTransform.localScale = new Vector3(Mathf.Abs(baseScaleX),
                                                     visualTransform.localScale.y,
                                                     visualTransform.localScale.z);
        }
    }
}
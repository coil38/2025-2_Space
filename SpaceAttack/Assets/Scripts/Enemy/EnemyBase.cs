using System;
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
    public int damage = 1;
    public float attackDuration = 0.5f;

    [Header("공통 탐지 설정")]
    [SerializeField]
    private float detectRadius = 5f;  

    protected Rigidbody rb;
    [SerializeField] protected Animator animator;

    [SerializeField] protected Transform visualTransform;

    protected bool isFacingRight = true;
    protected bool isDead;
    protected bool isHit;
    private float baseScaleX;

    protected Vector3 _currentPos;
    protected Vector3 attackDirection;
    protected LayerMask playerLayer;
   [SerializeField] protected LayerMask attackLayer;


    [Header("공통 주변탐색 설정")]
    protected Vector3 patrolTarget;
    
    [Header("공통 피격후 경직 시간")]
    [SerializeField] protected float hitInvincibleTime = 0.4f;  
    protected bool canBeHit = true;

    [Header("죽은 흔적 설정")]
    [SerializeField] protected GameObject deathMarkPrefab;
    [SerializeField] protected Transform footPosition;

    [Header("죽음 시 드롭 아이템")]
    [SerializeField] private GameObject heartPrefab;      // 드롭할 하트 프리팹
    [SerializeField] private float dropRadius = 1.5f;     // 몬스터 주변 랜덤 드롭 반경

    [Header("공통 사운드")]
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip hitSound;

    [Header("공통 체력 UI")]
    [SerializeField] protected MonsterHPUI monsterHPUI;
    protected float maxHP = 10f;

    protected PlayerStatus playerStatus;
    protected float DetectRadius => detectRadius;
    protected virtual void OnPlayerDetected(Transform player) { }

    protected Transform attackTarget;

    public Action<EnemyBase> OnDeathAction; // 몬스터 죽을 때 이벤트
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

        if (monsterHPUI == null)
            Debug.LogError("monsterHPUI가 할당되지 않았습니다.");
        else maxHP = hp;

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
    }
    protected void MoveTo(Vector3 target, float speed)
    {
        Vector3 direction = target - transform.position;
        direction.y = 0f; 
        direction = direction.normalized;

        transform.position += direction * speed * Time.deltaTime;

        Flip(direction.x);
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
        if (playerStatus.isDead || playerStatus.isBeingEaten || isHit || isDead)
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


    //죽음
    protected virtual void OnDeath()
    {
        float dropChance = 0.2f; // 20% 확률
        if (heartPrefab != null && UnityEngine.Random.value < dropChance)
        {
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-dropRadius, dropRadius),
                0.5f,
                UnityEngine.Random.Range(-dropRadius, dropRadius)
            );

            Vector3 spawnPos = transform.position + randomOffset;
            Instantiate(heartPrefab, spawnPos, Quaternion.identity);
        }
    }

    public virtual void ApplyDamage(AttackInfo attackInfo)
    {
        if (isDead || !canBeHit) return;

        hp -= attackInfo.damage;

        if (hitSound != null && audioSource != null)
            audioSource.PlayOneShot(hitSound);

        if (monsterHPUI != null)
            monsterHPUI.ReduceHP(maxHP, hp);

        // 사망 체크
        if (hp <= 0 && !isDead)
        {
            isDead = true;

            OnDeathAction?.Invoke(this);

            int exp = UnityEngine.Random.Range(3, 7);
            PlayerCore.GetDarkMatter(exp);

            OnDeath();

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
        else if (hp > 0) 
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(attackInfo.attackDirection * 0.5f, ForceMode.Impulse);
            StartCoroutine(HitProcess());
        }
    }
    protected virtual IEnumerator HitProcess()
    {
        isHit = true;
        animator.SetTrigger("Hit");

        yield return new WaitForSeconds(hitInvincibleTime);

        isHit = false;
    }



    protected void Flip(float moveX)
    {
        if (visualTransform == null) return;

        bool shouldFlipLeft = moveX < -0.01f;
        bool shouldFlipRight = moveX > 0.01f;

        if (shouldFlipLeft)
        {
            visualTransform.localScale = new Vector3(-Mathf.Abs(baseScaleX),
                                                     visualTransform.localScale.y,
                                                     visualTransform.localScale.z);
            isFacingRight = false;

        }
        else if (shouldFlipRight)
        {
            visualTransform.localScale = new Vector3(Mathf.Abs(baseScaleX),
                                                     visualTransform.localScale.y,
                                                     visualTransform.localScale.z);
            isFacingRight = true;
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeMonster : EnemyBase
{
    [Header("점프‑이동 설정")]
    [SerializeField] private float hopForce = 6f;   // 점프힘
    [SerializeField] private float hopCooldown = 0.7f;  //시간초

    [Header("투사체 설정")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float fireInterval = 2f;   // 공격 쿨타임
    [SerializeField] private float burstDelay = 0.12f; // 3발 사이 간격   
    [SerializeField] private float dashWindUp = 0.05f;  // Dash 애니 시작 후 총알 발사까지 지연
  

    private bool isAttacking = false;

    private Transform target;
    private bool canHop = true;
    private bool canFire = true;

    // ──────────────────────────────────────────────────────
    #region Unity Life Cycle
    protected override void Start()
    {
        base.Start();
        StartCoroutine(HopRoutine());
    }
    private void Update()
    {
        if (!isDead && !isHit)
        {
            Patrol();          
        }
    }
    private void FixedUpdate()
    {
        if (target)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            Flip(dir.x);
        }
    }
    #endregion
    protected override void Patrol()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, DetectRadius, playerLayer);
        if (hits.Length > 0)
        {
            OnPlayerDetected(hits[0].transform);
            return;                    
        }
    }

    #region EnemyBase Override
    protected override void OnPlayerDetected(Transform player)
    {
        Debug.Log($"[ShoeMonster] Player Detected: {player.name}");
        target = player;
        if (canFire) StartCoroutine(FireRoutine());
    }

    protected override void CheckAttack()
    {
        if (target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);
        Debug.Log($"[ShoeMonster] dist={dist:F2}, attackDistance={attackDistance}");

        if (dist <= attackDistance && canFire)
        {
            Debug.Log("[ShoeMonster] FireRoutine START");
            StartCoroutine(FireRoutine());
        }
    }
    #endregion

    #region Coroutines
    private IEnumerator HopRoutine()
    {
        while (!isDead)
        {
            if (canHop && !isHit & !isAttacking)
            {

                Vector3 horizDir;
                if (target)                      
                {
                    horizDir = (target.position - transform.position);
                    horizDir.y = 0;
                }
                else                             
                {
                    horizDir = Random.insideUnitSphere;
                    horizDir.y = 0;
                }
                horizDir = horizDir.normalized;

           
                Vector3 hopDir = horizDir * 1.0f + Vector3.up * 1.2f;  
                rb.AddForce(hopDir * hopForce, ForceMode.Impulse);


                canHop = false;
                yield return new WaitForSeconds(hopCooldown);
                canHop = true;
            }
            yield return null;
        }
    }

    private IEnumerator FireRoutine()
    {
        isAttacking = true;      
        canFire = false;

        rb.velocity = Vector3.zero;  

        for (int n = 0; n < 3; n++)
        {
            animator.ResetTrigger("Dash");   
            animator.SetTrigger("Dash");

            yield return new WaitForSeconds(dashWindUp); 

            FireSingleBullet();

            if (n < 2)
                yield return new WaitForSeconds(burstDelay);
        }

        yield return new WaitForSeconds(fireInterval);

        isAttacking = false;  
        canFire = true;
    }
    #endregion

    private void FireSingleBullet()  
    {
        if (!target) return;

        Vector3 flatDir = target.position - transform.position;
        flatDir.y = 0;
        Vector3 dir = flatDir.normalized;

        Vector3 spawnPos = transform.position + dir * 1.0f + Vector3.up * 0.5f;

        GameObject bulletObj = Instantiate(
            bulletPrefab,
            spawnPos,
            Quaternion.LookRotation(dir, Vector3.up));

        Vector3 velocity = dir * bulletSpeed;
        bulletObj.GetComponent<ShoeBullet>().Init(velocity);

        Collider myCol = GetComponent<Collider>();
        Collider bulletCol = bulletObj.GetComponent<Collider>();
        if (myCol && bulletCol) Physics.IgnoreCollision(bulletCol, myCol);
    }

    #region Attack
    protected override void Attack()
    {
        FireSingleBullet();   
    }

    #endregion
}

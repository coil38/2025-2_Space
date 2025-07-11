using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinMonsterState
{
    Patrol,         // 주변을 둘러봄
    Chase,          // 플레이어 추적
    ExplodeReady,   // 불붙고 천천히 따라감
    Exploding       // 폭발 실행
}

public class CoinMonster : EnemyBase
{
    [Header("코인 몬스터 설정")]
    public float chaseSpeed = 3f;
    public float explodeReadySpeed = 1f;
    public float explosionDelay = 2f;
    public float explosionDamage = 5f;
    public float explosionKnockbackForce = 5f;

    [Header("폭발 조건 및 범위")]
    public float triggerDistance = 1f;       // 플레이어가 붙으면 폭발 준비 시작 
    public float explodeDistance = 3f;       // 폭뎀범위
    
    
    private Transform player;
    private CoinMonsterState state = CoinMonsterState.Patrol;
    private float explodeTimer = 0f;

    //폭발반경범위 시각화
    public GameObject explodeRangeVisual;

    protected override void Start()
    {
        base.Start();
        state = CoinMonsterState.Patrol;

        if (explodeRangeVisual != null)
        {
            float baseRadius = 0.09f;  
            float scale = explodeDistance / baseRadius;

            explodeRangeVisual.transform.localScale = new Vector3(scale, scale, 1f);
            explodeRangeVisual.SetActive(false);
        }
    }

    private void Update()
    {
        if (isDead) return;

        switch (state)
        {
            case CoinMonsterState.Patrol:
                Patrol(); 
                break;

            case CoinMonsterState.Chase:
                ChasePlayer();
                break;

            case CoinMonsterState.ExplodeReady:
                ExplodeReady();
                break;
        }

        UpdateAnimation();
    }

    protected override void OnPlayerDetected(Transform detectedPlayer) //플레이어 찾음
    {
        player = detectedPlayer;
        state = CoinMonsterState.Chase;
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance < triggerDistance) 
        {
            state = CoinMonsterState.ExplodeReady;
            animator.SetTrigger("Dash");
            explodeTimer = 0f;
            return;
        }

        MoveTo(player.position, chaseSpeed);
    }

    private void ExplodeReady()
    {
        if (player == null) return;

        if (explodeRangeVisual != null)
            explodeRangeVisual.SetActive(true);

        explodeTimer += Time.deltaTime;

        animator.SetBool("IsMoving", false);

        if (explodeTimer >= explosionDelay)
        {
            Explode();
        }
    }

    private void Explode() //폭파
    {
        if (explodeRangeVisual != null)
            explodeRangeVisual.SetActive(false);

        Collider[] cols = Physics.OverlapSphere(transform.position, explodeDistance, attackLayer);
        foreach (var col in cols)
        {
            Vector3 dir = (col.transform.position - transform.position).normalized;
            AttackInfo info = new AttackInfo(explosionDamage, dir * explosionKnockbackForce);
            col.SendMessage("ApplyDamage", info, SendMessageOptions.DontRequireReceiver);
        }

        Destroy(gameObject);
    }
    private void UpdateAnimation()
    {
        bool isMoving = (state == CoinMonsterState.Chase || state == CoinMonsterState.ExplodeReady);
        animator.SetBool("IsMoving", isMoving);
    }
}

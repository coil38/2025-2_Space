using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    //플레이어 상태값--------------------------------------------
    [Header("PlayerInfo")]
    public float m_hp = 5f;               //체력
    public float m_speed = 5f;            //이동 속도
    public float m_DashDruation = 2.2f;   //대쉬 거리
    public float itemDetectDistance = 1.8f; //아이템 감지거리

    public ParticleSystem m_Particle;
    public ParticleSystem d_Particle;

    [HideInInspector] public bool isInvincibility = false;
    [HideInInspector] public bool isStuned = false;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isUsingSkill = false;

    public bool m_FacingRight { get; private set; }

    private Rigidbody rb;
    private Animator animator;

    private AttackInfo attackInfo = new AttackInfo();
    private Queue<AttackInfo> attackQueue = new Queue<AttackInfo>();
    private bool isDamageProcessing;
    private bool isCancleAttack;
    private void Start()
    {
        m_FacingRight = true;

        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        //각각의 상태 실행여부값 할당
        isDashing = TimeSystem.w_dashTimer.IsRunning();
        isInvincibility = TimeSystem.invincibilityTimer.IsRunning();
        isStuned = TimeSystem.stunTimer.IsRunning();

        if (TimeSystem.w_w_AttackTimer != null)
            isAttacking = TimeSystem.w_w_AttackTimer.IsRunning();

        if (TimeSystem.s_w_AttackTimer != null)
            isUsingSkill = TimeSystem.s_w_AttackTimer.IsRunning();


        CheckApplyDamage();    //피격 체킹
    }

    private void CheckApplyDamage()
    {
        if (isStuned) isDamageProcessing = false;   // 스턴상태일때, 데미지 처리 프로세스 해제

        if (isInvincibility || isStuned || isDead) return;   //대쉬 무적 상태 혹은 스턴 상태일 때, 피격 안됨

        if (!isDamageProcessing && attackQueue.Count == 1)   //현재 공격 대기 자가 1명일 때, 그 한명의 공격만 유효 처리
        {
            AttackInfo info = attackQueue.Dequeue();
            _ApplyDamage(info);

            isDamageProcessing = true;
        }
        else if (!isDamageProcessing && attackQueue.Count > 1)   //현재 공격 대기 자가 1명 이상일 때, 첫 한명의 공격만 유효 처리
        {
            float attackCount = attackQueue.Count - 1;
            AttackInfo info = attackQueue.Dequeue();
            _ApplyDamage(info);

            for (int i = 0; i < attackCount; i++)   //처음 받은 공격 개수만큼 취소(예외처리: 혹시 이 사이에 공격이 추가 되었을 수도 있음)
                attackQueue.Dequeue();

            isDamageProcessing = true;
        }
        else if (isDamageProcessing)   //현재 데미지 처리 중일 때, 모든 사람의 공격 무효 처리
        {
            attackQueue.Clear();
        }
    }

    public void ApplyDamage(AttackInfo info)
    {
        if (isInvincibility || isStuned || isDead) return;   //대쉬 무적 상태 혹은 스턴 상태일 때, 피격 안됨

        attackQueue.Enqueue(info);
    }

    private void _ApplyDamage(AttackInfo info)
    {
        float damage = info.damage;
        Vector3 dir = info.attackDirection;

        float mass = 1f;
        float attackForce = mass * 100f;

        m_hp -= damage;

        if (m_hp <= 0)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.StopAllSounds();

            //플레이어 사망 연출 시작
            isDead = true;
            animator.SetBool("Dead", true);
            //Destroy(gameObject, 1f);
        }
        else
        {
            //if (AudioManager.instance != null)
            //    AudioManager.instance.PlaySound("Hit");

            TimeSystem.stunTimer.Start();   //스턴 타이머 시작
                                            //스턴 연출 시작
            animator.SetTrigger("Hit");                          //피격 애니메이션
            Debug.Log("플레이어 피격");
            rb.AddForce(dir * attackForce);                 //넉백

        }
    }

    public void Flip()  //좌우반전 로직
    {
        m_FacingRight = !m_FacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}

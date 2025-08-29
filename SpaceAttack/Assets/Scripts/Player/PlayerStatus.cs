using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    //플레이어 상태값--------------------------------------------
    [Header("PlayerInfo")]
    public static int m_hp = 10;              //체력
    public static int m_maxhp = 10;           //최대 체력
    public float m_speed = 5f;            //이동 속도
    public float m_DashDistance = 3.2f;   //대쉬 거리
    public float itemDetectDistance = 1.8f; //아이템 감지거리
    public static float criticalRate = 50f;         //치명타 확률
    public static float criticalHitRate = 50f;     //치명타 피해
    public float missRate = 0f;             //회피율

    public ParticleSystem m_Particle;
    public ParticleSystem d_Particle;

    [HideInInspector] public bool isInvincibility = false;
    [HideInInspector] public bool isStuned = false;
    [HideInInspector] public bool isDashing = false;
    [HideInInspector] public bool isDead = false;
                      public bool isBeingEaten = false;  //먹히는 중인가?

    private bool _isRooted;
    public bool isRooted       //상태 이상: 속박
    {
        get {  return _isRooted; }
        set
        {
            Root(value);       //속박 여부 입력 후, 실행
            _isRooted = value;
        }
    }

    public bool m_FacingRight { get; private set; }

    private Rigidbody rb;
    private PlayerMovementAnimationController movemetAniController;

    private Queue<AttackInfo> attackQueue = new Queue<AttackInfo>();
    private bool isDamageProcessing;

    private void OnEnable()
    {
        m_FacingRight = true;
        rb = GetComponent<Rigidbody>();
        movemetAniController = GetComponent<PlayerMovementAnimationController>();
    }

    void Update()
    {
        //각각의 상태 실행여부값 할당
        isDashing = PlayerTimeSystem.w_dashTimer.IsRunning();
        isInvincibility = PlayerTimeSystem.invincibilityTimer.IsRunning();
        isStuned = PlayerTimeSystem.stunTimer.IsRunning();

        CheckApplyDamage();    //피격 체킹
    }

    private void Root(bool isRooted)
    {
        if (isRooted)  //상태이상 실행
        {
            GetComponent<PlayerAttack>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;   //플레이어 입력관련 스크립트

            PlayerMovementAnimationController temp = GetComponent<PlayerMovementAnimationController>();
            temp.ResetAnimationObj();        //모든 이미지 비활성화
            temp.ResetAttackAnimation();     //현재 공격 초기화

            GetComponent<Rigidbody>().useGravity = false;  //피격방지
            GetComponent<Collider>().enabled = false;
        }
        else           //상태이상 취소
        {
            GetComponent<PlayerAttack>().enabled = true;
            GetComponent<PlayerMovement>().enabled = true;   //플레이어 입력관련 스크립트

            PlayerMovementAnimationController temp = GetComponent<PlayerMovementAnimationController>();
            temp.SetDirection();           //현재에 맞는 이미지 활성화

            if (!isDead)
            {
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Collider>().enabled = true;
            }
        }
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
        if (info.attacker != null && info.attacker.CompareTag("SnackMonster"))  // 공격을 받는 대상인 스낵몬스터 일 경우
        {
            _ApplyDamage(info);
            return;
        }

        if (isInvincibility || isStuned || isDead) return;   //대쉬 무적 상태 혹은 스턴 상태일 때, 피격 안됨

        attackQueue.Enqueue(info);
    }

    private void _ApplyDamage(AttackInfo info)
    {
        int damage = (int)info.damage;
        Vector3 dir = info.attackDirection;

        float randomValue = Random.Range(0.01f, 100f);
        if (randomValue < missRate)
        {
            //회피성공
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowMiss(transform.position + transform.up * 0.3f);
        }
        else
        {
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowDamage(transform.position + transform.up * 0.3f, damage, true);
        }

        float mass = 1f;
        float attackForce = mass * 100f;

        if(PlayerUIManager.instance != null)
            PlayerUIManager.instance.ReducePlayerUI(m_hp, damage); //체력감소 UI적용

        m_hp -= damage;

        if (m_hp <= 0)
        {
            if (AudioManager.instance != null)
                AudioManager.instance.StopAllSounds();

            GetComponent<Rigidbody>().useGravity = false;  //피격방지
            GetComponent<Collider>().enabled = false;

            //플레이어 사망 연출 시작
            isDead = true;
            //animator.SetBool("Dead", true);
            //Destroy(gameObject, 1f);
        }
        else
        {
            if (AudioManager.instance != null)
                AudioManager.instance.PlaySound("Hit");

            if (!isRooted)
            {
                PlayerTimeSystem.stunTimer.Start();   //스턴 타이머 시작
                movemetAniController.PlayAnimation("Hit");  //피격 애니메이션 ( 속박상태가 아닐 경우 )
            }

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerStatus : MonoBehaviour
{
    public static PlayerStatus Instance { get;  private set; }

    //플레이어 상태값--------------------------------------------
    [Header("PlayerInfo")]
    public static int m_hp = 9;                     //체력
    public static int m_maxhp = 10;                  //최대 체력
    public static float m_speed = 5f;                //이동 속도
    public static float m_DashDistance = 3.2f;       //대쉬 거리
    public float itemDetectDistance = 1.8f;          //아이템 감지거리
    public static float criticalChanceRate = 0.05f;  //치명타 확률
    public static float criticalRate = 0.5f;         //치명타 피해
    public static float missRate = 0.01f;            //회피율
    public static float normalDamage = 6;            //기본공격력
    public static float hitRate = 1f;                //피격배율
    public static bool cannotHealing = false;        //회복불가
    public static bool maxHpFixing = false;          //최대체력고정
    public static int losedHp = 0;                   //최대체력 변경후, 잃어버린 체력
    public static int shild_hp = 0;                  //방어막 체력
    public static int maxDarkMaterialCount = 100;    //암흑물질 최대수치

    public ParticleSystem m_Particle;
    public ParticleSystem d_Particle;

    [HideInInspector] public bool isInvincibility = false;
    [HideInInspector] public bool isStuned = false;
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

    private GUIStyle labelStyle;

    private void Awake()   //싱글톤화
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            if (Instance != this)
            {
                LogUtil.Log("파괴됨.");
                Destroy(gameObject);
            }
        }


        labelStyle = new GUIStyle();
        labelStyle.fontSize = 25;
        labelStyle.normal.textColor = Color.white;
    }
    private void OnEnable()
    {
        m_FacingRight = true;
        rb = GetComponent<Rigidbody>();
        movemetAniController = GetComponent<PlayerMovementAnimationController>();
    }

    private void OnDestroy()
    {
        if (Instance == this) LogUtil.Log("플레이어 파괴");
    }

    public void InitializeEvent()  //이벤트 체인 함수
    {
        PlayerEvent.correctionEventHandler += SetCorrectionValue;  //플레이어 스텟 보정 이벤트 구독
    }

    private void OnDisable()
    {
        PlayerEvent.correctionEventHandler -= SetCorrectionValue;  //플레이어 스텟 보정 이벤트 구독 해지
    }

    private void Update()
    {
        //각각의 상태 실행여부값 할당
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
            temp.ResetAni();                          //애니메이션 초기화

            if (!isDead)
            {
                GetComponent<Rigidbody>().useGravity = true;
                GetComponent<Collider>().enabled = true;
            }
        }
    }

    private void CheckApplyDamage()  //다중 피격 받을 시, 예외처리 함수
    {
        //if (isStuned) isDamageProcessing = false;   // 스턴상태일때, 데미지 처리 프로세스 해제

        if (isInvincibility || isStuned || isDead) return;   //대쉬 무적 상태 혹은 스턴 상태일 때, 피격 안됨

        if (!isDamageProcessing && attackQueue.Count == 1)   //현재 공격 대기 자가 1명일 때, 그 한명의 공격만 유효 처리
        {
            //LogUtil.Log($"한명 공격_공격 개수: {attackQueue.Count}");
            isDamageProcessing = true;
            AttackInfo info = attackQueue.Dequeue();
            _ApplyDamage(info);
        }
        else if (!isDamageProcessing && attackQueue.Count > 1)   //현재 공격 대기 자가 1명 이상일 때, 첫 한명의 공격만 유효 처리
        {
            //LogUtil.Log($"다중 공격_공격 개수: {attackQueue.Count}");
            isDamageProcessing = true;
            float attackCount = attackQueue.Count - 1;
            AttackInfo info = attackQueue.Dequeue();
            _ApplyDamage(info);

            for (int i = 0; i < attackCount; i++)   //처음 받은 공격 개수만큼 취소(예외처리: 혹시 이 사이에 공격이 추가 되었을 수도 있음)
                attackQueue.Dequeue();
        }
        else if (isDamageProcessing)   //현재 데미지 처리 중일 때, 모든 사람의 공격 무효 처리
        {
            attackQueue.Clear();
        }
    }

    public static void ChangeShildHp(bool isAdd, int amount)
    {
        if (isAdd) shild_hp += amount;
        else shild_hp -= amount;
        shild_hp = Mathf.Max(0, shild_hp);
        if (PlayerUIManager.instance != null) PlayerUIManager.instance.ResetHpUI(); //체력UI 갱신
    }
    public static void AddHp(int amount)  //체력 추가 함수
    {
        if (cannotHealing) return;

        int targetHp = m_hp + amount;
        if (targetHp <= m_maxhp) m_hp = targetHp;
        else m_hp = m_maxhp;

        if (PlayerUIManager.instance != null) PlayerUIManager.instance.ResetHpUI(); //체력UI 갱신
    }

    public void ReduceHp(int amount)  // 체력 감소 함수
    {
        int temp = amount;
        int damage = (int)(amount * hitRate);
        //LogUtil.Log($"기본 데미지: {temp}, 바뀐 데미지: {damage}");

        if (damage <= 0) return;

        EventManager.relicEvent.OnPlayerLoseHp(damage);

        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.ReducePlayerUI(m_hp,shild_hp, damage); //체력감소 UI적용

        if (DamageEffectManager.instance != null)
            DamageEffectManager.instance.ShowDamage(transform.position + transform.up * 0.3f, damage, true);

        if (shild_hp > 0)
        {
            int remain = shild_hp >= damage ? 0 : damage - shild_hp;
            shild_hp = Math.Max(0, shild_hp - damage);
            m_hp -= remain;
        }
        else m_hp -= damage;

        CheckPlayerDead();
    }
    public static void ChangeMaxHp(bool isAdd, int amount)  //최대 체력 면경 함수
    {
        if (maxHpFixing) return;

        if(isAdd) m_maxhp += amount;
        else
        {
            m_maxhp -= amount;
            if (m_maxhp < m_hp)
            {
                LogUtil.Log($"감소량: {amount}, 감소후, 최대체력: {m_maxhp}, 현재체력: {m_hp}");
                losedHp = m_hp - m_maxhp;
                m_hp = m_maxhp;
            }
        }
        if (PlayerUIManager.instance != null) PlayerUIManager.instance.ResetHpUI(); //체력UI 갱신
    }

    public static void ChangeMaxDarkMatCount(bool isAdd, int amount)
    {
        amount = Math.Abs(amount);
        if (isAdd) maxDarkMaterialCount += amount;
        else maxDarkMaterialCount -= amount;

        PlayerUIManager.instance.ChangeMaxDarkMaterial(maxDarkMaterialCount);
    }

    public static void RecoverLosedHp(int amount)  //잃어버린 체력값 보충
    {
        LogUtil.Log($"플레이어 체력:{m_hp}, 잃어버린 체력: {losedHp}");
        m_hp += amount;
        losedHp = 0;
        if (PlayerUIManager.instance != null) PlayerUIManager.instance.ResetHpUI(); //체력UI 갱신
    }
    public static int GetLosedHp()                  //잃어버린 체력값 받기 함수
    {
        int temp = losedHp;
        losedHp = 0;
        return temp;
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

    private void CheckPlayerDead()
    {
        if (m_hp > 0) return;                            //유물중, 다시 살아나는 유물이 있어서 나누어 놓음
        isDead = true;
        m_hp = 0;
        EventManager.relicEvent.OnPlayerDeadEvent();

        if (isDead)
        {
            EventManager.relicEvent.OnPlayerDeadEvent();   //플레이어 사망 이벤트 실행

            if (AudioManager.instance != null)
                AudioManager.instance.StopAllSounds();

            GetComponent<Rigidbody>().useGravity = false;  //피격방지
            GetComponent<Collider>().enabled = false;
            if (!isRooted)
            {
                movemetAniController.PlayAnimation("Dead");   //사망 애니메이션 재생
            }
            //플레이어 사망 연출 시작
            LogUtil.Log("플레이어 사망");
        }
    }

    private void _ApplyDamage(AttackInfo info)
    {
        int damage = (int)info.damage;
        Vector3 dir = info.attackDirection;

        EventManager.relicEvent.OnPlayerHitEventStart(damage, info.attacker);    //플레이어 피격 이벤트 실행

        float randomValue = UnityEngine.Random.value;
        if (randomValue <= missRate)            //회피성공
        {
            if (DamageEffectManager.instance != null)
                DamageEffectManager.instance.ShowMiss();

            damage = 0;
        }

        float mass = 1f;
        float attackForce = mass * 100f;
        ReduceHp(damage);     //플레이어 체력 감소

        if (!isDead)
        {
            PlayerSoundManager.PlayPlayerHitSound();
            if (!isRooted)
            {
                PlayerTimeSystem.stunTimer.Start();   //스턴 타이머 시작
                movemetAniController.PlayAnimation("Hit");  //피격 애니메이션 ( 속박상태가 아닐 경우 )
            }
            LogUtil.Log($"플레이어 피격, 현재 체력: {m_hp}");
            rb.AddForce(dir * attackForce);                 //넉백
        }

        EventManager.relicEvent.OnPlayerHitEventEnd(); //피격 종료 이벤트 실행
        isDamageProcessing = false;
    }

    public void Flip()  //좌우반전 로직
    {
        m_FacingRight = !m_FacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    public void SetCorrectionValue(object obj, PlayerEvent e)
    {
        if (e.correctablility)  //플레이어 스텟 보정치 주입
        {
            //LogUtil.Log("플레이어 스텟 상승");
            m_maxhp += e.heartCorrection;
            m_speed += DataManager.instance.i_speed * e.speedCorrection;
            //LogUtil.Log($"{e.heartCorrection}, {e.speedCorrection} / 100");

            if (PlayerUIManager.instance != null)
                PlayerUIManager.instance.ResetHpUI(); //체력 초기화

            //플레이어 스텟보정 연출
        }
    }

    //InventoryManager inventory;
    //private void OnGUI()
    //{
    //    GUILayout.BeginArea(new Rect(10, 350, 800, 1650));

    //    GUILayout.Label("===플레이어 스텟===", labelStyle);
    //    GUILayout.Label($"체력: {m_hp}, 최대 체력: {m_maxhp}, 쉴드 체력: {shild_hp}", labelStyle);
    //    GUILayout.Label($"이동속도: {m_speed}, 기본 이동 속도: {DataManager.instance.i_speed}", labelStyle);
    //    GUILayout.Label($"치명타 피해률: {criticalRate}, 치명타 확률: {criticalChanceRate}", labelStyle);
    //    GUILayout.Label($"회피률: {missRate} , 기본공격력: {normalDamage}", labelStyle);
    //    GUILayout.Label($"레벨: {PlayerCore.Level} , 경험치: {PlayerCore.DarkMaterialCount}, 피격배율: {hitRate}", labelStyle);
    //    GUILayout.Label($"오염된 프로세스 드랍률: {RewardSystem.RelicDropRate}", labelStyle);

    //    GUILayout.Space(50);

    //    if (inventory == null)
    //        inventory = FindObjectOfType<InventoryManager>();

    //    GUILayout.Label($"보유중인 유물 개수: {inventory._relics.Length}", labelStyle);
    //    foreach (var relic in inventory._relics)
    //    {
    //        GUILayout.Label($"{relic.relicID}:{relic.relicName}", labelStyle);
    //    }

    //    GUILayout.EndArea();
    //}
}

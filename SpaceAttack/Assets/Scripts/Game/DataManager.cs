using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager instance;   //싱글톤 패턴

    [SerializeField] private SoundDatabaseSO soundDatabase;  //사운드 데이터 베이스
    [SerializeField] private LevelDatabaseSO levelDatabase;  //레벨 데이터 베이스
    [SerializeField] private RelicDatabaseSO relicDatabase;  //유물 데이터 베이스

    [SerializeField] private GameObject relicObject;         //유물 프리팹
    [SerializeField] private GameObject[] chipsetPrfs;       //칩셋 프리팹

    //플레이어 초기 상태 데이터
    private int m_hp = 10;                    //체력
    private int m_maxhp = 10;                 //최대 체력
    private float m_speed = 5f;               //이동 속도
    private float m_DashDistance = 3.2f;      //대쉬 거리
    private float criticalChanceRate = 0.05f; //치명타 확률
    private float criticalRate = 0.5f;        //치명타 피해
    private float missRate = 0.01f;           //회피율
    private float normalDamage = 50;          //기본공격력
    private float m_DashTime = 0.2f;          //대쉬 시간
    private float c_DashTime = 3f;            //대쉬 쿨타임
    private float m_stunTime = 0.3f;          //스턴 시간
    private float hitRate = 1f;               //피격배율
    private bool cannotHealing = false;       //회복불가
    private bool maxHpFixing = false;         //최대체력고정

    public int i_hp { get { return m_hp; } }
    public int i_maxhp { get { return m_maxhp; } }
    public float i_speed { get { return m_speed; } }
    public float i_DashDistance { get { return m_DashDistance; } }
    public float i_criticalChanceRate { get { return criticalChanceRate; } }
    public float i_criticalRate { get { return criticalRate; } }
    public float i_missRate { get { return missRate; } }
    public float i_normalDamage { get { return normalDamage; } }
    public float i_m_DashTime { get { return m_DashTime; } }
    public float i_c_DashTime { get { return c_DashTime; } }
    public float i_m_stunTime { get { return m_stunTime; } }
    public float i_hitRate { get { return hitRate; } }

    public GameObject _relicObject
    {
        get { return relicObject; }
    }

    public GameObject GetChipsetPrfByName(string name)
    {
        foreach(var chipset in chipsetPrfs)
            if (chipset.GetComponent<ChipSetType>().chipSetName == name)
                return chipset;
        return null;
    }

    public SoundDatabaseSO _soundDatabase
    {
        get { return soundDatabase; }
    }
    public LevelDatabaseSO _levelDatabase
    {
        get { return levelDatabase; }
    }

    public RelicDatabaseSO _RelicDatabase
    {
        get { return  relicDatabase; }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializePlayerStatus()
    {
        PlayerStatus.m_hp = m_hp;                                    //체력
        PlayerStatus.m_maxhp = m_maxhp;                              //최대 체력
        PlayerStatus.m_speed = m_speed;                              //이동 속도
        PlayerStatus.m_DashDistance = m_DashDistance;                //대쉬 거리
        PlayerStatus.criticalChanceRate = criticalChanceRate;        //치명타 확률
        PlayerStatus.criticalRate = criticalRate;                    //치명타 피해
        PlayerStatus.missRate = missRate;                            //회피율
        PlayerStatus.normalDamage = normalDamage;                    //기본공격력
        PlayerStatus.hitRate = hitRate;                              //피격 배율
        PlayerStatus.cannotHealing = cannotHealing;                  //회복불가여부
        PlayerStatus.maxHpFixing = maxHpFixing;                      //최대체력 고정여부
        PlayerStatus.losedHp = 0;                                    //잃어버린 체력
        PlayerStatus.shild_hp = 0;                                   //방어막 하트

        PlayerTimeSystem.c_DashTime = c_DashTime;                    //대시 쿨타임
        PlayerTimeSystem.m_DashTime = m_DashTime;                    //대시 시간
        PlayerTimeSystem.m_stunTime = m_stunTime;                    //스턴시간

        AttackEventManager.InitialEvent();                           //이벤트 초기화
        RelicEvent.InitializeEvent();
        PlayerEvent.Initialize();

    }
}

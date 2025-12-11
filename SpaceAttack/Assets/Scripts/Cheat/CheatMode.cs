using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatMode : MonoBehaviour
{
    [Header("치트 설정")]
    public float attackIncreaseAmount = 10f;  // C키로 증가시킬 공격력 수치
    public int healAmount = 5;                // V키로 회복할 체력 수치
    public KeyCode attackCheatKey = KeyCode.C;
    public KeyCode healKey = KeyCode.V;

    public KeyCode spawnNormalRelic = KeyCode.Alpha1;
    public KeyCode spawnRiskRelic = KeyCode.Alpha2;
    public KeyCode spawnSourceRelic = KeyCode.Alpha3;

    public static CheatMode Instance;

    private int[] riskRelicIds = {1034, 1035, 1036, 1037, 1038, 1039, 1040, 1041, 1043, 1045, 1046 };

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // 공격력 증가
        if (Input.GetKeyDown(attackCheatKey))
        {
            IncreasePlayerAttack();
        }

        // 체력 회복
        if (Input.GetKeyDown(healKey))
        {
            HealPlayer();
        }

        // 유물 드랍
        if (Input.GetKeyDown(spawnNormalRelic))
        {
            SpawnRelic(RelicType.NormalRelic);
        }
        else if (Input.GetKeyDown(spawnRiskRelic))
        {
            SpawnRiskRelic();
        }
        else if (Input.GetKeyDown(spawnSourceRelic))
        {
            SpawnRelic(RelicType.SourceRelic);
        }
    }

    private void IncreasePlayerAttack()
    {
        PlayerStatus.normalDamage += attackIncreaseAmount;
        Debug.Log($"[CheatMode] 공격력 +{attackIncreaseAmount}! 현재 공격력: {PlayerStatus.normalDamage}");
    }

    private void HealPlayer()
    {
        if (PlayerStatus.Instance == null)
        {
            return;
        }

        PlayerStatus.AddHp(healAmount);
    }

    private void SpawnRelic(RelicType relicType)  //일반, 근원용 유물 생성 코드
    {
        RelicSO[] relicSOs = DataManager.instance._RelicDatabase.GetRelicsByType(relicType);
        int randomValue = UnityEngine.Random.Range(0, relicSOs.Length);
        LogUtil.Log($"유물 타입: {relicType}, 뽑힌 유물 순서: {randomValue}, 대상 유물 개수: {relicSOs.Length}");
        RelicSO relic = DataManager.instance._RelicDatabase.GetRelicByIndex(randomValue);  //받은 유물중, 랜덤 유물 리스트index의 유물 받기

        UISoundManager.PlayDropItem();            //아이템 드랍 사운드 재생

        GameObject temp = DataManager.instance._relicObject;
        GameObject relicObj = Instantiate(temp, PlayerStatus.Instance.transform.position, temp.transform.rotation);

        relicObj.GetComponent<BaseRelic>().Initialize(relic.relicID, relic.relicName, relic.iconSprite); //생성한 유물 오브젝트에 유물정보 갱신
    }
    private void SpawnRiskRelic()    //리스크 유물 소환
    {
        int randomValue = UnityEngine.Random.Range(0, riskRelicIds.Length);

        RelicSO relic = DataManager.instance._RelicDatabase.GetRelicById(riskRelicIds[randomValue]);  //받은 유물중, 랜덤 유물 리스트index의 유물 받기

        UISoundManager.PlayDropItem();            //아이템 드랍 사운드 재생

        GameObject temp = DataManager.instance._relicObject;
        GameObject relicObj = Instantiate(temp, PlayerStatus.Instance.transform.position, temp.transform.rotation);

        relicObj.GetComponent<BaseRelic>().Initialize(relic.relicID, relic.relicName, relic.iconSprite); //생성한 유물 오브젝트에 유물정보 갱신
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum RewardType
{
    MonsterDrop,          //몬스터 드랍
    RewardBox,            //보상 상자
    HiddenBox,            //히든 상자
    SupplyBox,            //보급 상자
    MiddleBossBox,        //중간 보스 상자
    Purifier,             //정화기
    Exchanger             //교환기
}

public class RewardSystem : MonoBehaviour
{
    [SerializeField] private GameObject _halfHeartPrf;
    private static GameObject halfHeartPrf;

    public static float defualtDropRate = 0.00f;
    public static float defualtItemDropRate = 0.00f;

    public static float RelicDropRate = 0.00f; //0.05f;
    public static float itemDropRate = 0.00f;

    private static InventoryManager inventoryManager;  //플레이어 인벤토리

    private static RewardData[] rewardDatas = new RewardData[]
    {
        new RewardData(RewardType.MonsterDrop, 0.02f, 0f, 0f, 0.6f),
        new RewardData(RewardType.RewardBox, 1f, 0f, 0.01f, 0.8f),
        new RewardData(RewardType.HiddenBox, 1f, 0f, 0.6f, 0.9f),
        new RewardData(RewardType.SupplyBox, 1f, 0f, 0.7f, 0.9f),
        new RewardData(RewardType.MiddleBossBox, 1f, 0f, 1f, 1f),
        new RewardData(RewardType.Purifier, 0f, 1f, 0f, 0f),
        new RewardData(RewardType.Exchanger, 1f, 0f, 0f, 0f)
    };

    private void Awake()
    {
        halfHeartPrf = _halfHeartPrf;
    }

    public static void ChangeRewerdDataRate(RewardType rewardType, bool isAdd, float itemRate)         //유물에서 보상타입의 아이템 드랍확률 조정 함수
    {
        RewardData rewardData = Array.Find(rewardDatas, r => r.rewardType == rewardType);
        float temp = rewardData.dropItemRate;
        if (isAdd) rewardData.dropItemRate += itemRate;
        else rewardData.dropItemRate -= itemRate;
        LogUtil.Log($"아이템 드랍확률 조정 - 타입:{rewardType}, 증가여부:{isAdd}, 증가 수치: {itemRate}, 변경전 수치: {temp}, 변경후 수치: {rewardData.dropItemRate}");
    }

    public static void DropRewards(RewardType rewardType, Vector3 dropPos)                //공용 아이템 드랍 함수
    {
        if (inventoryManager == null)
            inventoryManager = PlayerStatus.Instance.GetComponent<InventoryManager>();

        RewardData rewardData = Array.Find(rewardDatas, r => r.rewardType == rewardType); //알맞은 보상 데이터 찾기

        if (GetRandomValue() <= rewardData.normalRelicRate + RelicDropRate + itemDropRate + rewardData.dropItemRate)   //일반 유물 드랍 확률
            DropRelicObjRandomly(RelicType.NormalRelic, dropPos);

        if (GetRandomValue() <= rewardData.purifiedRelicRate + RelicDropRate + itemDropRate + rewardData.dropItemRate) //정화된 유물 드랍 확률
            DropRelicObjRandomly(RelicType.PurifiedRelic, dropPos);

        if (GetRandomValue() <= rewardData.sourceRelicRate + RelicDropRate + itemDropRate + rewardData.dropItemRate)   //근원 유물 드랍 확률
            DropRelicObjRandomly(RelicType.SourceRelic, dropPos);

        if (GetRandomValue() <= rewardData.halfHpRate + itemDropRate + rewardData.dropItemRate)   //하트 절반 드랍 확률
            DropHeartObj(dropPos);
    }

    private static void DropRelicObjRandomly(RelicType relicType, Vector3 dropPos)           //유물 드랍 함수
    {
        if (DataManager.instance == null || DataManager.instance._RelicDatabase.GetRelicCount() == 0)
        {
            LogUtil.LogError("DataManager 인스턴스가 생성되지 않거나 유물이 할당되지 않았습니다.");
            return;
        }

        RelicSO[] relicSOs = DataManager.instance._RelicDatabase.GetRelicsByType(relicType);
        int randomValue = UnityEngine.Random.Range(0, relicSOs.Length);
        LogUtil.Log($"유물 타입: {relicType}, 뽑힌 유물 순서: {randomValue}, 대상 유물 개수: {relicSOs.Length}");
        RelicSO relic = DataManager.instance._RelicDatabase.GetRelicByIndex(randomValue);  //받은 유물중, 랜덤 유물 리스트index의 유물 받기

        int[] cannotGetIds = relic.cannotEquipRelicId;
        bool isExit = false;
        foreach (var id in cannotGetIds)
        {
            foreach (var item in inventoryManager._relics)
            {
                if (id == item.relicID) isExit = true;
            }
        }
        if (isExit) DropRelicObjRandomly(relicType, dropPos);    //대상이 존재할 경우 재실행

        GameObject temp = DataManager.instance._relicObject;
        GameObject relicObj = Instantiate(temp, dropPos, temp.transform.rotation);

        relicObj.GetComponent<BaseRelic>().Initialize(relic.relicID, relic.relicName, relic.iconSprite); //생성한 유물 오브젝트에 유물정보 갱신
    }

    private static void DropHeartObj(Vector3 dropPos)
    {
        if (halfHeartPrf != null)
        {
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-1.5f, 1.5f),
                0.5f,
                UnityEngine.Random.Range(-1.5f, 1.5f)
            );

            Vector3 spawnPos = dropPos + randomOffset;
            Instantiate(halfHeartPrf, spawnPos, halfHeartPrf.transform.rotation);
        }
        else
        {
            LogUtil.Log("하트프리팹이 존재하지 않습니다.");
        }
    }

    private static float GetRandomValue()
    {
        return UnityEngine.Random.value;
    }
}

public class RewardData
{
    public RewardType rewardType;
    public float dropItemRate;          //아이템 드랍 확률
    public float normalRelicRate;       //일반 유물 드랍 확률
    public float purifiedRelicRate;     //정화된 유물 드랍 확룰
    public float sourceRelicRate;       //근원 유물 드랍 확률
    public float halfHpRate;            //절반 체력 드랍 확률

    public RewardData(RewardType rewardType, float normalRelicRate, float purifiedRelicRate, float sourceRelicRate, float halfHpRate)
    {
        this.rewardType = rewardType;
        this.normalRelicRate = normalRelicRate;
        this.purifiedRelicRate = purifiedRelicRate;
        this.sourceRelicRate = sourceRelicRate;
        this.halfHpRate = halfHpRate;
    }
}
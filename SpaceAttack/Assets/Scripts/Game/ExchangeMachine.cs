using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExchangeMachine : MonoBehaviour
{
    [SerializeField] TextMeshPro remainCountUI;
    [SerializeField] TextMeshPro stateUI;
    [SerializeField] TextMeshPro rewardUI;

    const int maxUseCount = 5;
    const float detectDistance = 3f;
    int currentUseCount = 1;
    GameObject player;

    public RelicSO targetRelicSO
    {
        set 
        {
            _targetRelicSO = value;
            DropRandomReward(); 
        }
    }

    private RelicSO _targetRelicSO;

    void Start()
    {
        stateUI.text = "";
        rewardUI.text = "";

        player = PlayerStatus.Instance.gameObject;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance <= detectDistance)
        {
            if(string.IsNullOrEmpty(stateUI.text))
                stateUI.text = "F키를 눌러서 상호작용";

            if (currentUseCount > maxUseCount)
            {
                stateUI.text = "사용제한 초과";
                UnUse();
                return;
            }

            if (PlayerInputController.interactionAction.triggered)  //상호작용
                StartUse();
        }
        else
        {
            UnUse();
        }
    }

    void StartUse()   //교환기 사용 상태로 전환
    {
        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.ChangeSlotClickType(SlotClickType.Exchanger);
        else LogUtil.LogError("PlayerUIManager를 찾을 수 없습니다.");

        remainCountUI.text = $"{currentUseCount}/{maxUseCount}";
        stateUI.text = "유물 선택 대기중...";
        rewardUI.text = "";
    }

    void UnUse()    //교환기 사용 상태 취소
    {
        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.ChangeSlotClickType(SlotClickType.Item);
        else LogUtil.LogError("PlayerUIManager를 찾을 수 없습니다.");

        remainCountUI.text = "";
        stateUI.text = "";
        rewardUI.text = "";
    }

    void DropRandomReward()  //랜덤으로 보상 드랍
    {
        float randomValue = UnityEngine.Random.value;

        if (randomValue <= 0.7f)      //성공 : 투입된 유물을 소모하여 오염된 프로세스 1개 획득
        {
            rewardUI.text = "성공!";
            UISoundManager.PlaySuccessExchange();     //교환 성공 사운드 재생

            RelicSO relic = GetRelicRandomly();
            DropRelic(relic, transform.position - transform.forward * 0.5f);

            LogUtil.Log($"유물 이름: {relic.relicName}");

        }
        else if (randomValue <= 0.9f)   //대성공 : 투입된 유물을 소모하여 오염된 프로세스 2개 획득
        {
            rewardUI.text = "대성공!!!";

            RelicSO relic = GetRelicRandomly();
            RelicSO relic2 = GetRelicRandomly();
            DropRelic(relic, transform.position - transform.forward * 0.5f);
            DropRelic(relic2, transform.position - transform.forward * 0.4f);

            LogUtil.Log($"유물들 이름: {relic.relicName}, {relic2.relicName}");
        }
        else        //파괴 : 투입한 유물을 소모하여 아무것도 획득 불가
        {
            rewardUI.text = "실패";
            UISoundManager.PlayFailExchange();     //교환 실패 사운드 재생
        }

        currentUseCount++;
    }

    RelicSO GetRelicRandomly()
    {
        RelicSO[] relicSOs = DataManager.instance._RelicDatabase.GetRelicsByType(_targetRelicSO.relicType);
        int randomValue = UnityEngine.Random.Range(0, relicSOs.Length);
        LogUtil.Log($"유물 타입: {_targetRelicSO.relicType}, 뽑힌 유물 순서: {randomValue}, 대상 유물 개수: {relicSOs.Length}");
        RelicSO relic = DataManager.instance._RelicDatabase.GetRelicByIndex(randomValue);  //받은 유물중, 랜덤 유물 리스트index의 유물 받기

        return relic;
    }

    void DropRelic(RelicSO relic, Vector3 dropPos)
    {
        UISoundManager.PlayDropItem();     //아이템 드랍 사운드

        GameObject temp = DataManager.instance._relicObject;
        GameObject relicObj = Instantiate(temp, dropPos, temp.transform.rotation);

        relicObj.GetComponent<BaseRelic>().Initialize(relic.relicID, relic.relicName, relic.iconSprite); //생성한 유물 오브젝트에 유물정보 갱신
    }
}

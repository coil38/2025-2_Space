using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Purifier : MonoBehaviour
{
    [SerializeField] TextMeshPro remainCountUI;
    [SerializeField] TextMeshPro stateUI;
    [SerializeField] TextMeshPro rewardUI;

    const int maxUseCount = 3;
    const float detectDistance = 1.5f;
    int currentUseCount = 1;
    GameObject player;

    public RelicSO targetRelicSO
    {
        set
        {
            _targetRelicSO = value;
            PurifyRelic();
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
            if (string.IsNullOrEmpty(stateUI.text))
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
    }

    void UnUse()    //교환기 사용 상태 취소
    {
        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.ChangeSlotClickType(SlotClickType.Item);
        else LogUtil.LogError("PlayerUIManager를 찾을 수 없습니다.");

        remainCountUI.text = "";
        stateUI.text = "";
    }

    void PurifyRelic()  //랜덤으로 보상 드랍
    {
        RelicSO[] relicSOs = DataManager.instance._RelicDatabase.GetRelicsByType(RelicType.PurifiedRelic);

        foreach (var relicSO in relicSOs)
            if (relicSO.relicID == _targetRelicSO.pair)
            {
                rewardUI.text = "정화 성공";
                DropRelic(relicSO, transform.position - transform.forward * 0.5f);
            }

        currentUseCount++;
    }

    void DropRelic(RelicSO relic, Vector3 dropPos)
    {
        GameObject temp = DataManager.instance._relicObject;
        GameObject relicObj = Instantiate(temp, dropPos, temp.transform.rotation);

        relicObj.GetComponent<BaseRelic>().Initialize(relic.relicID, relic.relicName, relic.iconSprite); //생성한 유물 오브젝트에 유물정보 갱신
    }

    public bool CheckPair(RelicSO relic)
    {
        if(relic.pair == -1) rewardUI.text = "정화 대상 아이템X";

        return relic.pair != -1;
    }
}

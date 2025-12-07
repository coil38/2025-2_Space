using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChast : MonoBehaviour
{
    [SerializeField] RewardType type = RewardType.RewardBox;
    [SerializeField] float detectDis = 2; 

    private bool isOneTime = false;

    void Update()
    {
        float dis = Vector3.Distance(transform.position, PlayerStatus.Instance.transform.position);
        if (dis <= detectDis && !isOneTime)
        {
            StartCoroutine(OpenChest());
            isOneTime = true;
        }
    }

    IEnumerator OpenChest()
    {
        float openTime = 0.2f;
        float waitDisableTime = 1f;

        UISoundManager.PlayOpenRewardChast();     //상자 열기 사운드 재생
        yield return new WaitForSeconds(openTime);

        RewardSystem.DropRewards(type, transform.position + Vector3.back * 0.3f);
        yield return new WaitForSeconds(waitDisableTime);

        UISoundManager.PlayDisableRewardChast();   //상자 사라지는 사운드 재생
        gameObject.SetActive(false);
    }

    public void ChangeRewardType(RewardType type)     //보상 타입 변경함수
    {
        this.type = type;
    }
}

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
            RewardSystem.DropRewards(type, transform.position + Vector3.back * 0.3f);

            isOneTime = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_ChanceToHeal : RelicEffectDecorator
{
    public override void SetID() { relicEffectID = 101; }

    public override void Operater(bool isEquip)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip);

        //내부 구현
        if (isEquip)
        {
            PlayerStatus.playerHitEvent += ChanceToHeal;
        }
        else
        {
            PlayerStatus.playerHitEvent -= ChanceToHeal;
        }
        LogUtil.Log("회복패시브 획득");
    }

    //아마 피격Event사용하지 않을까? 장착, 또는 해제 --> 구독 또는 해지
    private void ChanceToHeal()  //피격 받았을 때, 일정확률로 체력회복 내부코드
    {
        float randomValue = Random.value;
        LogUtil.Log($"회복확률:{relicInfo.n}, 회복값: {relicInfo.z}, 랜덤값; {randomValue}");
        if (randomValue <= relicInfo.n)
        {
            LogUtil.Log("체력회복 성공");

            PlayerStatus.AddHp(relicInfo.z);  //플레이어 체력 회복
        }
        else
        {
            LogUtil.Log("체력회복 실패");
        }
    }
}

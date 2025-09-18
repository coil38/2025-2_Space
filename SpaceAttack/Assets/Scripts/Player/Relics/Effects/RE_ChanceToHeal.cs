using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_ChanceToHeal : RelicEffectDecorator
{
    public override void SetID() { relicID = 101; }

    public override void Operater(bool isEquip, RelicInfo info)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip, info);

        //내부 구현
        if (isEquip)
        {
            LogUtil.Log("회복패시브 획득");
        }
        else
        {
            LogUtil.Log("회복패시브 획득 해제");
        }
    }

    //아마 피격Event사용하지 않을까? 장착, 또는 해제 --> 구독 또는 해지
    private void ChanceToHeal()  //피격 받았을 때, 일정확률로 체력회복 내부코드
    {

    }
}

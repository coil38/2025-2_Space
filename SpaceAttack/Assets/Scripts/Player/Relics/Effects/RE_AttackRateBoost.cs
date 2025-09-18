using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_AttackRateBoost : RelicEffectDecorator
{
    public override void SetID() { relicID = 100; }
    
    public override void Operater(bool isEquip, RelicInfo info)
    {
        if(relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip, info);

        //내부 구현
        if (isEquip)
        {
            //공격력 상승
            LogUtil.Log("공격력 상승");
        }
        else
        {
            //공격력 상승 취소
            LogUtil.Log("공격력 상승효과 취소");
        }
    }
}

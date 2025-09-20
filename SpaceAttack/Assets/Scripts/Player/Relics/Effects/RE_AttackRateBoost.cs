using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_AttackRateBoost : RelicEffectDecorator
{
    public override void SetID() { relicEffectID = 100; }
    
    public override void Operater(bool isEquip)
    {
        if(relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip);

        //내부 구현
        if (isEquip)
        {
            //공격력 상승
            EventManager.playerEvent.SetRelicAttackValue(isEquip, relicInfo.n);
            LogUtil.Log("공격력 상승 + 정보 번호" + relicInfo.id);
        }
        else
        {
            //공격력 상승 취소
            EventManager.playerEvent.SetRelicAttackValue(isEquip, relicInfo.n);
            LogUtil.Log("공격력 상승효과 취소");
        }
    }
}

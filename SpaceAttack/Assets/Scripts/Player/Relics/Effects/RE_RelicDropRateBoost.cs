using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_RelicDropRateBoost : RelicEffectDecorator
{
    public override void SetID() { relicEffectID = 106; }
    public override void Operater(bool isEquip)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip);

        if (isEquip)
        {
            //칩셋 드롭확률 상승
            LogUtil.Log("칩셋 드롭확률 상승");
        }
        else
        {
            LogUtil.Log("칩셋 드롭확률 상승 해제");
        }
    }
}

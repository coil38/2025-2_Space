using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_EvasionRateBoost : RelicEffectDecorator
{
    public override void SetID() { relicEffectID = 104; }
    public override void Operater(bool isEquip)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip);

        if (isEquip)
        {
            PlayerStatus.missRate *= (1 + relicInfo.n);  //회피율 상승
            LogUtil.Log($"회피율 상승, 현재 회피율: {PlayerStatus.missRate}");
        }
        else
        {
            PlayerStatus.missRate /= (1 + relicInfo.n);
            LogUtil.Log($"회피율 상승 해제, 현재 회피율: {PlayerStatus.missRate}");
        }
    }
}

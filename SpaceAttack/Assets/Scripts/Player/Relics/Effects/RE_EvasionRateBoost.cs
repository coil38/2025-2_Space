using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_EvasionRateBoost : RelicEffectDecorator
{
    public override void SetID() { relicID = 104; }
    public override void Operater(bool isEquip, RelicInfo info)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip, info);

        if (isEquip)
        {
            PlayerStatus.missRate *= (1 + info.n);  //회피율 상승
            LogUtil.Log($"회피율 상승, 현재 회피율: {PlayerStatus.missRate}");
        }
        else
        {
            PlayerStatus.missRate /= (1 + info.n);
            LogUtil.Log($"회피율 상승 해제, 현재 회피율: {PlayerStatus.missRate}");
        }
    }
}

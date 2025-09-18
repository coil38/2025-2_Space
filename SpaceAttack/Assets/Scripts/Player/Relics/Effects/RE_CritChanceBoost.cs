using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_CritChanceBoost : RelicEffectDecorator
{
    public override void SetID() { relicID = 102; }
    public override void Operater(bool isEquip, RelicInfo info)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip, info);

        //내부 구현
        if (isEquip)
        {
            PlayerStatus.criticalChanceRate *= (1 + info.n);
            LogUtil.Log($"치명타율 상승, 현재 치명타율: {PlayerStatus.criticalChanceRate}");
        }
        else
        {
            PlayerStatus.criticalChanceRate /= (1 + info.n);
            LogUtil.Log($"치명타율 상승 해제, 현재 치명타율: {PlayerStatus.criticalChanceRate}");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_UnstableOperator : BaseRelic   //불안정 연산자
{
    private RelicEffectDecorator relicEffect;

    public override int relicId { get; protected set; }
    RelicInfo critChanceUpInfo;
    public void OnEnable()
    {
        relicId = 1002;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.GetRelicEffect(RelicEffectManager._critChanceUpId);
        }

        if (critChanceUpInfo == null)
        {
            critChanceUpInfo = infos[0];
        }
        relicEffect.SetInfo(critChanceUpInfo);
        relicEffect.Operater(isEquip);
    }
}

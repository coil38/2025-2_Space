using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_EmergencyRecoveryMatrix : BaseRelic  //비상회복 메트릭스
{
    private RelicEffectDecorator relicEffect;
    public override int relicId { get; protected set; }
    RelicInfo healChanceInfo;
    public void OnEnable()
    {
        relicId = 1001;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.GetRelicEffect(RelicEffectManager._healChanceUpId);
        }

        if (healChanceInfo == null)
        {
            healChanceInfo = infos[0];
        }
        relicEffect.SetInfo(healChanceInfo);
        relicEffect.Operater(isEquip);
    }
}

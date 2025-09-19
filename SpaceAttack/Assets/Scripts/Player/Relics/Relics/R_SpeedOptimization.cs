using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_SpeedOptimization : BaseRelic  //속도 최적화
{
    private RelicEffectDecorator relicEffect;

    public override int relicId { get; protected set; }
    RelicInfo speedUpInfo;
    public void OnEnable()
    {
        relicId = 1004;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.GetRelicEffect(RelicEffectManager._speedUpId);
        }

        if (speedUpInfo == null)
        {
            speedUpInfo = infos[0];
        }
        relicEffect.SetInfo(speedUpInfo);
        relicEffect.Operater(isEquip);
    }
}

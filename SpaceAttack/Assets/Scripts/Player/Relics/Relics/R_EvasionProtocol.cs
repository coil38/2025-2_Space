using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_EvasionProtocol : BaseRelic  //회피 프로토콜
{
    private RelicEffectDecorator relicEffect;

    public override int relicId { get; protected set; }
    RelicInfo misChanceUpInfo;
    public void OnEnable()
    {
        relicId = 1005;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.GetRelicEffect(RelicEffectManager._misChanceUpId);
        }

        if (misChanceUpInfo == null)
        {
            misChanceUpInfo = infos[0];
        }
        relicEffect.SetInfo(misChanceUpInfo);
        relicEffect.Operater(isEquip);
    }
}

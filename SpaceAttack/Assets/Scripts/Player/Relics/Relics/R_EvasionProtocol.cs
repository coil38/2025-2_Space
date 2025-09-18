using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_EvasionProtocol : BaseRelic  //회피 프로토콜
{
    private RelicEffectDecorator relicEffect;

    public override int relicId { get; protected set; }
    RelicInfo info;
    public void OnEnable()
    {
        relicId = 1005;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.EvasionUp();
        }

        foreach (var temp in infos)
        {
            if (relicEffect.relicID == temp.id)
            {
                info = temp;
            }
        }
        relicEffect.Operater(isEquip, info);
    }
}

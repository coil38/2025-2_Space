using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_ProcessAnalyzer : BaseRelic  //프로세스 분석기
{
    private RelicEffectDecorator relicEffect;

    public override int relicId { get; protected set; }
    RelicInfo info;
    public void OnEnable()
    {
        relicId = 1006;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.RelicDropUp();
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

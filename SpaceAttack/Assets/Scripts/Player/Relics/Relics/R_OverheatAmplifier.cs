using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_OverheatAmplifier : BaseRelic  //연산 과열 증폭기
{
    private RelicEffectDecorator relicEffect;

    public override int relicId { get; protected set; }
    RelicInfo info;
    public void OnEnable()
    {
        relicId = 1003;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.Cdr();
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_SpeedOptimization : BaseRelic  //속도 최적화
{
    private RelicEffectDecorator relicEffect;

    public override int relicId { get; protected set; }
    RelicInfo info;
    public void OnEnable()
    {
        relicId = 1004;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.MoveSpeedUp();
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

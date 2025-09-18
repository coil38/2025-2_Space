using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_EmergencyRecoveryMatrix : BaseRelic  //비상회복 메트릭스
{
    private RelicEffectDecorator relicEffect;
    public override int relicId { get; protected set; }
    RelicInfo info;
    public void OnEnable()
    {
        relicId = 1001;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.HealChance();
        }
        if (infos == null)
        {
            LogUtil.LogError("RelicInfo인스턴스가 존재하지 않습니다.");
            return;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_SuperComputer : BaseRelic  //슈퍼 컴퓨터
{
    private RelicEffectDecorator attckEft;
    private RelicEffectDecorator criChanceUpEft;
    private RelicEffectDecorator coolDownEft;
    private RelicEffectDecorator speedUpEft;
    private RelicEffectDecorator misChanceUpEft;

    private RelicInfo attackInfo;
    private RelicInfo criInfo;
    private RelicInfo coolDownInfo;
    private RelicInfo speedUpInfo;
    private RelicInfo misChanceInfo;

    public override int relicId { get; protected set; }
    RelicInfo info;
    public void OnEnable()
    {
        relicId = 1100;
    }

    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (attckEft == null || criChanceUpEft == null || coolDownEft == null || speedUpEft == null || misChanceUpEft == null)
        {
            attckEft = RelicEffectManager.GetRelicEffect(RelicEffectManager._atkUpId);
            criChanceUpEft = RelicEffectManager.GetRelicEffect(RelicEffectManager._critChanceUpId);
            coolDownEft = RelicEffectManager.GetRelicEffect(RelicEffectManager._coolDownId);
            speedUpEft = RelicEffectManager.GetRelicEffect(RelicEffectManager._speedUpId);
            misChanceUpEft = RelicEffectManager.GetRelicEffect(RelicEffectManager._misChanceUpId);
        }

        if (attackInfo == null || criInfo == null || coolDownInfo == null || speedUpInfo == null || misChanceInfo == null)
        {
            foreach (var info in infos)
            {
                if (info.id == RelicEffectManager._atkUpId) attackInfo = info;
                else if (info.id == RelicEffectManager._critChanceUpId) criInfo = info;
                else if (info.id == RelicEffectManager._coolDownId) coolDownInfo = info;
                else if (info.id == RelicEffectManager._speedUpId) speedUpInfo = info;
                else if (info.id == RelicEffectManager._misChanceUpId) misChanceInfo = info;
            }
        }
        attckEft.SetInfo(attackInfo);
        criChanceUpEft.SetInfo(criInfo);
        coolDownEft.SetInfo(coolDownInfo);
        speedUpEft.SetInfo(speedUpInfo);
        misChanceUpEft.SetInfo(misChanceInfo);

        attckEft.Set(criChanceUpEft.Set(coolDownEft.Set(speedUpEft.Set(misChanceUpEft))));

        attckEft.Operater(isEquip);
    }
}

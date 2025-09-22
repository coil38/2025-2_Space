using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_MoveSpeedBoost : RelicEffectDecorator
{
    public override void SetID() { relicEffectID = 105; }
    public override void Operater(bool isEquip)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip);

        if (isEquip)
        {
            PlayerStatus.m_speed += (PlayerStatus.m_defultSpeed * relicInfo.n);  //이속 상승
            LogUtil.Log($"이속 상승, 현재 이속: {PlayerStatus.m_speed}");
        }
        else
        {
            PlayerStatus.m_speed -= (PlayerStatus.m_defultSpeed * relicInfo.n);

            LogUtil.Log($"이속 상승 해제, 현재 이속: {PlayerStatus.m_speed}");
        }
    }
}

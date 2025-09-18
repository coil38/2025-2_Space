using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_MoveSpeedBoost : RelicEffectDecorator
{
    public override void SetID() { relicID = 105; }
    public override void Operater(bool isEquip, RelicInfo info)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip, info);

        if (isEquip)
        {
            PlayerStatus.m_speed += (PlayerStatus.m_speed * info.n);  //이속 상승
            LogUtil.Log($"이속 상승, 현재 이속: {PlayerStatus.m_speed}");
        }
        else
        {
            PlayerStatus.m_speed -= (PlayerStatus.m_speed * info.n);
            LogUtil.Log($"이속 상승 해제, 현재 이속: {PlayerStatus.m_speed}");
        }
    }
}

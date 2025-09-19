using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RE_SkillCoolDecrease : RelicEffectDecorator
{
    public override void SetID() { relicEffectID = 107; }
    public override void Operater(bool isEquip)
    {
        if (relicEffectDecoComponent != null)
            relicEffectDecoComponent.Operater(isEquip);

        if (isEquip)
        {
            //스킬 쿨타임 감소
            LogUtil.Log("칩스킬 쿨타임 감소");
        }
        else
        {
            LogUtil.Log("스킬 쿨타임 감소 해제");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicEffectType
{
    protected RelicInfo relicInfo { get; private set; }
    public virtual void Excute(bool isEquip, RelicInfo info)
    {
        relicInfo = info;
    }
}

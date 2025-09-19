using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RelicEffectDecorator : REDecoComponent
{
    protected REDecoComponent relicEffectDecoComponent;
    protected RelicInfo relicInfo;
    public int relicEffectID {  get; protected set; }

    public RelicEffectDecorator Set(REDecoComponent _relicEffectDecoComponent)
    {
        relicEffectDecoComponent = _relicEffectDecoComponent;
        return this;
    }

    public override void SetInfo(RelicInfo info)
    {
        relicInfo = info;
    }

    public void ResetDeco()   //초기화용 함수
    {
        relicEffectDecoComponent = null;
    }
}

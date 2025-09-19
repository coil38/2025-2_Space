using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_OverloadAlgorithm : BaseRelic   //알고리즘 과부화
{
    private RelicEffectDecorator relicEffect;

    public override int relicId { get; protected set; }
    RelicInfo attackInfo;

    public void OnEnable()
    {
        relicId = 1000;
    }
    public override void SetEffect(bool isEquip, RelicInfo[] infos)
    {
        if (relicEffect == null)
        {
            relicEffect = RelicEffectManager.GetRelicEffect(RelicEffectManager._atkUpId);
        }

        if (attackInfo == null)
        {
            attackInfo = infos[0];
        }
        relicEffect.SetInfo(attackInfo);
        relicEffect.Operater(isEquip);
    }

    //public RelicInfo[] ReOrderRelicInfos()  //위 코드들은 임시 ( 재정렬후, 여러번 Operater가 실행될 때마다, 하나씩 넘기는 형식 사용
    //{

    //}
}

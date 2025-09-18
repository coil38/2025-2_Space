using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicEffectManager : MonoBehaviour
{
    private static Dictionary<int, RelicEffectDecorator> relicsEffects = new Dictionary<int, RelicEffectDecorator>();

    public void Initialize()  //초기화 함수
    {
        RelicEffectDecorator[] relicComps = GetComponents<RelicEffectDecorator>();
        foreach (var relic in relicComps)
        {
            relic.SetID();  //각 유물효과 ID 설정
            relicsEffects.Add(relic.relicID, relic);
        }
    }
    public static RelicEffectDecorator AtkPercentUp()
    {
        return GetRelicEffect(100);
    }
    public static RelicEffectDecorator HealChance()
    {
        return GetRelicEffect(101);
    }
    public static RelicEffectDecorator CritChanceUp()
    {
        return GetRelicEffect(102);
    }
    public static RelicEffectDecorator Cdr()
    {
        return GetRelicEffect(107);
    }
    public static RelicEffectDecorator MoveSpeedUp()
    {
        return GetRelicEffect(105);
    }
    public static RelicEffectDecorator EvasionUp()
    {
        return GetRelicEffect(104);
    }
    public static RelicEffectDecorator RelicDropUp()
    {
        return GetRelicEffect(106);
    }

    public static RelicEffectDecorator GetRelicEffect(int id)
    {
        if (relicsEffects.Count <= 0)
        {
            LogUtil.LogError("유물효과 초기설정이 완료되지 않았습니다.");
            return null;
        }
        if (relicsEffects.TryGetValue(id, out RelicEffectDecorator effect))
        {
            return effect;
        }
        return null;
    }
}

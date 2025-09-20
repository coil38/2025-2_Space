using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicEffectDatabase", menuName = "RelicEffect/Database")]
public class RelicEffectDatabaseSO : ScriptableObject
{
    public List<RelicEffectSO> relicEffects = new List<RelicEffectSO>();

    private Dictionary<int, RelicEffectSO> relicEffectById;

    public void Initialize()
    {
        relicEffectById = new Dictionary<int, RelicEffectSO>();
        foreach (var relicEffect in relicEffects)
        {
            relicEffectById[relicEffect.relicEffectId] = relicEffect;
        }
    }

    public RelicEffectSO GetRelicEffect(int _relicEffectId)   //Id로 유물효과찾기
    {
        if (relicEffectById == null)
            Initialize();
        if (relicEffectById.TryGetValue(_relicEffectId, out RelicEffectSO relicEffect))
            return relicEffect;

        return null;
    }
}

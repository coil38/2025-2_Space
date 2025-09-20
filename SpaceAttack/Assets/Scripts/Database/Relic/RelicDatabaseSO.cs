using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicDatabase", menuName = "Relic/Database")]
public class RelicDatabaseSO : ScriptableObject
{
    public List<RelicSO> relics = new List<RelicSO>();
    public RelicEffectDatabaseSO relicEffectDatabase;

    private Dictionary<int, RelicSO> relicById;

    public void Initialize()
    {
        relicById = new Dictionary<int, RelicSO>();
        foreach (var relic in relics)
        {
            relicById[relic.relicID] = relic;
        }
    }

    public RelicSO GetRelic(int _relicId)   //Id로 유물찾기
    {
        if (relicById == null)
            Initialize();
        if (relicById.TryGetValue(_relicId, out RelicSO relic))
            return relic;

        return null;
    }
}

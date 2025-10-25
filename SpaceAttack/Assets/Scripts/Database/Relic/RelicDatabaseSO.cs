using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicDatabase", menuName = "Relic/Database")]
public class RelicDatabaseSO : ScriptableObject
{
    public List<RelicSO> relics = new List<RelicSO>();
    public RelicEffectDatabaseSO relicEffectDatabase;

    private Dictionary<int, RelicSO> relicById;

    private RelicSO[] normalRelics;
    private RelicSO[] purifiedRelics;
    private RelicSO[] sourceRelics;

    public void Initialize()
    {
        relicById = new Dictionary<int, RelicSO>();
        foreach (var relic in relics)
            relicById[relic.relicID] = relic;
    }

    private void InitializeRelicTypeDatas(RelicType relicType)
    {
        var temps = new List<RelicSO>();
        foreach (var rel in relics)
        {
            if (rel.relicType == relicType)
            {
                temps.Add(rel);
                LogUtil.Log($"유물 대상: {rel.relicName}");
            }
        }
        LogUtil.Log($"유물 리스트 존재여부: {relics != null}");

        switch (relicType)
        {
            case RelicType.NormalRelic:
                normalRelics = temps.ToArray(); break;
            case RelicType.PurifiedRelic:
                purifiedRelics = temps.ToArray(); break;
            case RelicType.SourceRelic:
                sourceRelics = temps.ToArray(); break;
        }
    }

    public RelicSO GetRelicById(int _relicId)   //Id로 유물찾기
    {
        if (relicById == null)
            Initialize();
        if (relicById.TryGetValue(_relicId, out RelicSO relic))
            return relic;

        return null;
    }
    public RelicSO[] GetRelicsByType(RelicType relicType)    //타입에 맞는 유물들 받기 함수
    {
        switch (relicType)
        {
            case RelicType.NormalRelic: 
                if(normalRelics == null || normalRelics.Length <= 0) 
                    InitializeRelicTypeDatas(relicType);
                return normalRelics;

            case RelicType.PurifiedRelic:
                if (purifiedRelics == null || purifiedRelics.Length <= 0)
                    InitializeRelicTypeDatas(relicType);
                return purifiedRelics;

            case RelicType.SourceRelic:
                if (sourceRelics == null || sourceRelics.Length <= 0)
                    InitializeRelicTypeDatas(relicType);
                return sourceRelics;
        }
        return null;
    }

    public int GetRelicCount()
    {
        return relics.Count;
    }

    public RelicSO GetRelicByIndex(int index)
    {
        return relics[index];
    }
}

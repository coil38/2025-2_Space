using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LevelMonsterType
{
    NormalOnly,
    EliteOnly,
    SpecificOnly,
    Mixed
}

[Serializable]
public class MonsterSpawnPool
{
    public GameObject[] monsters;
    public float weight = 1f;  
}

public class LevelConfig : MonoBehaviour
{
    public LevelMonsterType monsterType;

    public MonsterSpawnPool normalPool;
    public MonsterSpawnPool elitePool;
    public GameObject[] specificMonsters;

    [Header("몬스터 타입 등장 확률")]
    [Range(0f, 1f)] public float normalTypeChance = 0.5f;
    [Range(0f, 1f)] public float eliteTypeChance = 0.3f;
    [Range(0f, 1f)] public float specificTypeChance = 0.2f;

    private int monsterIndex;

    public void ApplyRandomConfig()
    {
        if (StageProgress.Instance != null)
        {
            if (StageProgress.Instance.unlockedStage < 2)
            {
                monsterType = LevelMonsterType.NormalOnly;
                return;
            }
        }

        float r = UnityEngine.Random.value;

        if (r <= normalTypeChance)
            monsterType = LevelMonsterType.NormalOnly;
        else if (r <= normalTypeChance + eliteTypeChance)
            monsterType = LevelMonsterType.EliteOnly;
        else
        {
            monsterType = LevelMonsterType.SpecificOnly;
            monsterIndex = UnityEngine.Random.Range(0, specificMonsters.Length);
        }
    }

    public GameObject GetRandomMonster()
    {
        switch (monsterType)
        {
            case LevelMonsterType.NormalOnly:
                return GetWeightedMonster(normalPool);

            case LevelMonsterType.EliteOnly:
                return GetWeightedMonster(elitePool);

            case LevelMonsterType.SpecificOnly:
                return specificMonsters[monsterIndex];
        }

        return null;
    }

    private GameObject GetWeightedMonster(MonsterSpawnPool pool)
    {
        if (pool.monsters == null || pool.monsters.Length == 0)
            return null;

        return pool.monsters[UnityEngine.Random.Range(0, pool.monsters.Length)];
    }
}

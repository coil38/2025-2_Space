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
    public Transform player;

    private void Start()
    {
        if (player == null)
        {
            // Player 오브젝트 자동 찾기
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    private void Update()
    {
        // 플레이어를 못 찾았으면 계속 찾기
        if (player == null)
        {
            TryFindPlayer();
        }

        if (Input.GetKeyDown(KeyCode.K) && player != null)
        {
            SpawnRandomEliteMonster();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            SpawnRandomNormalMonster();
        }
    }

    private void TryFindPlayer()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            player = p.transform;
            Debug.Log("Player Transform 연결됨");
        }
    }

    private void SpawnRandomEliteMonster()
    {
        if (player == null)
        {
            return;
        }

        if (elitePool.monsters == null || elitePool.monsters.Length == 0)
        {
            return;
        }

        GameObject prefab = elitePool.monsters[UnityEngine.Random.Range(0, elitePool.monsters.Length)];

        Vector3 spawnPos = player.position
                   + player.forward * 2f
                   + Vector3.up * 1f;

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }

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

    private void SpawnRandomNormalMonster()
    {
        if (player == null) return;
        if (normalPool.monsters == null || normalPool.monsters.Length == 0) return;

        GameObject prefab = normalPool.monsters[UnityEngine.Random.Range(0, normalPool.monsters.Length)];

        Vector3 spawnPos = player.position
                         + player.forward * 2f
                         + Vector3.up * 1.5f;

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}

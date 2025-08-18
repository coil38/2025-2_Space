using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("스테이지 몬스터 프리팹")]
    public GameObject[] monsterPrefabs;
    public Transform[] spawnPoints;

    private List<EnemyBase> aliveMonsters = new List<EnemyBase>();

    [Header("웨이브 설정")]
    public int monstersPerWave = 3;
    private int currentWave = 0;

    void Start()
    {
        //StartWave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))   //테스트용
        {
            StartWave();
        }
    }

    void StartWave()
    {
        currentWave++;
        aliveMonsters.Clear();

        for (int i = 0; i < monstersPerWave; i++)
        {
            Transform spawn = spawnPoints[i % spawnPoints.Length];

            // 배열에서 랜덤 몬스터 선택
            GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];

            GameObject monsterObj = Instantiate(prefab, spawn.position, Quaternion.identity);
            EnemyBase monster = monsterObj.GetComponent<EnemyBase>();

            if (monster != null)
            {
                aliveMonsters.Add(monster);
                // 몬스터가 죽을 때 호출될 이벤트 등록
                monster.OnDeathAction += OnMonsterDeath;
            }
        }
    }

    private void OnMonsterDeath(EnemyBase deadMonster)
    {
        aliveMonsters.Remove(deadMonster);
        Debug.Log(aliveMonsters.Count);
        if (aliveMonsters.Count == 0)
        {
            Debug.Log($"웨이브 {currentWave} 완료!");
            // 다음 웨이브 시작
            StartWave();
        }
    }
}

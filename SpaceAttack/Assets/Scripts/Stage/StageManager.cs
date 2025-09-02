using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI Text 사용


public class StageManager : MonoBehaviour
{
    [Header("스테이지 몬스터 프리팹")]
    public GameObject[] monsterPrefabs;

    [Header("UI")]
    public Text countdownText; // 화면에 표시할 Text

    private List<EnemyBase> aliveMonsters = new List<EnemyBase>();
    private GameObject[] planObjects;

    [Header("웨이브 설정")]
    public int monstersPerWave = 3;
    private int currentWave = 0;

    [Header("대기 시간")]
    public float startDelay = 4f;  // 스테이지 시작 전 대기
    public float nextWaveDelay = 8f; // 웨이브 완료 후 대기

    private void Start()
    {
        planObjects = GameObject.FindGameObjectsWithTag("Plan");
        StartCoroutine(StageStartDelay());
    }

    // 스테이지 시작 전 대기
    private IEnumerator StageStartDelay()
    {
        float timer = startDelay;
        while (timer > 0f)
        {
            if (countdownText != null)
                countdownText.text = "시작까지: " + Mathf.Ceil(timer).ToString();

            yield return null;
            timer -= Time.deltaTime;
        }

        if (countdownText != null)
            countdownText.text = "";

        StartWave();
    }

    void StartWave()
    {
        currentWave++;
        aliveMonsters.Clear();

        for (int i = 0; i < monstersPerWave; i++)
        {
            GameObject plan = planObjects[Random.Range(0, planObjects.Length)];
            Renderer renderer = plan.GetComponent<Renderer>();
            if (renderer == null) continue;

            Bounds bounds = renderer.bounds;
            Vector3 randomPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
            GameObject monsterObj = Instantiate(prefab, randomPos, Quaternion.identity);
            EnemyBase monster = monsterObj.GetComponent<EnemyBase>();

            if (monster != null)
            {
                aliveMonsters.Add(monster);
                monster.OnDeathAction += OnMonsterDeath;
            }
        }
    }

    private void OnMonsterDeath(EnemyBase deadMonster)
    {
        aliveMonsters.Remove(deadMonster);
        if (aliveMonsters.Count == 0)
        {
            Debug.Log($"웨이브 {currentWave} 완료!");
            StartCoroutine(NextWaveDelay());
        }
    }

    // 웨이브 종료 후 다음 웨이브 전 대기
    private IEnumerator NextWaveDelay()
    {
        float timer = nextWaveDelay;
        while (timer > 0f)
        {
            if (countdownText != null)
                countdownText.text = "다음 웨이브까지: " + Mathf.Ceil(timer).ToString();

            yield return null;
            timer -= Time.deltaTime;
        }

        if (countdownText != null)
            countdownText.text = "";

        StartWave();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))   // 테스트용
        {
            StartWave();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI Text 사용


public class StageManager : MonoBehaviour
{
    [Header("스테이지 몬스터 프리팹")]
    public GameObject[] monsterPrefabs;

    [Header("UI")]
    public Text countdownText;

    [Header("레벨 반복")]
    public int maxLevelRepeat = 4;     // 한 레벨 반복 횟수
    private int currentLevelRepeat = 0; // 현재 레벨 반복 카운트

    [Header("게임 종료 씬")]
    public string endSceneName = "ChipsetSelectScene";
    public float endSceneDelay = 3f;  // 마지막 후 대기 시간

    private List<EnemyBase> aliveMonsters = new List<EnemyBase>();
    private GameObject[] planObjects;
    float margin = 3.5f;
    private bool playerDeathHandled = false;

    [Header("웨이브 설정")]
    public int monstersPerWave = 3;
    private int currentWave = 0;
    public int maxWaveCount = 2;

    [Header("레벨 설정")]
    public GameObject levelPrefab;
    public GameObject currentLevel;

    [Header("대기 시간")]
    public float startDelay = 3f;
    public float nextWaveDelay = 6f;

    private void Start()
    {
        currentLevel = GameObject.FindWithTag("Level");
        if (currentLevel == null && levelPrefab != null)
        {
            currentLevel = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
            currentLevel.tag = "Level";
        }

        planObjects = GameObject.FindGameObjectsWithTag("Plan");
        StartCoroutine(StageStartDelay());
    }

    private void Update()
    {
        if (!playerDeathHandled && PlayerStatus.Instance != null && PlayerStatus.Instance.isDead)
        {
            playerDeathHandled = true;
            StartCoroutine(ReturnToChipsetScene());
        }
    }

    private IEnumerator ReturnToChipsetScene()
    {
        if (countdownText != null)
            countdownText.text = "플레이어가 사망했습니다... 칩셋 화면으로 돌아갑니다.";

        yield return new WaitForSeconds(5f); // 연출용 대기 시간 (3초)
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 1f);

        UnityEngine.SceneManagement.SceneManager.LoadScene("ChipsetSelectScene");

         if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 0f);
       
        SaveManager.instance.PlayerReset();
    }

    private IEnumerator StageStartDelay()
    {
        float timer = startDelay;
        while (timer > 0f)
        {
            if (countdownText != null)
                countdownText.text = "시작까지: " + Mathf.Ceil(timer);
            yield return null;
            timer -= Time.deltaTime;
        }

        if (countdownText != null) countdownText.text = "";
        StartWave();
    }

    public void StartWave()
    {
        currentWave++;
        aliveMonsters.Clear();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = playerObj != null ? playerObj.transform.position : Vector3.zero;

        float minSpawnDistance = 5f;

        for (int i = 0; i < monstersPerWave; i++)
        {
            GameObject plan = planObjects[Random.Range(0, planObjects.Length)];
            Renderer renderer = plan.GetComponent<Renderer>();
            if (renderer == null) continue;

            Bounds bounds = renderer.bounds;
            Vector3 randomPos;
            int safetyCount = 0;
            do
            {
                randomPos = new Vector3(
     Random.Range(bounds.min.x + margin, bounds.max.x - margin),
     bounds.max.y,
     Random.Range(bounds.min.z + margin, bounds.max.z - margin)
 );
                safetyCount++;
                if (safetyCount > 30) break;
            }
            while (Vector3.Distance(randomPos, playerPos) < minSpawnDistance);

            GameObject prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Length)];
            GameObject monsterObj = Instantiate(prefab, randomPos, prefab.transform.rotation);
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

            if (currentWave >= maxWaveCount)
                StartCoroutine(OpenNextMap());
            else
                StartCoroutine(NextWaveDelay());
        }
    }

    private IEnumerator NextWaveDelay()
    {
        float timer = nextWaveDelay;
        while (timer > 0f)
        {
            if (countdownText != null)
                countdownText.text = "다음 웨이브까지: " + Mathf.Ceil(timer);
            yield return null;
            timer -= Time.deltaTime;
        }

        if (countdownText != null) countdownText.text = "";
        StartWave();
    }

    private IEnumerator OpenNextMap()
    {
        currentLevelRepeat++;

        // 마지막 반복 →다른 씬으로 이동
        if (currentLevelRepeat >= maxLevelRepeat)
        {
            float timer = endSceneDelay; // 예: 3초
            while (timer > 0f)
            {
                if (countdownText != null)
                    countdownText.text = $"보스 출현 : {Mathf.Ceil(timer)}초전!";
                yield return null;
                timer -= Time.deltaTime;
            }

            if (countdownText != null)
                countdownText.text = "";

            UnityEngine.SceneManagement.SceneManager.LoadScene(endSceneName);
            yield break;
        }

        // 마지막 반복이 아닌 경우
        if (countdownText != null)
            countdownText.text = "통로가 열립니다...";
        yield return new WaitForSeconds(1f);

        Transform wallsParent = currentLevel.transform.Find("Walls");
        if (wallsParent == null)
        {
            Debug.LogError("Walls 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        List<Transform> wallList = new List<Transform>();
        foreach (Transform child in wallsParent)
            if (child.name.StartsWith("Wall")) wallList.Add(child);

        if (wallList.Count == 0) yield break;

        Transform chosenWall = wallList[Random.Range(0, wallList.Count)];

        Transform floor = chosenWall.Find("Floor");
        if (floor != null)
        {
            Renderer floorRenderer = floor.GetComponent<Renderer>();
            if (floorRenderer != null)
            {
                floorRenderer.enabled = true;                
                Material mat = floorRenderer.material;
                mat.color = Color.yellow;                   
            }
        }

        BoxCollider wallCollider = chosenWall.GetComponent<BoxCollider>();
        if (wallCollider == null)
            wallCollider = chosenWall.gameObject.AddComponent<BoxCollider>();
        wallCollider.isTrigger = true;

        NextStageTrigger triggerScript = chosenWall.gameObject.AddComponent<NextStageTrigger>();
        triggerScript.Setup(this, levelPrefab);
    }

    public void LoadNextLevel(Vector3 entryDirection)
    {
        GameObject[] splats = GameObject.FindGameObjectsWithTag("Splat");
        foreach (GameObject splat in splats)
            Destroy(splat);


        Vector3 spawnPos = Vector3.zero;
        if (currentLevel != null)
        {
            currentLevel.SetActive(false);
            spawnPos = currentLevel.transform.position;
        }

        GameObject newLevel = Instantiate(levelPrefab, spawnPos, Quaternion.identity);
        newLevel.SetActive(true);
        newLevel.tag = "Level";
        currentLevel = newLevel;

        planObjects = GameObject.FindGameObjectsWithTag("Plan");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Transform wallsParent = currentLevel.transform.Find("Walls");
            if (wallsParent != null)
            {
                Transform spawnWall = null;

                foreach (Transform child in wallsParent)
                {
                    string lowerName = child.name.ToLower();
                    if (entryDirection == Vector3.right && lowerName.Contains("left")) spawnWall = child;
                    else if (entryDirection == Vector3.left && lowerName.Contains("right")) spawnWall = child;
                    else if (entryDirection == Vector3.forward && (lowerName.Contains("back") || lowerName.Contains("bottom"))) spawnWall = child;
                    else if (entryDirection == Vector3.back && (lowerName.Contains("front") || lowerName.Contains("top"))) spawnWall = child;
                }
                if (spawnWall != null)
                {
                    Renderer wallRenderer = spawnWall.GetComponent<Renderer>();
                    spawnPos = spawnWall.position;

                    if (wallRenderer != null)
                    {
                        Vector3 offsetDir = entryDirection.normalized;

                        if (entryDirection == Vector3.forward || entryDirection == Vector3.back) // top/bottom
                        {
                            spawnPos.y = spawnWall.position.y - 0.5f; 
                            spawnPos.x += offsetDir.x * 2f;
                            spawnPos.z += offsetDir.z * 2f;
                        }
                        else
                        {
                            spawnPos += offsetDir * 2f;
                            spawnPos.y = currentLevel.transform.position.y + 0.5f;
                        }
                    }

                    player.transform.position = spawnPos;
                }
            }
        }
        if (player != null)
        {
            PlayerStatus ps = player.GetComponent<PlayerStatus>();
            if (ps != null)
                ps.isRooted = false; 
        }
        currentWave = 0;
        StartCoroutine(StageStartDelay());
    }

}

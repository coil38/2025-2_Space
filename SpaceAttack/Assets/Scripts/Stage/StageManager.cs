using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UI Text 사용


public class StageManager : MonoBehaviour
{

    [Header("UI")]
    public Text countdownText;

    [Header("레벨 반복")]
    public int maxLevelRepeat = 1;     // 한 레벨 반복 횟수
    private int currentLevelRepeat = 0; // 현재 레벨 반복 카운트

    [Header("게임 종료 씬")]
    public float endSceneDelay = 3f;  // 마지막 후 대기 시간

    private List<EnemyBase> aliveMonsters = new List<EnemyBase>();
    private GameObject[] planObjects;
    float margin = 12f;
    private bool playerDeathHandled = false;

    [HideInInspector]
    public int monstersPerWave = 3;
    [HideInInspector]
    private int currentWave = 0;
    [HideInInspector]
    public int maxWaveCount = 2;

    [Header("레벨 설정")]
    public GameObject levelPrefab;
    public GameObject currentLevel;

    [Header("대기 시간")]
    public float startDelay = 3f;
    public float nextWaveDelay = 6f;

    public int stageNumber;
    private float enemyHpMultiplier = 1f;
    [Header("보상 상자")]
    public GameObject rewardChestPrefab;

    [Header("스테이지 텔레포트 프리팹")]
    public GameObject stageTeleportPrefab;

    [Header("보상방 텔레포트")]
    public GameObject rewardTeleport;

    [Header("이벤트 방 확률")]
    [Range(0f, 1f)]
    public float eventRoomChance = 0.1f;

    private bool exchangeRoomUsed = false; // 교환기방 한 번만
    private bool purifierRoomUsed = false; // 정화기방 한 번만

    [Header("이벤트 방 프리팹")]
    public GameObject exchangePrefab;
    public GameObject purifierPrefab;

    private List<GameObject> spawnedEventObjects = new List<GameObject>();

    private bool nextStageOpened = false;

    private bool beforeIsSaveZon = false;

    private void Start()
    {
        stageNumber = StageGameData.SelectedStage;

        ApplyStageDifficulty(stageNumber);

        currentLevel = GameObject.FindWithTag("Level");
        if (currentLevel == null && levelPrefab != null)
        {
            currentLevel = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);
            currentLevel.tag = "Level";
        }

        LevelConfig conf = currentLevel.GetComponent<LevelConfig>();
        if (conf != null)
            conf.ApplyRandomConfig();

        planObjects = GameObject.FindGameObjectsWithTag("Plan");
        StartCoroutine(StageStartDelay());

        EventManager.relicEvent.OnStartStageEvent();     //스테이지 시작 이벤트 실행
    }

    private void Update()
    {
        if (!playerDeathHandled && PlayerStatus.Instance != null && PlayerStatus.Instance.isDead)
        {
            playerDeathHandled = true;
            StartCoroutine(ReturnToChipsetScene());
        }
    }

    private void Awake()
    {
        stageNumber = StageGameData.SelectedStage;
    }
    private IEnumerator ReturnToChipsetScene()
    {
        if (countdownText != null)
            countdownText.text = "플레이어가 사망했습니다... 칩셋 선택씬으로 돌아갑니다.";

        yield return new WaitForSeconds(5f); 
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 1f);

        SaveManager.instance.PlayerReset();

        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 0f);
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
        nextStageOpened = false;
        currentWave++;
        aliveMonsters.Clear();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPos = playerObj != null ? playerObj.transform.position : Vector3.zero;

        float minSpawnDistance = 5f;

        LevelConfig config = currentLevel.GetComponent<LevelConfig>();
        if (config == null)
        {
            Debug.LogError("현재 레벨에 LevelConfig가 없습니다!");
            return;
        }

        config.ApplyRandomConfig();

        int spawnCount = monstersPerWave;

        if (config.monsterType == LevelMonsterType.EliteOnly)    //엘리트 1마리가 아닌 엘리트 포함 나머지 일반
            spawnCount -= 2;

        for (int i = 0; i < spawnCount; i++)
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
                      plan.transform.position.y + 1.5f,
                      Random.Range(bounds.min.z + margin, bounds.max.z - margin)
                     );
                safetyCount++;

                if (Physics.CheckSphere(randomPos, 0.8f, LayerMask.GetMask("DestructableObject")))
                    continue;

                if (safetyCount > 30) break;

            } while (Vector3.Distance(randomPos, playerPos) < minSpawnDistance);

            GameObject prefab = config.GetRandomMonster();
            if (prefab == null)
            {
                Debug.LogWarning("LevelConfig에서 몬스터를 가져오지 못함!");
                continue;
            }

            if (config.monsterType == LevelMonsterType.EliteOnly)    //엘리트 1마리가 아닌 엘리트 포함 나머지 일반
                config.monsterType = LevelMonsterType.NormalOnly;

            GameObject monsterObj = Instantiate(prefab, randomPos, prefab.transform.rotation);
            EnemyBase monster = monsterObj.GetComponent<EnemyBase>();

            if (monster != null)
            {
                monster.hp *= enemyHpMultiplier;

                aliveMonsters.Add(monster);
                monster.OnDeathAction += OnMonsterDeath;
            }
        }
    }

    private void OnMonsterDeath(EnemyBase deadMonster)
    {
        aliveMonsters.Remove(deadMonster);

        if (aliveMonsters.Count == 0 && !nextStageOpened)
        {
            nextStageOpened = true; 

            if (currentWave >= maxWaveCount)
                StartCoroutine(OpenRewardRoom());
            else
                StartCoroutine(OpenNextMap());
        }
    }

    private IEnumerator OpenRewardRoom()
    {
        currentLevelRepeat++;

        if (countdownText != null)
            countdownText.text = "보상 방이 열립니다...";

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

        // 바닥 보이게 처리
        Transform floor = chosenWall.Find("Floor");
        if (floor != null)
        {
            Renderer floorRenderer = floor.GetComponent<Renderer>();
            if (floorRenderer != null)
            {
                floorRenderer.enabled = true;
                Material mat = floorRenderer.material;
                mat.color = Color.green; // 보상 방 색상
            }
        }

        BoxCollider wallCollider = chosenWall.GetComponent<BoxCollider>();
        if (wallCollider == null)
            wallCollider = chosenWall.gameObject.AddComponent<BoxCollider>();
        wallCollider.isTrigger = true;

        NextStageTrigger triggerScript = chosenWall.gameObject.AddComponent<NextStageTrigger>();
        triggerScript.SetupRewardRoom(this, levelPrefab); 
    }

    public void StartRewardCountdown(float delay = 10f)
    {
        StartCoroutine(RewardCountdownCoroutine(delay));
    }

    public IEnumerator RewardCountdownCoroutine(float delay)
    {
        float timer = delay;
        while (timer > 0f)
        {
            if (countdownText != null)
                countdownText.text = $"보상 방! {Mathf.Ceil(timer)}초 뒤 텔레포트 생성";
            yield return null;
            timer -= Time.deltaTime;
        }

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = false;

        StageProgress.Instance.ClearStage(StageGameData.SelectedStage);

        if (rewardTeleport != null)
            rewardTeleport.SetActive(true);

        if (countdownText != null)
            countdownText.text = "";
    }


    public void OnStageClear()
    {
        StageProgress.Instance.ClearStage(StageGameData.SelectedStage);

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = false;

        FadeManager.Instance.LoadScene("ChipsetSelectScene");
    }
    private IEnumerator OpenNextMap()
    {
        currentLevelRepeat++;

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

    public void LoadBossRome()    
    {
        if (PlayerStatus.Instance != null) PlayerStatus.Instance.isRooted = false;
        FadeManager.Instance.LoadScene(GameSceneManager.GetSceneNameByType(SceneType.MiddleBossScene));
    }

    public void LoadNextLevel(Vector3 entryDirection, bool spawnMonsters = true, bool isRewardRoom = false)
    {
        foreach (var obj in spawnedEventObjects)
            if (obj != null) Destroy(obj);
        spawnedEventObjects.Clear();

        GameObject[] splats = GameObject.FindGameObjectsWithTag("Splat");
        foreach (GameObject splat in splats)
            Destroy(splat);

        Vector3 spawnPos = Vector3.zero;
        if (currentLevel != null)
        {
            currentLevel.SetActive(false);
            spawnPos = currentLevel.transform.position;
        }
        bool isExchangeRoom = false;
        bool isEventRoom = false;

        if (!isRewardRoom) 
            isEventRoom = TryGetEventRoom(out isExchangeRoom);

        GameObject newLevel = Instantiate(levelPrefab, spawnPos, Quaternion.identity);

        LevelConfig config = newLevel.GetComponent<LevelConfig>();
        if (config != null)
        {
            config.ApplyRandomConfig();
        }



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

                        if (entryDirection == Vector3.forward || entryDirection == Vector3.back)
                        {
                            spawnPos.y = currentLevel.transform.position.y + 0.8f;
                            spawnPos.x += offsetDir.x * 2f;
                            spawnPos.z += offsetDir.z * 2f;
                        }
                        else
                        {
                            spawnPos += offsetDir * 2f;
                            spawnPos.y = currentLevel.transform.position.y + 0.8f;
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

        if (isEventRoom)
        {
            Debug.Log("이벤트 방 생성됨!");

            if (!beforeIsSaveZon) BGMManager.PlaySaveZone();
            beforeIsSaveZon = true;

            spawnMonsters = false;

            Vector3 center = newLevel.transform.position + Vector3.up * 0.5f;

            if (isExchangeRoom)
            {
                GameObject obj = Instantiate(exchangePrefab, center, exchangePrefab.transform.rotation);
                spawnedEventObjects.Add(obj);
                exchangeRoomUsed = true;
            }
            else
            {
                GameObject obj = Instantiate(purifierPrefab, center, purifierPrefab.transform.rotation);
                spawnedEventObjects.Add(obj);
                purifierRoomUsed = true;
            }

            StartCoroutine(OpenNextMapImmediately());
            return;
        }

        if (!isRewardRoom && spawnMonsters)
        {
            StartCoroutine(StageStartDelay());
        }

        if (isRewardRoom)
        {
            if (!beforeIsSaveZon) BGMManager.PlaySaveZone();
            beforeIsSaveZon = true;

            if (rewardChestPrefab != null)
            {
                Vector3 levelCenter = newLevel.transform.position;
                spawnPos = levelCenter + Vector3.up * 1f;        

                Instantiate(rewardChestPrefab, spawnPos, rewardChestPrefab.transform.rotation);
            }

            StartRewardCountdown(10f);
            return;
        }

        if (beforeIsSaveZon) BGMManager.PlayBattleSound();
        beforeIsSaveZon = false;
    }

    private IEnumerator OpenNextMapImmediately()
    {
        yield return new WaitForSeconds(0.5f); 

        if (countdownText != null)
            countdownText.text = "이벤트 방! 통로가 열렸습니다.";

        Transform wallsParent = currentLevel.transform.Find("Walls");
        if (wallsParent == null) yield break;

        List<Transform> wallList = new List<Transform>();
        foreach (Transform child in wallsParent)
            if (child.name.StartsWith("Wall")) wallList.Add(child);

        if (wallList.Count == 0) yield break;

        Transform chosenWall = wallList[Random.Range(0, wallList.Count)];

        Transform floor = chosenWall.Find("Floor");
        if (floor != null)
        {
            Renderer fr = floor.GetComponent<Renderer>();
            if (fr != null)
            {
                fr.enabled = true;
                fr.material.color = Color.cyan; 
            }
        }

        BoxCollider col = chosenWall.GetComponent<BoxCollider>();
        if (col == null)
            col = chosenWall.gameObject.AddComponent<BoxCollider>();
        col.isTrigger = true;

        NextStageTrigger triggerScript = chosenWall.gameObject.AddComponent<NextStageTrigger>();
        triggerScript.Setup(this, levelPrefab);
    }


    private bool TryGetEventRoom(out bool isExchangeRoom)
    {
        isExchangeRoom = false;

        if (exchangeRoomUsed && purifierRoomUsed)
            return false;

        if (Random.value > eventRoomChance)
            return false;

        List<bool> possible = new List<bool>();

        if (!exchangeRoomUsed) possible.Add(true);
        if (!purifierRoomUsed) possible.Add(false);

        isExchangeRoom = possible[Random.Range(0, possible.Count)];

        return true;
    }
    private void ApplyStageDifficulty(int stage)
    {
        switch (stage)
        {
            case 1:
                monstersPerWave = 5;
                maxWaveCount = 3;
                enemyHpMultiplier = 1.0f;
                break;

            case 2:
                monstersPerWave = 8;
                maxWaveCount = 4;
                enemyHpMultiplier = 1.2f;
                break;

            case 3:
                monstersPerWave = 8;
                maxWaveCount = 4;
                enemyHpMultiplier = 1.4f;
                break;

            case 4:
                monstersPerWave = 12;
                maxWaveCount = 5;
                enemyHpMultiplier = 1.6f;
                break;

            case 5:
                monstersPerWave = 15;
                maxWaveCount = 6;
                enemyHpMultiplier = 2.0f;
                break;
        }
    }
}

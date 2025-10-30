using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStageTrigger : MonoBehaviour
{
    private StageManager stageManager;
    private GameObject levelPrefab;
    private bool triggered = false;
    private bool isBoss = false;

    public void Setup(StageManager stageManager, GameObject prefab, bool isBoss)
    {
        this.stageManager = stageManager;
        this.levelPrefab = prefab;
        this.isBoss = isBoss;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 플레이어 Root 상태 ON (속박)
        if(PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = true;

        // 플레이어와 벽 위치를 기반으로 진입 방향 계산
        Vector3 toPlayer = other.transform.position - transform.position;
        float localX = Vector3.Dot(toPlayer, transform.right);   // 좌우
        float localZ = Vector3.Dot(toPlayer, transform.forward); // 앞뒤
        Vector3 entryDirection = new Vector3(localX, 0f, localZ).normalized;

        StartCoroutine(FadeAndLoadNextLevel(entryDirection));
    }

    private IEnumerator FadeAndLoadNextLevel(Vector3 entryDirection)
    {
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 1f);

        yield return new WaitForSeconds(0.5f);

        Vector3 fixedEntryDir = Vector3.zero;
        string wallName = gameObject.name.ToLower();

        if (wallName.Contains("right")) fixedEntryDir = Vector3.right;
        else if (wallName.Contains("left")) fixedEntryDir = Vector3.left;
        else if (wallName.Contains("top") || wallName.Contains("forward")) fixedEntryDir = Vector3.forward;
        else if (wallName.Contains("bottom") || wallName.Contains("back")) fixedEntryDir = Vector3.back;

        if (!isBoss)
        {
            stageManager.LoadNextLevel(fixedEntryDir);

            if (FadeManager.Instance != null)
                yield return FadeManager.Instance.StartCoroutine("Fade", 0f);

            if (PlayerStatus.Instance != null)
                PlayerStatus.Instance.isRooted = false;
        }
        else stageManager.LoadBossRome();
    }
}

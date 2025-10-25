using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStageTrigger : MonoBehaviour
{
    private StageManager stageManager;
    private GameObject levelPrefab;
    private bool triggered = false;

    public void Setup(StageManager stageManager, GameObject prefab)
    {
        this.stageManager = stageManager;
        this.levelPrefab = prefab;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        // 플레이어 Root 상태 ON (속박)
        PlayerStatus playerStatus = other.GetComponent<PlayerStatus>();
        if (playerStatus != null)
            playerStatus.isRooted = true;

        // 플레이어와 벽 위치를 기반으로 진입 방향 계산
        Vector3 toPlayer = other.transform.position - transform.position;
        float localX = Vector3.Dot(toPlayer, transform.right);   // 좌우
        float localZ = Vector3.Dot(toPlayer, transform.forward); // 앞뒤
        Vector3 entryDirection = new Vector3(localX, 0f, localZ).normalized;

        StartCoroutine(FadeAndLoadNextLevel(entryDirection, playerStatus));
    }

    private IEnumerator FadeAndLoadNextLevel(Vector3 entryDirection, PlayerStatus playerStatus)
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

        stageManager.LoadNextLevel(fixedEntryDir);

        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 0f);

        if (playerStatus != null)
            playerStatus.isRooted = false;
    }
}

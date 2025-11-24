using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextStageTrigger : MonoBehaviour
{
    private StageManager stageManager;
    private GameObject levelPrefab;
    private bool triggered = false;
    private bool isRewardRoom = false; // 보상 방 여부

    // 일반 방용
    public void Setup(StageManager stageManager, GameObject prefab)
    {
        this.stageManager = stageManager;
        this.levelPrefab = prefab;
        this.isRewardRoom = false;
    }

    // 보상 방용
    public void SetupRewardRoom(StageManager stageManager, GameObject prefab)
    {
        this.stageManager = stageManager;
        this.levelPrefab = prefab;
        this.isRewardRoom = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = true;

        Vector3 entryDirection = Vector3.zero;
        string wallName = gameObject.name.ToLower();

        if (wallName.Contains("right")) entryDirection = Vector3.right;
        else if (wallName.Contains("left")) entryDirection = Vector3.left;
        else if (wallName.Contains("top") || wallName.Contains("forward")) entryDirection = Vector3.forward;
        else if (wallName.Contains("bottom") || wallName.Contains("back")) entryDirection = Vector3.back;

        if (isRewardRoom)
        {
            StartCoroutine(FadeAndLoadRewardRoom(entryDirection));
        }
        else
        {
            StartCoroutine(FadeAndLoadNextLevel(other));
        }
    }

    private IEnumerator FadeAndLoadRewardRoom(Vector3 entryDirection)
    {
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 1f);

        yield return new WaitForSeconds(0.5f);

        stageManager.LoadNextLevel(entryDirection, spawnMonsters: false, isRewardRoom: true);

 
        if (FadeManager.Instance != null)
            yield return FadeManager.Instance.StartCoroutine("Fade", 0f);


        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = false;

    }

    private IEnumerator FadeAndLoadNextLevel(Collider player)
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

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = false;
    }
}

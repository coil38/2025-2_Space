using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyTeleport : MonoBehaviour
{
    private bool isOneTime = true;
    private ChipsetSelectUI chipsetSelectUI;

    private void Awake()
    {
        if (chipsetSelectUI == null)
            chipsetSelectUI = FindObjectOfType<ChipsetSelectUI>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (StageSelectUI.Instance != null)
            StageSelectUI.Instance.ShowUI();
        else
            Debug.LogError("StageSelectUI.Instance가 null입니다!");
        if (!other.CompareTag("Player") || !isOneTime) return;
        if (chipsetSelectUI != null && !chipsetSelectUI.isEquiping) return;

        StageSelectUI.Instance?.ShowUI(); // 안전하게 활성화

        PlayerStatus.Instance.isRooted = true;
        isOneTime = false;
    }
}


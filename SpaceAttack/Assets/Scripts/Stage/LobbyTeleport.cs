using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyTeleport : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject stageSelectUI; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (stageSelectUI != null)
                stageSelectUI.SetActive(true); // UI 활성화
            Time.timeScale = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (stageSelectUI != null)
                stageSelectUI.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}


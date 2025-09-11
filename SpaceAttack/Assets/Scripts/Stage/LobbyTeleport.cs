using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyTeleport : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject stageSelectUI;

    private bool isOneTime = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isOneTime)
        {
            if (stageSelectUI != null)
                stageSelectUI.SetActive(true); // UI 활성화

            PlayerStatus.Instance.isRooted = true;  //플레이어 속박처리
            isOneTime = false;
        }
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        if (stageSelectUI != null)
    //            stageSelectUI.SetActive(false);
    //        Time.timeScale = 1f;
    //    }
    //}
}


using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyTeleport : MonoBehaviour
{
    private bool isOneTime = true;
  //private ChipsetSelectUI chipsetSelectUI;
    private ExSelectUi exselectui;

    [Header("스테이지 선택 UI")]
    public StageSelectUI stageSelectUI;

    private void Awake()
    {
        //chipsetSelectUI = FindObjectOfType<ChipsetSelectUI>(true);
        exselectui = FindObjectOfType<ExSelectUi>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !isOneTime) return;

        //if (chipsetSelectUI != null && !chipsetSelectUI.isEquiping) return;

        // Inspector에서 참조한 UI가 null인지 체크
        if (stageSelectUI != null)
        {
            stageSelectUI.ShowUI();
        }
        else
        {
            Debug.LogError("StageSelectUI가 Inspector에서 연결되지 않았습니다!");
        }

        PlayerStatus.Instance.isRooted = true;
        isOneTime = false;
    }
}



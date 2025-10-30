using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyTeleport : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject stageSelectUI;

    private bool isOneTime = true;
    private ChipsetSelectUI chipsetSelectUI;
    private void Awake()
    {
        if(chipsetSelectUI == null) 
            chipsetSelectUI = FindObjectOfType<ChipsetSelectUI>(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (chipsetSelectUI != null)
            if (!chipsetSelectUI.isEquiping) return;   //아직 유물을 장착하지 않았을 경우, 예외처리

        if (other.CompareTag("Player") && isOneTime)
        {
            if (stageSelectUI != null)
            {
                stageSelectUI.SetActive(true); // UI 활성화
                UIESCSystem.ChangeUIType(UIType.SelectStageUI);    //스테이지 선택UI으로 변경
            }

            PlayerStatus.Instance.isRooted = true;  //플레이어 속박처리
            isOneTime = false;
        }
    }
}


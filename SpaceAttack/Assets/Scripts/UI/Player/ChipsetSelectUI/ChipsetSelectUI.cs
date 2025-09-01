using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChipsetSelectUI : MonoBehaviour
{
    [Header("칩셋 데이터베이스")]
    [SerializeField] private ChipsetDatabaseSO chipsetDatabase;

    [Header("오브젝트")]
    [SerializeField] private GameObject chipsetLayoutObject;  //칩셋 슬롯의 부모 오브젝트
    [SerializeField] private ChipsetSelectDetailUI chipsetDetailPanel;   //칩셋(상세)창
    [SerializeField] private TextMeshProUGUI feedBackText;

    [Header("아이콘")]
    [SerializeField] private Sprite lockedIcon;

    private Image[] iconImages;
    private Button[] buttons;
    private Timer feedBackTimer = new Timer(2f);

    private void OnEnable()
    {
        if (chipsetDatabase == null || chipsetLayoutObject == null || lockedIcon == null)  //없을 경우, 반환처리
            return;

        if (chipsetDatabase == null) chipsetDetailPanel.chipsetDatabase = chipsetDatabase;

        if (iconImages == null) SetChipsetUI();   //초기 한번 설정
    }

    private void OnDisable()
    {
        
    }

    private void Update()
    {
        feedBackTimer.Update();
        if (!feedBackTimer.IsRunning())
        {
            feedBackText.text = string.Empty;
        }
    }

    private void SetChipsetUI()
    {
        int chipsetCount = chipsetDatabase.chipsets.Count;

        buttons = chipsetLayoutObject.GetComponentsInChildren<Button>();       //자식의 모든 슬롯의 버튼 받기
        iconImages = chipsetLayoutObject.GetComponentsInChildren<Image>();     //자식의 모든 아이콘 받기
        for (int i = 0; i < iconImages.Length; i++)
        {
            iconImages[i] = iconImages[i].gameObject.transform.GetComponentInChildren<Image>();  //자식의 모든 자식 아이콘 받기

            if (chipsetCount > 0)
            {
                iconImages[i].sprite = chipsetDatabase.chipsets[i].iconSprite;  //칩셋 스프라이트 할당
            }
            else
            {
                iconImages[i].sprite = lockedIcon;  //잠금 아이콘 할당
            }

            chipsetCount--;
        }

        chipsetCount = chipsetDatabase.chipsets.Count;

        for (int i = 0; i < buttons.Length; i++)  //이벤트 구독
        {
            if (chipsetCount > 0)
            {
                buttons[i].onClick.AddListener(() =>
                {
                    chipsetDetailPanel.currentChipset = chipsetDatabase.chipsets[i];
                    chipsetDetailPanel.gameObject.SetActive(true);
                });
            }
            else
            {
                buttons[i].onClick.AddListener(() =>
                {
                    feedBackText.text = "This Chipset is unLocked";
                    feedBackTimer.Start();
                });
            }

            chipsetCount--;
        }
    }
}

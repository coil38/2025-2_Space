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

    [Header("기타")]
    [SerializeField] private Sprite lockedIcon; //잠금 아이콘
    [SerializeField] private Button selectButton;  //선택 버튼

    private Image[] iconImages;
    private Button[] buttons;
    private GameObject[] equipedTexts;
    private Timer feedBackTimer = new Timer(2f);

    private void OnEnable()
    {
        UISoundManager.PlayeOnAndOffPanelSound(); //패널열기혹은 닫기 사운드 재생

        if (chipsetDatabase == null || chipsetLayoutObject == null || lockedIcon == null)  //없을 경우, 반환처리
            return;

        if (chipsetDatabase == null) chipsetDetailPanel.chipsetDatabase = chipsetDatabase;

        if (iconImages == null) SetChipsetUI();   //초기 한번 설정

        chipsetDetailPanel.gameObject.SetActive(false);  //칩셋 상세창 비활성화 처리

        UIESCSystem.ChangeUIType(UIType.PauseUI);   //일시정지 UI 상태로 변경
        Time.timeScale = 0f;  //게임 일시정지
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;  //게임 일시정지 해제
        UIESCSystem.ChangeUIType(UIType.SelectChipsetUI);   //칩셋 선택UI로 변경

        UISoundManager.PlayeOnAndOffPanelSound(); //패널열기혹은 닫기 사운드 재생
    }

    private void Update()
    {
        feedBackTimer.Update();
        if (!feedBackTimer.IsRunning())
        {
            feedBackText.text = string.Empty;
        }

        if (chipsetDetailPanel.gameObject.activeSelf) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndSelectChipset();
        }
    }

    private void SetChipsetUI()
    {
        int chipsetCount = chipsetDatabase.chipsets.Count;

        buttons = chipsetLayoutObject.GetComponentsInChildren<Button>();       //자식의 모든 슬롯의 버튼 받기
        iconImages = new Image[buttons.Length];                                //아이콘 이미지 배열 크기 할당
        equipedTexts = new GameObject[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            iconImages[i] = buttons[i].gameObject.transform.GetChild(0).GetComponent<Image>();  //아이콘 할당 이미지 받기
            equipedTexts[i] = buttons[i].gameObject.transform.GetChild(1).gameObject;           //장착중 텍스트 오브젝트 받기

            if (chipsetCount > 0)
            {
                string key = chipsetDatabase.chipsets[i].chipsetKey;
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
            int index = i;

            if (chipsetCount > 0)
            {
                buttons[index].onClick.AddListener(() =>
                {
                    chipsetDetailPanel.chipsetIndex = index;
                    chipsetDetailPanel.currentChipset = chipsetDatabase.chipsets[index];
                    chipsetDetailPanel.gameObject.SetActive(true);
                });
            }
            else
            {
                buttons[index].onClick.AddListener(() =>
                {
                    feedBackText.text = "This Chipset is unLocked";
                    feedBackTimer.Start();
                });
            }

            chipsetCount--;
        }

        chipsetDetailPanel.chipsetDatabase = chipsetDatabase;  //칩셋 데이터 베이스 할당
        selectButton.onClick.AddListener(EndSelectChipset);  //선택 버튼 이벤트 구독
    }

    private void EndSelectChipset()
    {
        gameObject.SetActive(false);
    }

    public void SetEquipmentText(int index)
    {
        foreach (var text in equipedTexts)
            text.SetActive(false);
        equipedTexts[index].SetActive(true);
    }
}

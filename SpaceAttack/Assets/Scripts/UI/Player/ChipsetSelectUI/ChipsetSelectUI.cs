using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.ComponentModel;

public class ChipsetSelectUI : MonoBehaviour
{
    [Header("칩셋 데이터베이스")]
    [SerializeField] private ChipsetDatabaseSO chipsetDatabase;

    [Header("오브젝트")]
    [SerializeField] private GameObject chipsetLayoutObject;  //칩셋 슬롯의 부모 오브젝트
    [SerializeField] private ChipsetSelectDetailUI chipsetDetailPanel;   //칩셋(상세)창
    [SerializeField] private TextMeshProUGUI feedBackText;

    [Header("기타")]
    [SerializeField] private Sprite lockedSlotIcon;
    [SerializeField] private Sprite unlockedSlotIcon;
    [SerializeField] private Sprite hightLingthingSlotIcon;
    [SerializeField] private Sprite warriorIcon;
    [SerializeField] private Sprite archerIcon;
    [SerializeField] private Button selectButton;  //선택 버튼

    private Image[] iconImages;
    private Button[] buttons;
    private GameObject[] equipedTexts;
    public bool isEquiping { get; private set; }   //장착여부 체크 불값
    private void OnEnable()
    {
        UISoundManager.PlayeOnAndOffPanelSound(); //패널열기혹은 닫기 사운드 재생

        if (chipsetDatabase == null || chipsetLayoutObject == null)  //없을 경우, 반환처리
            return;

        if (chipsetDatabase == null) chipsetDetailPanel.chipsetDatabase = chipsetDatabase;

        if (iconImages == null) SetChipsetUI();   //초기 한번 설정

        chipsetDetailPanel.gameObject.SetActive(false);  //칩셋 상세창 비활성화 처리

        UIESCSystem.ChangeUIType(UIType.SelectChipsetUI);   //일시정지 UI 상태로 변경
        UIESCSystem.SetUIDepth(UIType.SelectChipsetUI, EndSelectChipset, gameObject);
        Time.timeScale = 0f;  //게임 일시정지
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;  //게임 일시정지 해제
        UIESCSystem.ChangeUIType(UIType.None);   //칩셋 선택UI로 변경

        UISoundManager.PlayeOnAndOffPanelSound(); //패널열기혹은 닫기 사운드 재생
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
                //iconImages[i].sprite = chipsetDatabase.chipsets[i].iconSprite;  //칩셋 스프라이트 할당
                switch (chipsetDatabase.chipsets[i].chipsetName)
                {
                    case "Warrior":
                        iconImages[i].sprite = warriorIcon; break;
                    case "Archer":
                        iconImages[i].sprite = archerIcon; break;
                }

                buttons[i].GetComponent<Image>().sprite = unlockedSlotIcon;     //해금된 칩셋슬롯 적용
                if (buttons[i].transition == Selectable.Transition.SpriteSwap)
                {
                    SpriteState spriteState = buttons[i].spriteState;
                    spriteState.highlightedSprite = hightLingthingSlotIcon;
                    spriteState.pressedSprite = hightLingthingSlotIcon;
                    buttons[i].spriteState = spriteState;
                }
            }
            else
            {
                buttons[i].GetComponent<Image>().sprite = lockedSlotIcon;     //해금된 칩셋슬롯 적용
                Color color = iconImages[i].color;
                color.a = 0;
                iconImages[i].color = color;
            }

            chipsetCount--;
        }

        chipsetCount = chipsetDatabase.chipsets.Count;

        for (int i = 0; i < buttons.Length; i++)  //버튼 이벤트 구독
        {
            int index = i;

            if (chipsetCount > 0)
            {
                buttons[index].onClick.AddListener(() =>
                {
                    UIESCSystem.SetUIDepth(UIType.SelectChipsetUI, chipsetDetailPanel.ESCDetailPanel, chipsetDetailPanel.gameObject);    //Esp용 UI_Depth설정 함수
                    chipsetDetailPanel.chipsetIndex = index;
                    chipsetDetailPanel.currentChipset = chipsetDatabase.chipsets[index];
                    chipsetDetailPanel.gameObject.SetActive(true);
                });
            }
            else
            {
                buttons[index].onClick.AddListener(() =>
                {
                    buttons[index].GetComponent<HighLingthingButtonUI>().isCanInteracting = false;
                    feedBackText.text = "아직 해금되지 않은 칩셋입니다.";
                    feedBackText.DOFade(0f, 2f).SetUpdate(true).OnComplete(() =>
                    {
                        feedBackText.text = string.Empty;
                        feedBackText.alpha = 1f;
                    });

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

        isEquiping = true;   //장착완료로 변경
    }


    public void ResetEquipState()
    {
        isEquiping = false;
    }
}

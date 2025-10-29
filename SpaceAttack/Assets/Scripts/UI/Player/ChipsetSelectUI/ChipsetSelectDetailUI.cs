using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChipsetSelectDetailUI : MonoBehaviour
{
    [Header("칩셋 선택창")]
    [SerializeField] private ChipsetSelectUI chipsetSelectUI;

    [Header("칩셋 정보창")]
    [SerializeField] private TextMeshProUGUI chipsetTitle;
    [SerializeField] private Image chipsetIcon;
    [SerializeField] private TextMeshProUGUI chipsetDescription;
    [SerializeField] private GameObject[] skills;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;

    [Header("칩셋 정보창 버튼")]
    [SerializeField] private Button equipChipsetButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI equipChipsetText;

    [HideInInspector] public ChipsetSO currentChipset;
    [HideInInspector] public ChipsetDatabaseSO chipsetDatabase;
    [HideInInspector] public int chipsetIndex;

    private ChipsetSO equipedChipset;  //장착중인 칩셋SO
    private HighLingthingButtonUI highLingthingButtonUI;  //장착 버튼 애니메이션 Component
    private Vector3 defaultIconSize;   //초기 스킬 아이콘 크기
    private InventoryManager inventoryManager;   //인벤토리 컴포넌트

    private Image[] skillImages;
    private Button[] buttons;
    private string[] descriptions;

    void OnEnable()
    {
        UISoundManager.PlayeOnAndOffPanelSound(); //패널열기 사운드 재생

        if (inventoryManager == null) 
            inventoryManager = FindAnyObjectByType<InventoryManager>();

        if(skillImages == null && buttons == null)  //처음 한번만 실행 ( 변수 생성 )
            SetChipsetDetailOneTime();

        if (currentChipset == null) return;
        SetChipsetDetail();  //아이콘 할당 및 아이콘 버튼 이벤트 구독 처리
        SetEquipmentChipsetButton();  //장착여부 체크
    }

    void OnDisable()
    {
        UISoundManager.PlayeOnAndOffPanelSound(); //패널 닫기 사운드 재생

        if (currentChipset == null) return;
        foreach (var button in buttons)
        {
            button.onClick.RemoveAllListeners();
        }
        equipChipsetButton.onClick.RemoveAllListeners();  //장착 버튼 구독 해제
    }

    public void ESCDetailPanel()
    {
        gameObject.SetActive(false);
    }

    private void SetChipsetDetailOneTime()
    {
        List<Image> i_temp = new List<Image>();       //이미지, 버튼 리스트 생성 및 할당
        List<Button> b_temp = new List<Button>();
        foreach (var skill in skills)
        {
            i_temp.Add(skill.GetComponent<Image>());
            b_temp.Add(skill.GetComponent<Button>());
        }
        skillImages = i_temp.ToArray();
        buttons = b_temp.ToArray();

        descriptions = new string[skills.Length];

        equipChipsetButton.onClick.AddListener(EquipChipset); //장착 버튼 구독
        cancelButton.onClick.AddListener(Cancel);             //취소 버튼 구독

        highLingthingButtonUI = equipChipsetButton.gameObject.GetComponent<HighLingthingButtonUI>();  //할당
        defaultIconSize = skillImages[0].rectTransform.localScale;  //할당
    }

    private void SetChipsetDetail()  
    {
        chipsetTitle.text = currentChipset.chipsetName;  //칩셋 이름, 칩셋 스프라이트 이미지, 칩셋 설명 텍스트 할당
        chipsetIcon.sprite = currentChipset.iconSprite;
        chipsetDescription.text = currentChipset.description;

        Sprite iconTemp;
        ChipsetComponentSO chipsetComponent = null;
        for (int i = 0; i < buttons.Length; i++)    //스킬 아이콘들 +아이콘 버튼 상호작용 이벤트 구독
        {
            if (i == 0)
            {
                chipsetComponent = chipsetDatabase.GetChipsetComponent(currentChipset.chipsetKey, ChipsetComponentType.BASEATTACK);
            }
            else if (i == 1)
            {
                chipsetComponent = chipsetDatabase.GetChipsetComponent(currentChipset.chipsetKey, ChipsetComponentType.SKILL1);
            }
            else if (i == 2)
            {
                chipsetComponent = chipsetDatabase.GetChipsetComponent(currentChipset.chipsetKey, ChipsetComponentType.SKILL2);
            }
            else if (i == 3)
            {
                chipsetComponent = chipsetDatabase.GetChipsetComponent(currentChipset.chipsetKey, ChipsetComponentType.SKILL3);
            }

            if (chipsetComponent == null) return;
            descriptions[i] = chipsetComponent.description;
            iconTemp = chipsetComponent.iconSprite;

            int index = i;

            skillImages[index].sprite = iconTemp;

            buttons[index].onClick.AddListener(() =>   //스킬 설명 상호작용 버튼 이벤트 구독
            {
                skillDescriptionText.text = descriptions[index];
                HighLingthingSkillIcon(skillImages[index]);
            });
        }
    }

    private void EquipChipset()   //칩셋 장착
    {
        Sprite[] sprites = new Sprite[skillImages.Length];  //스킬 스프라이트 저장
        for (int i = 0; i < skillImages.Length; i++)
            sprites[i] = skillImages[i].sprite;
        sprites[0] = currentChipset.iconSprite;

        //칩셋 장착 내부 코드
        switch (currentChipset.chipsetName)
        {
            case "Warrior": LogUtil.Log("전사 칩셋 장착"); break;
            case "Archer": LogUtil.Log("궁수 칩셋 장착"); break;
        }
        GameObject prefab = DataManager.instance.GetChipsetPrfByName(currentChipset.chipsetName);
        GameObject chipset = Instantiate(prefab, transform.position, prefab.transform.rotation);
        if (inventoryManager != null)
            inventoryManager.chipSet = chipset.GetComponent<ChipSetType>();

        if (PlayerUIManager.instance != null)
            PlayerUIManager.instance.SetChipsetInfo(sprites[0], sprites[1], sprites[2], sprites[3]);  //메인UI 이미지 변경

        equipedChipset = currentChipset;  //현재 칩셋을 장착중인 칩셋에 할당
        chipsetSelectUI.SetEquipmentText(chipsetIndex);  //장착중인 칩셋 텍스트 표시
        SetEquipmentChipsetButton();
        Cancel();                    //장착 디테일창 비활성화
    }

    private void Cancel()
    {
        gameObject.SetActive(false);
    }

    private void SetEquipmentChipsetButton()
    {
        if (equipedChipset != currentChipset)
        {
            equipChipsetText.text = "장착";
            equipChipsetButton.onClick.AddListener(EquipChipset); //장착 버튼 구독 처리

            //버튼 하이라이트 작동방지 켜기 (버튼 인터렉션 활성화)
            highLingthingButtonUI.isCanInteracting = true;
            //LogUtil.Log("")

            //버튼 입력 색상 변경
            ColorBlock cb = equipChipsetButton.colors;
            cb.pressedColor = new Color32(200, 200, 200, 255);
            equipChipsetButton.colors = cb;
        }
        else
        {
            equipChipsetText.text = "장착 중";
            equipChipsetButton.onClick.RemoveAllListeners();  //장착 버튼 구독 해제

            //버튼 하이라이트 작동방지 끄기 (버튼 인터렉션 비활성화) - 일부로 쓰지 않음. 버튼이 알아서 처리해줌

            //버튼 입력 색상 변경
            ColorBlock cb = equipChipsetButton.colors;
            cb.pressedColor = Color.white;
            equipChipsetButton.colors = cb;
        }
    }

    private void HighLingthingSkillIcon(Image targetImage)  //스킬 아이콘 애니메이션 실행함수
    {
        foreach (var image in skillImages)  //초기화
        {
            image.rectTransform.localScale = defaultIconSize;
        }

        targetImage.rectTransform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 0.1f).SetUpdate(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GuideUIManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI[] mainTitles;
    [SerializeField] TextMeshProUGUI pageTitleUI;
    [SerializeField] TextMeshProUGUI descriptionUI;
    [SerializeField] Image descriptionImage;
    [SerializeField] Button nextPageButton;
    [SerializeField] Button previousButton;
    [SerializeField] TextMeshProUGUI pageNumberTextUI;
    [SerializeField] Button exitButton;

    [Header("비주얼 변수")]
    [SerializeField] Color normalColor;
    [SerializeField] Color selectedColor;

    private int currentPage;
    private int currentSubId;
    private int currentMaxPage;

    private bool isOneTime = false;
    private Button currentButton;

    private void OnEnable()
    {
        if (mainTitles == null || mainTitles.Length <= 0)
        {
            LogUtil.LogError("가이드UI의 mainTiltle이 할당되지 않았습니다. ");
            return;
        }

        if(!isOneTime) Initialize();
    }

    private void Initialize()   //카테고리 버튼 이벤트 연결 ( 처음 초기화 때, 꺼져 있어야됨)
    {
        GuideCatecorySO[] gcSOs = DataManager.instance._guideDatabase.GetGuideCats();
        
        for (int i = 0; i < mainTitles.Length; i++)
        {
            int index1 = i;
            mainTitles[i].GetComponent<TextMeshProUGUI>().text = gcSOs[i].gcName;
            Button[] subButtons = mainTitles[i].GetComponentsInChildren<Button>();

            for (int j = 0; j < subButtons.Length; j++)
            {
                int index2 = j;
                int subId = gcSOs[i].subIds[j];
                GuideSO[] pages = DataManager.instance._guideDatabase.GetGuidesBySubId(subId);

                subButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = gcSOs[i].subNames[j];   //서브 버튼에 서브 타이틀 갱신

                subButtons[index2].onClick.AddListener(() =>
                {
                    currentSubId = subId;
                    currentPage = 1;
                    currentMaxPage = pages.Length;

                    UISoundManager.PlayeButtonClickSound();   //버튼 클릭 사운드 재생

                    if (currentButton != null) currentButton.GetComponent<Image>().color = normalColor;
                    subButtons[index2].GetComponent<Image>().color = selectedColor;
                    currentButton = subButtons[index2];

                    pageNumberTextUI.text = $"{currentPage}/{currentMaxPage} 페이지";
                    pageTitleUI.text = pages[0].pageTitle;
                    descriptionUI.text = pages[0].description;
                    descriptionImage.sprite = pages[0].pageSprite;
                });
            }
        }

        nextPageButton.onClick.AddListener(() =>
        {
            if (currentPage >= currentMaxPage) return;

            currentPage++;

            UpdatePageInfo();
        });

        previousButton.onClick.AddListener(() =>
        {
            if (currentPage <= 1) return;

            currentPage--;

            UpdatePageInfo();
        });

        exitButton.onClick.AddListener(() =>
        {
            OffGuideUI();
        });

        isOneTime = true;

    }

    void UpdatePageInfo()
    {
        GuideSO[] pages = DataManager.instance._guideDatabase.GetGuidesBySubId(currentSubId);

        pageNumberTextUI.text = $"{currentPage}/{currentMaxPage} 페이지";
        pageTitleUI.text = pages[currentPage - 1].pageTitle;
        descriptionUI.text = pages[currentPage - 1].description;
        descriptionImage.sprite = pages[currentPage - 1].pageSprite;
    }

    public void OffGuideUI()
    {
        UISoundManager.PlayeOnAndOffPanelSound();  //패널닫기 사운드 재생
        gameObject.SetActive(false);
    }
}

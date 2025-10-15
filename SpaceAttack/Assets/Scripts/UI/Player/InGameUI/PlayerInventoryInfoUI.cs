using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;


public class PlayerInventoryInfoUI : MonoBehaviour
{
    [Header("UI정보전달 대상들")]
    [SerializeField] private TextMeshProUGUI relicNameText;
    [SerializeField] private TextMeshProUGUI relicDivisionText;
    [SerializeField] private TextMeshProUGUI relicDarkMatCountText;
    [SerializeField] private TextMeshProUGUI relicDescription;
    [SerializeField] private Image relicIconImage;
    [SerializeField] private Button removeButton;
    [SerializeField] private Button closeButton;

    [Header("UI이동 정보들")]
    [SerializeField] private Vector2 openPos = Vector2.zero;
    [SerializeField] private Vector2 closedPos = new Vector2(458, 0);
    [SerializeField] private float duration = 0.4f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private RectTransform UITransform;
    private RelicSO currentRelic;

    private void OnEnable()
    {
        removeButton.onClick.RemoveAllListeners();
        closeButton.onClick.RemoveAllListeners();

        removeButton.onClick.AddListener(RemoveRelic);
        closeButton.onClick.AddListener(OFFPlayerInventoryInfoUI);
    }
    private void Awake()
    {
        //gameObject.SetActive(false);
        UITransform = GetComponent<RectTransform>();
        UITransform.position = UITransform.position + (Vector3)closedPos;
    }
    public void SetRelicInfoUI(RelicSO relicSO)
    {
        OnPlayerInventoryInfoUI();
        LogUtil.Log("인벤토리 유물정보UI 활성화");

        if (relicSO.relicID < 100)
        {
            LogUtil.LogWarning("유물에 ID가 존재하지 않습니다.");
            return;
        }
        currentRelic = relicSO;

        //유물 정보 입력
        relicNameText.text = relicSO.relicName;                             //유물 이름할당
        relicDivisionText.text = relicSO.relicDivision;                     //유물 분류 할당
        relicDarkMatCountText.text = relicSO.darkMaterialCount.ToString();  //유물 암흑물질수 할당
        relicIconImage.sprite = relicSO.iconSprite;                         //유물 이미지 할당

        //유물 능력 설명 할당
        relicDescription.text = relicSO.description;
    }

    private void OnPlayerInventoryInfoUI()
    {
        gameObject.SetActive(true);
        UITransform.DOAnchorPos(openPos, duration).SetEase(ease);
    }
    private void OFFPlayerInventoryInfoUI()
    {
        UITransform.DOAnchorPos(closedPos, duration).SetEase(ease).OnComplete(() => gameObject.SetActive(false));
    }

    private void RemoveRelic()
    {
        PlayerUIManager.instance.RemovePlayerItem(currentRelic);
        gameObject.SetActive(false);
    }
}

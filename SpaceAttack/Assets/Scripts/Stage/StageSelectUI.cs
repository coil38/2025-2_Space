using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    public static StageSelectUI Instance { get; private set; }

    [Header("버튼 목록 (1~5)")]
    public Button[] stageButtons;

    [Header("색상 설정")]
    public Color clearedColor = Color.green;  // 클리어됨
    public Color availableColor = Color.white;
    public Color lockedColor = Color.red;

    public int clearedStage = 0;

    [Header("보스 버튼")]
    public Button bossButton; // 보스 버튼

    [Header("UI패널")]
    public GameObject UIPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
{
    if (gameObject.activeInHierarchy)
        UpdateButtons();
}

    public void UpdateButtons()
    {
        if (StageProgress.Instance == null)
        {
            Debug.LogError("StageProgress.Instance가 null입니다!");
            return;
        }

        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (stageButtons[i] == null) continue;

            int stageNumber = i + 1;
            Image img = stageButtons[i].GetComponent<Image>();
            bool unlocked = stageNumber <= StageProgress.Instance.unlockedStage + 1;
            stageButtons[i].interactable = unlocked;

            if (stageNumber <= StageProgress.Instance.clearedStage)
            {
                img.color = clearedColor;                
            }
            else if (stageNumber == StageProgress.Instance.clearedStage + 1)
            {
                img.color = availableColor;           
            }
            else
            {
                img.color = lockedColor;                
            }
        }

        if (bossButton != null)
        {
            bool available = StageProgress.Instance.clearedStage >= 6;
            bossButton.interactable = available;

            Image bossImg = bossButton.GetComponent<Image>();
            bossImg.color = available ? clearedColor : lockedColor;
        }
    }

    public void SelectStage(int stageNumber)
    {
        StageGameData.SelectedStage = stageNumber;
        StartCoroutine(C_LoadScene("BattleTestNormalScene"));
    }


    public void SelectBoss()
    {
        if (StageProgress.Instance.unlockedStage < 6)
            return;

        StartCoroutine(C_LoadScene("MiddleBossScene"));
    }

    public void CloseUI()
    {
        HideUI(); 
    }

    private IEnumerator C_LoadScene(string sceneName)
    {
        PlayerStatus.Instance.isRooted = false;
        yield return new WaitUntil(() => PlayerStatus.Instance.isRooted == false);

        FadeManager.Instance.LoadScene(sceneName);

        yield return null;

        HideUI();
    }
    public void ShowUI()
    {
        Transform root = UIPanel.transform;
        while (root.parent != null)
            root = root.parent;

        Debug.LogWarning($"[ShowUI] Before activation — Root active? {root.gameObject.activeSelf}");

        root.gameObject.SetActive(true);
        UIPanel.SetActive(true);

        Debug.LogWarning($"[ShowUI] After activation — Root active? {root.gameObject.activeSelf}");

        StartCoroutine(FixUIActivation());
    }

    private IEnumerator FixUIActivation()
    {
        yield return null;   
        Transform root = UIPanel.transform;
        while (root.parent != null)
            root = root.parent;
        root.gameObject.SetActive(true);
        UIPanel.SetActive(true);
        Debug.LogWarning("[FixUI] Parent forced active after all player status callbacks");
    }

    public void HideUI()
    {
        UIPanel.SetActive(false);
        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.UIRoot(false);
    }

}
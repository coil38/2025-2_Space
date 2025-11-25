using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectUI : MonoBehaviour
{
    public static StageSelectUI Instance;

    [Header("버튼 목록 (1~5)")]
    public Button[] stageButtons; 

    [Header("색상 설정")]
    public Color unlockedColor = Color.green;
    public Color lockedColor = Color.red;

    [Header("보스 버튼")]
    public Button bossButton; // 보스 버튼


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        HideUI(); 
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

        if (stageButtons == null || stageButtons.Length == 0)
            return;

        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (stageButtons[i] == null) continue;

            int stageNumber = i + 1;
            bool unlocked = StageProgress.Instance.IsStageUnlocked(stageNumber);

            stageButtons[i].interactable = unlocked;
            Image img = stageButtons[i].GetComponent<Image>();
            if (img != null)
                img.color = unlocked ? unlockedColor : lockedColor;
        }

        if (bossButton != null)
        {
            bool bossUnlocked = StageProgress.Instance.unlockedStage >= 5;
            bossButton.interactable = bossUnlocked;
            Image bossImg = bossButton.GetComponent<Image>();
            if (bossImg != null)
                bossImg.color = bossUnlocked ? unlockedColor : lockedColor;
        }
    }

    public void SelectStage(int stageNumber)
    {
        StageGameData.SelectedStage = stageNumber;
        StartCoroutine(C_LoadScene("BattleTestNormalScene"));
    }


    public void SelectBoss()
    {
        if (StageProgress.Instance.unlockedStage < 5)
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
        gameObject.SetActive(true);

        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = true;

        UpdateButtons();
        UIESCSystem.ChangeUIType(UIType.SelectStageUI);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
        if (PlayerStatus.Instance != null)
            PlayerStatus.Instance.isRooted = false;
    }

}
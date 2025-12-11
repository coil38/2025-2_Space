using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class PauseUIManager : MonoBehaviour
{
    public static PauseUIManager Instance;

    [Header("UI버튼")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button guideButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingUIButton;

    [Header("UI창")]
    [SerializeField] private GameObject pauseUIPanel;
    [SerializeField] private GameObject guideUIPanel;
    [SerializeField] private GameObject settingUIPanel;

    [Header("블러효과")]
    [SerializeField] private Volume pauseVolume;

    private SettingUIManager settingUIManager;
    private GuideUIManager guideUIManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //버튼들 인터렉션 델리게이트 체인
        resumeButton.onClick.AddListener(ResumeGame);
        newGameButton.onClick.AddListener(PlayNewGame);
        guideButton.onClick.AddListener(OnGuideUI);
        exitButton.onClick.AddListener(ExitGame);
        settingUIButton.onClick.AddListener(OnSettingPanel);

        pauseUIPanel.SetActive(false);
        settingUIPanel.SetActive(false);
        guideUIPanel.SetActive(false);

        UIESCSystem.SetPauseUI(OnPauseUI);     //일시정지 활성화_델리게이트 체인

        pauseVolume.weight = 0;                //블러효과 종료
    }

    public void OnPauseUI()
    {
        pauseUIPanel.SetActive(true);
        UISoundManager.PlayeOnAndOffPanelSound();  //패널열기 사운드 재생
        Time.timeScale = 0f;           //일시정지

        UIESCSystem.SetUIDepth(UIType.PauseUI, ResumeGame, pauseUIPanel);
        pauseVolume.weight = 1;                //블러효과 실행
    }

    private void ResumeGame()
    {
        LogUtil.Log("게임을 재개합니다.");
        UISoundManager.PlayeOnAndOffPanelSound();  //패널닫기 사운드 재생
        pauseUIPanel.SetActive(false);
        Time.timeScale = 1f;           //일시정지 해제
        pauseVolume.weight = 0;                //블러효과 종료
    }

    private void PlayNewGame()
    {
        LogUtil.Log("새로운 게임을 시작합니다.");
        //if (GameSceneManager.instance.currentScene == SceneType.TutorialScene)
        //{
        //    pauseUIPanel.SetActive(false);
        //    Time.timeScale = 1f;           //일시정지 해제
        //    pauseVolume.weight = 0;                //블러효과 종료

        //    SaveManager.instance.InitializePlayerDatas();
        //    SceneLoadManager.instance.
        //        LoadScene(GameSceneManager.GetSceneNameByType(SceneType.TutorialScene));
        //}
        //else
        //{
        //    pauseUIPanel.SetActive(false);
        //    Time.timeScale = 1f;           //일시정지 해제
        //    pauseVolume.weight = 0;                //블러효과 종료

        //    SaveManager.instance.PlayerReset();
        //}

        pauseUIPanel.SetActive(false);
        Time.timeScale = 1f;           //일시정지 해제
        pauseVolume.weight = 0;                //블러효과 종료

        SaveManager.instance.InitializePlayerDatas();
        SceneLoadManager.instance.
            LoadScene(GameSceneManager.GetSceneNameByType(SceneType.TutorialScene));
    }

    private void OnAchievementPanel()
    {
        LogUtil.Log("업적UI가 활성화되었습니다.");
    }

    private void OnGuideUI()
    {
        LogUtil.Log("가이드UI가 활성화되었습니다.");
        guideUIPanel.SetActive(true);
        UISoundManager.PlayeOnAndOffPanelSound();  //패널열기 사운드 재생

        if (guideUIManager == null)
            guideUIManager = GetComponentInChildren<GuideUIManager>();

        UIESCSystem.SetUIDepth(UIType.PauseUI, guideUIManager.OffGuideUI, guideUIPanel);
    }

    private void OnSettingPanel()
    {
        LogUtil.Log("설정UI가 활성화되었습니다.");
        settingUIPanel.SetActive(true);

        if (settingUIManager == null)
            settingUIManager = GetComponentInChildren<SettingUIManager>();

        UIESCSystem.SetUIDepth(UIType.PauseUI, settingUIManager.EscapeSetting, settingUIPanel);
    }

    private void ExitGame()
    {
        LogUtil.Log("게임을 시작화면으로 돌아갑니다.");
        if (SceneLoadManager.instance != null)
        {
            if(GameSceneManager.instance.currentScene != SceneType.StartGameScene)
                Time.timeScale = 1f;           //일시정지 취소
            pauseUIPanel.SetActive(false);  //비활성화 처리
            pauseVolume.weight = 0;                //블러효과 종료
            SceneLoadManager.instance.LoadScene("StartUIScene");
        }
    }
}

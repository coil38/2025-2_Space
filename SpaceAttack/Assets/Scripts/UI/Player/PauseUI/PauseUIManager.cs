using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUIManager : MonoBehaviour
{
    public static PauseUIManager Instance;

    [Header("UI버튼")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button achievementButton;
    [SerializeField] private Button helpButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button settingUIButton;

    [Header("UI창")]
    [SerializeField] private GameObject pauseUIPanel;
    [SerializeField] private GameObject achievementUIPanel;
    [SerializeField] private GameObject helpUIPanel;
    [SerializeField] private GameObject settingUIPanel;

    private bool cannotEscape;  //일시정지창 이외에 또다른 뎁스가 있을 경우, false처리 (esc입력 안됨)
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
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
        achievementButton.onClick.AddListener(OnAchievementPanel);
        helpButton.onClick.AddListener(OnHelpPanel);
        exitButton.onClick.AddListener(ExitGame);
        settingUIButton.onClick.AddListener(OnSettingPanel);

        pauseUIPanel.SetActive(false);
        settingUIPanel.SetActive(false);
        achievementUIPanel.SetActive(false);
        helpUIPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        CheckEscape();         //일시정지 취소 가능여부 실시간 체크
    }

    private void CheckOnPausePanel()  //설정UI가 활성화여부 실시간 체크
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //특정씬에서만 활성화하게 예외처리-----------------//
            pauseUIPanel.SetActive(true);
            Time.timeScale = 0f;           //일시정지
        }
    }

    private void CheckEscape()
    {
        if (pauseUIPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!cannotEscape) ResumeGame();  //게임재개
            }
        }
        else CheckOnPausePanel();

        if (!pauseUIPanel.activeSelf) return;  //일시정지 창이 비활성화일 경우, return 처리

        if (!cannotEscape)  //escape가능 여부 판단 처리
        {
            if (achievementUIPanel.activeSelf || helpUIPanel.activeSelf || settingUIPanel.activeSelf)
            {
                //Debug.Log("나갈수 없음");
                cannotEscape = true;
            }
        }
        else
        {
            if (!achievementUIPanel.activeSelf && !helpUIPanel.activeSelf && !settingUIPanel.activeSelf)
            {
                //Debug.Log("나갈수 있음");
                cannotEscape = false;
            }
        }
    }

    private void ResumeGame()
    {
        Debug.Log("게임을 재개합니다.");
        pauseUIPanel.SetActive(false);
        Time.timeScale = 1f;           //일시정지 해제
    }

    private void PlayNewGame()
    {
        Debug.Log("새로운 게임을 시작합니다.");
    }

    private void OnAchievementPanel()
    {
        Debug.Log("업적UI가 활성화되었습니다.");
    }

    private void OnHelpPanel()
    {
        Debug.Log("도움말UI가 활성화되었습니다.");
    }

    private void OnSettingPanel()
    {
        Debug.Log("설정UI가 활성화되었습니다.");
        settingUIPanel.SetActive(true);
    }

    private void ExitGame()
    {
        Debug.Log("게임을 종료합니다.");
    }
}

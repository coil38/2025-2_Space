using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUI2 : MonoBehaviour
{
    [Header("버튼 UI")]
    [SerializeField] Button startGameButton;
    [SerializeField] Button settingButton;
    [SerializeField] Button exitGameButton;

    private SettingUIManager settingUIManager;

    private void OnEnable()
    {
        startGameButton.onClick.AddListener(StartGame);
        settingButton.onClick.AddListener(OnSettingPanel);
        exitGameButton.onClick.AddListener(ExitGame);
    }

    private void OnDisable()
    {
        startGameButton.onClick.RemoveListener(StartGame);
        settingButton.onClick.RemoveListener(OnSettingPanel);
        exitGameButton.onClick.RemoveListener(ExitGame);
    }

    public void StartGame()
    {
        if (!PlayerPrefs.HasKey("Tutorial"))
        {
            PlayerPrefs.SetInt("Tutorial", 1);
            SceneLoadManager.instance.
                LoadScene(GameSceneManager.GetSceneNameByType(SceneType.TutorialScene));  //튜토리얼 씬으로 이동 처리
        }
        else
        {
            SceneLoadManager.instance.
                LoadScene(GameSceneManager.GetSceneNameByType(SceneType.ChipsetSelectScene));  //칩셋 선택씬으로 이동
        }
    }
    private void OnSettingPanel()        //설정화면 활성화
    {
        if (settingUIManager == null)
            settingUIManager = PauseUIManager.Instance?.GetComponentInChildren<SettingUIManager>(true);

        settingUIManager.gameObject.SetActive(true);
        UIESCSystem.SetUIDepth(UIType.StartSceneUI, settingUIManager.EscapeSetting, settingUIManager.gameObject); //Esp용 UI_Depth설정 함수
    }

    private void ExitGame()
    {
        LogUtil.Log("게임을 종료합니다.");
        Application.Quit();
    }
}

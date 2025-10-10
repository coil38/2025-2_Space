using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartGameUI : MonoBehaviour
{
    [Header("메인 Layout 변수")]
    [SerializeField] private GameObject mainLayout;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitGameButton;

    [Header("저장용 Layout 그룹")]
    [SerializeField] private GameObject subLayout;
    [SerializeField] private Button[] files;
    [SerializeField] private Button deleteFileButton;
    [SerializeField] private Button returnButton;

    private string defualtName = "New File";
    public static Dictionary<int, string> fileDatas = new Dictionary<int, string>();  //파일의 인덱스, 이름

    void Start()
    {
        mainLayout.gameObject.SetActive(true);
        subLayout.gameObject.SetActive(false);

        startGameButton.onClick.AddListener(StartGame);
        settingButton.onClick.AddListener(OnSettingPanel);
        exitGameButton.onClick.AddListener(ExitGame);

        SaveManager.instance.SaveAndLoadButtonInfo(false);   //fileDatas 데이터 로드

        for (int i = 0; i < files.Length; i++)
        {
            if (fileDatas.TryGetValue(i, out string name))
            {
                //LogUtil.Log($"작동한다ㅏㅏ --> {i}: {name}");
                files[i].GetComponentInChildren<TextMeshProUGUI>().text = name;  //이름 설정 (초기화)
            }
            else
            {
                fileDatas.Add(i, defualtName);  //저장 대상이 없을 경우, 새로 만들기
                files[i].GetComponentInChildren<TextMeshProUGUI>().text = defualtName;  //이름 설정 (초기화)
                //LogUtil.Log("없는데요?");
            }
        }
        SaveManager.instance.SaveAndLoadButtonInfo(true);  //버튼 데이터 저장

        for (int i = 0; i < files.Length; i++)
        {
            int index = i;
            files[index].onClick.RemoveAllListeners();
            files[index].onClick.AddListener(() =>
            {
                if (fileDatas.TryGetValue(index, out string name))
                {
                    string fileName = $"SaveFile_{index}";
                    LogUtil.Log(fileName);
                    if (name == defualtName)
                    {
                        SaveManager.instance.StartNewSaveFile(fileName);   //현재 시간으로 저장
                        fileDatas[index] = fileName;                       //fileDatas 데이터 이름 설정
                        SaveManager.instance.SaveAndLoadButtonInfo(true);  //fileDatas 데이터를 JSON파일에 저장
                    }
                    else         //이미 저장된 파일일 경우,
                    {
                        SaveManager.instance.LoadSaveFile(name);   //설정된 이름으로 데이터 로드
                    }
                }
            });
        }
    }

    void OnDisable()
    {
        startGameButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        exitGameButton.onClick.RemoveAllListeners();
    }

    private void StartGame()
    {
        mainLayout.gameObject.SetActive(false);
        subLayout.gameObject.SetActive(true);
    }

    private void OnSettingPanel()
    {
        //설정화면 활성화
    }

    private void ExitGame()
    {
        LogUtil.Log("게임을 종료합니다.");
        Application.Quit();
    }

    private void DeleteFile()
    {
        //삭제 아이콘이 나온다.
        //해당 버튼 옆의 아이콘 클릭시,
        //해당 파일의 제이슨 파일을 찾아서 지운다.

        //+ 다시 묻는 UI가 나온다.
    }

    private void ReturnManu()
    {
        //이전 뎁스의 화면으로 돌아간다.
    }
}

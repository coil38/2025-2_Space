using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StartGameUI : MonoBehaviour
{
    [Header("메인 Layout 변수")]
    [SerializeField] private GameObject mainLayout;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button exitGameButton;

    [Header("저장용 Layout 그룹")]
    [SerializeField] private GameObject subLayout;
    [SerializeField] private Button[] fileButtons;
    [SerializeField] private Button startDeleteButton;
    [SerializeField] private Button returnButton;

    [Header("되묻기창용 변수")]
    [SerializeField] private ConfirmationUI confirmationUI;
    [SerializeField] private DeleteFileConfirmationUI deleteFileConfirmationUI;

    private string defualtName = "New File";
    public static Dictionary<int, string> fileDatas = new Dictionary<int, string>();  //파일의 인덱스, 이름
    private Button[] deleteButtons; //삭제 아이콘 버튼
    private bool isFileDeleting;    //현재 삭제시작여부
    private int deletedIndex;       //삭제될 버튼 인덱스

    private SettingUIManager settingUIManager;

    void Start()
    {
        mainLayout.gameObject.SetActive(true);
        subLayout.gameObject.SetActive(false);

        startGameButton.onClick.AddListener(StartGame);
        settingButton.onClick.AddListener(OnSettingPanel);
        exitGameButton.onClick.AddListener(ExitGame);

        SaveManager.instance.SaveAndLoadButtonInfo(false);   //fileDatas 데이터 로드

        if (fileDatas.Count <= 0)  //파일 데이터 없을 때, 처음 한번만 실행
        {
            for (int i = 0; i < fileButtons.Length; i++)
            {
                fileDatas.Add(i, defualtName);  //저장 대상이 없을 경우, 새로 만들기
                fileButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = defualtName;  //이름 설정 (초기화)
                //LogUtil.Log("없는데요?");

                InitializeDeleteButton(i);
            }
        }
        else
        {
            for (int i = 0; i < fileButtons.Length; i++)
            {
                if (fileDatas.TryGetValue(i, out string name))
                {
                    //LogUtil.Log($"작동한다ㅏㅏ --> {i}: {name}");
                    fileButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = name;  //이름 설정 (초기화)
                }

                InitializeDeleteButton(i);
            }
        }
        SaveManager.instance.SaveAndLoadButtonInfo(true);  //버튼 데이터 저장

        //----------------------------------------------------------------- 버튼 이벤트 설정 ----------------------------------------------------------------------||

        for (int i = 0; i < fileButtons.Length; i++)
        {
            int index = i;
            fileButtons[index].onClick.RemoveAllListeners();     //파일관련 버튼 이벤트 설정
            fileButtons[index].onClick.AddListener(() =>
            {
                if (isFileDeleting) return;                      //파일 삭제 중일 경우, 반환처리

                if (fileDatas.TryGetValue(index, out string name))
                {
                    string fileName = GetFileName(index);
                    LogUtil.Log(fileName);
                    LogUtil.Log($"버튼이름: {name}, 버튼 인덱스: {index}, 기본 이름: {defualtName}");
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

            //LogUtil.Log("삭제 버튼 개수: " + deleteButtons.Length);
            deleteButtons[index].onClick.RemoveAllListeners();        //파일 삭제관련 버튼 이벤트 설정
            deleteButtons[index].onClick.AddListener(() =>
            {
                confirmationUI.Show(deleteFileConfirmationUI);        //되묻기 창 활성화
                deletedIndex = index;                                 //삭제 대상 등록
            });
            deleteButtons[index].gameObject.SetActive(false);
        }

        startDeleteButton.onClick.RemoveAllListeners();    //삭제 시작 버튼 이벤트 구독
        startDeleteButton.onClick.AddListener(StartDeleteFile);

        returnButton.onClick.RemoveAllListeners();         //되돌아가기 버튼 이벤트 구독
        returnButton.onClick.AddListener(() =>
        {
            if (isFileDeleting) ReturnToFileSelectManu();  //현재 파일삭제 중일 경우
            else ReturnToMainManu();
        });
    }

    void OnDisable()
    {
        startGameButton.onClick.RemoveAllListeners();
        settingButton.onClick.RemoveAllListeners();
        exitGameButton.onClick.RemoveAllListeners();
    }

    private void StartGame()
    {
        mainLayout.SetActive(false);
        subLayout.SetActive(true);

        UIESCSystem.SetUIDepth(UIType.StartSceneUI, ReturnToMainManu, subLayout);    //Esp용 UI_Depth설정 함수
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

    public void DeleteFile()  //파일삭제용 함수 (되묻기용)
    {
        //해당 파일이 삭제되고 버튼 텍스트도 변경
        if (fileDatas.TryGetValue(deletedIndex, out string name))
        {
            SaveManager.instance.DeleteFile(name);  //해당 파일 삭제
            fileDatas[deletedIndex] = defualtName;  //기본 이름으로 변경
            SaveManager.instance.SaveAndLoadButtonInfo(true);  //변경된 파일정보 갱신
        }
        fileButtons[deletedIndex].GetComponentInChildren<TextMeshProUGUI>().text = defualtName;  //이름 설정 (초기화)
    }

    public void PlayeNewGame()  //삭제후, 재시작용 함수 (되묻기용)
    {
        string fileName = GetFileName(deletedIndex);
        SaveManager.instance.StartNewSaveFile(fileName);   //현재 시간으로 저장
        fileDatas[deletedIndex] = fileName;                       //fileDatas 데이터 이름 설정
        SaveManager.instance.SaveAndLoadButtonInfo(true);  //fileDatas 데이터를 JSON파일에 저장
    }

    private string GetFileName(int index)  //파일 이름 받기용 함수
    {
        return $"SaveFile_{index}";
    }

    private void InitializeDeleteButton(int index)    //삭제 버튼 변수 할당(초기화)용 함수
    {
        if (deleteButtons == null || deleteButtons.Length <= 0)
            deleteButtons = new Button[fileButtons.Length];  //삭제 버튼이 없을 경우, 파일 개수만큼 생성

        Button[] temp = fileButtons[index].gameObject.GetComponentsInChildren<Button>();
        deleteButtons[index] = Array.Find(temp, p => p != fileButtons[index]);                  //부모의 버튼이 아닌 컴포넌트 찾고 할당
        //LogUtil.Log(deleteButtons[i].name);
    }

    private void StartDeleteFile()
    {
        bool cannotDelete = true;
        GameObject deleteIconObj = null;

        for (int i = 0; i < fileButtons.Length; i++)
        {
            if (fileDatas.TryGetValue(i, out string name))
            {
                if (name != defualtName)
                {
                    deleteButtons[i].gameObject.SetActive(true);   //저장파일이 있는 삭제아이콘만 활성화
                    cannotDelete = false;
                    deleteIconObj = deleteButtons[i].gameObject;
                }
            }
            fileButtons[i].gameObject.GetComponent<HighLingthingButtonUI>().isCanInteracting = false;  //모든 버튼 상호작용 비활성화 처리
        }

        if (cannotDelete)        //삭제할 수 없다면
        {
            InitializeDelete();  //초기화 처리
            startDeleteButton.gameObject.GetComponent<HighLingthingButtonUI>().isCanInteracting = false;  //삭제 시작 버튼 상호작용 비 활성화 처리
            isFileDeleting = false;
        }
        else
        {
            UIESCSystem.SetUIDepth(UIType.StartSceneUI, ReturnToFileSelectManu, deleteIconObj);    //Esp용 UI_Depth설정 함수

            startDeleteButton.gameObject.GetComponent<HighLingthingButtonUI>().isCanInteracting = true;  //삭제 시작 버튼 상호작용 활성화 처리
            startDeleteButton.gameObject.SetActive(false);          //버튼 비활성화 처리
            isFileDeleting = true;
        }
    }

    public void InitializeDelete()
    {
        for (int i = 0; i < fileButtons.Length; i++)
        {
            deleteButtons[i].gameObject.SetActive(false);
            fileButtons[i].gameObject.GetComponent<HighLingthingButtonUI>().isCanInteracting = true;  //모든 버튼 상호작용 활성화 처리
        }

        isFileDeleting = false;
        //LogUtil.Log("초기화");
    }

    private void ReturnToMainManu()
    {
        //이전 뎁스의 화면으로 돌아간다.
        mainLayout.gameObject.SetActive(true);
        subLayout.gameObject.SetActive(false);
    }

    private void ReturnToFileSelectManu()
    {
        startDeleteButton.gameObject.SetActive(true);          //버튼 활성화 처리
        InitializeDelete();
    }
}

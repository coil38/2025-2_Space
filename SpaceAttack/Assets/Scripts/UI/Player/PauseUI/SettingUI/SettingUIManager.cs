using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEditor.Search;

public class SettingUIManager : MonoBehaviour
{
    [Header("버튼용 변수")]
    [SerializeField] private Button exitButton;  //되돌아가기 버튼
    [SerializeField] private Button saveButton;    //저장 버튼
    [SerializeField] private Button resetButton;   //초기화 버튼

    [Header("되묻기창용 변수")]
    [SerializeField] private ConfirmationUI confirmationUI;
    [SerializeField] private SettingUIStrategy saveConfirmationUI;   //저장
    [SerializeField] private SettingUIStrategy exitConfirmationUI;   //나가기
    [SerializeField] private SettingUIStrategy resetConfirmationUI;  //초기화

    public Dictionary<TMP_InputField, string> initialComands = new Dictionary<TMP_InputField, string>();  //초기값 ( 개발자 설정 )
    public Dictionary<TMP_InputField, string> currentComands = new Dictionary<TMP_InputField, string>();  //변경값 ( 변경하면서 최신 갱신 )
    public Dictionary<TMP_InputField, string> savedComands = new Dictionary<TMP_InputField, string>();    //저장값 ( 저장버튼을 누르고 저장된 설정 )

    public Dictionary<string, float> savedVolumes = new Dictionary<string, float>();  //사운드 저장값 ( 저장버튼을 누르고 저장된 설정 + 변경하면서 최신 갱신 )

    public event Action cancelSaveEvent;   //저장 취소용 이벤트 델리게이트
    public event Action saveEvent;         //저장용 이벤트 델리게이트
    public event Action resetEvent;        //초기화용 이벤트 델리게이트

    [HideInInspector] public InputSettingUI currentInputSetting;   //한번에 하나의 InputField만 작동할 수 있음 (예외처리)
    public InputActionRebindingExtensions.RebindingOperation rebindingOperation;   //변경할 ActionOperation (오직 하나)

    [HideInInspector] public Queue<bool> isChanged = new Queue<bool>();  //변경사항 유무 체크

    private void OnEnable() //입력 실행 비활성화 처리 및 버튼 이벤트 구독
    {
        isChanged.Clear(); //변경사항 유무 초기화

        PlayerInputController.DisableAction();         //입력 비활성화 처리 ( 리바이딩을 위해서 )

        confirmationUI.gameObject.SetActive(true);     //경고창 설정 초기화
        exitButton.onClick.AddListener(() =>
        {
            if (isChanged.Count > 0)  //변경사항이 있을 경우
            {
                if (confirmationUI != null && exitConfirmationUI != null)
                    confirmationUI.Show(exitConfirmationUI);
            }
            else  //변경사항이 없을 경우
            {
                gameObject.SetActive(false);
            }
        });

        saveButton.onClick.AddListener(() =>
        {
            if (isChanged.Count > 0)  //변경사항이 있을 경우
            {
                if (confirmationUI != null && saveConfirmationUI != null)
                    confirmationUI.Show(saveConfirmationUI);
            }
        });

        resetButton.onClick.AddListener(() =>
        {
            if (confirmationUI != null && resetConfirmationUI != null)
                confirmationUI.Show(resetConfirmationUI);
        });

        confirmationUI.gameObject.SetActive(false);
    }

    private void OnDisable() //입력 실행 활성화 처리 및 버튼 이벤트 구독 해지
    {
        PlayerInputController.EnableAction();

        exitButton.onClick.RemoveAllListeners();
        saveButton.onClick.RemoveAllListeners();
        resetButton.onClick.RemoveAllListeners();
    }

    public void ExitSettingPanel()  //설정 패널 비활성화 함수
    {
        gameObject.SetActive(false);
    }

    public void SaveSetting()  //입력키 저장 내부 실행함수
    {
        //바꾼 값들 저장
        saveEvent.Invoke();
    }

    public void CancelSave()  //입력키 취소 내부 실행함수
    {
        cancelSaveEvent.Invoke();
    }

    public void ResetSetting()  //입력키 초기화 내부 실행함수
    {
        //이전 값으로 초기화
        resetEvent.Invoke();
    }

    public static void SaveRebinds(InputActionAsset asset)
    {
        var json = asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", json);
        PlayerPrefs.Save();
    }

    private void LoadRebinds(InputActionAsset asset)
    {
        var json = PlayerPrefs.GetString("rebinds", string.Empty);
        if(!string.IsNullOrEmpty(json))
            asset.LoadBindingOverridesFromJson(json);
    }
}

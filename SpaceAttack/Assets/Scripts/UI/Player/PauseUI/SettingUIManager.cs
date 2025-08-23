using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System;

public class SettingUIManager : MonoBehaviour
{
    [Header("버튼용 변수")]
    [SerializeField] private Button returnButton;  //되돌아가기 버튼
    [SerializeField] private Button saveButton;    //저장 버튼
    [SerializeField] private Button resetButton;   //초기화 버튼

    public Dictionary<TMP_InputField, string> initialComands = new Dictionary<TMP_InputField, string>();  //초기값 ( 개발자 설정 )
    public Dictionary<TMP_InputField, string> currentComands = new Dictionary<TMP_InputField, string>();  //변경값 ( 변경하면서 최신 갱신 )
    public Dictionary<TMP_InputField, string> savedComands = new Dictionary<TMP_InputField, string>();    //저장값 ( 저장버튼을 누르고 저장된 설정 )

    public event Action cancelSaveEvent;   //저장 취소용 이벤트 델리게이트
    public event Action saveEvent;         //저장용 이벤트 델리게이트
    public event Action resetEvent;        //초기화용 이벤트 델리게이트

    public InputSettingUI currentInputSetting;   //한번에 하나의 InputField만 작동할 수 있음 (예외처리)
    public InputActionRebindingExtensions.RebindingOperation rebindingOperation;   //변경할 ActionOperation (오직 하나)

    void Start()   //초기 입력값들 설정 및 저장
    {
        returnButton.onClick.AddListener(ExitSettingPanel);
        saveButton.onClick.AddListener(SaveSetting);
        resetButton.onClick.AddListener(ResetSetting);
    }

    private void OnEnable()
    {
        PlayerInputController.DisableAction();
    }

    private void OnDisable()
    {
        PlayerInputController.EnableAction();
    }

    private void ExitSettingPanel()
    {
        gameObject.SetActive(false);
    }

    private void SaveSetting()
    {
        //바꾼 값들 저장
        saveEvent.Invoke();
    }

    private void CancleSave()
    {
        cancelSaveEvent.Invoke();
    }

    private void ResetSetting()
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

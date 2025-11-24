using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSettingUI : MonoBehaviour
{
    [SerializeField] private InputActionReference inputReference;//본인이 바꿀 인풋 래퍼런스
    [SerializeField] private SettingUIManager settingUIManager;  //설정 UIMangaer
    [SerializeField] private TextMeshProUGUI preMeshProUGUI;     //이전 텍스트 매쉬

    [Header("이동용 변수 설정")]
    [SerializeField] private bool isUsingAxis;                   //입력이 Axis를 사용 여부
    [SerializeField] private bool isPositive;                    //그 입력이 긍정값인지 여부

    [Header("비주얼용")]
    [SerializeField] private GameObject focusingImage;          //집중이미지

    private TMP_InputField inputField;
    private bool isInputting;
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        settingUIManager.initialComands.Add(inputField, preMeshProUGUI.text.ToUpper());
        settingUIManager.currentComands.Add(inputField, preMeshProUGUI.text.ToUpper());
        settingUIManager.savedComands.Add(inputField, preMeshProUGUI.text.ToUpper());
    }

    private void OnEnable()
    {
        settingUIManager.cancelSaveEvent += CancelSavedRebinding;
        settingUIManager.saveEvent += SaveRebinging;
        settingUIManager.resetEvent += ResetRebinding;

        if(inputField != null) inputField.text = "";  //입력칸 초기화
    }

    private void OnDisable()
    {
        settingUIManager.cancelSaveEvent -= CancelSavedRebinding;
        settingUIManager.saveEvent -= SaveRebinging;
        settingUIManager.resetEvent -= ResetRebinding;
    }

    void Update()
    {
        CheckChanges();
    }

    private void CheckChanges()
    {
        if (inputField.isFocused && !isInputting)  //입력준비중이고 아무 키나 눌렀을 때, 실행
        {
            focusingImage.SetActive(true);
            focusingImage.transform.position = transform.position;

            settingUIManager.currentInputSetting = this;  //입력준비 중일 때, 최근 인스턴스로 본인을 갱신
            RebindKey();  //변경후, 입력
            //Debug.Log(gameObject.name + "이 작동한다.");
        }

        if (settingUIManager.currentInputSetting != this && isInputting)        //최근 인스턴스가 본인 아닐 경우(변경대상이 본인이 아닐 경우)
        {
            focusingImage.SetActive(false);

            isInputting = false;
            //inputField.text = "";
            //Debug.Log($"{gameObject.name} 인스턴스는 변경 대상이 빠뀜.");
        }
    }
    public void RebindKey()
    {
        // 기본적으로 첫 번째 바인딩을 대상으로
        var action = inputReference.action;
        int targetIndex = 0;

        // ActionType과 ControllerType에 따라서 변경되는 값
        if (isUsingAxis)
        {
            if (isPositive) targetIndex = 2;
            else targetIndex = 1;
        }

        isInputting = true;
        settingUIManager.rebindingOperation?.Cancel();   //이미 op가 존재할 경우, 해당 op 초기화

        //if (settingUIManager.currentInputSetting == this)  //입력 대기 중, 텍스트 설정
        //{
        //    preMeshProUGUI.text = "ReadyToSet";
        //}

        settingUIManager.rebindingOperation = action.PerformInteractiveRebinding(targetIndex)
              .WithControlsExcluding("Mouse")    // 원하면 마우스 제외 등 필터
              .WithCancelingThrough("<Keyboard>/escape")
              .OnCancel(op =>
              {
                  inputField.DeactivateInputField();   //입력준비 비활성화
                  //Debug.Log("Rebind 취소");
                  op.Dispose();
                  isInputting = false;
                  preMeshProUGUI.text = settingUIManager.currentComands[inputField];  //입력 대기 중, 텍스트 설정 취소
              })
              .OnComplete(op =>
              {
                  string key = action.bindings[targetIndex].effectivePath.ToUpper();
                  key = key.Substring(key.IndexOf("/") + 1);

                  //Debug.Log(key);
                  inputField.DeactivateInputField();   //입력준비 비활성화

                  if (settingUIManager.currentComands.ContainsValue(key))   //중복되는 키가 있는지 체크
                  {
                      //Debug.Log($"이미 {key}에 대한 입력값이 존재합니다.");
                      inputField.text = "";
                      op.Dispose();
                      isInputting = false;

                      preMeshProUGUI.text = settingUIManager.currentComands[inputField];  //입력 대기 중, 텍스트 설정 취소

                      string currentKey = settingUIManager.currentComands[inputField].ToLower();
                      action.ApplyBindingOverride(targetIndex, "<Keyboard>/" + currentKey);   //입력을 통해서 바뀌었던 키를 다시 되돌리기
                      return;
                  }

                  //Debug.Log($"{gameObject.name}이 Rebind 완료: " + action.bindings[targetIndex].effectivePath);
                  op.Dispose();
                  preMeshProUGUI.text = key;
                  inputField.text = "";
                  settingUIManager.currentComands[inputField] = key;       //최근 키로 저장
                  isInputting = false;

                  if (settingUIManager.savedComands[inputField] != key)    //이전 저장값과 변경될 입력값이 같지 않을 경우
                  {
                      settingUIManager.isChanged.Enqueue(true);            //변경사항 있음 처리
                  }
                  else
                  {
                      settingUIManager.isChanged.TryDequeue(out bool temp);    //변경사항 없음 처리
                  }
                  SettingUIManager.SaveRebinds(action.actionMap.asset);                   // 저장
              })
              .Start();
    }


    public void CancelSavedRebinding()
    {
        settingUIManager.rebindingOperation?.Cancel();   //이미 op가 존재할 경우, 해당 op 초기화

        // 기본적으로 첫 번째 바인딩을 대상으로
        var action = inputReference.action;
        int targetIndex = 0;

        // ActionType과 ControllerType에 따라서 변경되는 값
        if (isUsingAxis)
        {
            if (isPositive) targetIndex = 2;
            else targetIndex = 1;
        }

        string currentKey = settingUIManager.savedComands[inputField].ToLower();
        action.ApplyBindingOverride(targetIndex, "<Keyboard>/" + currentKey);   //입력을 통해서 바뀌었던 키를 다시 되돌리기

        preMeshProUGUI.text = currentKey;  //현재 입력 칸 값 변경

        settingUIManager.currentComands[inputField] = settingUIManager.savedComands[inputField];  //최근 입력값을 이전 저장된 입력값으로 덮어쓰기
    }

    public void SaveRebinging()
    {
        settingUIManager.rebindingOperation?.Cancel();   //이미 op가 존재할 경우, 해당 op 초기화

        settingUIManager.savedComands[inputField] = settingUIManager.currentComands[inputField];  //저장 커맨드 갱신
    }

    public void ResetRebinding()
    {
        settingUIManager.rebindingOperation?.Cancel();   //이미 op가 존재할 경우, 해당 op 초기화

        inputReference.action.RemoveAllBindingOverrides();   //모든 Rebinding 제거
        preMeshProUGUI.text = settingUIManager.initialComands[inputField];
        inputField.text = "";

        settingUIManager.currentComands[inputField] = settingUIManager.initialComands[inputField];  //최근 입력값을 초기 입력값으로 덮어쓰기
        settingUIManager.savedComands[inputField] = settingUIManager.initialComands[inputField];  //저장된 입력값을 초기 입력값으로 덮어쓰기
    }
}

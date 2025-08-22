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
    [SerializeField] private TextMeshProUGUI inputMeshProUGUI;   //입력 텍스트 매쉬

    private TMP_InputField inputField;
    private string initialInputKey;     //초기 입력값
    private string currentInputText;
    private bool isOneTime;
    void Start()
    {
        inputField = GetComponent<TMP_InputField>();
        currentInputText = preMeshProUGUI.text;
        settingUIManager.initialComands.Add(inputField, preMeshProUGUI.text);

        //초기화용 델리게이트 구독
        initialInputKey = preMeshProUGUI.text;
        //settingUIManager.resetEvent += RebindKeyToInitial;
    }

    void Update()
    {
        CheckChanges();
    }

    private void CheckChanges()
    {
        //if (inputField.text != currentInputText)  //방금 입력한 텍스트와 최근 텍스트가 다를 경우 (E | W) ( E | E ) (W | E)
        //{
        //    if (!isOneTime && currentInputText == preMeshProUGUI.text)  //최근 텍스트와 저장전 텍스트와 같을 때 (W | W) ( E | W ) ( E | W )
        //    {
        //        settingUIManager.saveEvent += RebindKey;
        //        settingUIManager.saveEvent += ChangePlaceholder;
        //        currentInputText = inputField.text;                     //최근 텍스트 갱신 ( W <= E )
        //        isOneTime = true;
        //    }
        //    else if (isOneTime && inputField.text == preMeshProUGUI.text) // 예외처리 ( W | W )
        //    {
        //        settingUIManager.saveEvent -= RebindKey;
        //        settingUIManager.saveEvent -= ChangePlaceholder;
        //        currentInputText = inputField.text;                    //최근 텍스트 갱신 ( E <= W )
        //        isOneTime = false;
        //    }
        //}

        if (inputField.isFocused)
        {
            RebindKey();  //변경후, 입력
        }
    }

    //private void RebindKey()
    //{
    //    var action = inputReference.action;
    //    int targetIndex = 0;
    //    action.ApplyBindingOverride(targetIndex, "<Keyboard>/" + inputField.text);

    //    Debug.Log("저장되었다");
    //}

    public void RebindKey()
    {
        // 기본적으로 첫 번째 바인딩을 대상으로
        var action = inputReference.action;
        int targetIndex = 0;

        action.PerformInteractiveRebinding()
              .WithControlsExcluding("Mouse")    // 원하면 마우스 제외 등 필터
              .OnComplete(op =>
              {
                  op.Dispose();
                  Debug.Log("Rebind 완료: " + action.bindings[targetIndex].effectivePath);
                  //preMeshProUGUI.text = 
                  SettingUIManager.SaveRebinds(action.actionMap.asset);                   // 저장
              })
              .Start();
    }

    //private void RebindKeyToInitial()
    //{
    //    var action = inputReference.action;
    //    int targetIndex = 0;
    //    action.ApplyBindingOverride(targetIndex, "<Keyboard>/" + initialInputKey);
    //}

    private void ChangePlaceholder()
    {
        //preMeshProUGUI.text = inputField.text;
        //settingUIManager.saveEvent -= RebindKey;          //구독 해지
        //settingUIManager.saveEvent -= ChangePlaceholder;
    }
}

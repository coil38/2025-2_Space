using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveConfirmationUI : SettingUIStrategy
{
    [SerializeField] Button _leftButton, _midleButton, _rightButton;
    [SerializeField] TextMeshProUGUI _leftButtonText, _midleButtonText, _rightButtonText;
    [SerializeField] TextMeshProUGUI _contentText;
    [SerializeField] Sprite _exitLeftSprite, _exitMidleSprite, _exitRightSprite;
    [SerializeField] ConfirmationUI confirmationUI;

    [Header("설정UI")]
    [SerializeField] private SettingUIManager settingUIManager;
    public override void Execute()
    {
        _leftButtonText.text = "Save";
        _midleButtonText.text = "Cancel";
        _contentText.text = "Do you want to save your changes?";
        //각각의 버튼들 Sprite추가
        _rightButton.gameObject.SetActive(false);
        confirmationUI.gameObject.SetActive(true);

        _leftButton.onClick.AddListener(() =>
        {
            LogUtil.Log("저장 버튼 동작");
            if (settingUIManager != null)
            {
                settingUIManager.isChanged.Clear();  //저장 체크용 변경 사항 초기화
                settingUIManager.SaveSetting();  //저장
            }
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
        });

        _midleButton.onClick.AddListener(() =>
        {
            LogUtil.Log("취소 버튼 동작");
            if (confirmationUI != null) confirmationUI.gameObject.SetActive(false);
        });
    }
}
